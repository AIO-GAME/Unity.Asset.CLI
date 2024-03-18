using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace AIO.UEngine
{
    partial class ASProxy
    {
        /// <summary>
        /// 同步实例化GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="parent">父位置</param>
        /// <returns><see cref="GameObject"/></returns>
        public abstract GameObject InstGameObject(string location, Transform parent);

        /// <summary>
        /// Task实例化GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="parent">父位置</param>
        /// <returns><see cref="GameObject"/></returns>
        public abstract Task<GameObject> InstGameObjectTask(string location, Transform parent);

        /// <summary>
        /// 同步GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="cb">回调函数</param>
        /// <param name="parent">父位置</param>
        /// <returns><see cref="GameObject"/></returns>
        public abstract IEnumerator InstGameObjectCO(string location, Action<GameObject> cb, Transform parent);

        /// <summary>
        /// 同步实例化GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="parent">父位置</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="GameObject"/></returns>
        public async void InstGameObject(string location, Transform parent, Action<GameObject> cb)
        {
            if (cb is null) return;
            await InstGameObjectTask(location, parent).ContinueWith(t => cb.Invoke(t.Result));
        }

        /// <summary>
        /// 同步实例化GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="GameObject"/></returns>
        public async void InstGameObject(string location, Action<GameObject> cb)
        {
            if (cb is null) return;
            await InstGameObjectTask(location, null).ContinueWith(t => cb.Invoke(t.Result));
        }

        /// <summary>
        /// 同步实例化GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <returns><see cref="GameObject"/></returns>
        public GameObject InstGameObject(string location)
            => InstGameObject(location, parent: null);

        /// <summary>
        /// Task实例化GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <returns><see cref="GameObject"/></returns>
        public Task<GameObject> InstGameObjectTask(string location)
            => InstGameObjectTask(location, null);

        /// <summary>
        /// 同步GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="cb">回调函数</param>
        /// <returns><see cref="GameObject"/></returns>
        public IEnumerator InstGameObjectCO(string location, Action<GameObject> cb) =>
            InstGameObjectCO(location, cb, null);
    }
}