/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-21
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
        public static async void InstGameObject(string location, Transform parent, Action<GameObject> cb)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadAssetAsync<GameObject>(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.InstantiateSync(parent));
        }

        public static async void InstGameObject(string location, Action<GameObject> cb)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadAssetAsync<GameObject>(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.InstantiateSync());
        }

        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets<TObject>(string location, Action<TObject[]> cb) where TObject : Object
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadSubAssetsAsync<TObject>(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.GetSubAssetObjects<TObject>());
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets(AssetInfo location, Action<Object[]> cb)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadSubAssetsAsync(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.AllAssetObjects);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets(string location, Type type, Action<Object[]> cb)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadSubAssetsAsync(location, type);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.AllAssetObjects);
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset<TObject>(string location, Action<TObject> cb) where TObject : Object
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadAssetAsync<TObject>(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.GetAssetObject<TObject>());
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset(string location, Type type, Action<Object> cb)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadAssetAsync(location, type);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.AssetObject);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset(AssetInfo location, Action<Object> cb)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadAssetAsync(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.AssetObject);
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
        /// <param name="cb">回调</param>
        public static async void LoadScene(
            Action<Scene> cb,
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            var operation = GetHandle<SceneOperationHandle>(location);
            if (operation != null)
            {
                var handle = operation.UnloadAsync();
                await handle.Task;
                FreeHandle(location);
            }

            var package = await GetAutoPackageTask(location);
            if (package is null) throw new Exception(
                $"The scenario configuration is incorrect : {location} {sceneMode}");

            operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            if (!await LoadCheckOPTask(operation))
                throw new Exception(
                    $"Loading scenario resources is abnormal : {package.PackageName} {location} {sceneMode}");
            AddHandle(location, operation);

            cb?.Invoke(operation.SceneObject);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        /// <param name="cb">回调</param>
        public static async void LoadScene(
            Action<Scene> cb,
            AssetInfo location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            var operation = GetHandle<SceneOperationHandle>(location);
            if (operation != null)
            {
                var handle = operation.UnloadAsync();
                await handle.Task;
                FreeHandle(location);
            }

            var package = await GetAutoPackageTask(location);
            if (package is null)
                throw new Exception(
                    $"The scenario configuration is incorrect : {location} {sceneMode}");

            operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            if (!await LoadCheckOPTask(operation))
                throw new Exception(
                    $"Loading scenario resources is abnormal : {package.PackageName} {location} {sceneMode}");
            AddHandle(location, operation);

            cb?.Invoke(operation.SceneObject);
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static async void LoadRawFileData(AssetInfo location, Action<byte[]> cb)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.GetRawFileData());
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadRawFileData(string location, Action<byte[]> cb)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.GetRawFileData());
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static async void LoadRawFileText(AssetInfo location, Action<string> cb)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.GetRawFileText());
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadRawFileText(string location, Action<string> cb)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null)
                {
                    cb?.Invoke(null);
                    return;
                }

                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation))
                {
                    cb?.Invoke(null);
                    return;
                }

                AddHandle(location, operation);
            }

            cb?.Invoke(operation.GetRawFileText());
        }

        #endregion
    }
}
#endif