/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-15
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
        #region 实例化GameObject

        public static async void InstGameObject(string packagename, string location, Transform parent, Action<GameObject> cb)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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

        public static async void InstGameObject(string packagename, string location, Action<GameObject> cb)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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

        #endregion

        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets<TObject>(string packagename, string location, Action<TObject[]> cb) where TObject : Object
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets(string packagename, AssetInfo location, Action<Object[]> cb)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets(string packagename, string location, Type type, Action<Object[]> cb)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset<TObject>(string packagename, string location, Action<TObject> cb) where TObject : Object
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset(string packagename, string location, Type type, Action<Object> cb)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset(string packagename, AssetInfo location, Action<Object> cb)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">场景的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public static async void LoadScene(
            Action<Scene> cb,
            string packagename,
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            var operation = GetHandle<SceneOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
                if (package is null) throw new Exception(string.Format("场景配置 异常错误:{0} {1} {2}", package.PackageName, location, sceneMode));

                operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
                if (!await LoadCheckOPTask(operation)) throw new Exception(string.Format("加载场景 资源异常:{0} {1} {2}", package.PackageName, location, sceneMode));
                AddHandle(location, operation);
            }

            cb?.Invoke(operation.SceneObject);
        }

        /// <summary>
        /// 协程加载场景
        /// </summary>
        /// <param name="cb">回调</param>
        /// <param name="packagename">包名</param>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public static async void LoadScene(
            Action<Scene> cb,
            string packagename,
            AssetInfo location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            var operation = GetHandle<SceneOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
                if (package is null) throw new Exception(string.Format("场景配置 异常错误:{0} {1} {2}", package.PackageName, location, sceneMode));

                operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
                if (!await LoadCheckOPTask(operation)) throw new Exception(string.Format("加载场景 资源异常:{0} {1} {2}", package.PackageName, location, sceneMode));
                AddHandle(location, operation);
            }

            cb?.Invoke(operation.SceneObject);
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static async void LoadRawFileData(string packagename, AssetInfo location, Action<byte[]> cb)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadRawFileData(string packagename, string location, Action<byte[]> cb)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static async void LoadRawFileText(string packagename, AssetInfo location, Action<string> cb)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadRawFileText(string packagename, string location, Action<string> cb)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPakcageTask(packagename, location);
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
