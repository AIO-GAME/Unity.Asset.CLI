using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal abstract class ASHandle<TObject> : AssetSystem.IHandle<TObject>, AssetSystem.IHandle
    {
        public byte                  Progress   { get; protected set; }
        public bool                  IsDone     { get; protected set; }
        public string                Address    { get; protected set; }
        public Type                  AssetType  { get; protected set; }
        public TObject               Result     { get; protected set; }
        public bool                  IsValidate { get; protected set; }
        public string                Guid       { get; private set; }
        public bool                  IsRunning  => !IsDone;
        public event Action<TObject> Completed;

        protected abstract IEnumerator          CO { get; }
        protected abstract TaskAwaiter<TObject> GetAwaiterObject();
        protected abstract void                 Reset();
        protected abstract void                 OnDispose();

        public TaskAwaiter<TObject> GetAwaiter()
        {
            return IsValidate ? GetAwaiterObject() : Task.FromResult<TObject>(default).GetAwaiter();
        }

        public void Dispose()
        {
            if (IsValidate)
            {
                var count = AssetSystem.ReferenceHandleCount.Decrement(Guid);
                if (count <= 0)
                {
                    AssetSystem.HandleDic.Remove(Guid);
                    AssetSystem.UnloadAsset(Address);
                    Result = default;
                }
            }

            Guid      = null;
            Completed = null;
            Address   = null;
            OnDispose();
        }

        protected virtual bool MoveNext()
        {
            return CO.MoveNext();
        }

        protected virtual void InvokeOnCompleted()
        {
            if (Completed == null) return;
            Completed.Invoke(Result);
            Completed = null;
        }

        #region Constructor

        protected ASHandle() { }

        protected ASHandle(string location)
        {
            Address    = AssetSystem.SettingToLocalPath(location);
            IsValidate = AssetSystem.CheckLocationValid(Address);
            Guid       = string.Concat(Address, GetType().FullName);
            if (IsValidate)
            {
                if (AssetSystem.ReferenceHandleCount.Increment(Guid) == 1)
                {
                    AssetSystem.HandleDic[Guid] = this;
                }
            }
            else AssetSystem.LogWarningFormat("资源地址无效: {0}", location);

            IsDone   = false;
            Progress = 0;
        }

        /// <inheritdoc />
        protected ASHandle(string location, Action<TObject> onCompleted) : this(location)
        {
            AssetType = typeof(TObject);
            Completed = onCompleted;
        }

        /// <inheritdoc />
        protected ASHandle(string location, Type type, Action<TObject> onCompleted) : this(location)
        {
            AssetType = type;
            Completed = onCompleted;
        }

        protected ASHandle(string location, Type type) : this(location)
        {
            AssetType = type;
        }

        #endregion

        #region operator implicit

        public static implicit operator TObject(ASHandle<TObject> asHandle) => asHandle.Result;
        public static implicit operator Type(ASHandle<TObject>    asHandle) => asHandle.AssetType;

        public static implicit operator TaskAwaiter<TObject>(ASHandle<TObject> asHandle)
        {
            return asHandle.IsValidate ? asHandle.GetAwaiterObject() : Task.FromResult<TObject>(default).GetAwaiter();
        }

        #endregion

        #region AssetSystem.IHandle<TObject>

        /// <inheritdoc />
        event Action<TObject> AssetSystem.IHandle<TObject>.Completed
        {
            add => Completed += value;
            remove => Completed -= value;
        }

        /// <inheritdoc />
        TaskAwaiter<TObject> AssetSystem.IHandle<TObject>.GetAwaiter() => GetAwaiter();

        /// <inheritdoc />
        TObject AssetSystem.IHandle<TObject>.Result => Result;

        #endregion

        #region AssetSystem.IHandleBase

        /// <inheritdoc />
        string AssetSystem.IHandleBase.Address => Address;

        /// <inheritdoc />
        byte AssetSystem.IHandleBase.Progress => Progress;

        /// <inheritdoc />
        bool AssetSystem.IHandleBase.IsDone => IsDone;

        /// <inheritdoc />
        bool AssetSystem.IHandleBase.IsValidate => IsValidate;

        /// <inheritdoc />
        bool AssetSystem.IHandleBase.IsRunning => IsRunning;

        /// <inheritdoc />
        Type AssetSystem.IHandleBase.AssetType => AssetType;

        #endregion

        #region IDisposable

        /// <inheritdoc />
        void IDisposable.Dispose() => Dispose();

        #endregion

        #region IEnumerator

        /// <inheritdoc />
        void IEnumerator.Reset() => Reset();

        /// <inheritdoc />
        bool IEnumerator.MoveNext() => MoveNext();

        /// <inheritdoc />
        object IEnumerator.Current => CO.Current;

        #endregion

        #region AssetSystem.IHandle

        /// <inheritdoc />
        object AssetSystem.IHandle.Result => Result;

        /// <inheritdoc />
        event Action<object> AssetSystem.IHandle.Completed
        {
            add => Completed += value as Action<TObject>;
            remove => Completed -= value as Action<TObject>;
        }

        /// <inheritdoc />
        TaskAwaiter<object> AssetSystem.IHandle.GetAwaiter()
        {
            return IsValidate
                ? GetAwaiterObject().To<TaskAwaiter<object>>()
                : Task.FromResult<object>(null).GetAwaiter();
        }

        #endregion
    }
}