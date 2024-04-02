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
        private IEnumerator _CO;
        private Transform   parent;

        protected override IEnumerator CO
        {
            get
            {
                if (_CO is null) _CO = AssetSystem.Proxy.InstGameObjectCO(Address, OnCompletedCO, parent);
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

        private void OnCompletedCO(GameObject asset)
        {
            Progress = 100;
            Result   = asset;
            IsDone   = true;
            InvokeOnCompleted();
            Dispose();
        }

        #endregion

        #region Task

        private void OnCompletedTaskObject()
        {
            Progress = 100;
            Result   = AwaiterObject.GetResult();
            IsDone   = true;
            InvokeOnCompleted();
            Dispose();
        }

        private TaskAwaiter<GameObject> AwaiterObject;

        protected override TaskAwaiter<GameObject> GetAwaiterObject()
        {
            AwaiterObject = AssetSystem.Proxy.InstGameObjectTask(Address, parent).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
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