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

        private IEnumerator _CO;

        private TaskAwaiter _Awaiter;

        /// <inheritdoc />
        protected override IEnumerator CO
        {
            get
            {
                if (_CO is null) _CO = AssetSystem.Proxy.UnloadSceneCO(Address, OnCompletedTaskObject);
                return _CO;
            }
        }

        protected override void Reset()
        {
            Progress = 0;
            IsDone   = false;
            CO.Reset();
        }

        protected override void OnDispose()
        {
            Address = null;
        }


        #region Task

        private void OnCompletedTaskObject()
        {
            _CO      = null;
            Progress = 100;
            IsDone   = true;
            InvokeOnCompleted();
        }

        protected override TaskAwaiter GetAwaiter()
        {
            _Awaiter = AssetSystem.Proxy.UnloadSceneTask(Address).GetAwaiter();
            _Awaiter.OnCompleted(OnCompletedTaskObject);
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