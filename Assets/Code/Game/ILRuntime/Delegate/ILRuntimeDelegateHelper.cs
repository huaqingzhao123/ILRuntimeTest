using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public class ILRuntimeDelegateHelper
{
    static public void Register(AppDomain appdomain)
    {
        #region 基本类型 bool, int, string, float(single)
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) =>
            {
                ((Action<System.Boolean>)act)(arg0);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Int32>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Int32>((arg0) =>
            {
                ((Action<System.Int32>)act)(arg0);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32>();

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.String>((arg0) =>
            {
                ((Action<System.String>)act)(arg0);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.String>();

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
            {
                ((Action<System.Single>)act)(arg0);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Single>();

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single, System.Single>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Single, System.Single>((arg0, arg1) =>
            {
                ((Action<System.Single, System.Single>)act)(arg0, arg1);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Single, System.Single>();

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single, System.Int32>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Single, System.Int32>((arg0, arg1) =>
            {
                ((Action<System.Single, System.Int32>)act)(arg0, arg1);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Single, System.Int32>();
        #endregion
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, UnityEngine.GameObject>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.List<System.Object>>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.IDictionary<System.String, UnityEngine.Object>>();

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Object>>((act) =>
        {
            return new System.Predicate<System.Object>((obj) =>
            {
                return ((Func<System.Object, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Boolean>();

    }
}
