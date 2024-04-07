using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AIO
{
    internal partial class ASHandleActionUnloadSceneTask
    {
        public static AssetSystem.IHandleAction Create(string location, Action complete)
        {
            return new ASHandleActionUnloadSceneTask(location, complete);
        }

        public static AssetSystem.IHandleAction Create(string location)
        {
            return new ASHandleActionUnloadSceneTask(location);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal partial class ASHandleActionUnloadSceneTask : ASHandleAction
    {
        private string Address;

        protected override void OnInvoke()
        {
            var temp = AssetSystem.Proxy.UnloadSceneTask(Address);
            while (temp.IsCompleted == false) temp.Wait();
        }

        protected override IEnumerator OnCreateCO()
        {
            return AssetSystem.Proxy.UnloadSceneCO(Address, InvokeOnCompleted);
        }


        protected override void OnDispose()
        {
            Address = null;
        }


        #region Task

        private TaskAwaiter _Awaiter;

        protected override TaskAwaiter OnAwaiter()
        {
            _Awaiter = AssetSystem.Proxy.UnloadSceneTask(Address).GetAwaiter();
            _Awaiter.OnCompleted(InvokeOnCompleted);
            return _Awaiter;
        }

        #endregion

        #region Constructor

        public ASHandleActionUnloadSceneTask(string location)
        {
            Address = AssetSystem.SettingToLocalPath(location);
        }

        public ASHandleActionUnloadSceneTask(string location, Action complete)
        {
            Address   =  AssetSystem.SettingToLocalPath(location);
            Completed += complete;
        }

        #endregion
    }
}