using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 异步处理器
        /// </summary>
        public interface IHandleBase : IEnumerator, IDisposable
        {
            /// <summary>
            /// 可寻址资源地址
            /// </summary>
            string Address { get; }

            /// <summary>
            /// 是否已经完成
            /// </summary>
            bool IsDone { get; }

            /// <summary>
            /// 处理进度
            /// </summary>
            byte Progress { get; }
        }

        /// <summary>
        /// 异步处理器
        /// </summary>
        public interface IHandleArray : IHandleBase
        {
            /// <summary>
            /// 资源类型
            /// </summary>
            Type AssetType { get; }

            /// <summary>
            /// 结果
            /// </summary>
            Object[] Result { get; }

            /// <summary>
            /// 完成回调
            /// </summary>
            event Action<Object[]> OnCompleted;

            /// <summary>
            /// 获取异步等待器
            /// </summary>
            TaskAwaiter<Object[]> GetAwaiter();
        }

        /// <summary>
        /// 异步处理器
        /// </summary>
        public interface IHandle : IHandleBase
        {
            /// <summary>
            /// 资源类型
            /// </summary>
            Type AssetType { get; }

            /// <summary>
            /// 结果
            /// </summary>
            Object Result { get; }

            /// <summary>
            /// 完成回调
            /// </summary>
            event Action<Object> OnCompleted;

            /// <summary>
            /// 获取异步等待器
            /// </summary>
            TaskAwaiter<Object> GetAwaiter();
        }

        /// <summary>
        /// 异步处理器
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        public interface IHandle<TObject> : IHandle where TObject : Object
        {
            /// <summary>
            /// 结果
            /// </summary>
            new TObject Result { get; }

            /// <summary>
            /// 获取异步等待器
            /// </summary>
            new TaskAwaiter<TObject> GetAwaiter();

            /// <summary>
            /// 完成回调
            /// </summary>
            new event Action<TObject> OnCompleted;
        }
    }
}