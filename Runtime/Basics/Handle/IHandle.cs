using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AIO
{
    public static class IHandleExtensions
    {
        /// <summary>
        /// 安全转换
        /// </summary>
        /// <returns> 转换后的对象 </returns>
        public static Object SafeCast(this AssetSystem.IHandle<Object> handle)
        {
            return handle.Result;
        }

        /// <summary>
        /// 安全转换
        /// </summary>
        /// <returns> 转换后的对象 </returns>
        public static TObject SafeCast<TObject>(this AssetSystem.IHandle<TObject> handle)
        where TObject : Object
        {
            return handle.Result;
        }
    }

    partial class AssetSystem
    {
        internal static readonly Dictionary<string, int> ReferenceHandleCount
            = new Dictionary<string, int>();

        internal static readonly Dictionary<string, IHandle> HandleDic
            = new Dictionary<string, IHandle>();
    }

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

            TObject Invoke();
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

            object Invoke();
        }
    }
}