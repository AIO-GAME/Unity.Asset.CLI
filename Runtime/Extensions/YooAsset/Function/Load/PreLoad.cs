/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-23
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        public static async void PreLoadSubAssets<TObject>(string location) where TObject : Object
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return;

                operation = package.LoadSubAssetsAsync<TObject>(location);
                if (!await LoadCheckOPTask(operation)) return;
                AddHandle(location, operation);
            }
        }

        public static async void PreLoadAsset<TObject>(string location) where TObject : Object
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return;

                operation = package.LoadAssetAsync<TObject>(location);
                if (!await LoadCheckOPTask(operation)) return;
                AddHandle(location, operation);
            }
        }

        public static async void PreLoadRaw(string location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return;

                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return;
                AddHandle(location, operation);
            }
        }
    }
}

#endif
