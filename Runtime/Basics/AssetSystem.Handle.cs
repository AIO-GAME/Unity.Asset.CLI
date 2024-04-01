using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Object = UnityEngine.Object;

namespace AIO
{
    partial class AssetSystem
    {
        private static readonly Dictionary<string, int> ReferenceHandleCount = new Dictionary<string, int>();

        private static readonly Dictionary<string, IHandle> HandleDic = new Dictionary<string, IHandle>();

        [DebuggerNonUserCode, DebuggerHidden]
        private static IHandle CreateLoadAssetHandle(string location, Type type, Action<Object> cb)
        {
            if (HandleDic.TryGetValue(location, out var handle))
            {
                cb?.Invoke(handle.Result);
                return handle;
            }

            {
                handle = new LoadAssetHandle(location, type, cb);
                HandleDic.Add(location, handle);
                return handle;
            }
        }

        [DebuggerNonUserCode, DebuggerHidden]
        private static IHandle CreateLoadAssetHandle(string location, Type type)
        {
            if (HandleDic.TryGetValue(location, out var handle)) return handle;
            handle = new LoadAssetHandle(location, type);
            HandleDic.Add(location, handle);
            return handle;
        }

        [DebuggerNonUserCode, DebuggerHidden]
        private static IHandle<TObject> CreateLoadAssetHandle<TObject>(string location, Action<TObject> cb)
            where TObject : Object
        {
            if (HandleDic.TryGetValue(location, out var handle))
            {
                if (handle is LoadAssetHandle<TObject> loadAssetHandle)
                {
                    cb?.Invoke(loadAssetHandle.Result);
                    return loadAssetHandle;
                }
            }

            {
                var loadAssetHandle = new LoadAssetHandle<TObject>(location, cb);
                HandleDic.Add(location, loadAssetHandle);
                return loadAssetHandle;
            }
        }

        [DebuggerNonUserCode, DebuggerHidden]
        private static IHandle<TObject> CreateLoadAssetHandle<TObject>(string location)
            where TObject : Object
        {
            if (HandleDic.TryGetValue(location, out var handle))
            {
                if (handle is LoadAssetHandle<TObject> loadAssetHandle)
                {
                    return loadAssetHandle;
                }
            }

            {
                var loadAssetHandle = new LoadAssetHandle<TObject>(location);
                HandleDic.Add(location, loadAssetHandle);
                return loadAssetHandle;
            }
        }

        [DebuggerNonUserCode, DebuggerHidden]
        private static IHandle CreateLoadAssetHandle(string location, Action<Object> cb)
        {
            if (HandleDic.TryGetValue(location, out var handle))
            {
                cb?.Invoke(handle.Result);
                return handle;
            }

            {
                var loadAssetHandle = new LoadAssetHandle(location, cb);
                HandleDic.Add(location, loadAssetHandle);
                return loadAssetHandle;
            }
        }

        [DebuggerNonUserCode, DebuggerHidden]
        private static IHandle CreateLoadAssetHandle(string location)
        {
            if (HandleDic.TryGetValue(location, out var handle))
            {
                return handle;
            }

            {
                var loadAssetHandle = new LoadAssetHandle(location);
                HandleDic.Add(location, loadAssetHandle);
                return loadAssetHandle;
            }
        }
    }

    partial class AssetSystem
    {
        [StructLayout(LayoutKind.Auto)]
        internal class LoadAssetHandleArray : IHandleArray
        {
            public byte Progress { get; private set; }

            public bool IsDone { get; private set; }

            public string Address { get; private set; }

            public Type AssetType { get; }

            public Object[] Result { get; private set; }

            Object[] IHandleArray.Result => Result;

            private Action<Object[]> OnCompleted { get; set; }

            event Action<Object[]> IHandleArray.OnCompleted
            {
                add => OnCompleted += value;
                remove => OnCompleted -= value;
            }

            private LoadAssetHandleArray()
            {
                IsDone   = false;
                Progress = 0;
                Result   = null;
                _CO      = null;
            }

            public LoadAssetHandleArray(string location, Action<Object[]> onCompleted) : this()
            {
                Address     = SettingToLocalPath(location);
                OnCompleted = onCompleted;
                AssetType   = typeof(Object);
                ReferenceHandleCount.Increment(Address);
            }

            public LoadAssetHandleArray(string location, Type type, Action<Object[]> onCompleted) : this()
            {
                Address     = SettingToLocalPath(location);
                OnCompleted = onCompleted;
                AssetType   = type;
                ReferenceHandleCount.Increment(Address);
            }

            public LoadAssetHandleArray(string location, Type type) : this()
            {
                Address   = SettingToLocalPath(location);
                AssetType = type;
                ReferenceHandleCount.Increment(Address);
            }

