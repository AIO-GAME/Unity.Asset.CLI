#region

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;
using Scene = UnityEngine.SceneManagement.Scene;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

#endregion

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
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static TObject[] LoadSubAssets<TObject>(string location)
        where TObject : Object => Proxy.LoadSubAssets<TObject>(location, typeof(TObject)).Invoke();

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Object[] LoadSubAssets(string location, Type type)
            => Proxy.LoadSubAssets<Object>(location, type).Invoke();

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Object[] LoadSubAssets(string location)
            => Proxy.LoadSubAssets<Object>(location, typeof(Object)).Invoke();

        #endregion

        #region 异步加载原生文件

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static void LoadSubAssets<TObject>(string location, Action<TObject[]> completed)
        where TObject : Object => Proxy.LoadSubAssets(location, typeof(TObject), completed).Invoke();

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<TObject[]> LoadSubAssetsTask<TObject>(string location)
        where TObject : Object => Proxy.LoadSubAssets<TObject>(location, typeof(TObject));

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<TObject[]> LoadSubAssetsTask<TObject>(string location, Action<Object[]> completed)
        where TObject : Object => Proxy.LoadSubAssets<TObject>(location, typeof(TObject), completed);

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="type">子对象类型</param>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadSubAssets(string location, Type type, Action<Object[]> completed)
            => await Proxy.LoadSubAssets(location, type, completed);

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadSubAssets(string location, Action<Object[]> completed)
            => await Proxy.LoadSubAssets(location, typeof(Object), completed);

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object[]> LoadSubAssetsTask(string location, Type type)
            => Proxy.LoadSubAssets<Object>(location, type);

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="type">子对象类型</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object[]> LoadSubAssetsTask(string location, Type type, Action<Object[]> completed)
            => Proxy.LoadSubAssets(location, type, completed);

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object[]> LoadSubAssetsTask(string location, Action<Object[]> completed)
            => Proxy.LoadSubAssets(location, typeof(Object), completed);

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object[]> LoadSubAssetsTask(string location)
            => Proxy.LoadSubAssets<Object>(location, typeof(Object));

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
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadScene(
            string        location,
            Action<Scene> completed,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100
        ) => await Proxy.LoadScene(location, completed, sceneMode, suspendLoad, priority);

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadScene(
            string        location,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100
        ) => await Proxy.LoadScene(location, null, sceneMode, suspendLoad, priority);

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Scene> LoadSceneTask(
            string        location,
            Action<Scene> completed,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100
        ) => Proxy.LoadScene(location, completed, sceneMode, suspendLoad, priority);

        /// <summary>
        ///     异步加载场景
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Scene> LoadSceneTask(
            string        location,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100
        ) => Proxy.LoadScene(location, null, sceneMode, suspendLoad, priority);

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
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<TObject> LoadAssetTask<TObject>(string location)
        where TObject : Object => Proxy.LoadAsset<TObject>(location, typeof(TObject));

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<TObject> LoadAssetTask<TObject>(string location, Action<TObject> completed)
        where TObject : Object => Proxy.LoadAsset(location, typeof(TObject), completed);

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object> LoadAssetTask(string location, Type type)
            => Proxy.LoadAsset<Object>(location, type);

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="type">资源类型</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object> LoadAssetTask(string location, Type type, Action<Object> completed)
            => Proxy.LoadAsset(location, type, completed);

        /// <summary>
        ///     异步加载资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope, CanBeNull]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<Object> LoadAssetTask(string location)
            => Proxy.LoadAsset<Object>(location, typeof(Object));

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAsset<TObject>(string location, Action<TObject> completed)
        where TObject : Object => await Proxy.LoadAsset(location, typeof(TObject), completed);

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAsset(string location, Action<Object> completed)
            => await Proxy.LoadAsset(location, typeof(Object), completed);

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAsset(string location, Type type, Action<Object> completed)
            => await Proxy.LoadAsset(location, type, completed);

        #endregion

        #region 同步加载

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static TObject LoadAsset<TObject>(string location)
        where TObject : Object => Proxy.LoadAsset<TObject>(location, typeof(TObject)).Invoke();

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Object LoadAsset(string location, Type type)
            => Proxy.LoadAsset<Object>(location, type).Invoke();

        /// <summary>
        ///     同步加载资源对象
        /// </summary>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Object LoadAsset(string location)
            => Proxy.LoadAsset<Object>(location, typeof(Object)).Invoke();

        #endregion
    }

    #endregion

    #region 原生文件 文本

    partial class AssetSystem
    {
        #region 同步加载

        /// <summary>
        ///     同步加载原生文件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static string LoadRawFileText(string location)
            => Proxy.LoadRawFileText(location).Invoke();

        #endregion

        #region 异步加载

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="completed">回调</param>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadRawFileText(string location, Action<string> completed)
            => await Proxy.LoadRawFileText(location, completed);

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<string> LoadRawFileTextTask(string location)
            => Proxy.LoadRawFileText(location);

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="completed">回调</param>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<string> LoadRawFileTextTask(string location, Action<string> completed)
            => Proxy.LoadRawFileText(location, completed);

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
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static byte[] LoadRawFileData(string location)
            => Proxy.LoadRawFileData(location).Invoke();

        #endregion

        #region 异步加载

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="completed">回调</param>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadRawFileData(string location, Action<byte[]> completed)
            => await Proxy.LoadRawFileData(location, completed);

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<byte[]> LoadRawFileDataTask(string location)
            => Proxy.LoadRawFileData(location);

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandle<byte[]> LoadRawFileDataTask(string location, Action<byte[]> completed)
            => Proxy.LoadRawFileData(location, completed);

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
        /// <param name="location">可寻址路径</param>
        /// <param name="parent">父级节点</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static GameObject InstGameObject(string location, Transform parent)
            => Proxy.InstGameObject(location, null, parent).Invoke();

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static GameObject InstGameObject(string location)
            => Proxy.InstGameObject(location).Invoke();

        #endregion

        #region 异步实例化

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">可寻址路径</param>
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
            => await Proxy.InstGameObject(location, completed, parent);

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void InstGameObject(string location, Action<GameObject> completed)
            => await Proxy.InstGameObject(location, completed);

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="parent">父级节点</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<GameObject> InstGameObjectTask(string location, Transform parent)
            => Proxy.InstGameObject(location, null, parent);

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<GameObject> InstGameObjectTask(string location)
            => Proxy.InstGameObject(location);

        /// <summary>
        ///     实例预制件
        /// </summary>
        /// <param name="location">可寻址路径</param>
        /// <param name="completed">回调</param>
        /// <returns>
        ///     <see cref="UnityEngine.GameObject" />
        /// </returns>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<GameObject> InstGameObjectTask(string location, Action<GameObject> completed)
            => Proxy.InstGameObject(location, completed);

        #endregion
    }

    #endregion
}