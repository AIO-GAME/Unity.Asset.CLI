/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-23
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET

using System;
using System.Threading.Tasks;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        public static async Task PreLoadSubAssets(string location, Type type)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return;

                operation = package.LoadSubAssetsAsync(location, type);
                if (!await LoadCheckOPTask(operation)) return;
                AddHandle(location, operation);
            }
        }

        public static async Task PreLoadAsset(string location, Type type)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (!(operation is null)) return;
            var package = await GetAutoPackageTask(location);
            if (package is null) return;

            operation = package.LoadAssetAsync(location, type);
            if (!await LoadCheckOPTask(operation)) return;
            AddHandle(location, operation);
        }

        public static async Task PreLoadRaw(string location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return;

                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return;
                AddHandle(location, operation);
            }
        }
    }
}

#endif