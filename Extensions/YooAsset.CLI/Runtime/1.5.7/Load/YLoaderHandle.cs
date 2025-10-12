#if SUPPORT_YOOASSET
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        [Preserve]
        [StructLayout(LayoutKind.Auto)]
        internal abstract class YLoaderHandle<TObject> : OperationGenerics<TObject>, ILoaderHandle<TObject>
        {
            [Preserve]
            protected string Address { get; private set; }

            [Preserve]
            protected Type AssetType { get; }

            [Preserve]
            protected string AssetPath;

            [Preserve]
            protected override void OnDispose()
            {
                if (IsValidate)
                {
                    Instance.HandleFree(Address);
                    IsValidate = false;
                }

                if (IsDone) Result = default;
                Address = null;
            }

            #region Constructor

            protected YLoaderHandle() { }

            private YLoaderHandle(string location)
            {
                if (string.IsNullOrEmpty(location))
                {
                    IsDone     = true;
                    IsValidate = false;
                    return;
                }

                Address    = AssetSystem.SettingToLocalPath(location);
                IsValidate = Instance.CheckLocationValid(Address, out AssetPath);
                if (IsValidate)
                {
                    IsDone   = false;
                    Progress = 0;
                }
                else
                {
                    AssetSystem.LOG.W("资源地址无效: {0}", Address);
                    IsDone   = true;
                    Progress = 100;
                }
            }

            protected YLoaderHandle(string location, Action<TObject> onCompleted) : this(location)
            {
                AssetType =  typeof(TObject);
                Completed += onCompleted;
            }

            protected YLoaderHandle(string location, Type type, Action<TObject> onCompleted) : this(location)
            {
                AssetType = type;
                if (onCompleted is null) return;
                Completed += onCompleted;
            }

            protected YLoaderHandle(string location, Type type) : this(location) { AssetType = type; }

            #endregion

            #region operator implicit

            public static implicit operator TObject(YLoaderHandle<TObject>              loaderHandle) => loaderHandle.Result;
            public static implicit operator Type(YLoaderHandle<TObject>                 loaderHandle) => loaderHandle.AssetType;
            public static implicit operator TaskAwaiter<TObject>(YLoaderHandle<TObject> loaderHandle) => loaderHandle.GetAwaiter();

            #endregion

            #region IASOperation

            [Preserve]
            string ILoaderHandle.AssetPath => AssetPath;

            [Preserve]
            string ILoaderHandle.Address => Address;

            [Preserve]
            Type ILoaderHandle.AssetType => AssetType;

            #endregion
        }
    }
}
#endif