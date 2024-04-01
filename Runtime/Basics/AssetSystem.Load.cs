using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AIO
{
    partial class AssetSystem
    {
        #region 子资源加载

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void LoadSubAssets(string location, Action<Object[]> cb)
        {
            Proxy.LoadSubAssetsTask<Object>(SettingToLocalPath(location)).
                  ContinueWith(task => { cb?.Invoke(task.Result); });
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="type">子对象类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void LoadSubAssets(string location, Type type, Action<Object[]> cb)
        {
            Proxy.LoadSubAssetsTask(SettingToLocalPath(location), type).
                  ContinueWith(task => { cb?.Invoke(task.Result); });
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void LoadSubAssets<TObject>(string location, Action<TObject[]> cb) where TObject : Object
        {
            Proxy.LoadSubAssetsTask<TObject>(SettingToLocalPath(location)).
                  ContinueWith(task => { cb?.Invoke(task.Result); });
        }

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadSubAssetsCO<TObject>(string location, Action<TObject[]> cb) where TObject : Object
        {
            return Proxy.LoadSubAssetsCO(SettingToLocalPath(location), cb);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadSubAssetsCO(string location, Type type, Action<Object[]> cb)
        {
            return Proxy.LoadSubAssetsCO(SettingToLocalPath(location), type, cb);
        }

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static TObject[] LoadSubAssets<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadSubAssetsSync<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Object[] LoadSubAssets(string location, Type type)
        {
            return Proxy.LoadSubAssetsSync(SettingToLocalPath(location), type);
        }

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Object[] LoadSubAssets(string location)
        {
            return Proxy.LoadSubAssetsSync(SettingToLocalPath(location), typeof(Object));
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<TObject[]> LoadSubAssetsTask<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadSubAssetsTask<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<Object[]> LoadSubAssetsTask(string location, Type type)
        {
            return Proxy.LoadSubAssetsTask(SettingToLocalPath(location), type);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<Object[]> LoadSubAssetsTask(string location)
        {
            return Proxy.LoadSubAssetsTask(SettingToLocalPath(location), typeof(Object));
        }

        #endregion

        #region 资源加载

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset<TObject>(string location, Action<TObject> cb) where TObject : Object
        {
            await CreateLoadAssetHandle(location, cb);
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset(string location, Action<Object> cb)
        {
            await CreateLoadAssetHandle(location, cb);
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset(string location, Type type, Action<Object> cb)
        {
            await CreateLoadAssetHandle(location, cb);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<TObject> LoadAssetCO<TObject>(string location) where TObject : Object
        {
            return CreateLoadAssetHandle<TObject>(location);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<TObject> LoadAssetCO<TObject>(string location, Action<TObject> cb)
            where TObject : Object
        {
            return CreateLoadAssetHandle(location, cb);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle LoadAssetCO(string location, Action<Object> cb)
        {
            return CreateLoadAssetHandle(location, cb);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle LoadAssetCO(string location)
        {
            return CreateLoadAssetHandle(location);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle LoadAssetCO(string location, Type type, Action<Object> cb)
        {
            return CreateLoadAssetHandle(location, type, cb);
        }

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static TObject LoadAsset<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadAssetSync<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Object LoadAsset(string location, Type type)
        {
            return Proxy.LoadAssetSync(SettingToLocalPath(location), type);
        }

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Object LoadAsset(string location)
        {
            return Proxy.LoadAssetSync(SettingToLocalPath(location), typeof(Object));
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<TObject> LoadAssetTask<TObject>(string location) where TObject : Object
        {
            return CreateLoadAssetHandle<TObject>(location);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle LoadAssetTask(string location, Type type)
        {
            return CreateLoadAssetHandle(location, type);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle LoadAssetTask(string location)
        {
            return CreateLoadAssetHandle(location);
        }

        #endregion

        #region 场景加载

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadScene(
            string        location,
            Action<Scene> cb,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100)
        {
            var scene = await Proxy.LoadSceneTask(
                SettingToLocalPath(location),
                sceneMode,
                suspendLoad,
                priority);
            cb?.Invoke(scene);
        }

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadSceneCO(
            string        location,
            Action<Scene> cb,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100)
        {
            return Proxy.LoadSceneCO(
                SettingToLocalPath(location),
                cb,
                sceneMode,
                suspendLoad,
                priority);
        }

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<Scene> LoadSceneTask(
            string        location,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100)
        {
            return Proxy.LoadSceneTask(
                SettingToLocalPath(location),
                sceneMode,
                suspendLoad,
                priority);
        }

        #endregion

        #region 原生文件

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static string LoadRawFileText(string location)
        {
            return Proxy.LoadRawFileTextSync(SettingToLocalPath(location));
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="cb">回调</param>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadRawFileText(string location, Action<string> cb)
        {
            cb?.Invoke(await Proxy.LoadRawFileTextTask(SettingToLocalPath(location)));
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<string> LoadRawFileTextTask(string location)
        {
            return Proxy.LoadRawFileTextTask(SettingToLocalPath(location));
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="cb">回调</param>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadRawFileData(string location, Action<byte[]> cb)
        {
            cb?.Invoke(await Proxy.LoadRawFileDataTask(SettingToLocalPath(location)));
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static byte[] LoadRawFileData(string location)
        {
            return Proxy.LoadRawFileDataSync(SettingToLocalPath(location));
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<byte[]> LoadRawFileDataTask(string location)
        {
            return Proxy.LoadRawFileDataTask(SettingToLocalPath(location));
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadRawFileDataCO(string location, Action<byte[]> cb)
        {
            return Proxy.LoadRawFileDataCO(SettingToLocalPath(location), cb);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadRawFileTextCO(string location, Action<string> cb)
        {
            return Proxy.LoadRawFileTextCO(SettingToLocalPath(location), cb);
        }

        #endregion
    }
}