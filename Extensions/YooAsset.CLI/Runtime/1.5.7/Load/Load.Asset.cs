#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Threading.Tasks;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        public override IEnumerator LoadAssetCO<TObject>(string location, Action<TObject> cb)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                ResPackage package = null;
                yield return GetAutoPackageCO(location, ya => package = ya);
                if (package is null)
                {
                    cb?.Invoke(null);
                    yield break;
                }

                operation = package.LoadAssetAsync<TObject>(location);
                var check = false;
                yield return LoadCheckOPCo(operation, ya => check = ya);
                if (!check)
                {
                    cb?.Invoke(null);
                    yield break;
                }

                HandleAdd(location, operation);
            }

            cb?.Invoke(operation?.GetAssetObject<TObject>());
        }

        public override IEnumerator LoadAssetCO(string location, Type type, Action<Object> cb)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                ResPackage package = null;
                yield return GetAutoPackageCO(location, ya => package = ya);
                if (package is null)
                {
                    cb?.Invoke(null);
                    yield break;
                }

                operation = package.LoadAssetAsync(location, type);
                var check = false;
                yield return LoadCheckOPCo(operation, ya => check = ya);
                if (!check)
                {
                    cb?.Invoke(null);
                    yield break;
                }

                HandleAdd(location, operation);
            }

            cb?.Invoke(operation?.AssetObject);
        }


        public override TObject LoadAssetSync<TObject>(string location)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(location);
                if (package is null) return null;
                operation = package.LoadAssetSync<TObject>(location);
                if (!LoadCheckOPSync(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation?.GetAssetObject<TObject>();
        }


        public override Object LoadAssetSync(string location, Type type)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(location);
                if (package is null) return null;
                operation = package.LoadAssetSync(location, type);
                if (!LoadCheckOPSync(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation?.AssetObject;
        }


        public override async Task<TObject> LoadAssetTask<TObject>(string location)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return null;
                operation = package.LoadAssetAsync<TObject>(location);
                if (!await LoadCheckOPTask(operation)) return null;

                HandleAdd(location, operation);
            }

            return operation?.GetAssetObject<TObject>();
        }


        public override async Task<Object> LoadAssetTask(string location, Type type)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return null;
                operation = package.LoadAssetAsync(location, type);
                if (!await LoadCheckOPTask(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation?.AssetObject;
        }
    }
}

#endif