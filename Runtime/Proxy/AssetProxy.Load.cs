/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AIO.UEngine
{
    public partial class AssetProxy
    {
        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadSubAssetsCO<TObject>(string location, Action<TObject[]> cb) where TObject : Object;

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadSubAssetsCO(string location, Type type, Action<Object[]> cb);

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public abstract TObject[] LoadSubAssetsSync<TObject>(string location) where TObject : Object;

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public abstract Object[] LoadSubAssetsSync(string location, Type type);

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public abstract Task<TObject[]> LoadSubAssetsTask<TObject>(string location) where TObject : Object;

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public abstract Task<Object[]> LoadSubAssetsTask(string location, Type type);

        #endregion

        #region 资源加载

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadAssetCO<TObject>(string location, Action<TObject> cb) where TObject : Object;

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadAssetCO(string location, Type type, Action<Object> cb);

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public abstract TObject LoadAssetSync<TObject>(string location) where TObject : Object;

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public abstract Object LoadAssetSync(string location, Type type);

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public abstract Task<TObject> LoadAssetTask<TObject>(string location) where TObject : Object;

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public abstract Task<Object> LoadAssetTask(string location, Type type);

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
        public abstract IEnumerator LoadSceneCO(string location, Action<Scene> cb, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100);

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public abstract Task<Scene> LoadSceneTask(
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = true,
            int priority = 100);

        #endregion

        #region 原生文件

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract string LoadRawFileTextSync(string location);

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract Task<string> LoadRawFileTextTask(string location);

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract byte[] LoadRawFileDataSync(string location);

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract Task<byte[]> LoadRawFileDataTask(string location);

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadRawFileDataCO(string location, Action<byte[]> cb);

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadRawFileTextCO(string location, Action<string> cb);

        #endregion

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        public abstract void PreLoadSubAssets<TObject>(string location) where TObject : Object;

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        public abstract void PreLoadAsset<TObject>(string location) where TObject : Object;

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract void PreLoadRaw(string location);
    }
}
