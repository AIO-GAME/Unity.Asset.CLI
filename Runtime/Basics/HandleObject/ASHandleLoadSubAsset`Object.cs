using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AIO
{
    internal partial class ASHandleLoadSubAsset : ASHandleList<Object>
    {
        private IEnumerator _CO;

        protected override IEnumerator CO
        {
            get
            {
                if (_CO is null) _CO = AssetSystem.Proxy.LoadSubAssetsCO(Address, AssetType, OnCompletedCO);
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

        private void OnCompletedCO(Object[] asset)
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

        private TaskAwaiter<Object[]> AwaiterObject;

        protected override TaskAwaiter<Object[]> GetAwaiterObject()
        {
            AwaiterObject = AssetSystem.Proxy.LoadSubAssetsTask(Address, AssetType).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        private ASHandleLoadSubAsset(string location)
            : base(location) { }

        private ASHandleLoadSubAsset(string location, Action<Object[]> onCompleted)
            : base(location, onCompleted) { }

        private ASHandleLoadSubAsset(string location, Type type, Action<Object[]> onCompleted)
            : base(location, type, onCompleted) { }

        private ASHandleLoadSubAsset(string location, Type type)
            : base(location, type) { }

        #endregion
    }

    internal partial class ASHandleLoadSubAsset
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandleList<Object> Create(string location, Type type)
        {
            return AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandleList<Object> assetHandle
                ? assetHandle
                : new ASHandleLoadSubAsset(location, type);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandleList<Object> Create(string location)
        {
            return AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandleList<Object> assetHandle
                ? assetHandle
                : new ASHandleLoadSubAsset(location);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandleList<Object> Create(string location, Action<Object[]> completed)
        {
            if (completed is null) return Create(location);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandleList<Object> assetHandle)
            {
                if (assetHandle.IsDone) completed.Invoke(assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new ASHandleLoadSubAsset(location, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandleList<Object> Create(string location, Type type, Action<Object[]> cb)
        {
            if (cb is null) return Create(location);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandleList<Object> assetHandle)
            {
                if (assetHandle.IsDone) cb.Invoke(assetHandle.Result);
                else assetHandle.Completed += cb;
                return assetHandle;
            }

            return new ASHandleLoadSubAsset(location, type, cb);
        }
    }

}