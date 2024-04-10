using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal abstract class LoaderHandleList<TObject>
        : OperationGenericsList<TObject>,
          ILoaderHandleList<TObject>,
          ILoaderHandle
    {
        public string Address   { get; protected set; }
        public Type   AssetType { get; protected set; }
        object IOperationList.this[int index] => Result[index];


        #region IOperationList

        event Action<object[]> IOperationList.Completed
        {
            add => Completed += value as Action<object>;
            remove => Completed -= value as Action<object>;
        }

        TaskAwaiter<object[]> ITaskObjectArray.GetAwaiter() => GetAwaiter().To<TaskAwaiter<object[]>>();
        object[] IOperationList.               Result       => Result.To<object[]>();
        object[] IOperationList.               Invoke()     => Invoke().To<object[]>();

        #endregion

        #region Constructor

        protected LoaderHandleList() { }

        protected LoaderHandleList(string location)
        {
            Address  = location;
            IsDone   = false;
            Progress = 0;
        }

        protected LoaderHandleList(string location, Action<TObject[]> onCompleted) : this(location)
        {
            AssetType =  typeof(TObject);
            Completed += onCompleted;
        }

        protected LoaderHandleList(string location, Type type, Action<TObject[]> onCompleted) : this(location)
        {
            AssetType =  type;
            Completed += onCompleted;
        }

        protected LoaderHandleList(string location, Type type) : this(location)
        {
            AssetType = type;
        }

        #endregion
    }
}