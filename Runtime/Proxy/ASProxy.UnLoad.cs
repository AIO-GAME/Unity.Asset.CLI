#region

using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace AIO.UEngine
{
    partial class ASProxy
    {
        /// <summary>
        ///     资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">强制回收所有资源</param>
        public abstract void UnloadUnusedAssets(bool isForce = false);

        /// <summary>
        ///     释放资源句柄
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public abstract void HandleFree(string location);

        /// <summary>
        ///     释放资源句柄
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public virtual void HandleFree(IEnumerable<string> locations)
        {
            foreach (var location in locations) HandleFree(location);
        }

        /// <summary>
        ///     释放资源句柄
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public virtual void HandleFree(IList<string> locations)
        {
            for (var index = 0; index < locations.Count; index++) HandleFree(locations[index]);
        }

        /// <summary>
        ///     卸载场景 - 任务
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        public abstract IOperationAction UnloadSceneTask(string location, Action completed = null);

        /// <summary>
        ///     清理包裹未使用的缓存文件
        /// </summary>
        /// <param name="completed">回调</param>
        public abstract IOperationAction<bool> ClearUnusedCacheTask(Action<bool> completed = null);

        /// <summary>
        ///     清理包裹全部缓存文件
        /// </summary>
        /// <param name="completed">回调</param>
        public abstract IOperationAction<bool> ClearAllCacheTask(Action<bool> completed = null);
    }
}