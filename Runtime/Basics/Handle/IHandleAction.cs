using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 异步处理器
        /// </summary>
        public interface IHandleAction : IEnumerator, IDisposable, INotifyCompletion
        {
            /// <summary>
            /// 回调函数
            /// </summary>
            event Action Completed;

            /// <summary>
            /// 是否已经完成
            /// </summary>
            bool IsDone { get; }

            /// <summary>
            /// 处理进度
            /// </summary>
            byte Progress { get; }

            /// <summary>
            /// 是否正在运行
            /// </summary>
            bool IsRunning { get; }

            /// <summary>
            /// 获取异步等待器
            /// </summary>
            TaskAwaiter GetAwaiter();
        }
    }
}