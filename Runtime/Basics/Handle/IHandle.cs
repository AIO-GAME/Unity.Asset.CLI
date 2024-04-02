using System;
using System.Runtime.CompilerServices;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 异步处理器
        /// </summary>
        public interface IHandle<TObject> : IHandleBase
        {
            /// <summary>
            /// 结果
            /// </summary>
            TObject Result { get; }

            /// <summary>
            /// 完成回调
            /// </summary>
            event Action<TObject> Completed;

            /// <summary>
            /// 获取异步等待器
            /// </summary>
            TaskAwaiter<TObject> GetAwaiter();
        }

        /// <summary>
        /// 异步处理器
        /// </summary>
        public interface IHandle : IHandleBase
        {
            /// <summary>
            /// 结果
            /// </summary>
            object Result { get; }

            /// <summary>
            /// 完成回调
            /// </summary>
            event Action<object> Completed;

            /// <summary>
            /// 获取异步等待器
            /// </summary>
            TaskAwaiter<object> GetAwaiter();
        }
    }
}