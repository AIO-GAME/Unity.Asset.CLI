/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-21
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        public static async Task<GameObject> InstGameObjectTask(string location, Transform parent)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadAssetAsync<GameObject>(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.InstantiateSync(parent);
        }

        public static async Task<GameObject> InstGameObjectTask(string location)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadAssetAsync<GameObject>(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.InstantiateSync();
        }

        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public static async Task<TObject[]> LoadSubAssetsTask<TObject>(string location) where TObject : Object
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadSubAssetsAsync<TObject>(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetSubAssetObjects<TObject>();
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源信息</param>
        public static async Task<Object[]> LoadSubAssetsTask(AssetInfo location)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadSubAssetsAsync(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.AllAssetObjects;
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public static async Task<Object[]> LoadSubAssetsTask(string location, Type type)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadSubAssetsSync(location, type);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.AllAssetObjects;
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public static async Task<TObject> LoadAssetTask<TObject>(string location) where TObject : Object
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadAssetAsync<TObject>(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }


            return operation.GetAssetObject<TObject>();
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public static async Task<Object> LoadAssetTask(string location, Type type)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadAssetAsync(location, type);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.AssetObject;
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源信息</param>
        public static async Task<Object> LoadAssetTask(AssetInfo location)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadAssetAsync(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.AssetObject;
        }

        #endregion

        #region 场景加载

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public static async Task<Scene> LoadSceneTask(
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            var operation = GetHandle<SceneOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) throw new Exception(string.Format("场景配置 异常错误:{0} {1} {2}", package.PackageName, location, sceneMode));

                operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
                if (!await LoadCheckOPTask(operation)) throw new Exception(string.Format("加载场景 资源异常:{0} {1} {2}", package.PackageName, location, sceneMode));
                AddHandle(location, operation);
            }

            return operation.SceneObject;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public static async Task<Scene> LoadSceneTask(
            AssetInfo location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            var operation = GetHandle<SceneOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) throw new Exception(string.Format("场景配置 异常错误:{0} {1} {2}", package.PackageName, location, sceneMode));

                operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
                if (!await LoadCheckOPTask(operation)) throw new Exception(string.Format("加载场景 资源异常:{0} {1} {2}", package.PackageName, location, sceneMode));
                AddHandle(location, operation);
            }

            return operation.SceneObject;
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源信息</param>
        public static async Task<Byte[]> LoadRawFileDataTask(AssetInfo location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetRawFileData();
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static async Task<Byte[]> LoadRawFileDataTask(string location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetRawFileData();
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源信息</param>
        public static async Task<String> LoadRawFileTextTask(AssetInfo location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetRawFileText();
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static async Task<String> LoadRawFileTextTask(string location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(location);
                if (package is null) return null;
                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.GetRawFileText();
        }

        #endregion
    }
}

#endif
