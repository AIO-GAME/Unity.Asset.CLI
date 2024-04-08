using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AIO
{
    internal partial class LoaderHandleLoadSubAsset : LoaderHandleList<Object>
    {
        #region Sync

        protected override void CreateSync()
        {
            Result = AssetSystem.Proxy.LoadSubAssetsSync(Address, AssetType);
        }

        #endregion

        #region CO

        protected override IEnumerator CreateCoroutine()
        {
            return AssetSystem.Proxy.LoadSubAssetsCO(Address, AssetType, OnCompletedCO);
        }

        private void OnCompletedCO(Object[] asset)
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

        private TaskAwaiter<Object[]> AwaiterObject;

        protected override TaskAwaiter<Object[]> CreateAsync()
        {
            AwaiterObject = AssetSystem.Proxy.LoadSubAssetsTask(Address, AssetType).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        private LoaderHandleLoadSubAsset(string location, Action<Object[]> onCompleted) : this(location)
        {
            AssetType =  typeof(Object);
            Completed += onCompleted;
        }

        private LoaderHandleLoadSubAsset(string location, Type type, Action<Object[]> onCompleted) : this(location)
        {
            AssetType =  type;
            Completed += onCompleted;
        }

        private LoaderHandleLoadSubAsset(string location, Type type) : this(location)
        {
            AssetType = type;
        }

        #endregion

        private LoaderHandleLoadSubAsset(string location)
        {
            Address    = location;
            IsValidate = AssetSystem.CheckLocationValid(Address);
            if (IsValidate)
            {
                if (AssetSystem.ReferenceHandleCount.Increment(Address) == 1)
                {
                    AssetSystem.HandleDic[Address] = this;
                }
            }
            else AssetSystem.LogWarningFormat("资源地址无效: {0}", location);

            IsDone   = !IsValidate;
            Progress = 0;
        }

        protected override void OnDispose()
        {
            if (IsValidate)
            {
                var count = AssetSystem.ReferenceHandleCount.Decrement(Address);
                if (count <= 0)
                {
                    AssetSystem.HandleDic.Remove(Address);
                    AssetSystem.UnloadAsset(Address);
                    Result = default;
                }

                IsValidate = false;
            }

            Address = null;
        }
    }

    internal partial class LoaderHandleLoadSubAsset
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandleList<Object> Create(string location, Type type)
        {
            var key = AssetSystem.SettingToLocalPath(location);
            return AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandleList<Object> assetHandle
                ? assetHandle
                : new LoaderHandleLoadSubAsset(key, type);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandleList<Object> Create(string location)
        {
            var key = AssetSystem.SettingToLocalPath(location);
            return AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandleList<Object> assetHandle
                ? assetHandle
                : new LoaderHandleLoadSubAsset(key);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandleList<Object> Create(string location, Action<Object[]> completed)
        {
            if (completed is null) return Create(location);
            var key = AssetSystem.SettingToLocalPath(location);
            if (AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandleList<Object> assetHandle)
            {
                if (assetHandle.IsDone) completed.Invoke(assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new LoaderHandleLoadSubAsset(key, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandleList<Object> Create(string location, Type type, Action<Object[]> cb)
        {
            if (cb is null) return Create(location);
            var key = AssetSystem.SettingToLocalPath(location);
            if (AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandleList<Object> assetHandle)
            {
                if (assetHandle.IsDone) cb.Invoke(assetHandle.Result);
                else assetHandle.Completed += cb;
                return assetHandle;
            }

            return new LoaderHandleLoadSubAsset(key, type, cb);
        }
    }
}