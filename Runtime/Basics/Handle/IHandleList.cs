using System.Collections.Generic;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 异步处理器
        /// </summary>
        public interface IHandleList<TObject> : IEnumerable<TObject>, IHandle<TObject[]>
        {
            /// <summary>
            /// 结果数量
            /// </summary>
            int Count { get; }

            /// <summary>
            /// 索引器
            /// </summary>
            TObject this[int index] { get; }

            /// <summary>
            /// 获取结果
            /// </summary>
            new TObject[] Invoke();
        }
    }
}