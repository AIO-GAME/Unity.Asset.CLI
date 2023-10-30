
#if SUPPORT_WHOOTHOT
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

namespace Rol.Game.Resource
{

    public class ResourceManagerImpl : IResourceManager, IResourceCache
    {
        private readonly Dictionary<string, IAssetRecord> assetRecords = new Dictionary<string, IAssetRecord>();

        public System.Collections.IEnumerator Initialize()
        {
            yield return Addressables.InitializeAsync();
        }

        public bool IsAddressCached(string address)
        {
            if (assetRecords.TryGetValue(address, out var record))
            {
                return record.isLoaded;
            }
            return false;
        }

        public bool TryGetAsset<T>(string address, out T asset) where T : UnityEngine.Object
        {
            asset = null;
            if (assetRecords.TryGetValue(address, out var record) && record.isLoaded)
            {
                asset = record.asset as T;
                return true;
            }
            return false;
        }

        public IAsyncHandle<T> LoadAsset<T>(string address, Action<T> onLoadComplete, CacheLevel cacheLevel) 
            where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                onLoadComplete?.Invoke(null);
                return new ConstAsyncHandle<T>(null);
            }

            if (assetRecords.TryGetValue(address, out var assetRecord))
            {
                assetRecord.Regtain();
                if (assetRecord.isLoaded)
                {
                    var result = assetRecord.asset as T;
                    onLoadComplete?.Invoke(result);
                    return new ConstAsyncHandle<T>(result);
                }
                else
                {
                    if (onLoadComplete != null)
                    {
                        assetRecord.onComplete += (x) => onLoadComplete.Invoke(x as T);
                    }
                    return new AsyncHandle<T>(assetRecord);
                }
            }
            else
            {
                AsyncOperationHandle<T> handle;
                try
                {
                    handle = Addressables.LoadAssetAsync<T>(address);
                    assetRecord = new SingleAssetRecord(address, cacheLevel, handle);
                    assetRecords.Add(address, assetRecord);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"������Դ{address}�쳣, {ex.ToString()}");
                }

