/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        public override GameObject InstGameObject(string location, Transform parent)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(location);
                if (package is null) return null;
                operation = package.LoadAssetSync<GameObject>(location);
                if (!LoadCheckOPSync(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation.InstantiateSync(parent);
        }

        public override async Task<GameObject> InstGameObjectTask(string location, Transform parent)
        {
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return null;
                operation = package.LoadAssetAsync<GameObject>(location);
                if (!await LoadCheckOPTask(operation)) return null;
                HandleAdd(location, operation);
            }

            return operation.InstantiateSync(parent);
        }

        public override IEnumerator InstGameObjectCO(string location, Action<GameObject> cb, Transform parent)
        {
            if (cb is null) yield break;
            var operation = HandleGet<AssetOperationHandle>(location);
            if (operation is null)
            {
                ResPackage package = null;
                yield return GetAutoPackageCO(location, ya => package = ya);
                if (package is null)
                {
                    cb.Invoke(null);
                    yield break;
                }

                operation = package.LoadAssetAsync<GameObject>(location);
                var check = false;
                yield return LoadCheckOPCo(operation, ya => check = ya);
                if (!check)
                {
                    cb.Invoke(null);
                    yield break;
                }

                HandleAdd(location, operation);
            }

            cb.Invoke(operation.InstantiateSync(parent));
        }
    }
}
#endif