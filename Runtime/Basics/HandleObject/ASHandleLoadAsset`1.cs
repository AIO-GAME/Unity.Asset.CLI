using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Object = UnityEngine.Object;

namespace AIO
{
    internal partial class ASHandleLoadAsset<TObject>
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<TObject> Create(string location)
        {
            return AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<TObject> assetHandle
                ? assetHandle
                : new ASHandleLoadAsset<TObject>(location);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<TObject> Create(string location, Action<TObject> completed)
        {
            if (completed is null) return Create(location);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<TObject> assetHandle)
            {
                if (assetHandle.IsDone) completed.Invoke(assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new ASHandleLoadAsset<TObject>(location, completed);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal partial class ASHandleLoadAsset<TObject> : ASHandle<TObject>
    where TObject : Object
    {
        protected override void OnDispose()
        {
            _CO = null;
        }

        #region CO

        private IEnumerator _CO { get; set; }

        protected override IEnumerator CO
        {
            get
            {
                if (_CO is null) _CO = AssetSystem.Proxy.LoadAssetCO<TObject>(Address, OnCompletedCO);
                return _CO;
            }
        }

        private void OnCompletedCO(TObject asset)
        {
            Progress = 100;
            Result   = asset;
            IsDone   = true;
            InvokeOnCompleted();
        }

        /// <inheritdoc />
        protected override bool MoveNext()
        {
            return CO.MoveNext();
        }

        /// <inheritdoc />
        protected override void Reset()
        {
            Progress = 0;
            IsDone   = false;
            CO.Reset();
        }

        #endregion

        #region Task

        private void OnCompletedTaskGeneric()
        {
            Progress = 100;
            Result   = AwaiterGeneric.GetResult();
            IsDone   = true;
            InvokeOnCompleted();
        }

        private TaskAwaiter<TObject> AwaiterGeneric;

        protected override TaskAwaiter<TObject> GetAwaiterObject()
        {
            AwaiterGeneric = AssetSystem.Proxy.LoadAssetTask<TObject>(Address).GetAwaiter();
            AwaiterGeneric.OnCompleted(OnCompletedTaskGeneric);
            return AwaiterGeneric;
        }

        #endregion

        #region Constructor

        public ASHandleLoadAsset(string location)
            : base(location) { }

        public ASHandleLoadAsset(string location, Action<TObject> onCompleted)
            : base(location, onCompleted) { }

        #endregion
    }
}