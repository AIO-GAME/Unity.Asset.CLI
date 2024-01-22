/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan 
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Threading.Tasks;
using AIO.UEngine.YooAsset;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AIO.UEngine
{
    public partial class YAssetProxy
    {
        public override bool IsAlreadyLoad(string location)
        {
            return YAssetSystem.IsAlreadyLoad(location);
        }

        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadSubAssetsCO<TObject>(string location, Action<TObject[]> cb)
        {
            return YAssetSystem.LoadSubAssetsCO(location, cb);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadSubAssetsCO(string location, Type type, Action<Object[]> cb)
        {
            return YAssetSystem.LoadSubAssetsCO(location, type, cb);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override TObject[] LoadSubAssetsSync<TObject>(string location)
        {
            return YAssetSystem.LoadSubAssets<TObject>(location);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public override Object[] LoadSubAssetsSync(string location, Type type)
        {
            return YAssetSystem.LoadSubAssets(location, type);
        }


        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override Task<TObject[]> LoadSubAssetsTask<TObject>(string location)
        {
            return YAssetSystem.LoadSubAssetsTask<TObject>(location);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public override Task<Object[]> LoadSubAssetsTask(string location, Type type)
        {
            return YAssetSystem.LoadSubAssetsTask(location, type);
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadAssetCO<TObject>(string location, Action<TObject> cb)
        {
            return YAssetSystem.LoadAssetCO(location, cb);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadAssetCO(string location, Type type, Action<Object> cb)
        {
            return YAssetSystem.LoadAssetCO(location, type, cb);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override TObject LoadAssetSync<TObject>(string location)
        {
            return YAssetSystem.LoadAsset<TObject>(location);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public override Object LoadAssetSync(string location, Type type)
        {
            return YAssetSystem.LoadAsset(location, type);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override Task<TObject> LoadAssetTask<TObject>(string location)
        {
            return YAssetSystem.LoadAssetTask<TObject>(location);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public override Task<Object> LoadAssetTask(string location, Type type)
        {
            return YAssetSystem.LoadAssetTask(location, type);
        }

        #endregion

        #region 场景加载

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public override IEnumerator LoadSceneCO(string location, Action<Scene> cb,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            return YAssetSystem.LoadSceneCO(cb, location, sceneMode, suspendLoad, priority);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public override Task<Scene> LoadSceneTask(
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = true,
            int priority = 100)
        {
            return YAssetSystem.LoadSceneTask(location, sceneMode, suspendLoad, priority);
        }

        #endregion

        #region 原生文件

        public override string LoadRawFileTextSync(string location)
        {
            return YAssetSystem.LoadRawFileText(location);
        }

        public override Task<string> LoadRawFileTextTask(string location)
        {
            return YAssetSystem.LoadRawFileTextTask(location);
        }

        public override byte[] LoadRawFileDataSync(string location)
        {
            return YAssetSystem.LoadRawFileData(location);
        }

        public override Task<byte[]> LoadRawFileDataTask(string location)
        {
            return YAssetSystem.LoadRawFileDataTask(location);
        }

        public override IEnumerator LoadRawFileDataCO(string location, Action<byte[]> cb)
        {
            return YAssetSystem.LoadRawFileDataCO(location, cb);
        }

        public override IEnumerator LoadRawFileTextCO(string location, Action<string> cb)
        {
            return YAssetSystem.LoadRawFileTextCO(location, cb);
        }

        public override Task PreLoadSubAssets(string location, Type type)
        {
            return YAssetSystem.PreLoadSubAssets(location, type);
        }

        public override Task PreLoadAsset(string location, Type type)
        {
            return YAssetSystem.PreLoadAsset(location, type);
        }

        public override Task PreLoadRaw(string location)
        {
            return YAssetSystem.PreLoadRaw(location);
        }

        #endregion
    }
}
#endif