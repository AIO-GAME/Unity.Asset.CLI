﻿/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-11
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        public static GameObject InstGameObject(string packagename, string location, Transform parent)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(packagename, location);
                if (package is null) return null;
                operation = package.LoadAssetSync<GameObject>(location);
                if (!LoadCheckOPSync(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.InstantiateSync(parent);
        }

        public static GameObject InstGameObject(string packagename, string location)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(packagename, location);
                if (package is null) return null;
                operation = package.LoadAssetSync<GameObject>(location);
                if (!LoadCheckOPSync(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.InstantiateSync();
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public static TObject[] LoadSubAssets<TObject>(in string packagename, in string location) where TObject : Object
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(packagename, location);
                if (package is null) return null;
                operation = package.LoadSubAssetsSync<TObject>(location);
                if (!LoadCheckOPSync(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetSubAssetObjects<TObject>();
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public static Object[] LoadSubAssets(in string packagename, in string location, in Type type)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(packagename, location);
                if (package is null) return null;
                operation = package.LoadSubAssetsSync(location, type);
                if (!LoadCheckOPSync(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.AllAssetObjects;
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public static TObject LoadAsset<TObject>(in string packagename, in string location) where TObject : Object
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(packagename, location);
                if (package is null) return null;
                operation = package.LoadAssetSync<TObject>(location);
                if (!LoadCheckOPSync(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetAssetObject<TObject>();
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public static Object LoadAsset(in string packagename, in string location, in Type type)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(packagename, location);
                if (package is null) return null;
                operation = package.LoadAssetAsync(location, type);
                if (!LoadCheckOPSync(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.AssetObject;
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        public static string LoadRawFileText(in string packagename, in string location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(packagename, location);
                if (package is null) return null;
                operation = package.LoadRawFileSync(location);
                if (!LoadCheckOPSync(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetRawFileText();
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        public static byte[] LoadRawFileData(in string packagename, in string location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(packagename, location);
                if (package is null) return null;
                operation = package.LoadRawFileSync(location);
                if (!LoadCheckOPSync(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetRawFileData();
        }
    }
}
#endif
