using System;
using UnityEngine.Scripting;

namespace AIO
{
    [Preserve]
    public interface ILoaderHandle<TObject> : IOperation<TObject>, ILoaderHandle { }

    [Preserve]
    public interface ILoaderHandle : IOperation
    {
        /// <summary>
        /// 资源类型
        /// </summary>
        [Preserve]
        Type AssetType { get; }

        /// <summary>
        /// 可寻址资源地址
        /// </summary>
        [Preserve]
        string Address { get; }

        /// <summary>
        /// 资源路径
        /// </summary>
        [Preserve]
        string AssetPath { get; }
    }

    [Preserve]
    public interface ILoaderHandleList<TObject> : IOperationList<TObject>, ILoaderHandleList { }

    [Preserve]
    public interface ILoaderHandleList : IOperationList
    {
        /// <summary>
        /// 资源类型
        /// </summary>
        [Preserve]
        Type AssetType { get; }

        /// <summary>
        /// 可寻址资源地址
        /// </summary>
        [Preserve]
        string Address { get; }
    }
}