                if (onLoadComplete != null)
                {
                    assetRecord.onComplete += (x) => onLoadComplete.Invoke(x as T);
                }
                return new AsyncHandle<T>(assetRecord);
            }
        }

        public IAsyncHandle<IAssetCache<string, T>> LoadAssets<T>(ICollection<string> addresses, Action<IAssetCache<string, T>> onComplete, 
            IAssetCache<string, T> loadInto, CacheLevel cacheLevel) where T : UnityEngine.Object
        {
            if (null == addresses || 0 == addresses.Count)
            {
                onComplete?.Invoke(null);
                return new ConstAsyncHandle<IAssetCache<string, T>>(loadInto);
            }

            var keys = new List<string>();
            var asyncHandle = new AsyncCacheHandle<T>();
            asyncHandle.loadingRecords = new List<IAssetRecord>();
            Action<object> onLoadingUpdate = (x) =>
            {
                if (null == asyncHandle.Result && asyncHandle.IsLoadingDone())
                {
                    if (null == loadInto)
                    {
                        loadInto = new AssetCache<string, T>();
                    }
                    GetLoadedAsset<T>(addresses, loadInto);
                    asyncHandle.Result = loadInto;
                    onComplete?.Invoke(loadInto);
                    onComplete = null;
                }
            };

            var batchLoadData = new BatchAssetRecordHandleData();
            foreach (var addr in addresses)
            {
                if (string.IsNullOrWhiteSpace(addr))
                {
                    continue;
                }

                if (null != loadInto && loadInto.ContainsKey(addr))
                {
                    continue;
                }

                if (assetRecords.TryGetValue(addr, out var record))
                {
                    record.Regtain();
                    if (!record.isLoaded)
                    {
                        asyncHandle.loadingRecords.Add(record);
                        record.onComplete += onLoadingUpdate;
                    }
                    continue;
                }

                keys.Add(addr);
                record = new BatchAssetRecord(addr, cacheLevel, batchLoadData);
                if (null == asyncHandle.batchRecord)
                {
                    asyncHandle.batchRecord = record;
                    record.onComplete += onLoadingUpdate;
                }
                assetRecords.Add(addr, record);
            }

            asyncHandle.batchCount = keys.Count;
            asyncHandle.loadingCount = asyncHandle.loadingRecords.Count;

            if (asyncHandle.loadingRecords.Count == 0 && keys.Count == 0)
            {
                if (null == loadInto)
                {
                    loadInto = new AssetCache<string, T>();
                }
                GetLoadedAsset<T>(addresses, loadInto);
                onComplete?.Invoke(loadInto);
                return new ConstAsyncHandle<IAssetCache<string, T>>(loadInto);
            }

            if (keys.Count > 0)
            {
                var locationHandle = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Union, typeof(UnityEngine.Object));
                batchLoadData.locationHandle = locationHandle;
                locationHandle.CompletedTypeless += (x) =>
                {
                    var locations = x.Result as IList<IResourceLocation>;
                    if (locations == null || locations.Count == 0)
                    {
                        batchLoadData.locations = null == locations ? new List<IResourceLocation>() : locations;
                        OnBatchLoadComplete(x, batchLoadData, keys);
                    }
                    else
                    {
                        var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(locations, null);
                        handle.Completed += (xx) => OnBatchLoadComplete(xx, batchLoadData, keys);
                        batchLoadData.loadHandle = handle;
                        batchLoadData.locations = locations;
                    }
                };
            }
            return asyncHandle;
        }

        private void OnBatchLoadComplete(AsyncOperationHandle xx, BatchAssetRecordHandleData handleData, List<string> keys)
        {
            foreach (var k in keys)
            {
                var addr = k as string;
                if (assetRecords.TryGetValue(addr, out var record))
                {
                    if (null != record.onComplete)
                    {
                        record.onComplete(record.asset);
                        record.onComplete = null;
                    }
                }
            }
        }

        public void ReleaseAsset(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return;
            }

            if (assetRecords.TryGetValue(address, out var record))
            {
                record.Release(removingKeys);
            }
            ClearRemoveAssetRecord();
        }

        public void ReleaseAsset(object asset)
        {
            if (null != asset)
            {
                IAssetRecord record;
                foreach (var kv in assetRecords)
                {
                    record = kv.Value;
                    if (record.asset == asset)
                    {
                        record.Release(removingKeys);
                    }
                }
                ClearRemoveAssetRecord();
            }
        }

        public void ReleaseAssets(ICollection<string> addresses, bool forceClear = false)
        {
            if (null == addresses || addresses.Count == 0)
            {
                return;
            }

            foreach (var addr in addresses)
            {
                if (assetRecords.TryGetValue(addr, out var record))
                {
                    record.Release(removingKeys, forceClear);
                }
            }
            ClearRemoveAssetRecord();
        }

        public void ReleaseAssets<T>(IAssetCache<string, T> assets, bool forceClear = false) where T : UnityEngine.Object
        {
            if (null == assets)
            {
                return;
            }

            foreach (var kv in assets)
            {
                if (assetRecords.TryGetValue(kv.Key, out var record))
                {
                    record.Release(removingKeys, forceClear);
                }
                ClearRemoveAssetRecord();
            }
            assets.Clear();
        }

        public void CleanUpTag(CacheLevel tag)
        {
            foreach (var kv in assetRecords)
            {
                var record = kv.Value;
                if (record.cacheLevel == tag)
                {
                    record.Release(removingKeys, true);
                }
            }
            ClearRemoveAssetRecord();
        }

        private void GetLoadedAsset<T>(ICollection<string> address, IAssetCache<string, T> cache) where T : UnityEngine.Object
        {
            foreach (var addr in address)
            {
                if (string.IsNullOrWhiteSpace(addr))
                {
                    continue;
                }

                if (cache.ContainsKey(addr))
                {
                    continue;
                }

                cache.PushInto(addr, assetRecords[addr].asset as T);
            }
        }

        private readonly List<string> removingKeys = new List<string>();
        private void ClearRemoveAssetRecord()
        {
            foreach (var key in removingKeys)
            {
                assetRecords.Remove(key);
            }
            removingKeys.Clear();
        }

        private AsyncOperationHandle lastLoadScenehandle;
        public IAsyncHandle LoadScene(string address, LoadSceneMode loadMode = LoadSceneMode.Single, Action onComplete = null)
        {
            // var handle = Addressables.LoadSceneAsync(address);
            lastLoadScenehandle = Addressables.LoadSceneAsync(address, loadMode, true);
            if (null != onComplete)
            {
                lastLoadScenehandle.Completed += (x) => onComplete();
            }
            return new AsyncHandle(lastLoadScenehandle);
        }

        public IAsyncHandle ReleaseScene(string address, Action onComplete = null)
        {
            if (!lastLoadScenehandle.IsValid())
            {
                return new ConstAsyncHandle<object>(null);
            }

            var handle = Addressables.UnloadSceneAsync(lastLoadScenehandle, false);
            if (null != onComplete)
            {
                handle.Completed += (x) => onComplete();
            }
            return new AsyncHandle(handle);
        }
    }
}
#endif