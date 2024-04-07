using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal abstract class ASHandleList<TObject> : ASHandle<TObject[]>, AssetSystem.IHandleList<TObject>
    {
        public int Count => IsValidate && IsDone && Result != null ? Result.Length : 0;

        public TObject this[int index] => IsValidate && IsDone && Result != null &&
                                          index >= 0 && index < Result.Length
            ? Result[index]
            : default(TObject);

        TObject[] AssetSystem.IHandleList<TObject>.Invoke() => Invoke();

        #region IEnumerator<TObject>

        IEnumerator<TObject> IEnumerable<TObject>.GetEnumerator()
        {
            if (!IsValidate || !IsDone || Result is null || Result.Length == 0) yield break;
            foreach (var obj in Result) yield return obj;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!IsValidate || !IsDone || Result is null || Result.Length == 0) yield break;
            foreach (var obj in Result) yield return obj;
        }

        #endregion

        #region Constructor

        protected ASHandleList(string location)
            : base(location) { }

        protected ASHandleList(string location, Action<TObject[]> onCompleted)
            : base(location, onCompleted) { }

        protected ASHandleList(string location, Type type, Action<TObject[]> onCompleted)
            : base(location, type, onCompleted) { }

        protected ASHandleList(string location, Type type)
            : base(location, type) { }

        #endregion
    }
}