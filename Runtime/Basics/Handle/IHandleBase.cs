using System;
using System.Collections;

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
            /// 资源类型
            /// </summary>
            Type AssetType { get; }

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

            /// <summary>
            /// 是否有效
            /// </summary>
            bool IsValidate { get; }

            /// <summary>
            /// 是否正在运行
            /// </summary>
            bool IsRunning { get; }
            
        }
    }
}