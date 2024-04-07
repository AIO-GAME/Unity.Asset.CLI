using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal partial class ASHandleLoadScene : ASHandle<Scene>
    {
        private readonly LoadSceneMode _SceneMode;
        private readonly bool          _SuspendLoad;
        private readonly int           _Priority;


        #region Sync

        protected override void OnInvoke()
        {
            var task = AssetSystem.Proxy.LoadSceneTask(Address, _SceneMode, _SuspendLoad, _Priority);
            while (!task.IsCompleted) task.Wait();
            Result = task.Result;
        }

        #endregion

        #region CO

        protected override IEnumerator OnCreateCO()
        {
            return AssetSystem.Proxy.LoadSceneCO(Address, OnCompletedCO, _SceneMode, _SuspendLoad, _Priority);
        }

        private void OnCompletedCO(Scene asset)
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

        private TaskAwaiter<Scene> AwaiterObject;

        protected override TaskAwaiter<Scene> OnAwaiterObject()
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