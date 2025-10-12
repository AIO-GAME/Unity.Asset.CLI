#if SUPPORT_YOOASSET

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// 资源加载管理器
    /// </summary>
    [IgnoreConsoleJump(true)]
    public partial class Proxy : ASProxy
    {
        [Preserve]
        private static Proxy Instance;

        public Proxy()
        {
            Instance                 = this;
            InitializationOperations = new List<InitializationOperation>();
            ReferenceOPHandle        = new Dictionary<string, OperationHandleBase>();
            DownloaderOperations     = new Dictionary<string, DownloaderOperation>(64);
            Dic                      = new Dictionary<string, ResPackage>();
        }

        [Preserve]
        public override void Dispose()
        {
            if (IsInitialize == false) return;

            EventParameter = null;

            foreach (var handle in ReferenceOPHandle.Values.Where(handle => handle.IsValid))
                ReleaseInternal?.Invoke(handle, null);

            ReferenceOPHandle.Clear();
            InitializationOperations.Clear();

            YooAssets.Destroy();
        }

        [Preserve]
        public override bool AlreadyLoad(string location) { return ReferenceOPHandle.ContainsKey(location); }

        [Preserve]
        public override bool CheckNeedDownloadFromRemote(string location)
        {
            if (AssetSystem.Parameter.ASMode != EASMode.Remote) return false;
            return (from package in Dic.Values
                    where package.CheckLocationValid(location)
                    select package.IsNeedDownloadFromRemote(location)
                ).FirstOrDefault();
        }

        [Preserve]
        public override bool CheckLocationValid(string location)
        {
            try
            {
                return Dic.Values.Any(package => package.CheckLocationValid(location));
            }
            catch
            {
                return false;
            }
        }

        [Preserve]
        public bool CheckLocationValid(string location, out string assetPath)
        {
            try
            {
                foreach (var assetInfo in Dic.Values.
                                              Where(package => package.CheckLocationValid(location)).
                                              Select(package => package.GetAssetInfo(location)))
                {
                    assetPath = assetInfo.AssetPath;
                    return true;
                }

                assetPath = string.Empty;
                return false;
            }
            catch
            {
                assetPath = string.Empty;
                return false;
            }
        }

        [Preserve]
        public override IASNetLoading GetLoadingHandle() { return new LoadingInfo(); }
    }
}
#endif