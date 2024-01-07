/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        public static IEnumerator InstGameObjectDefaultCO(string location, Transform parent, Action<GameObject> cb)
        {
            yield return InstGameObjectCO(DefaultPackageName, location, parent, cb);
        }

        public static IEnumerator InstGameObjectDefaultCO(string location, Action<GameObject> cb)
        {
            yield return InstGameObjectCO(DefaultPackageName, location, cb);
        }

        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadSubAssetsDefaultCO<TObject>(string location, Action<TObject[]> cb)
            where TObject : Object
        {
            yield return LoadSubAssetsCO(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadSubAssetsDefaultCO(string location, Type type, Action<Object[]> cb)
        {
            yield return LoadSubAssetsCO(DefaultPackageName, location, type, cb);
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadAssetDefaultCO<TObject>(string location, Action<TObject> cb)
            where TObject : Object
        {
            yield return LoadAssetCO(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadAssetDefaultCO(string location, Type type, Action<Object> cb)
        {
            yield return LoadAssetCO(DefaultPackageName, location, type, cb);
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
        public static IEnumerator LoadSceneDefaultCO(
            Action<Scene> cb,
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            yield return LoadSceneCO(cb, DefaultPackageName, location, sceneMode, suspendLoad, priority);
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadRawFileDataDefaultCO(string location, Action<byte[]> cb)
        {
            yield return LoadRawFileDataCO(DefaultPackageName, location, cb);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadRawFileTextDefaultCO(string location, Action<string> cb)
        {
            yield return LoadRawFileTextCO(DefaultPackageName, location, cb);
        }

        #endregion
    }
}
#endif
