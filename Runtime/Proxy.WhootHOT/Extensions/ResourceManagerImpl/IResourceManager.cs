
#if SUPPORT_WHOOTHOT
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rol.Game
{
    /// <summary>
    /// 资源缓存级别
    /// </summary>
    public enum CacheLevel
    {
        None = 0,
        ResidentLevel = 1, //常驻内存不卸载资源
        SceneLevel = 2, //场景依赖资源
        GameLevel = 3, //业务动态加载资源
    }

    public interface IAsyncHandle : IEnumerator
    {
        bool IsDone { get; }
        float PercentComplete { get; }
    }

    public interface IAsyncHandle<T> : IAsyncHandle where T : class
    {
        T Result { get; }
    }

    public interface IResourceManager
    {
        IEnumerator Initialize();

        IAsyncHandle<T> LoadAsset<T>(string address, Action<T> onComplete, CacheLevel cacheLevel) where T : UnityEngine.Object;
        IAsyncHandle<IAssetCache<string, T>> LoadAssets<T>(ICollection<string> addresses, Action<IAssetCache<string, T>> onComplete, IAssetCache<string, T> loadInto, CacheLevel cacheLevel) where T : UnityEngine.Object;
        IAsyncHandle LoadScene(string address, LoadSceneMode loadMode = LoadSceneMode.Single, Action onComplete = null);

        void ReleaseAsset(string address);
        void ReleaseAsset(object asset);
        void ReleaseAssets(ICollection<string> addresses, bool forceClear = false);
        void ReleaseAssets<T>(IAssetCache<string, T> cache, bool forceClear = false) where T : UnityEngine.Object;
        IAsyncHandle ReleaseScene(string address, Action onComplete = null);

        void CleanUpTag(CacheLevel tag); // 清理相应CacheLevel的资源
    }
}
#endif