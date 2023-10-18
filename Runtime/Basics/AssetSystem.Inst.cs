/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace AIO
{
    public static partial class AssetSystem
    {
        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        public static GameObject InstGameObject(in string location, Transform parent)
        {
            return Proxy.InstGameObject(Parameter.LoadPathToLower ? location.ToLower() : location, parent);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        public static GameObject InstGameObject(in string location)
        {
            return Proxy.InstGameObject(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        public static async void InstGameObject(string location, Transform parent, Action<GameObject> cb)
        {
            cb?.Invoke(await Proxy.InstGameObjectTask(Parameter.LoadPathToLower ? location.ToLower() : location, parent));
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        public static async void InstGameObject(string location, Action<GameObject> cb)
        {
            cb?.Invoke(await Proxy.InstGameObjectTask(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        public static Task<GameObject> InstGameObjectTask(string location, Transform parent)
        {
            return Proxy.InstGameObjectTask(Parameter.LoadPathToLower ? location.ToLower() : location, parent);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        public static Task<GameObject> InstGameObjectTask(string location)
        {
            return Proxy.InstGameObjectTask(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        public static IEnumerator InstGameObjectCO(string location, Transform parent, Action<GameObject> cb)
        {
            return Proxy.InstGameObjectCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb, parent);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        public static IEnumerator InstGameObjectCO(string location, Action<GameObject> cb)
        {
            return Proxy.InstGameObjectCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }
    }
}
