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
        #region Sync

        protected override void CreateSync()
        {
            Result = AssetSystem.Proxy.LoadRawFileDataSync(Address);
        }

        #endregion Task

        #region CO

        protected override IEnumerator CreateCoroutine()
        {
            return AssetSystem.Proxy.LoadRawFileDataCO(Address, OnCompletedCO);
        }

        private void OnCompletedCO(byte[] asset)
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

        private TaskAwaiter<byte[]> AwaiterObject;

        protected override TaskAwaiter<byte[]> CreateAsync()
        {
            AwaiterObject = AssetSystem.Proxy.LoadRawFileDataTask(Address).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        public ASHandleLoadRawFileData(string location) : base(location) { }
        public ASHandleLoadRawFileData(string location, Action<byte[]> onCompleted) : base(location, onCompleted) { }

        #endregion
    }
}