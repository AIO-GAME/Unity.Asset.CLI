/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_UNITASK
#define UNITASK
#endif

#if SUPPORT_YOOASSET
#define YOOASSET
#endif

#if UNITASK
using Cysharp.Threading.Tasks;
using ATask = Cysharp.Threading.Tasks.UniTask;
using ATaskObject = Cysharp.Threading.Tasks.UniTask<UnityEngine.Object>;
using ATaskGameObject = Cysharp.Threading.Tasks.UniTask<UnityEngine.GameObject>;
using ATaskObjectArray = Cysharp.Threading.Tasks.UniTask<UnityEngine.Object[]>;
using ATaskScene = Cysharp.Threading.Tasks.UniTask<UnityEngine.SceneManagement.Scene>;
using ATaskString = Cysharp.Threading.Tasks.UniTask<string>;
using ATaskByteArray = Cysharp.Threading.Tasks.UniTask<byte[]>;
using ATaskBoolean = Cysharp.Threading.Tasks.UniTask<bool>;
#else
using ATask = System.Threading.Tasks.Task;
using ATaskObject = System.Threading.Tasks.Task<UnityEngine.Object>;
using ATaskGameObject = System.Threading.Tasks.Task<UnityEngine.GameObject>;
using ATaskObjectArray = System.Threading.Tasks.Task<UnityEngine.Object[]>;
using ATaskScene = System.Threading.Tasks.Task<UnityEngine.SceneManagement.Scene>;
using ATaskString = System.Threading.Tasks.Task<string>;
using ATaskByteArray = System.Threading.Tasks.Task<byte[]>;
using ATaskBoolean = System.Threading.Tasks.Task<bool>;
#endif
#if YOOASSET
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        #region Coroutine

        /// <summary>
        /// 协程 加载资源包
        /// </summary>
        public static IEnumerator LoadCO()
        {
            if (GetParameter is null)
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
#if UNITY_EDITOR
                AssetSystem.LogError("Parameter is null");
#endif
                yield break;
            }

            var tasks = new List<IEnumerator>();
            foreach (var item in Dic.Values)
            {
                var args = GetParameter.Invoke(item);
                if (args is null)
                {
                    AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
#if UNITY_EDITOR
                    throw new Exception($"AssetSystem {item.Config.Name} Parameter is null");
#endif
                }

                var operation = item.InitializeAsync(args);
                if (operation.Task != null) tasks.Add(operation);
                else
                {
                    AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                    AssetSystem.LogException("{0} -> {1} -> {2}", nameof(LoadTask), item.Config, operation.Error);
                }
            }

            foreach (var task in tasks) yield return task;
        }

        /// <summary>
        /// 协程 清空缓存资源
        /// </summary>
        public static IEnumerator ClearCacheResourceCO()
        {
            var tasks = Dic.Keys.ToArray()
                .Select(item => Dic[item].ClearUnusedCacheFilesAsync())
                .Cast<IEnumerator>();
            foreach (var task in tasks) yield return task;
        }

        #endregion

        #region Sync

        public static async void Load(Action<bool> cb)
        {
            if (GetParameter is null)
            {
                AssetSystem.LogError("Parameter is null");
                cb?.Invoke(false);
                return;
            }

            var tasks = new List<ATask>();
            foreach (var item in Dic.Values)
            {
                var args = GetParameter.Invoke(item);
                var operation = item.InitializeAsync(args);
                if (operation.Task != null)
#if UNITASK
                    tasks.Add(operation.ToUniTask());
#else
                    tasks.Add(operation.Task);
#endif
                else AssetSystem.LogError("{0} -> {1}", nameof(LoadTask), item.Config);
            }

#if UNITY_WEBGL
            foreach (var task in tasks) await task;
#else
            if (tasks.Count > 0) await ATask.WhenAll(tasks);
#endif
            cb?.Invoke(true);
        }

        /// <summary>
        /// 清空缓存资源
        /// </summary>
        public static async void ClearCacheResource(Action<bool> cb)
        {
            var tasks = new List<ATask>();
            foreach (var item in Dic.Keys.ToArray())
            {
#if UNITASK
                tasks.Add(Dic[item].ClearUnusedCacheFilesAsync().ToUniTask());
#else
                tasks.Add(Dic[item].ClearUnusedCacheFilesAsync().Task);
#endif
            }

#if UNITY_WEBGL
            foreach (var task in tasks) await task;
#else
            if (tasks.Count > 0) await ATask.WhenAll(tasks);
#endif
            cb?.Invoke(true);
        }

        #endregion

        #region Task

        public static async ATask LoadTask()
        {
            if (GetParameter is null)
            {
                AssetSystem.LogError("Parameter is null");
                return;
            }

            var tasks = new List<ATask>();
            foreach (var item in Dic.Values)
            {
                var args = GetParameter.Invoke(item);
                var operation = item.InitializeAsync(args);
#if UNITASK
                if (operation.Task != null) tasks.Add(operation.Task.AsUniTask());
#else
                if (operation.Task != null) tasks.Add(operation.Task);
#endif
                else AssetSystem.Log("{0} -> {1} -> {2}", nameof(LoadTask), item.Config, operation.Error);
            }

#if UNITY_WEBGL
            foreach (var task in tasks) await task;
#else
            if (tasks.Count > 0) await ATask.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// 清空缓存资源
        /// </summary>
        public static async ATask ClearCacheResourceTask()
        {
            var tasks = new List<ATask>();
            foreach (var item in Dic.Keys.ToArray())
            {
#if UNITASK
                tasks.Add(Dic[item].ClearUnusedCacheFilesAsync().ToUniTask());
#else
                tasks.Add(Dic[item].ClearUnusedCacheFilesAsync().Task);
#endif
            }

#if UNITY_WEBGL
            foreach (var task in tasks) await task;
#else
            if (tasks.Count > 0) await ATask.WhenAll(tasks);
#endif
        }

        #endregion
    }
}
#endif