using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal abstract class LoaderHandle<TObject> : OperationGenerics<TObject>, ILoaderHandle<TObject>
    {
        private string Address   { get; set; }
        public  string AssetPath { get; }
        private Type   AssetType { get; set; }

        protected override void OnDispose()
        {
            if (IsValidate)
            {
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
            if (IsValidate)
            {
                IsDone   = false;
                Progress = 0;
            }
            else
            {
                AssetSystem.LogWarningFormat("资源地址无效: {0}", location);
                IsDone   = true;
                Progress = 100;
            }
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