using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Scene = UnityEngine.SceneManagement.Scene;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

namespace AIO
{
    #region 子资源加载

    partial class AssetSystem
    {
        #region 同步加载子资源对象

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static TObject[] LoadSubAssets<TObject>(string location)
        where TObject : Object
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

        #endregion

        #region 异步加载原生文件

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadSubAssets<TObject>(string location, Action<TObject[]> completed)
        where TObject : Object
        {
            await ASHandleLoadSubAsset<TObject>.Create(location, completed);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandleList<TObject> LoadSubAssetsTask<TObject>(string location)
        where TObject : Object
        {
            return ASHandleLoadSubAsset<TObject>.Create(location);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="type">子对象类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadSubAssets(string location, Type type, Action<Object[]> completed)
        {
            await ASHandleLoadSubAsset.Create(location, type, completed);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadSubAssets(string location, Action<Object[]> completed)
        {
            await ASHandleLoadSubAsset.Create(location, completed);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandleList<Object> LoadSubAssetsTask(string location, Type type)
        {
            return ASHandleLoadSubAsset.Create(location, type);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandleList<Object> LoadSubAssetsTask(string location, Type type, Action<Object[]> completed)
        {
            return ASHandleLoadSubAsset.Create(location, type, completed);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandleList<Object> LoadSubAssetsTask(string location, Action<Object[]> completed)
        {
            return ASHandleLoadSubAsset.Create(location, completed);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandleList<Object> LoadSubAssetsTask(string location)
        {
            return ASHandleLoadSubAsset.Create(location);
        }

        #endregion
    }

    #endregion

    #region 场景加载

    partial class AssetSystem
    {
        #region 异步加载

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadScene(string location, Action<Scene> completed, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            await ASHandleLoadScene.Create(location, sceneMode, suspendLoad, priority, completed);
        }

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadScene(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            await ASHandleLoadScene.Create(location, sceneMode, suspendLoad, priority);
        }

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<Scene> LoadSceneTask(string location, Action<Scene> completed, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            return ASHandleLoadScene.Create(location, sceneMode, suspendLoad, priority, completed);
        }

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<Scene> LoadSceneTask(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            return ASHandleLoadScene.Create(location, sceneMode, suspendLoad, priority);
        }

        #endregion
    }

    #endregion

    #region 资源加载

    partial class AssetSystem
    {
        #region 异步加载

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<TObject> LoadAssetTask<TObject>(string location)
        where TObject : Object
        {
            return ASHandleLoadAsset<TObject>.Create(location);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<TObject> LoadAssetTask<TObject>(string location, Action<TObject> completed)
        where TObject : Object
        {
            return ASHandleLoadAsset<TObject>.Create(location, completed);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<Object> LoadAssetTask(string location, Type type)
        {
            return ASHandleLoadAsset.Create(location, type);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<Object> LoadAssetTask(string location, Type type, Action<Object> completed)
        {
            return ASHandleLoadAsset.Create(location, type, completed);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<Object> LoadAssetTask(string location)
        {
            return ASHandleLoadAsset.Create(location);
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset<TObject>(string location, Action<TObject> completed)
        where TObject : Object
        {
            await ASHandleLoadAsset<TObject>.Create(location, completed);
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset(string location, Action<Object> completed)
        {
            await ASHandleLoadAsset.Create(location, completed);
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset(string location, Type type, Action<Object> completed)
        {
            await ASHandleLoadAsset.Create(location, type, completed);
        }

        #endregion

        #region 同步加载

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static TObject LoadAsset<TObject>(string location)
        where TObject : Object
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

        #endregion
    }

    #endregion

    #region 原生文件 文本

    partial class AssetSystem
    {
        #region 异步加载

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="completed">回调</param>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadRawFileText(string location, Action<string> completed)
        {
            await ASHandleLoadRawFileText.Create(location, completed);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<string> LoadRawFileTextTask(string location)
        {
            return ASHandleLoadRawFileText.Create(location);
        }

        #endregion

        #region 同步加载

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static string LoadRawFileText(string location)
        {
            return Proxy.LoadRawFileTextSync(SettingToLocalPath(location));
        }

        #endregion
    }

    #endregion

    #region 原生文件 二进制

    partial class AssetSystem
    {
        #region 同步加载

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static byte[] LoadRawFileData(string location)
        {
            return Proxy.LoadRawFileDataSync(SettingToLocalPath(location));
        }

        #endregion

        #region 异步加载

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="completed">回调</param>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadRawFileData(string location, Action<byte[]> completed)
        {
            await ASHandleLoadRawFileData.Create(location, completed);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<byte[]> LoadRawFileDataTask(string location)
        {
            return ASHandleLoadRawFileData.Create(location);
        }

        #endregion
    }

    #endregion

    #region 实例预制件

    partial class AssetSystem
    {
        #region 同步实例化

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static GameObject InstGameObject(string location, Transform parent)
        {
            return Proxy.InstGameObject(SettingToLocalPath(location), parent);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static GameObject InstGameObject(string location)
        {
            return Proxy.InstGameObject(SettingToLocalPath(location));
        }

        #endregion

        #region 异步实例化

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <param name="completed">回调</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void InstGameObject(string location, Transform parent, Action<GameObject> completed)
        {
            await ASHandleInstGameObject.Create(location, completed, parent);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void InstGameObject(string location, Action<GameObject> completed)
        {
            await ASHandleInstGameObject.Create(location, completed);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<GameObject> InstGameObjectTask(string location, Transform parent)
        {
            return ASHandleInstGameObject.Create(location, parent);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<GameObject> InstGameObjectTask(string location)
        {
            return ASHandleInstGameObject.Create(location);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IHandle<GameObject> InstGameObjectTask(string location, Action<GameObject> completed)
        {
            return ASHandleInstGameObject.Create(location, completed);
        }

        #endregion
    }

    #endregion
}