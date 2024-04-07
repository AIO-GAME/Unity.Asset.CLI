using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Object = UnityEngine.Object;

namespace AIO
{
    internal partial class ASHandleLoadAsset
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<Object> Create(string location, Type type)
        {
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<Object> assetHandle)
                return assetHandle;
            return new ASHandleLoadAsset(location, type);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<Object> Create(string location)
        {
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<Object> assetHandle)
                return assetHandle;
            return new ASHandleLoadAsset(location);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<Object> Create(string location, Type type, Action<Object> completed)
        {
            if (completed is null) return Create(location, type);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<Object> assetHandle)
            {
                if (assetHandle.IsDone) completed.Invoke(assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new ASHandleLoadAsset(location, type, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<Object> Create(string location, Action<Object> completed)
        {
            if (completed is null) return Create(location);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<Object> assetHandle)
            {
                if (assetHandle.IsDone) completed.Invoke(assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new ASHandleLoadAsset(location, completed);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal partial class ASHandleLoadAsset : ASHandle<Object>
    {
        protected override void OnInvoke()
        {
            Result = AssetSystem.Proxy.LoadAssetSync(Address, AssetType);
        }

        #region CO

        protected override IEnumerator OnCreateCO()
        {
            return AssetSystem.Proxy.LoadAssetCO<Object>(Address, OnCompletedCO);
        }

        private void OnCompletedCO(Object asset)
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

        private TaskAwaiter<Object> AwaiterObject;

        protected override TaskAwaiter<Object> OnAwaiterObject()
        {
            AwaiterObject = AssetSystem.Proxy.LoadAssetTask(Address, AssetType).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        private ASHandleLoadAsset(string location)
            : base(location) { }

        private ASHandleLoadAsset(string location, Action<Object> onCompleted)
            : base(location, onCompleted) { }

        private ASHandleLoadAsset(string location, Type type, Action<Object> onCompleted)
            : base(location, type, onCompleted) { }

        private ASHandleLoadAsset(string location, Type type)
            : base(location, type) { }

        #endregion
    }
}