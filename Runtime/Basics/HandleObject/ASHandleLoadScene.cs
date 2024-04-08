using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal partial class LoaderHandleLoadScene : LoaderHandle<Scene>
    {
        private readonly LoadSceneMode _SceneMode;
        private readonly bool          _SuspendLoad;
        private readonly int           _Priority;

        #region Sync

        protected override void CreateSync()
        {
            var task = AssetSystem.Proxy.LoadSceneTask(Address, _SceneMode, _SuspendLoad, _Priority);
            task.RunSynchronously();
            Result = task.Result;
        }

        #endregion

        #region CO

        protected override IEnumerator CreateCoroutine()
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

        protected override TaskAwaiter<Scene> CreateAsync()
        {
            AwaiterObject = AssetSystem.Proxy.LoadSceneTask(Address, _SceneMode, _SuspendLoad, _Priority).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        /// <inheritdoc />
        private LoaderHandleLoadScene(
            string        location,
            LoadSceneMode sceneMode,
            bool          suspendLoad,
            int           priority,
            Action<Scene> onCompleted
        ) : this(location)
        {
            _SceneMode   =  sceneMode;
            _SuspendLoad =  suspendLoad;
            _Priority    =  priority;
            Completed    += onCompleted;
        }

        /// <inheritdoc />
        private LoaderHandleLoadScene(
            string        location,
            LoadSceneMode sceneMode,
            bool          suspendLoad,
            int           priority
        ) : this(location)
        {
            _SceneMode   = sceneMode;
            _SuspendLoad = suspendLoad;
            _Priority    = priority;
        }

        #endregion

        private LoaderHandleLoadScene(string location)
        {
            Address    = location;
            IsValidate = AssetSystem.CheckLocationValid(Address);
            if (!IsValidate) AssetSystem.LogWarningFormat("资源地址无效: {0}", location);
            IsDone   = !IsValidate;
            Progress = 0;
        }

        protected override void OnDispose()
        {
            if (IsValidate)
            {
                Result     = default;
                IsValidate = false;
            }

            Address = null;
        }
    }

    internal partial class LoaderHandleLoadScene
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<Scene> Create(
            string        location,
            LoadSceneMode sceneMode,
            bool          suspendLoad,
            int           priority,
            Action<Scene> completed)
        {
            var key = AssetSystem.SettingToLocalPath(location);
            return completed is null
                ? new LoaderHandleLoadScene(key, sceneMode, suspendLoad, priority)
                : new LoaderHandleLoadScene(key, sceneMode, suspendLoad, priority, completed);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<Scene> Create(
            string        location,
            LoadSceneMode sceneMode,
            bool          suspendLoad,
            int           priority)
        {
            return new LoaderHandleLoadScene(AssetSystem.SettingToLocalPath(location), sceneMode, suspendLoad, priority);
        }
    }
}