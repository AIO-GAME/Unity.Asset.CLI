/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-02-27
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace AIO.UEngine
{
    partial class AssetProxy
    {
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public abstract IEnumerator LoadSceneCO(string location,
            Action<Scene> cb,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100);

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
            bool suspendLoad = false,
            int priority = 100);
    }
}