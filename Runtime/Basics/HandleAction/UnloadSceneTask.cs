using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AIO
{
    internal partial class OperationActionUnloadSceneTask
    {
        public static IOperationAction Create(string location, Action complete)
        {
            return new OperationActionUnloadSceneTask(location, complete);
        }

        public static IOperationAction Create(string location)
        {
            return new OperationActionUnloadSceneTask(location);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal partial class OperationActionUnloadSceneTask : OperationAction
    {
        private string Address;

        protected override void CreateSync()
        {
            var temp = AssetSystem.Proxy.UnloadSceneTask(Address);
            while (temp.IsCompleted == false) temp.Wait();
        }

        protected override IEnumerator CreateCoroutine()
        {
            return AssetSystem.Proxy.UnloadSceneCO(Address, InvokeOnCompleted);
        }


        protected override void OnDispose()
        {
            Address = null;
        }


        #region Task

        private TaskAwaiter Awaiter;

        protected override TaskAwaiter CreateAsync()
        {
            Awaiter = AssetSystem.Proxy.UnloadSceneTask(Address).GetAwaiter();
            Awaiter.OnCompleted(InvokeOnCompleted);
            return Awaiter;
        }

        #endregion

        #region Constructor

        public OperationActionUnloadSceneTask(string location)
        {
            Address    = AssetSystem.SettingToLocalPath(location);
            IsValidate = AssetSystem.Proxy.CheckLocationValid(Address);
        }

        public OperationActionUnloadSceneTask(string location, Action complete)
        {
            Address    =  AssetSystem.SettingToLocalPath(location);
            IsValidate =  AssetSystem.Proxy.CheckLocationValid(Address);
            Completed  += complete;
        }

        #endregion
    }
}