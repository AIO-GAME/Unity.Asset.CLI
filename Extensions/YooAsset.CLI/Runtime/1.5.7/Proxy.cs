#if SUPPORT_YOOASSET

using System.Linq;
using UnityEngine.Scripting;
using YooAsset;

[assembly: Preserve]
#if UNITY_2018_3_OR_NEWER
[assembly: AlwaysLinkAssembly]
#endif

namespace AIO.UEngine.YooAsset
{
    [IgnoreConsoleJump(true)]
    public partial class Proxy : ASProxy
    {
        private static Proxy Instance;

        public Proxy() { Instance = this; }

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

        public override bool AlreadyLoad(string location) { return ReferenceOPHandle.ContainsKey(location); }

        public override bool CheckNeedDownloadFromRemote(string location)
        {
            if (AssetSystem.Parameter.ASMode != EASMode.Remote) return false;
            return (
                from package in Dic.Values
                where package.CheckLocationValid(location)
                select package.IsNeedDownloadFromRemote(location)
            ).FirstOrDefault();
        }

        public override bool CheckLocationValid(string location) { return Dic.Values.Any(asset => asset.CheckLocationValid(location)); }

        public bool CheckLocationValid(string location, out string assetPath)
        {
            foreach (var asset in Dic.Values.Where(asset => asset.CheckLocationValid(location)))
            {
                assetPath = asset.GetAssetInfo(location).AssetPath;
                return true;
            }

            assetPath = string.Empty;
            return false;
        }

        public override IASNetLoading GetLoadingHandle() { return new LoadingInfo(); }
    }
}
#endif