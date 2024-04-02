using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AIO
{
    internal partial class ASHandleLoadRawFileData
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<byte[]> Create(string location, Action<byte[]> completed)
        {
            if (completed is null) return Create(location);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<byte[]> handles)
            {
                if (handles.IsDone) completed.Invoke(handles.Result);
                else handles.Completed += completed;
                return handles;
            }

            return new ASHandleLoadRawFileData(location, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<byte[]> Create(string location)
        {
            return AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<byte[]> handles
                ? handles
                : new ASHandleLoadRawFileData(location);
        }
    }

    internal partial class ASHandleLoadRawFileData : ASHandle<byte[]>
    {
        private IEnumerator _CO;

        protected override IEnumerator CO
        {
            get
            {
                if (_CO is null) _CO = AssetSystem.Proxy.LoadRawFileDataCO(Address, OnCompletedCO);
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

        private void OnCompletedCO(byte[] asset)
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

        private TaskAwaiter<byte[]> AwaiterObject;

        protected override TaskAwaiter<byte[]> GetAwaiterObject()
        {
            AwaiterObject = AssetSystem.Proxy.LoadRawFileDataTask(Address).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        /// <inheritdoc />
        public ASHandleLoadRawFileData(string location)
            : base(location) { }

        /// <inheritdoc />
        public ASHandleLoadRawFileData(string location, Action<byte[]> onCompleted)
            : base(location, onCompleted) { }

        #endregion
    }
}