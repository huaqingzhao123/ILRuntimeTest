using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_GameObject_Binding.Register(app);
            Move_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            MonoBehaviourAdapter_Binding_Adaptor_Binding.Register(app);
            ILRuntime_Runtime_Intepreter_ILTypeInstance_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
