#if SUPPORT_UNITASK
#define UNITASK
#endif

#if SUPPORT_YOOASSET
#define YOOASSET
#endif


#if UNITASK
using Cysharp.Threading.Tasks;
using ATask = Cysharp.Threading.Tasks.UniTask;
using ATaskObject = Cysharp.Threading.Tasks.UniTask<UnityEngine.Object>;
using ATaskGameObject = Cysharp.Threading.Tasks.UniTask<UnityEngine.GameObject>;
using ATaskObjectArray = Cysharp.Threading.Tasks.UniTask<UnityEngine.Object[]>;
using ATaskScene = Cysharp.Threading.Tasks.UniTask<UnityEngine.SceneManagement.Scene>;
using ATaskString = Cysharp.Threading.Tasks.UniTask<System.String>;
using ATaskByteArray = Cysharp.Threading.Tasks.UniTask<System.Byte[]>;
using ATaskBoolean = Cysharp.Threading.Tasks.UniTask<System.Boolean>;
#else
using System.Threading.Tasks;
using ATask = System.Threading.Tasks.Task;
using ATaskObject = System.Threading.Tasks.Task<UnityEngine.Object>;
using ATaskGameObject = System.Threading.Tasks.Task<UnityEngine.GameObject>;
using ATaskObjectArray = System.Threading.Tasks.Task<UnityEngine.Object[]>;
using ATaskScene = System.Threading.Tasks.Task<UnityEngine.SceneManagement.Scene>;
using ATaskString = System.Threading.Tasks.Task<System.String>;
using ATaskByteArray = System.Threading.Tasks.Task<System.Byte[]>;
using ATaskBoolean = System.Threading.Tasks.Task<System.Boolean>;
#endif

#if YOOASSET
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        public static async ATaskGameObject InstGameObjectTask(string packagename, string location, Transform parent)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(packagename, location);
                if (package is null) return null;
                operation = package.LoadAssetAsync<GameObject>(location);
                if (!await LoadCheckOPTask(operation)) return null;
                AddHandle(location, operation);
            }

            return operation.InstantiateSync(parent);
        }

        public static async ATaskGameObject InstGameObjectTask(string packagename, string location)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
#if UNITASK
        public static async UniTask<TObject[]> LoadSubAssetsTask<TObject>(string packagename, string location) where TObject : Object
#else
        public static async Task<TObject[]> LoadSubAssetsTask<TObject>(string packagename, string location)
            where TObject : Object
#endif
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public static async ATaskObjectArray LoadSubAssetsTask(string packagename, string location, Type type)
        {
            var operation = GetHandle<SubAssetsOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(packagename, location);
                if (package is null) return null;
                operation = package.LoadSubAssetsAsync(location, type);
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
        /// <param name="packagename">包名</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
#if UNITASK
        public static async UniTask<TObject> LoadAssetTask<TObject>(string packagename, string location) where TObject : Object
#else
        public static async Task<TObject> LoadAssetTask<TObject>(string packagename, string location)
            where TObject : Object
#endif
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public static async ATaskObject LoadAssetTask(string packagename, string location, Type type)
        {
            var operation = GetHandle<AssetOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(packagename, location);
                if (package is null) return null;
                operation = package.LoadAssetAsync(location, type);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public static async ATaskScene LoadSceneTask(
            string packagename,
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

            var package = await GetAutoPackageTask(packagename, location);
            if (package is null)
                throw new Exception(string.Format("场景配置 异常错误:{0} {1} {2}", packagename, location, sceneMode));

            operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            if (!await LoadCheckOPTask(operation))
                throw new Exception(
                    string.Format("加载场景 资源异常:{0} {1} {2}", package.PackageName, location, sceneMode));
            AddHandle(location, operation);

            return operation.SceneObject;
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        public static async ATaskByteArray LoadRawFileDataTask(string packagename, string location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(packagename, location);
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
        /// <param name="packagename">包名</param>
        /// <param name="location">资源的定位地址</param>
        public static async ATaskString LoadRawFileTextTask(string packagename, string location)
        {
            var operation = GetHandle<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(packagename, location);
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