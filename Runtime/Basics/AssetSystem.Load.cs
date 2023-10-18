/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AIO
{
    public static partial class AssetSystem
    {
        #region 子资源加载

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets<TObject>(string location, Action<TObject[]> cb) where TObject : Object
        {
            cb?.Invoke(await Proxy.LoadSubAssetsTask<TObject>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets(string location, Action<Object[]> cb)
        {
            cb?.Invoke(await Proxy.LoadSubAssetsTask<Object>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="type">子对象类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadSubAssets(string location, Type type, Action<Object[]> cb)
        {
            cb?.Invoke(await Proxy.LoadSubAssetsTask(Parameter.LoadPathToLower ? location.ToLower() : location, type));
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadSubAssetsCO<TObject>(string location, Action<TObject[]> cb) where TObject : Object
        {
            return Proxy.LoadSubAssetsCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadSubAssetsCO(string location, Type type, Action<Object[]> cb)
        {
            return Proxy.LoadSubAssetsCO(Parameter.LoadPathToLower ? location.ToLower() : location, type, cb);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public static TObject[] LoadSubAssets<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadSubAssetsSync<TObject>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public static Object[] LoadSubAssets(string location, Type type)
        {
            return Proxy.LoadSubAssetsSync(Parameter.LoadPathToLower ? location.ToLower() : location, type);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Object[] LoadSubAssets(string location)
        {
            return Proxy.LoadSubAssetsSync(Parameter.LoadPathToLower ? location.ToLower() : location, typeof(Object));
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public static Task<TObject[]> LoadSubAssetsTask<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadSubAssetsTask<TObject>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public static Task<Object[]> LoadSubAssetsTask(string location, Type type)
        {
            return Proxy.LoadSubAssetsTask(Parameter.LoadPathToLower ? location.ToLower() : location, type);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<Object[]> LoadSubAssetsTask(string location)
        {
            return Proxy.LoadSubAssetsTask(Parameter.LoadPathToLower ? location.ToLower() : location, typeof(Object));
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset<TObject>(string location, Action<TObject> cb) where TObject : Object
        {
            cb?.Invoke(await Proxy.LoadAssetTask<TObject>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset(string location, Action<Object> cb)
        {
            cb?.Invoke(await Proxy.LoadAssetTask<Object>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadAsset(string location, Type type, Action<Object> cb)
        {
            cb?.Invoke(await Proxy.LoadAssetTask(Parameter.LoadPathToLower ? location.ToLower() : location, type));
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadAssetCO<TObject>(string location, Action<TObject> cb) where TObject : Object
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadAssetCO(string location, Action<Object> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadAssetCO(string location, Type type, Action<Object> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, type, cb);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public static TObject LoadAsset<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadAssetSync<TObject>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public static Object LoadAsset(string location, Type type)
        {
            return Proxy.LoadAssetSync(Parameter.LoadPathToLower ? location.ToLower() : location, type);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Object LoadAsset(string location)
        {
            return Proxy.LoadAssetSync(Parameter.LoadPathToLower ? location.ToLower() : location, typeof(Object));
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public static Task<TObject> LoadAssetTask<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadAssetTask<TObject>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public static Task<Object> LoadAssetTask(string location, Type type)
        {
            return Proxy.LoadAssetTask(Parameter.LoadPathToLower ? location.ToLower() : location, type);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<Object> LoadAssetTask(string location)
        {
            return Proxy.LoadAssetTask(Parameter.LoadPathToLower ? location.ToLower() : location, typeof(Object));
        }

        #endregion

        #region 场景加载

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public static async void LoadScene(
            string location,
            Action<Scene> cb,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            cb?.Invoke(await Proxy.LoadSceneTask(Parameter.LoadPathToLower ? location.ToLower() : location, sceneMode, suspendLoad, priority));
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public static IEnumerator LoadSceneCO(
            string location,
            Action<Scene> cb,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            return Proxy.LoadSceneCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb, sceneMode, suspendLoad, priority);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public static Task<Scene> LoadSceneTask(
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = true,
            int priority = 100)
        {
            return Proxy.LoadSceneTask(Parameter.LoadPathToLower ? location.ToLower() : location, sceneMode, suspendLoad, priority);
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static string LoadRawFileText(string location)
        {
            return Proxy.LoadRawFileTextSync(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="cb">回调</param>
        /// <param name="location">资源的定位地址</param>
        public static async void LoadRawFileText(string location, Action<string> cb)
        {
            cb?.Invoke(await Proxy.LoadRawFileTextTask(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<string> LoadRawFileTextTask(string location)
        {
            return Proxy.LoadRawFileTextTask(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="cb">回调</param>
        /// <param name="location">资源的定位地址</param>
        public static async void LoadRawFileData(string location, Action<byte[]> cb)
        {
            cb?.Invoke(await Proxy.LoadRawFileDataTask(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static byte[] LoadRawFileData(string location)
        {
            return Proxy.LoadRawFileDataSync(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<byte[]> LoadRawFileDataTask(string location)
        {
            return Proxy.LoadRawFileDataTask(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadRawFileDataCO(string location, Action<byte[]> cb)
        {
            return Proxy.LoadRawFileDataCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadRawFileTextCO(string location, Action<string> cb)
        {
            return Proxy.LoadRawFileTextCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

        #endregion
    }
}