            public LoadAssetHandleArray(string location) : this()
            {
                Address   = SettingToLocalPath(location);
                AssetType = typeof(Object);
                ReferenceHandleCount.Increment(Address);
            }

            #region CO

            private IEnumerator CO
            {
                get
                {
                    if (_CO is null) _CO = Proxy.LoadSubAssetsCO<Object>(Address, OnCompletedCO);
                    return _CO;
                }
            }


            private IEnumerator _CO;

            private void OnCompletedCO(Object[] asset)
            {
                Progress = 100;
                Result   = asset;
                IsDone   = true;
                if (OnCompleted is null) return;
                OnCompleted.Invoke(Result);
                OnCompleted = null;
            }

            bool IEnumerator.MoveNext()
            {
                return CO.MoveNext();
            }

            void IEnumerator.Reset()
            {
                Progress = 0;
                IsDone   = false;
                CO.Reset();
            }

            object IEnumerator.Current => CO.Current;

            #endregion

            #region Task

            private void OnCompletedTaskObject()
            {
                Progress = 100;
                Result   = AwaiterObject.GetResult();
                IsDone   = true;
                if (OnCompleted is null) return;
                OnCompleted.Invoke(Result);
                OnCompleted = null;
            }

            private TaskAwaiter<Object[]> AwaiterObject;

            TaskAwaiter<Object[]> IHandleArray.GetAwaiter()
            {
                AwaiterObject = Proxy.LoadSubAssetsTask(Address, AssetType).GetAwaiter();
                AwaiterObject.OnCompleted(OnCompletedTaskObject);
                return AwaiterObject;
            }

            #endregion

            public void Dispose()
            {
                var count = ReferenceHandleCount.Decrement(Address);
                if (count <= 0)
                {
                    Result = null;
                    UnloadAsset(Address);
                }

                OnCompleted = null;
                _CO         = null;
                Address     = null;
            }
        }

        #region Nested type: LoadAssetHandle

        [StructLayout(LayoutKind.Auto)]
        internal class LoadAssetHandle<T> : IHandle<T> where T : Object
        {
            private LoadAssetHandle()
            {
                IsDone   = false;
                Progress = 0;
                Result   = null;
                _CO      = null;
            }

            public LoadAssetHandle(string location, Action<T> onCompleted) : this()
            {
                Address     = SettingToLocalPath(location);
                OnCompleted = onCompleted;
                AssetType   = typeof(T);
                ReferenceHandleCount.Increment(Address);
            }

            public LoadAssetHandle(string location, Type type, Action<T> onCompleted) : this()
            {
                Address     = SettingToLocalPath(location);
                OnCompleted = onCompleted;
                AssetType   = type;
                ReferenceHandleCount.Increment(Address);
            }

            public LoadAssetHandle(string location, Type type) : this()
            {
                Address   = SettingToLocalPath(location);
                AssetType = type;
                ReferenceHandleCount.Increment(Address);
            }

            public LoadAssetHandle(string location) : this()
            {
                Address   = SettingToLocalPath(location);
                AssetType = typeof(T);
                ReferenceHandleCount.Increment(Address);
            }

            public T Result { get; private set; }

            public Action<T> OnCompleted { get; set; }

            #region IHandle<T> Members

            public byte Progress { get; private set; }

            public bool IsDone { get; private set; }

            public string Address { get; private set; }

            public Type AssetType { get; }

            Object IHandle.Result => Result;

            T IHandle<T>.Result => Result;

            event Action<Object> IHandle.OnCompleted
            {
                add => OnCompleted += value;
                remove => OnCompleted -= value;
            }

            event Action<T> IHandle<T>.OnCompleted
            {
                add => OnCompleted += value;
                remove => OnCompleted -= value;
            }

            public void Dispose()
            {
                var count = ReferenceHandleCount.Decrement(Address);
                if (count <= 0)
                {
                    Result = null;
                    UnloadAsset(Address);
                }

                OnCompleted = null;
                _CO         = null;
                Address     = null;
            }

            #endregion

            #region CO

            private IEnumerator CO
            {
                get
                {
                    if (_CO is null) _CO = Proxy.LoadAssetCO<T>(Address, OnCompletedCO);
                    return _CO;
                }
            }


            private IEnumerator _CO;

            private void OnCompletedCO(T asset)
            {
                Progress = 100;
                Result   = asset;
                IsDone   = true;
                if (OnCompleted == null) return;
                OnCompleted.Invoke(Result);
                OnCompleted = null;
            }

            bool IEnumerator.MoveNext()
            {
                return CO.MoveNext();
            }

            void IEnumerator.Reset()
            {
                Progress = 0;
                IsDone   = false;
                CO.Reset();
            }

