/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#if SUPPORT_YOOASSET
using YooAsset;
namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        public static void InstGameObjectDefault(string location, Transform parent, Action<GameObject> cb)
        {
            InstGameObject(DefaultPackageName, location, parent, cb);
        }

        public static void InstGameObjectDefault(string location, Action<GameObject> cb)
        {
            InstGameObject(DefaultPackageName, location, cb);
        }

        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static void LoadSubAssetsDefault<TObject>(string location, Action<TObject[]> cb) where TObject : Object
        {
            LoadSubAssets(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static void LoadSubAssetsDefault(AssetInfo location, Action<Object[]> cb)
        {
            LoadSubAssets(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public static void LoadSubAssetsDefault(string location, Type type, Action<Object[]> cb)
        {
            LoadSubAssets(DefaultPackageName, location, type, cb);
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static void LoadAssetDefault<TObject>(string location, Action<TObject> cb) where TObject : Object
        {
            LoadAsset(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public static void LoadAssetDefault(string location, Type type, Action<Object> cb)
        {
            LoadAsset(DefaultPackageName, location, type, cb);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static void LoadAssetDefault(AssetInfo location, Action<Object> cb)
        {
            LoadAsset(DefaultPackageName, location, cb);
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
        public static void LoadSceneDefault(
            Action<Scene> cb,
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            LoadScene(cb, DefaultPackageName, location, sceneMode, suspendLoad, priority);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        /// <param name="cb">回调</param>
        public static void LoadSceneDefault(
            Action<Scene> cb,
            AssetInfo location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            LoadScene(cb, DefaultPackageName, location, sceneMode, suspendLoad, priority);
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static void LoadRawFileDataDefault(AssetInfo location, Action<byte[]> cb)
        {
            LoadRawFileData(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static void LoadRawFileDataDefault(string location, Action<byte[]> cb)
        {
            LoadRawFileData(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源信息</param>
        /// <param name="cb">回调</param>
        public static void LoadRawFileTextDefault(AssetInfo location, Action<string> cb)
        {
            LoadRawFileText(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static void LoadRawFileTextDefault(string location, Action<string> cb)
        {
            LoadRawFileText(DefaultPackageName, location, cb);
        }

        #endregion
    }
}
#endif
