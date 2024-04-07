using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AIO
{
    internal partial class ASHandleInstGameObject
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<GameObject> Create(string location, Type type, Transform transform = null)
        {
            return new ASHandleInstGameObject(location, type, transform);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<GameObject> Create(string location, Transform transform = null)
        {
            return new ASHandleInstGameObject(location, transform);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<GameObject> Create(string location, Type type, Action<GameObject> completed, Transform transform = null)
        {
            if (completed is null) return Create(location, type);
            return new ASHandleInstGameObject(location, type, completed, transform);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static AssetSystem.IHandle<GameObject> Create(string location, Action<GameObject> completed, Transform transform = null)
        {
            if (completed is null) return Create(location);
            return new ASHandleInstGameObject(location, completed, transform);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal partial class ASHandleInstGameObject : ASHandle<GameObject>
    {
        private Transform parent;

        #region Sync

        protected override void OnInvoke()
        {
            Result = AssetSystem.Proxy.InstGameObject(Address, parent);
        }

        #endregion

        #region CO

        protected override IEnumerator OnCreateCO()
        {
            return AssetSystem.Proxy.InstGameObjectCO(Address, OnCompletedCO, parent);
        }

        private void OnCompletedCO(GameObject asset)
        {
            Result = asset;
            InvokeOnCompleted();
            Dispose();
        }

        #endregion

        #region Task

        private TaskAwaiter<GameObject> AwaiterObject;

        protected override TaskAwaiter<GameObject> OnAwaiterObject()
        {
            AwaiterObject = AssetSystem.Proxy.InstGameObjectTask(Address, parent).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        private void OnCompletedTaskObject()
        {
            Result = AwaiterObject.GetResult();
            InvokeOnCompleted();
            Dispose();
        }

        #endregion

        #region Constructor

        private ASHandleInstGameObject(string location, Transform trans)
            : base(location)
        {
            parent = trans;
        }

        private ASHandleInstGameObject(string location, Action<GameObject> onCompleted, Transform trans)
            : base(location, onCompleted)
        {
            parent = trans;
        }

        private ASHandleInstGameObject(string location, Type type, Action<GameObject> onCompleted, Transform trans)
            : base(location, type, onCompleted)
        {
            parent = trans;
        }

        private ASHandleInstGameObject(string location, Type type, Transform trans)
            : base(location, type)
        {
            parent = trans;
        }

        #endregion
    }
}