
#if SUPPORT_WHOOTHOT
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Rol.Game.Resource
{
    internal enum LoadingMethod
    {
        LoadSingleAsset = 0,
        LoadMultyAsset = 1,
        LoadScene = 2,
        UnLoadScene = 3
    }

    internal interface IAssetRecord
    {
        int refCount { get; }
        string address { get; }
        CacheLevel cacheLevel { get; }
        LoadingMethod loadMethod { get; }
        object asset { get; }
        void Regtain();
        void Release(ICollection<string> removeingRecord, bool forceClear = false);
        bool isLoaded { get; }
        System.Action<object> onComplete { get; set; }
        float loadingProgress { get; }
    }

    internal abstract class AssetRecordBase : IAssetRecord
    {
        public AssetRecordBase(string address, LoadingMethod loadMethod, CacheLevel cacheLevel)
        {
            this.address = address;
            this.loadMethod = loadMethod;
            this.cacheLevel = cacheLevel;
            refCount = 1;
        }

        public int refCount { get; private set; }
        public string address { get; private set; }
        public CacheLevel cacheLevel { get; private set; }
        public LoadingMethod loadMethod { get; private set; }
        public virtual object asset { get; set; }
        public virtual bool isLoaded { get; protected set; }
        public System.Action<object> onComplete { get; set; }
        public virtual float loadingProgress => 0f;

        public void Regtain()
        {
            if (refCount == 0)
            {
                OnReget();
            }
            refCount++;
        }

        public void Release(ICollection<string> removeingRecord, bool forceClear = false)
        {
            if (forceClear)
            {
                refCount = 0;
            }

            refCount--;

            if (0 >= refCount)
            {
                OnClear(removeingRecord);
            }
        }

        protected virtual void OnClear(ICollection<string> removeingRecord) { }

        protected virtual void OnReget() { }

        public virtual void OnLoadDone()
        {
            isLoaded = true;
            onComplete?.Invoke(asset);
            onComplete = null;
        }
    }

    internal class SingleAssetRecord : AssetRecordBase
    {
        public override object asset => handle.IsDone ? handle.Result : null;
        public override float loadingProgress => handle.PercentComplete;

        private AsyncOperationHandle handle;

        protected override void OnClear(ICollection<string> removeingRecord)
        {
            removeingRecord.Add(base.address);
            Addressables.Release(handle);
        }

        protected override void OnReget()
        {
            throw new System.Exception("[ResourceManager] Internal This Record supposed to be deleted!");
        }

        public SingleAssetRecord(string address, CacheLevel level, AsyncOperationHandle handle) : base(address, LoadingMethod.LoadSingleAsset, level)
        {
            this.handle = handle;
            if (handle.IsDone)
            {
                OnLoadingComplete(handle);
            }
            else
            {
                handle.Completed += OnLoadingComplete;
            }
        }

        public void OnLoadingComplete(AsyncOperationHandle handle)
        {
            OnLoadDone();
        }
    }

    internal class BatchAssetRecordHandleData
    {
        public IList<IResourceLocation> locations; //before loading location complete it is null;
        public AsyncOperationHandle<IList<Object>> loadHandle;         
        public AsyncOperationHandle<IList<IResourceLocation>> locationHandle;
        
        public float GetProgress()
        {
            if (loadHandle.IsValid())
            {
                return loadHandle.PercentComplete;
            }
            else
            {
                return 0f;
            }
        }

        public bool IsLoading()
        {
            return locations == null || (locations.Count > 0 && (!loadHandle.IsValid() || !loadHandle.IsDone));
        }

        public object GetAsset(string address, out bool isInRecord)
        {
            isInRecord = false;
            if (IsLoading())
            {
                return null;
            }

            for(int i = 0, max = locations.Count; i < max; ++i)
            {
                var location = locations[i];
                if (location.PrimaryKey == address)
                {
                    isInRecord = true;
                    return loadHandle.Result[i];
                }
            }
            return null;
        }
    }

    internal class BatchAssetRecord : AssetRecordBase
    {
        private object _asset;
        public override object asset
        {
            get
            {
                if (null != _asset || data == null)
                {
                    return _asset;
                }

                if (data.IsLoading())
                {
                    return null;
                }

                _asset = data.GetAsset(address, out bool isIn);
                if (!isIn)
                {
                    data = null;
                    if (null != onComplete)
                    {
                        onComplete.Invoke(null);
                        onComplete = null;
                    }
                }
                return _asset;
            }
        }

        private BatchAssetRecordHandleData data;

        public override bool isLoaded => null == data || !data.IsLoading();
        public override float loadingProgress => null == data ? 1f : data.GetProgress();

        public BatchAssetRecord(string address, CacheLevel level, BatchAssetRecordHandleData data) : base(address, LoadingMethod.LoadMultyAsset, level)
        {
            this.data = data;
        }

        protected override void OnClear(ICollection<string> removeingRecord)
        {
            if (null != data && !data.IsLoading())
            {
                Addressables.Release(data.locationHandle);
                Addressables.Release(data.loadHandle);
                foreach (var key in data.locations)
                {
                    removeingRecord.Add(key.PrimaryKey);
                }
            }
            _asset = null;
        }

        protected override void OnReget(){}
    }

    internal class SceneAssetRecord : AssetRecordBase
    {
        private AsyncOperationHandle<SceneInstance> handle;
        public SceneAssetRecord(string address, AsyncOperationHandle<SceneInstance> handle) : base(address, LoadingMethod.LoadScene, CacheLevel.None)
        {
            this.handle = handle;
        }

        protected override void OnClear(ICollection<string> removeingRecord)
        {
            removeingRecord.Add(base.address);
        }
    }

    internal class UnloadSceneRecord : AssetRecordBase
    {
        private AsyncOperationHandle<SceneInstance> handle;
        public UnloadSceneRecord(string address, AsyncOperationHandle<SceneInstance> handle) : base(address, LoadingMethod.UnLoadScene, CacheLevel.None)
        {
            this.handle = handle;
        }

        protected override void OnClear(ICollection<string> removeingRecord)
        {
            removeingRecord.Add(base.address);
        }
    }

}
#endif