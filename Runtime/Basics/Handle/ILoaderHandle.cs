using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AIO
{
    partial class AssetSystem
    {
        internal static readonly Dictionary<string, ILoaderHandle> HandleDic
            = new Dictionary<string, ILoaderHandle>();
    }

    public interface ILoaderHandle<TObject> : IOperation<TObject>, ILoaderHandle { }

    public interface ILoaderHandle : IOperation
    {
        /// <summary>
        /// 资源类型
        /// </summary>
        Type AssetType { get; }

        /// <summary>
        /// 可寻址资源地址
        /// </summary>
        string Address { get; }
    }

    public interface ILoaderHandleList<TObject> : IOperationList<TObject>, ILoaderHandleList { }

    public interface ILoaderHandleList : IOperationList
    {
        /// <summary>
        /// 资源类型
        /// </summary>
        Type AssetType { get; }

        /// <summary>
        /// 可寻址资源地址
        /// </summary>
        string Address { get; }
    }

    [StructLayout(LayoutKind.Auto)]
    internal abstract class LoaderHandle<TObject> : OperationGenerics<TObject>, ILoaderHandle<TObject>
    {
        public string Address   { get; protected set; }
        public Type   AssetType { get; protected set; }

        protected override void OnDispose()
        {
            if (IsValidate)
            {
                AssetSystem.HandleDic.Remove(Address);
                AssetSystem.UnloadAsset(Address);
                IsValidate = false;
            }

            if (IsDone) Result = default;
            Address = null;
        }

        #region Constructor

        protected LoaderHandle() { }

        protected LoaderHandle(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                IsDone     = true;
                IsValidate = false;
                return;
            }

            Address    = location;
            IsValidate = AssetSystem.Proxy.CheckLocationValid(Address);
            if (IsValidate) AssetSystem.HandleDic[Address] = this;
            else AssetSystem.LogWarningFormat("资源地址无效: {0}", location);

            IsDone   = !IsValidate;
            Progress = 0;
        }

        protected LoaderHandle(string location, Action<TObject> onCompleted) : this(location)
        {
            AssetType =  typeof(TObject);
            Completed += onCompleted;
        }

        protected LoaderHandle(string location, Type type, Action<TObject> onCompleted) : this(location)
        {
            AssetType =  type;
            Completed += onCompleted;
        }

        protected LoaderHandle(string location, Type type) : this(location)
        {
            AssetType = type;
        }

        #endregion

        #region operator implicit

        public static implicit operator TObject(LoaderHandle<TObject>              loaderHandle) => loaderHandle.Result;
        public static implicit operator Type(LoaderHandle<TObject>                 loaderHandle) => loaderHandle.AssetType;
        public static implicit operator TaskAwaiter<TObject>(LoaderHandle<TObject> loaderHandle) => loaderHandle.GetAwaiter();

        #endregion

        #region IASOperation

        string ILoaderHandle.Address   => Address;
        Type ILoaderHandle.  AssetType => AssetType;

        #endregion
    }
}