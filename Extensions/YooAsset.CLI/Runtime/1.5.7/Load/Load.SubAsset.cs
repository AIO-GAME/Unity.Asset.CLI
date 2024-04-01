#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        #region 子资源加载

        public override IEnumerator LoadSubAssetsCO<TObject>(string location, Action<TObject[]> cb)
        {
            var operation = HandleGet<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                ResPackage package = null;
                yield return GetAutoPackageCO(location, ya => package = ya);
                if (package is null)
                {
                    cb?.Invoke(null);
                    yield break;
                }

                operation = package.LoadSubAssetsAsync<GameObject>(location);
                var check = false;
                yield return LoadCheckOPCo(operation, ya => check = ya);
                if (!check)
                {
                    cb?.Invoke(null);
                    yield break;
                }

                HandleAdd(location, operation);
            }

            cb?.Invoke(operation.GetSubAssetObjects<TObject>());
        }

        public override IEnumerator LoadSubAssetsCO(string location, Type type, Action<Object[]> cb)
        {
            var operation = HandleGet<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                ResPackage package = null;
                yield return GetAutoPackageCO(location, ya => package = ya);
                if (package is null)
                {
                    cb?.Invoke(null);
                    yield break;
                }

                operation = package.LoadSubAssetsAsync(location, type);
                var check = false;
                yield return LoadCheckOPCo(operation, ya => check = ya);
                if (!check)
                {
                    cb?.Invoke(null);
                    yield break;
                }

                HandleAdd(location, operation);
            }

            cb?.Invoke(operation.AllAssetObjects);
        }

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override TObject[] LoadSubAssetsSync<TObject>(string location)
        {
            var operation = HandleGet<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(location);
                if (package is null) return null;
                operation = package.LoadSubAssetsSync<TObject>(location);
                if (!LoadCheckOPSync(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation.GetSubAssetObjects<TObject>();
        }

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public override Object[] LoadSubAssetsSync(string location, Type type)
        {
            var operation = HandleGet<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(location);
                if (package is null) return null;
                operation = package.LoadSubAssetsSync(location, type);
                if (!LoadCheckOPSync(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation.AllAssetObjects;
        }


        public override async Task<TObject[]> LoadSubAssetsTask<TObject>(string location)
        {
            var operation = HandleGet<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return null;
                operation = package.LoadSubAssetsAsync<TObject>(location);
                if (!await LoadCheckOPTask(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation.GetSubAssetObjects<TObject>();
        }

        public override async Task<Object[]> LoadSubAssetsTask(string location, Type type)
        {
            var operation = HandleGet<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return null;
                operation = package.LoadSubAssetsSync(location, type);
                if (!await LoadCheckOPTask(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation.AllAssetObjects;
        }

        #endregion
    }
}
#endif