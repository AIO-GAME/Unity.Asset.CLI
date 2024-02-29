#if SUPPORT_YOOASSET
using System;
using System.Threading.Tasks;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        public override async Task PreLoadSubAssetsTask(string location, Type type)
        {
            var operation = HandleGet<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return;

                operation = package.LoadSubAssetsAsync(location, type);
                if (!await LoadCheckOPTask(operation)) return;
                HandleAdd(location, operation);
            }
        }

        public override async Task PreLoadAssetTask(string location, Type type)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (!(operation is null)) return;
            var package = await GetAutoPackageTask(location);
            if (package is null) return;

            operation = package.LoadAssetAsync(location, type);
            if (!await LoadCheckOPTask(operation)) return;
            HandleAdd(location, operation);
        }

        public override async Task PreLoadRawTask(string location)
        {
            var operation = HandleGet<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return;

                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return;
                HandleAdd(location, operation);
            }
        }
    }
}
#endif