using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal abstract partial class ASHandleAction : AssetSystem.IHandleAction
    {
        public byte         Progress  { get; protected set; }
        public bool         IsDone    { get; protected set; }
        public bool         IsRunning => !IsDone;
        public event Action Completed;

        protected abstract IEnumerator CO { get; }
        protected abstract TaskAwaiter GetAwaiter();
        protected abstract void        Reset();
        protected abstract void        OnDispose();
        protected virtual  bool        MoveNext() => CO.MoveNext();

        public void Dispose()
        {
            Completed = null;
            OnDispose();
        }


        protected virtual void InvokeOnCompleted()
        {
            if (Completed == null) return;
            Completed.Invoke();
            Completed = null;
        }

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
    }
}