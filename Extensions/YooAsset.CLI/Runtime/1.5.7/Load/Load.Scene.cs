#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        #region 场景加载

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public override IEnumerator LoadSceneCO(
            string location,
            Action<Scene> cb,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            var operation = HandleGet<SceneOperationHandle>(location);
            if (operation != null)
            {
                yield return operation.UnloadAsync();
                HandleFree(location);
            }

            ResPackage package = null;
            yield return GetAutoPackageCO(location, ya => package = ya);
            if (package is null) throw new Exception($"场景配置 异常错误 : {location} {sceneMode}");

            operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            yield return LoadCheckOPCo(operation, succeed =>
            {
                if (succeed)
                {
                    HandleAdd(location, operation);
                    operation.ActivateScene();
                    cb?.Invoke(operation.SceneObject);
                }
                else
                {
                    AssetSystem.LogException($"场景配置 异常错误 : {package.PackageName} {location} {sceneMode}");
                    cb?.Invoke(SceneManager.GetActiveScene());
                }
            });
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
            bool suspendLoad = false,
            int priority = 100)
        {
            var operation = HandleGet<SceneOperationHandle>(location);
            if (operation != null)
            {
                await operation.UnloadAsync().Task;
                HandleFree(location);
            }

            var package = await GetAutoPackageTask(location);
            if (package is null)
            {
                AssetSystem.LogException("场景配置 异常错误:{0} {1}", location, sceneMode);
                return SceneManager.GetActiveScene();
            }

            operation = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            if (await LoadCheckOPTask(operation))
            {
                HandleAdd(location, operation);
                return operation.SceneObject;
            }

            AssetSystem.LogException("加载场景 资源异常:{0} {1} {2}", package.PackageName, location, sceneMode);
            return SceneManager.GetActiveScene();
        }

        #endregion
    }
}
#endif