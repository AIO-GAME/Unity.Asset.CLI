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
        private IEnumerator _CO;

        protected override IEnumerator CO
        {
            get
            {
                if (_CO is null) _CO = AssetSystem.Proxy.LoadRawFileTextCO(Address, OnCompletedCO);
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
            _CO = null;
        }

        #region CO

        private void OnCompletedCO(string asset)
        {
            Progress = 100;
            Result   = asset;
            IsDone   = true;
            InvokeOnCompleted();
        }

        #endregion

        #region Task

        private void OnCompletedTaskObject()
        {
            Progress = 100;
            Result   = AwaiterObject.GetResult();
            IsDone   = true;
            InvokeOnCompleted();
        }

        private TaskAwaiter<string> AwaiterObject;

        protected override TaskAwaiter<string> GetAwaiterObject()
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