using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static TObject[] LoadSubAssets<TObject>(string location)
        where TObject : Object
        {
            return LoaderHandleLoadSubAsset<TObject>.Create(location).Invoke();
        }

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Object[] LoadSubAssets(string location, Type type)
        {
            return LoaderHandleLoadSubAsset.Create(location, type).Invoke();
        }

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Object[] LoadSubAssets(string location)
        {
            return LoaderHandleLoadSubAsset.Create(location).Invoke();
        }

        #endregion

        #region 异步加载原生文件

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadSubAssets<TObject>(string location, Action<TObject[]> completed)
        where TObject : Object
        {
            await LoaderHandleLoadSubAsset<TObject>.Create(location, completed);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandleList<TObject> LoadSubAssetsTask<TObject>(string location)
        where TObject : Object
        {
            return LoaderHandleLoadSubAsset<TObject>.Create(location);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="type">子对象类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadSubAssets(string location, Type type, Action<Object[]> completed)
        {
            await LoaderHandleLoadSubAsset.Create(location, type, completed);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadSubAssets(string location, Action<Object[]> completed)
        {
            await LoaderHandleLoadSubAsset.Create(location, completed);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandleList<Object> LoadSubAssetsTask(string location, Type type)
        {
            return LoaderHandleLoadSubAsset.Create(location, type);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandleList<Object> LoadSubAssetsTask(string location, Type type, Action<Object[]> completed)
        {
            return LoaderHandleLoadSubAsset.Create(location, type, completed);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandleList<Object> LoadSubAssetsTask(string location, Action<Object[]> completed)
        {
            return LoaderHandleLoadSubAsset.Create(location, completed);
        }

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandleList<Object> LoadSubAssetsTask(string location)
        {
            return LoaderHandleLoadSubAsset.Create(location);
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
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadScene(string location, Action<Scene> completed, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            await LoaderHandleLoadScene.Create(location, sceneMode, suspendLoad, priority, completed);
        }

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadScene(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            await LoaderHandleLoadScene.Create(location, sceneMode, suspendLoad, priority);
        }

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Scene> LoadSceneTask(string location, Action<Scene> completed, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            return LoaderHandleLoadScene.Create(location, sceneMode, suspendLoad, priority, completed);
        }

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Scene> LoadSceneTask(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            return LoaderHandleLoadScene.Create(location, sceneMode, suspendLoad, priority);
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
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<TObject> LoadAssetTask<TObject>(string location)
        where TObject : Object
        {
            return LoaderHandleLoadAsset<TObject>.Create(location);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<TObject> LoadAssetTask<TObject>(string location, Action<TObject> completed)
        where TObject : Object
        {
            return LoaderHandleLoadAsset<TObject>.Create(location, completed);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object> LoadAssetTask(string location, Type type)
        {
            return LoaderHandleLoadAsset.Create(location, type);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object> LoadAssetTask(string location, Type type, Action<Object> completed)
        {
            return LoaderHandleLoadAsset.Create(location, type, completed);
        }

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object> LoadAssetTask(string location)
        {
            return LoaderHandleLoadAsset.Create(location);
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAsset<TObject>(string location, Action<TObject> completed)
        where TObject : Object
        {
            await LoaderHandleLoadAsset<TObject>.Create(location, completed);
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAsset(string location, Action<Object> completed)
        {
            await LoaderHandleLoadAsset.Create(location, completed);
        }

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAsset(string location, Type type, Action<Object> completed)
        {
            await LoaderHandleLoadAsset.Create(location, type, completed);
        }

        #endregion

        #region 同步加载

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static TObject LoadAsset<TObject>(string location)
        where TObject : Object
        {
            return LoaderHandleLoadAsset<TObject>.Create(location).Invoke();
        }

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Object LoadAsset(string location, Type type)
        {
            return LoaderHandleLoadAsset.Create(location, type).Invoke();
        }

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Object LoadAsset(string location)
        {
            return LoaderHandleLoadAsset.Create(location).Invoke();
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
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadRawFileText(string location, Action<string> completed)
        {
            await LoaderHandleLoadRawFileText.Create(location, completed);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<string> LoadRawFileTextTask(string location)
        {
            return LoaderHandleLoadRawFileText.Create(location);
        }

        #endregion

        #region 同步加载

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static string LoadRawFileText(string location)
        {
            return LoaderHandleLoadRawFileText.Create(location).Invoke();
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
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static byte[] LoadRawFileData(string location)
        {
            return LoaderHandleLoadRawFileData.Create(location).Invoke();
        }

        #endregion

        #region 异步加载

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="completed">回调</param>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadRawFileData(string location, Action<byte[]> completed)
        {
            await LoaderHandleLoadRawFileData.Create(location, completed);
        }

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<byte[]> LoadRawFileDataTask(string location)
        {
            return LoaderHandleLoadRawFileData.Create(location);
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
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static GameObject InstGameObject(string location, Transform parent)
        {
            return LoaderHandleInstGameObject.Create(location, parent).Invoke();
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static GameObject InstGameObject(string location)
        {
            return LoaderHandleInstGameObject.Create(location).Invoke();
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
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void InstGameObject(string location, Transform parent, Action<GameObject> completed)
        {
            await LoaderHandleInstGameObject.Create(location, completed, parent);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void InstGameObject(string location, Action<GameObject> completed)
        {
            await LoaderHandleInstGameObject.Create(location, completed);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<GameObject> InstGameObjectTask(string location, Transform parent)
        {
            return LoaderHandleInstGameObject.Create(location, parent);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<GameObject> InstGameObjectTask(string location)
        {
            return LoaderHandleInstGameObject.Create(location);
        }

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<GameObject> InstGameObjectTask(string location, Action<GameObject> completed)
        {
            return LoaderHandleInstGameObject.Create(location, completed);
        }

        #endregion
    }

    #endregion
}