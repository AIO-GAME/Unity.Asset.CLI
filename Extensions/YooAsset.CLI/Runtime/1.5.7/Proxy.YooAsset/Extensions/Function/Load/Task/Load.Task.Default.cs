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
#else
using System.Threading.Tasks;
using ATask = System.Threading.Tasks.Task;
using ATaskObject = System.Threading.Tasks.Task<UnityEngine.Object>;
using ATaskGameObject = System.Threading.Tasks.Task<UnityEngine.GameObject>;
using ATaskObjectArray = System.Threading.Tasks.Task<UnityEngine.Object[]>;
using ATaskScene = System.Threading.Tasks.Task<UnityEngine.SceneManagement.Scene>;
using ATaskString = System.Threading.Tasks.Task<System.String>;
using ATaskByteArray = System.Threading.Tasks.Task<System.Byte[]>;
#endif

#if YOOASSET
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        #region 实例化资源

        public static ATaskGameObject InstGameObjectDefaultTask(string location, Transform parent)
        {
            return InstGameObjectTask(DefaultPackageName, location, parent);
        }

        public static ATaskGameObject InstGameObjectDefaultTask(string location)
        {
            return InstGameObjectTask(DefaultPackageName, location);
        }

        #endregion

        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
#if UNITASK
        public static UniTask<TObject[]> LoadSubAssetsDefaultTask<TObject>(string location) where TObject : Object
#else
        public static Task<TObject[]> LoadSubAssetsDefaultTask<TObject>(string location) where TObject : Object
#endif
        {
            return LoadSubAssetsTask<TObject>(DefaultPackageName, location);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public static ATaskObjectArray LoadSubAssetsDefaultTask(string location, Type type)
        {
            return LoadSubAssetsTask(DefaultPackageName, location, type);
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
#if UNITASK
        public static UniTask<TObject> LoadAssetDefaultTask<TObject>(string location) where TObject : Object
#else
        public static Task<TObject> LoadAssetDefaultTask<TObject>(string location) where TObject : Object
#endif
        {
            return LoadAssetTask<TObject>(DefaultPackageName, location);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public static ATaskObject LoadAssetDefaultTask(string location, Type type)
        {
            return LoadAssetTask(DefaultPackageName, location, type);
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
        public static ATaskScene LoadSceneDefaultTask(
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            return LoadSceneTask(DefaultPackageName, location, sceneMode, suspendLoad, priority);
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static ATaskByteArray LoadRawFileDataDefaultTask(string location)
        {
            return LoadRawFileDataTask(DefaultPackageName, location);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static ATaskString LoadRawFileTextDefaultTask(string location)
        {
            return LoadRawFileTextTask(DefaultPackageName, location);
        }

        #endregion
    }
}
#endif