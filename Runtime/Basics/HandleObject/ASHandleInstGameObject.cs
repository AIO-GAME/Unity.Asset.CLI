using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AIO
{
    internal partial class LoaderHandleInstGameObject
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<GameObject> Create(string location, Type type, Transform transform = null)
        {
            return new LoaderHandleInstGameObject(location, type, transform);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<GameObject> Create(string location, Transform transform = null)
        {
            return new LoaderHandleInstGameObject(location, transform);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<GameObject> Create(string location, Type type, Action<GameObject> completed, Transform transform = null)
        {
            return completed is null ? Create(location, type) : new LoaderHandleInstGameObject(location, type, completed, transform);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<GameObject> Create(string location, Action<GameObject> completed, Transform transform = null)
        {
            return completed is null ? Create(location) : new LoaderHandleInstGameObject(location, completed, transform);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal partial class LoaderHandleInstGameObject : LoaderHandle<GameObject>
    {
        private Transform Parent { get; }

        #region Sync

        protected override void CreateSync()
        {
            Result = AssetSystem.Proxy.InstGameObject(Address, Parent);
        }

        #endregion

        #region CO

        protected override IEnumerator CreateCoroutine()
        {
            return AssetSystem.Proxy.InstGameObjectCO(Address, OnCompletedCO, Parent);
        }

        private void OnCompletedCO(GameObject asset)
        {
            Result = asset;
            InvokeOnCompleted();
        }

        #endregion

        #region Task

        private TaskAwaiter<GameObject> AwaiterObject;

        protected override TaskAwaiter<GameObject> CreateAsync()
        {
            AwaiterObject = AssetSystem.Proxy.InstGameObjectTask(Address, Parent).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        private void OnCompletedTaskObject()
        {
            Result = AwaiterObject.GetResult();
            InvokeOnCompleted();
        }

        #endregion

        #region Constructor

        private LoaderHandleInstGameObject(string location)
        {
            Address    = AssetSystem.SettingToLocalPath(location);
            IsValidate = AssetSystem.Proxy.CheckLocationValid(Address);
            IsDone     = !IsValidate;
            Progress   = 0;
        }

        private LoaderHandleInstGameObject(string location, Transform trans) : this(location)
        {
            Parent = trans;
        }

        private LoaderHandleInstGameObject(string location, Action<GameObject> onCompleted, Transform trans) : this(location)
        {
            Parent    =  trans;
            Completed += onCompleted;
        }

        private LoaderHandleInstGameObject(string location, Type type, Action<GameObject> onCompleted, Transform trans) : this(location)
        {
            Parent    =  trans;
            AssetType =  type;
            Completed += onCompleted;
        }

        private LoaderHandleInstGameObject(string location, Type type, Transform trans) : this(location)
        {
            Parent    = trans;
            AssetType = type;
        }

        #endregion
    }
}