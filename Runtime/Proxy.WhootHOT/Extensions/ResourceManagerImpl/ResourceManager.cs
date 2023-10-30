
#if SUPPORT_WHOOTHOT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rol.Game.Resource;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rol.Game
{
    public static class ResourceManager
    {
        private const string GO_NAME = "ResourceManager";
        private static IResourceManager mgr = null;
        public static bool bIsInitialize
        {
            get{ return mgr != null; }
        }

        public static IEnumerator Initialize()
        {
            if (null != mgr) return null;
            mgr = new ResourceManagerImpl();
            return mgr.Initialize();
        }

        public static IAsyncHandle<T> LoadAsset<T>(string address, Action<T> onLoadComplete = null, CacheLevel cacheLevel = CacheLevel.GameLevel) where T : UnityEngine.Object
        {
            return mgr?.LoadAsset(address, onLoadComplete, cacheLevel);
        }

        public static IAsyncHandle<IAssetCache<string, T>> LoadAssets<T>(ICollection<string> addresses, Action<IAssetCache<string, T>> onLoadComplete = null, 
            IAssetCache<string, T> loadInto = null, CacheLevel cacheLevel = CacheLevel.GameLevel) where T : UnityEngine.Object
        {
            return mgr.LoadAssets(addresses, onLoadComplete, loadInto, cacheLevel);
        }

        public static IAsyncHandle LoadScene(string address, LoadSceneMode loadMode = LoadSceneMode.Single, Action onLoadComplete = null)
        {
            return mgr.LoadScene(address, loadMode, onLoadComplete);
        }

        public static void ReleaseAsset(string address)
        {
            mgr?.ReleaseAsset(address);
        }

        public static void ReleaseAsset(UnityEngine.Object asset)
        {
            mgr?.ReleaseAsset(asset);
        }

        public static void ReleaseAssets(ICollection<string> addresses, bool forceClear = false)
        {
            mgr?.ReleaseAssets(addresses);
        }

        public static void ReleaseAssets<T>(IAssetCache<string, T> cache, bool forceClear = false) where T : UnityEngine.Object
        {
            mgr?.ReleaseAssets<T>(cache);
        }

        public static IAsyncHandle ReleaseScene(string address, Action onLoadComplete = null)
        {
            return mgr.ReleaseScene(address, onLoadComplete);
        }

        /// <summary>
        /// 清理指定CacheLevel的资源
        /// </summary>
        public static void CleanUpTag(CacheLevel tag)
        {
            mgr.CleanUpTag(tag);
        }

        /// <summary>
        /// 仅仅供过度时期使用， 如非必要，请不要使用此接口
        /// </summary>
        /// <returns></returns>
        public static IResourceCache GetCache()
        {
            return mgr as IResourceCache;
        }
    }
}
#endif