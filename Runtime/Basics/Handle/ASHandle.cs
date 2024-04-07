using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal abstract class ASHandle<TObject> : AssetSystem.IHandle<TObject>, AssetSystem.IHandle
    {
        public byte    Progress   { get; protected set; }
        public bool    IsDone     { get; protected set; }
        public string  Address    { get; protected set; }
        public Type    AssetType  { get; protected set; }
        public TObject Result     { get; protected set; }
        public bool    IsValidate { get; protected set; }
        public string  Guid       { get; private set; }
        public bool    IsRunning  => !IsDone;

        public TObject Invoke()
        {
            if (!IsDone)
            {
                CreateSync();
                IsDone = true;
                InvokeOnCompleted();
            }

            return Result;
        }

        public event Action<TObject> Completed;

        protected abstract IEnumerator          CreateCoroutine();
        protected abstract TaskAwaiter<TObject> CreateAsync();
        protected abstract void                 CreateSync();
        protected virtual  void                 OnReset()   { }
        protected virtual  void                 OnDispose() { }

        private IEnumerator _CO;

        protected IEnumerator CO
        {
            get
            {
                if (_CO is null) _CO = CreateCoroutine();
                return _CO;
            }
        }

        protected void Reset()
        {
            OnReset();
            Progress  = 0;
            IsDone    = false;
            Completed = null;
            CO.Reset();
        }

        public TaskAwaiter<TObject> GetAwaiter()
        {
            return IsDone || !IsValidate ? Task.FromResult(default(TObject)).GetAwaiter() : CreateAsync();
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
                    Result = default(TObject);
                }
            }

            _CO       = null;
            Guid      = null;
            Completed = null;
            Address   = null;
            OnDispose();
        }

        protected bool MoveNext()
        {
            if (IsDone) return false;
            if (!IsValidate) return false;
            return CO.MoveNext();
        }

        protected void InvokeOnCompleted()
        {
            IsDone   = true;
            Progress = 100;
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

        protected ASHandle(string location, Action<TObject> onCompleted) : this(location)
        {
            AssetType = typeof(TObject);
            Completed = onCompleted;
        }

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

        public static implicit operator TObject(ASHandle<TObject>              asHandle) => asHandle.Result;
        public static implicit operator Type(ASHandle<TObject>                 asHandle) => asHandle.AssetType;
        public static implicit operator TaskAwaiter<TObject>(ASHandle<TObject> asHandle) => asHandle.GetAwaiter();

        #endregion

        #region AssetSystem.IHandle<TObject>

        object AssetSystem.IHandle.                       Invoke()     => Invoke();
        TaskAwaiter<TObject> AssetSystem.IHandle<TObject>.GetAwaiter() => GetAwaiter();
        TObject AssetSystem.IHandle<TObject>.             Result       => Result;

        event Action<TObject> AssetSystem.IHandle<TObject>.Completed
        {
            add => Completed += value;
            remove => Completed -= value;
        }

        #endregion

        #region AssetSystem.IHandleBase

        string AssetSystem.IHandleBase.Address    => Address;
        byte AssetSystem.IHandleBase.  Progress   => Progress;
        bool AssetSystem.IHandleBase.  IsDone     => IsDone;
        bool AssetSystem.IHandleBase.  IsValidate => IsValidate;
        bool AssetSystem.IHandleBase.  IsRunning  => IsRunning;
        Type AssetSystem.IHandleBase.  AssetType  => AssetType;

        #endregion

        #region IDisposable

        void IDisposable.Dispose() => Dispose();

        #endregion

        #region IEnumerator

        void IEnumerator.  Reset()    => Reset();
        bool IEnumerator.  MoveNext() => MoveNext();
        object IEnumerator.Current    => CO.Current;

        #endregion

        #region AssetSystem.IHandle

        object AssetSystem.IHandle.Result => Result;

        event Action<object> AssetSystem.IHandle.Completed
        {
            add => Completed += value as Action<TObject>;
            remove => Completed -= value as Action<TObject>;
        }

        TaskAwaiter<object> AssetSystem.IHandle.GetAwaiter()
        {
            return IsValidate
                ? CreateAsync().To<TaskAwaiter<object>>()
                : Task.FromResult<object>(null).GetAwaiter();
        }

        #endregion

        public sealed override string ToString()         => string.Empty;
        public sealed override bool   Equals(object obj) => false;
        public sealed override int    GetHashCode()      => 0;
    }
}