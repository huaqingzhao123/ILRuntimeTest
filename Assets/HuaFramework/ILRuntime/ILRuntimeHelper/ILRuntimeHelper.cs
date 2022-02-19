using System.IO;
using ILRuntime.Runtime.CLRBinding;
using ILRuntime.Runtime.Enviorment;
using LitJson;
//using Mono.Cecil.Pdb;
using UnityEngine;


namespace HuaFramework
{
    static public class ILRuntimeHelper
    {
        public static AppDomain AppDomain { get; private set; }
        public static bool IsRunning { get; private set; }

        static private MemoryStream fsDll = null;

        /// <summary>
        /// 加载热更dll并注册跨域继承适配器和CLR绑定
        /// </summary>
        /// <param name="hotfixdll"></param>
        /// <param name="hotfixpdb"></param>
        public static void LoadHotfix(byte[] hotfixdll, byte[] hotfixpdb)
        {
            //
            IsRunning = true;
            fsDll = new MemoryStream(hotfixdll);

            //加载dll
            AppDomain = new AppDomain();
            //这里的流不能释放，头铁的老哥别试了
            AppDomain.LoadAssembly(fsDll);


            //绑定的初始化
            //注册跨域继承适配器
            AdapterRegister.RegisterCrossBindingAdaptor(AppDomain);
            ////正常在CLR绑定代码生成后进行注册，这一步很重要，注册后生成的绑定代码才能生效！
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(AppDomain);
            //ILRuntime.Runtime.Generated.CLRManualBindings.Initialize(AppDomain);
            //注册委托适配器和转换器
            ILRuntimeDelegateHelper.Register(AppDomain);
            //
            JsonMapper.RegisterILRuntimeCLRRedirection(AppDomain);
            if (Application.isEditor)
            {
                AppDomain.DebugService.StartDebugService(56000);
                //Debug.Log("热更调试器 准备待命~");
            }
            //
            AppDomain.Invoke("HotfixCheck", "Log", null, null);
        }


        public static void Close()
        {
            AppDomain = null;

            if (fsDll != null)
            {
                fsDll.Dispose();
            }
        }
    }
}