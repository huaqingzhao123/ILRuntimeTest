using HuaFramework;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System;
public class GameRoot : MonoBehaviour
{
    /// <summary>
    /// 是否开启热更新
    /// </summary>
    public bool HotFix;
    public ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    void Start()
    {
        LoadHotFixDll();
        if (HotFix)
            HotFixLogic();
        else
        {
            Logic();
        }
    }

    /// <summary>
    /// 加载热更dll
    /// </summary>
    void LoadHotFixDll()
    {
        var dllBytes = File.ReadAllBytes(Application.dataPath+"/Resource/Hotfix/hotfix.bytes");
        ILRuntimeHelper.LoadHotfix(dllBytes, null);
        appdomain = ILRuntimeHelper.AppDomain;
        //在热更DLL里面使用MonoBehaviour是可以做到的，但是并不推荐这么做
        //因为即便能做到使用，要完全支持MonoBehaviour的所有特性，会需要很多额外的工作量
        //而且通过MonoBehaviour做游戏逻辑当项目规模大到一定程度之后会是个噩梦，因此应该尽量避免
        SetupCLRRedirection();
        SetupCLRRedirection2();
    }

    void HotFixLogic()
    {
        //为游戏物体添加一个热更脚本中的mono脚本
        //var I_test = appdomain.LoadedTypes["MonoBridge"];
        //var test = I_test.ReflectionType;
        //注释代码为通过系统反射接口调用，目前采用ILRuntime接口调用，两种方式等效
        //MethodInfo mi = test.GetMethod("foo");
        var instanceTest = appdomain.Instantiate("MonoBridge");
        var cube = GameObject.Find("Cube");
        //mi.Invoke(instanceTest, new object[] { cube });
        //直接调用GameObject.AddComponent<T>会报错，这是因为这个方法是Unity实现的，他并不可能取到热更DLL内部的类型
        appdomain.Invoke("MonoBridge", "AddMonoMoveScript", instanceTest, cube);        
    }
    void Logic()
    {
        var test = Assembly.GetExecutingAssembly().GetType("MonoBridge");
        var cube = GameObject.Find("Cube");
        var instanceTest = Activator.CreateInstance(test);
        //这里用反射是为了不访问具体的类，防止编译热更dll失败
        var method = test.GetMethod("AddMonoMoveScript");
        method.Invoke(instanceTest,new object[] { cube});
    }

    unsafe void SetupCLRRedirection()
    {
        //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
            {
                appdomain.RegisterCLRMethodRedirection(i, AddComponent);
            }
        }
    }

    unsafe void SetupCLRRedirection2()
    {
        //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
            {
                appdomain.RegisterCLRMethodRedirection(i, GetComponent);
            }
        }
    }

    MonoBehaviourAdapter.Adaptor GetComponent(ILType type)
    {
        var arr = GetComponents<MonoBehaviourAdapter.Adaptor>();
        for (int i = 0; i < arr.Length; i++)
        {
            var instance = arr[i];
            if (instance.ILInstance != null && instance.ILInstance.Type == type)
            {
                return instance;
            }
        }
        return null;
    }

    unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        //CLR重定向的说明请看相关文档和教程，这里不多做解释
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        //成员方法的第一个参数为this
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        //AddComponent应该有且只有1个泛型参数
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.AddComponent(type.TypeForCLR);
            }
            else
            {
                //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                var ilInstance = new ILTypeInstance(type as ILType, false);//手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                //接下来创建Adapter实例
                var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                clrInstance.ILInstance = ilInstance;
                clrInstance.AppDomain = __domain;
                //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                ilInstance.CLRInstance = clrInstance;

                res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance

                clrInstance.Awake();//因为Unity调用这个方法时还没准备好所以这里补调一次
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }

    unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        //CLR重定向的说明请看相关文档和教程，这里不多做解释
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        //成员方法的第一个参数为this
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        //AddComponent应该有且只有1个泛型参数
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res = null;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.GetComponent(type.TypeForCLR);
            }
            else
            {
                //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                for (int i = 0; i < clrInstances.Length; i++)
                {
                    var clrInstance = clrInstances[i];
                    if (clrInstance.ILInstance != null)//ILInstance为null, 表示是无效的MonoBehaviour，要略过
                    {
                        if (clrInstance.ILInstance.Type == type)
                        {
                            res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance
                            break;
                        }
                    }
                }
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }
}
