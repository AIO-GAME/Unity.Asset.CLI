using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AIO
{
    internal partial class ASHandleLoadRawFileText
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<string> Create(string location, Action<string> completed)
        {
            if (completed is null) return Create(location);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<string> handles)
            {
                if (handles.IsDone) completed.Invoke(handles.Result);
                else handles.Completed += completed;
                return handles;
            }

            return new ASHandleLoadRawFileText(location, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<string> Create(string location)
        {
            return AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<string> handles
                ? handles
                : new ASHandleLoadRawFileText(location);
        }
    }

    internal partial class ASHandleLoadRawFileText : ASHandle<string>
    {
        #region Sync

        protected override void OnInvoke()
        {
            Result = AssetSystem.Proxy.LoadRawFileTextSync(Address);
        }

        #endregion

        #region CO

        protected override IEnumerator OnCreateCO()
        {
            return AssetSystem.Proxy.LoadRawFileTextCO(Address, OnCompletedCO);
        }

        private void OnCompletedCO(string asset)
        {
            Result = asset;
            InvokeOnCompleted();
        }

        #endregion

        #region Task

        private void OnCompletedTaskObject()
        {
            Result = AwaiterObject.GetResult();
            InvokeOnCompleted();
        }

        private TaskAwaiter<string> AwaiterObject;

        protected override TaskAwaiter<string> OnAwaiterObject()
        {
            AwaiterObject = AssetSystem.Proxy.LoadRawFileTextTask(Address).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        /// <inheritdoc />
        private ASHandleLoadRawFileText(string location)
            : base(location) { }

        /// <inheritdoc />
        private ASHandleLoadRawFileText(string location, Action<string> onCompleted)
            : base(location, onCompleted) { }

        #endregion
    }
}