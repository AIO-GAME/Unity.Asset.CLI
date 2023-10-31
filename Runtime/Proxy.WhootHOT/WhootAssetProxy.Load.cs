/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_WHOOTHOT
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Rol.Game
{
    public partial class WhootAssetProxy
    {
        #region 子资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadSubAssetsCO<TObject>(string location, Action<TObject[]> cb)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadSubAssetsCO(string location, Type type, Action<Object[]> cb)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override TObject[] LoadSubAssetsSync<TObject>(string location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public override Object[] LoadSubAssetsSync(string location, Type type)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override Task<TObject[]> LoadSubAssetsTask<TObject>(string location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public override Task<Object[]> LoadSubAssetsTask(string location, Type type)
        {
            throw new NotImplementedException();
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
            var handle = Addressables.LoadAssetAsync<TObject>(location);
            yield return handle;
            cb?.Invoke(handle.Result);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadAssetCO(string location, Type type, Action<Object> cb)
        {
            var handle = Addressables.LoadAssetAsync<Object>(location);
            yield return handle;
            cb?.Invoke(handle.Result);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override TObject LoadAssetSync<TObject>(string location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public override Object LoadAssetSync(string location, Type type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public override async Task<TObject> LoadAssetTask<TObject>(string location)
        {
            var handle = Addressables.LoadAssetAsync<TObject>(location);
            await handle.Task;
            return handle.Result;
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public override async Task<Object> LoadAssetTask(string location, Type type)
        {
            var handle = Addressables.LoadAssetAsync<Object>(location);
            await handle.Task;
            return handle.Result;
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
            var handle = Addressables.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            yield return handle;
            cb?.Invoke(handle.Result.Scene);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public override async Task<Scene> LoadSceneTask(
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = true,
            int priority = 100)
        {
            var handle = Addressables.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            await handle.Task;
            return handle.Result.Scene;
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override string LoadRawFileTextSync(string location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override Task<string> LoadRawFileTextTask(string location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override byte[] LoadRawFileDataSync(string location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override Task<byte[]> LoadRawFileDataTask(string location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadRawFileDataCO(string location, Action<byte[]> cb)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public override IEnumerator LoadRawFileTextCO(string location, Action<string> cb)
        {
            throw new NotImplementedException();
        }

        public override void PreLoadSubAssets<TObject>(string location)
        {
            throw new NotImplementedException();
        }

        public override void PreLoadAsset<TObject>(string location)
        {
            throw new NotImplementedException();
        }

        public override void PreLoadRaw(string location)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
#endif