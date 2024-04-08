using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Object = UnityEngine.Object;

namespace AIO
{
    partial class LoaderHandleLoadAsset<TObject>
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<TObject> Create(string location)
        {
            var key = AssetSystem.SettingToLocalPath(location);
            return AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandle<TObject> assetHandle
                ? assetHandle
                : new LoaderHandleLoadAsset<TObject>(key);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<TObject> Create(string location, Action<TObject> completed)
        {
            if (completed is null) return Create(location);
            var key = AssetSystem.SettingToLocalPath(location);
            if (AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandle<TObject> assetHandle)
            {
                if (!assetHandle.IsValidate) return assetHandle;
                if (assetHandle.IsDone) Runner.Update(completed, assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new LoaderHandleLoadAsset<TObject>(key, completed);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal partial class LoaderHandleLoadAsset<TObject> : LoaderHandle<TObject>
    where TObject : Object
    {
        #region Sync

        protected override void CreateSync()
        {
            Result = AssetSystem.Proxy.LoadAssetSync<TObject>(Address);
        }

        #endregion

        #region CO

        protected override IEnumerator CreateCoroutine()
        {
            return AssetSystem.Proxy.LoadAssetCO<TObject>(Address, OnCompletedCO);
        }

        private void OnCompletedCO(TObject asset)
        {
            Result = asset;
            InvokeOnCompleted();
        }

        #endregion

        #region Task

        private void OnCompletedTaskGeneric()
        {
            Result = AwaiterGeneric.GetResult();
            InvokeOnCompleted();
        }

        private TaskAwaiter<TObject> AwaiterGeneric;

        protected override TaskAwaiter<TObject> CreateAsync()
        {
            AwaiterGeneric = AssetSystem.Proxy.LoadAssetTask<TObject>(Address).GetAwaiter();
            AwaiterGeneric.OnCompleted(OnCompletedTaskGeneric);
            return AwaiterGeneric;
        }

        #endregion

        #region Constructor

        private LoaderHandleLoadAsset(string location, Action<TObject> onCompleted) : this(location)
        {
            Completed += onCompleted;
        }

        #endregion

        private LoaderHandleLoadAsset(string location)
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
}