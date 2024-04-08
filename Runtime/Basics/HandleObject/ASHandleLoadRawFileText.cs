using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AIO
{
    internal partial class LoaderHandleLoadRawFileText
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<string> Create(string location, Action<string> completed)
        {
            if (completed is null) return Create(location);
            var key = AssetSystem.SettingToLocalPath(location);
            if (AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandle<string> handles)
            {
                if (handles.IsDone) completed.Invoke(handles.Result);
                else handles.Completed += completed;
                return handles;
            }

            return new LoaderHandleLoadRawFileText(key, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<string> Create(string location)
        {
            var key = AssetSystem.SettingToLocalPath(location);
            return AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandle<string> handles
                ? handles
                : new LoaderHandleLoadRawFileText(key);
        }
    }

    internal partial class LoaderHandleLoadRawFileText : LoaderHandle<string>
    {
        #region Sync

        protected override void CreateSync()
        {
            Result = AssetSystem.Proxy.LoadRawFileTextSync(Address);
        }

        #endregion

        #region CO

        protected override IEnumerator CreateCoroutine()
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

        protected override TaskAwaiter<string> CreateAsync()
        {
            AwaiterObject = AssetSystem.Proxy.LoadRawFileTextTask(Address).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        private LoaderHandleLoadRawFileText(string location, Action<string> onCompleted) : this(location)
        {
            Completed += onCompleted;
        }

        #endregion

        private LoaderHandleLoadRawFileText(string location)
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