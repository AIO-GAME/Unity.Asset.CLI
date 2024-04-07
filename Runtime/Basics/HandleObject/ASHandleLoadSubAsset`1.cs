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
        #region Sync

        protected override void CreateSync()
        {
            Result = AssetSystem.Proxy.LoadSubAssetsSync<TObject>(Address);
        }

        #endregion

        #region CO

        protected override IEnumerator CreateCoroutine()
        {
            return AssetSystem.Proxy.LoadSubAssetsCO<TObject>(Address, OnCompletedCO);
        }

        private void OnCompletedCO(TObject[] asset)
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

        private TaskAwaiter<TObject[]> AwaiterObject;

        protected override TaskAwaiter<TObject[]> CreateAsync()
        {
            AwaiterObject = AssetSystem.Proxy.LoadSubAssetsTask<TObject>(Address).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        private ASHandleLoadSubAsset(string location) : base(location) { }
        private ASHandleLoadSubAsset(string location, Action<TObject[]> onCompleted) : base(location, onCompleted) { }

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