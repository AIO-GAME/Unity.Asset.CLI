using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal abstract partial class ASHandleAction : AssetSystem.IHandleAction
    {
        public byte         Progress  { get; protected set; }
        public bool         IsDone    { get; protected set; }
        public bool         IsRunning => !IsDone;
        public event Action Completed;

        protected abstract TaskAwaiter CreateAsync();
        protected abstract IEnumerator CreateCoroutine();
        protected abstract void        CreateSync();
        protected virtual  void        OnReset()   { }
        protected virtual  void        OnDispose() { }

        public TaskAwaiter GetAwaiter()
        {
            if (IsDone) return Task.CompletedTask.GetAwaiter();
            return CreateAsync();
        }


        public void Invoke()
        {
            if (IsDone) return;
            CreateSync();
            IsDone = true;
            InvokeOnCompleted();
        }


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

        protected bool MoveNext()
        {
            if (IsDone) return false;
            return CO.MoveNext();
        }

        public void Dispose()
        {
            Completed = null;
            OnDispose();
        }

        protected void InvokeOnCompleted()
        {
            Progress = 100;
            IsDone   = true;
            OnCompleted();
            if (Completed == null) return;
            Completed.Invoke();
            Completed = null;
        }

        protected virtual void OnCompleted() { }

        #region Constructor

        protected ASHandleAction()
        {
            IsDone   = false;
            Progress = 0;
        }


        /// <inheritdoc />
        protected ASHandleAction(Action completed) : this()
        {
            Completed = completed;
        }

        #endregion

        #region operator implicit

        public static implicit operator Action(ASHandleAction      ASHandleAction) => ASHandleAction.Completed;
        public static implicit operator TaskAwaiter(ASHandleAction ASHandleAction) => ASHandleAction.GetAwaiter();

        #endregion

        #region AssetSystem.IHandle<TObject>

        /// <inheritdoc />
        TaskAwaiter AssetSystem.IHandleAction.GetAwaiter() => GetAwaiter();

        /// <inheritdoc />
        event Action AssetSystem.IHandleAction.Completed
        {
            add => Completed += value;
            remove => Completed -= value;
        }

        #endregion

        #region AssetSystem.IHandleBase

        /// <inheritdoc />
        byte AssetSystem.IHandleAction.Progress => Progress;

        /// <inheritdoc />
        bool AssetSystem.IHandleAction.IsDone => IsDone;

        /// <inheritdoc />
        bool AssetSystem.IHandleAction.IsRunning => IsRunning;

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

        #region INotifyCompletion

        /// <inheritdoc />
        void INotifyCompletion.OnCompleted(Action continuation)
        {
            if (IsDone)
            {
                continuation?.Invoke();
            }
            else
            {
                Completed += continuation;
            }
        }

        #endregion

        public sealed override string ToString()         => string.Empty;
        public sealed override bool   Equals(object obj) => false;
        public sealed override int    GetHashCode()      => 0;
    }
}