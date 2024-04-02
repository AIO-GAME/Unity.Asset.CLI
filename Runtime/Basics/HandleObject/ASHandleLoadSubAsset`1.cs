using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AIO
{
    internal partial class ASHandleLoadSubAsset<TObject> : ASHandleList<TObject>
    where TObject : Object
    {
        private IEnumerator _CO;

        protected override IEnumerator CO
        {
            get
            {
                if (_CO is null) _CO = AssetSystem.Proxy.LoadSubAssetsCO<TObject>(Address, OnCompletedCO);
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

        private void OnCompletedCO(TObject[] asset)
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

        private TaskAwaiter<TObject[]> AwaiterObject;

        protected override TaskAwaiter<TObject[]> GetAwaiterObject()
        {
            AwaiterObject = AssetSystem.Proxy.LoadSubAssetsTask<TObject>(Address).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        /// <inheritdoc />
        private ASHandleLoadSubAsset(string location)
            : base(location) { }

        /// <inheritdoc />
        private ASHandleLoadSubAsset(string location, Action<TObject[]> onCompleted)
            : base(location, onCompleted) { }

        #endregion
    }

    internal partial class ASHandleLoadSubAsset<TObject>
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandleList<TObject> Create(string location)
        {
            return AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandleList<TObject> assetHandle
                ? assetHandle
                : new ASHandleLoadSubAsset<TObject>(location);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandleList<TObject> Create(string location, Action<TObject[]> completed)
        {
            if (completed is null) return Create(location);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandleList<TObject> assetHandle)
            {
                if (assetHandle.IsDone) completed.Invoke(assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new ASHandleLoadSubAsset<TObject>(location, completed);
        }
    }

}