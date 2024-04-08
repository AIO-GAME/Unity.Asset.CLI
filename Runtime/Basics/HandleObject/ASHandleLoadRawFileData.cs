using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AIO
{
    internal partial class LoaderHandleLoadRawFileData
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<byte[]> Create(string location, Action<byte[]> completed)
        {
            if (completed is null) return Create(location);
            var key = AssetSystem.SettingToLocalPath(location);
            if (AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandle<byte[]> handles)
            {
                if (handles.IsDone) completed.Invoke(handles.Result);
                else handles.Completed += completed;
                return handles;
            }

            return new LoaderHandleLoadRawFileData(key, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<byte[]> Create(string location)
        {
            var key = AssetSystem.SettingToLocalPath(location);
            return AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandle<byte[]> handles
                ? handles
                : new LoaderHandleLoadRawFileData(key);
        }
    }

    internal partial class LoaderHandleLoadRawFileData : LoaderHandle<byte[]>
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

        private LoaderHandleLoadRawFileData(string location, Action<byte[]> onCompleted) : this(location)
        {
            Completed += onCompleted;
        }

        #endregion

        private LoaderHandleLoadRawFileData(string location)
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