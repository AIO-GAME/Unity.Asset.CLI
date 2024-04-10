#region

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#endregion

namespace AIO.UEngine
{
    partial class ASProxy
    {
        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract ILoaderHandle<byte[]> LoadRawFileData(string location, Action<byte[]> cb = null);

        /// <summary>
        ///     异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract ILoaderHandle<string> LoadRawFileText(string location, Action<string> cb = null);

        /// <summary>
        ///     加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="completed">回调</param>
        public abstract ILoaderHandle<TObject[]> LoadSubAssets<TObject>(string location, Type type, Action<TObject[]> completed = null)
        where TObject : Object;

        /// <summary>
        ///     加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="completed">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public abstract ILoaderHandle<Scene> LoadScene(
            string        location,
            Action<Scene> completed   = null,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100);

        /// <summary>
        ///     加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <param name="type">资源类型</param>
        public abstract ILoaderHandle<TObject> LoadAsset<TObject>(string location, Type type, Action<TObject> completed = null)
        where TObject : Object;

        /// <summary>
        ///     加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        /// <param name="parent">父位置</param>
        public abstract ILoaderHandle<GameObject> InstGameObject(string location, Action<GameObject> completed = null, Transform parent = null);
    }
}