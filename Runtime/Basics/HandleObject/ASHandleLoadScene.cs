using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal partial class ASHandleLoadScene : ASHandle<Scene>
    {
        private readonly LoadSceneMode _SceneMode;
        private readonly bool          _SuspendLoad;
        private readonly int           _Priority;
        private          IEnumerator   _CO;

        protected override IEnumerator CO
        {
            get
            {
                if (_CO is null)
                    _CO = AssetSystem.Proxy.LoadSceneCO(Address, OnCompletedCO, _SceneMode, _SuspendLoad, _Priority);
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

        private void OnCompletedCO(Scene asset)
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

        private TaskAwaiter<Scene> AwaiterObject;

        protected override TaskAwaiter<Scene> GetAwaiterObject()
        {
            AwaiterObject = AssetSystem.Proxy.LoadSceneTask(Address, _SceneMode, _SuspendLoad, _Priority).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        /// <inheritdoc />
        private ASHandleLoadScene(
            string        location,
            LoadSceneMode sceneMode,
            bool          suspendLoad,
            int           priority,
            Action<Scene> onCompleted
        ) : base(location, onCompleted)
        {
            _SceneMode   = sceneMode;
            _SuspendLoad = suspendLoad;
            _Priority    = priority;
        }

        /// <inheritdoc />
        private ASHandleLoadScene(
            string        location,
            LoadSceneMode sceneMode,
            bool          suspendLoad,
            int           priority
        ) : base(location)
        {
            _SceneMode   = sceneMode;
            _SuspendLoad = suspendLoad;
            _Priority    = priority;
        }

        #endregion
    }

    internal partial class ASHandleLoadScene
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<Scene> Create(
            string        location,
            LoadSceneMode sceneMode,
            bool          suspendLoad,
            int           priority,
            Action<Scene> completed)
        {
            if (completed is null) return Create(location, sceneMode, suspendLoad, priority);
            if (AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<Scene> assetHandle)
            {
                if (assetHandle.IsDone) completed.Invoke(assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new ASHandleLoadScene(location, sceneMode, suspendLoad, priority, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<Scene> Create(
            string        location,
            LoadSceneMode sceneMode,
            bool          suspendLoad,
            int           priority)
        {
            return AssetSystem.HandleDic.TryGetValue(location, out var handle) && handle is AssetSystem.IHandle<Scene> assetHandle
                ? assetHandle
                : new ASHandleLoadScene(location, sceneMode, suspendLoad, priority);
        }
    }
}