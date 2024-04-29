using System;

namespace AIO
{
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

        /// <summary>
        /// 资源路径
        /// </summary>
        string AssetPath { get; }
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
}