            object IEnumerator.Current => CO.Current;

            #endregion

            #region Task

            private void OnCompletedTaskGeneric()
            {
                Progress = 100;
                Result   = AwaiterGeneric.GetResult();
                IsDone   = true;
                if (OnCompleted == null) return;
                OnCompleted.Invoke(Result);
                OnCompleted = null;
            }

            private void OnCompletedTaskObject()
            {
                Progress = 100;
                Result   = AwaiterObject.GetResult() as T;
                IsDone   = true;
                if (OnCompleted == null) return;
                OnCompleted.Invoke(Result);
                OnCompleted = null;
            }

            private TaskAwaiter<T>      AwaiterGeneric;
            private TaskAwaiter<Object> AwaiterObject;

            TaskAwaiter<Object> IHandle.GetAwaiter()
            {
                AwaiterObject = Proxy.LoadAssetTask(Address, AssetType).GetAwaiter();
                AwaiterObject.OnCompleted(OnCompletedTaskObject);
                return AwaiterObject;
            }

            TaskAwaiter<T> IHandle<T>.GetAwaiter()
            {
                AwaiterGeneric = Proxy.LoadAssetTask<T>(Address).GetAwaiter();
                AwaiterGeneric.OnCompleted(OnCompletedTaskGeneric);
                return AwaiterGeneric;
            }

            #endregion
        }

        [StructLayout(LayoutKind.Auto)]
        internal class LoadAssetHandle : IHandle
        {
            public Object Result { get; private set; }

            private LoadAssetHandle()
            {
                IsDone   = false;
                Progress = 0;
                Result   = null;
                _CO      = null;
            }

            public LoadAssetHandle(string location, Action<Object> onCompleted) : this()
            {
                Address   = SettingToLocalPath(location);
                AssetType = typeof(Object);
                ReferenceHandleCount.Increment(Address);
                OnCompleted = onCompleted;
            }

            public LoadAssetHandle(string location, Type type, Action<Object> onCompleted) : this()
            {
                Address   = SettingToLocalPath(location);
                AssetType = type;
                ReferenceHandleCount.Increment(Address);
                OnCompleted = onCompleted;
            }

            public LoadAssetHandle(string location, Type type) : this()
            {
                Address   = SettingToLocalPath(location);
                AssetType = type;
                ReferenceHandleCount.Increment(Address);
            }

            public LoadAssetHandle(string location) : this()
            {
                Address   = SettingToLocalPath(location);
                AssetType = typeof(Object);
                ReferenceHandleCount.Increment(Address);
            }

            private Action<Object> OnCompleted { get; set; }

            #region IHandle Members

            public byte Progress { get; private set; }

            public bool IsDone { get; private set; }

            public string Address { get; private set; }

            public Type AssetType { get; }

            Object IHandle.Result => Result;

            event Action<Object> IHandle.OnCompleted
            {
                add => OnCompleted += value;
                remove => OnCompleted -= value;
            }

            public void Dispose()
            {
                var count = ReferenceHandleCount.Decrement(Address);
                if (count <= 0)
                {
                    Result = null;
                    UnloadAsset(Address);
                }

                OnCompleted = null;
                _CO         = null;
                Address     = null;
            }

            #endregion

            #region CO

            private IEnumerator CO
            {
                get
                {
                    if (_CO is null) _CO = Proxy.LoadAssetCO<Object>(Address, OnCompletedCO);
                    return _CO;
                }
            }


            private IEnumerator _CO;

            private void OnCompletedCO(Object asset)
            {
                Progress = 100;
                Result   = asset;
                IsDone   = true;
                if (OnCompleted == null) return;
                OnCompleted?.Invoke(Result);
                OnCompleted = null;
            }

            bool IEnumerator.MoveNext()
            {
                return CO.MoveNext();
            }

            void IEnumerator.Reset()
            {
                Progress = 0;
                IsDone   = false;
                CO.Reset();
            }

            object IEnumerator.Current => CO.Current;

            #endregion

            #region Task

            private void OnCompletedTaskObject()
            {
                Progress = 100;
                Result   = AwaiterObject.GetResult();
                IsDone   = true;
                if (OnCompleted == null) return;
                OnCompleted?.Invoke(Result);
                OnCompleted = null;
            }

            private TaskAwaiter<Object> AwaiterObject;

            TaskAwaiter<Object> IHandle.GetAwaiter() => GetAwaiter();

            public TaskAwaiter<Object> GetAwaiter()
            {
                AwaiterObject = Proxy.LoadAssetTask(Address, AssetType).GetAwaiter();
                AwaiterObject.OnCompleted(OnCompletedTaskObject);
                return AwaiterObject;
            }

            #endregion
        }

        #endregion
    }
}