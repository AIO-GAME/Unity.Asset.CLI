using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AIO.UEngine
{
    partial class ASProxy
    {
        /// <summary>
        /// 卸载场景 - 任务 
        /// </summary>
        /// <param name="location">可寻址路径</param>
        public abstract Task UnloadSceneTask(string location);

        /// <summary>
        /// 卸载场景 - 协程 
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator UnloadSceneCO(string location, Action cb);

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">强制回收所有资源</param>
        public abstract void UnloadUnusedAssets(bool isForce = false);

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public abstract void HandleFree(string location);

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public virtual void FreeHandle(string[] locations)
        {
            foreach (var location in locations) HandleFree(location);
        }

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public virtual void FreeHandle(IList<string> locations)
        {
            for (var index = 0; index < locations.Count; index++) HandleFree(locations[index]);
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件
        /// </summary>
        public abstract Task<bool> ClearUnusedCacheTask();

        /// <summary>
        /// 清理包裹未使用的缓存文件
        /// </summary>
        public abstract IEnumerator ClearUnusedCacheCO(Action<bool> cb);

        /// <summary>
        /// 清理包裹未使用的缓存文件
        /// </summary>
        public abstract Task<bool> ClearAllCacheTask();

        /// <summary>
        /// 清理包裹未使用的缓存文件
        /// </summary>
        public abstract IEnumerator ClearAllCacheCO(Action<bool> cb);
    }
}