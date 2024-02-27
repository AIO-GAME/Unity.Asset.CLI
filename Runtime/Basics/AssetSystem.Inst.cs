/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static GameObject InstGameObject(in string location, Transform parent)
        {
            return Proxy.InstGameObject(SettingToLocalPath(location), parent);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static GameObject InstGameObject(in string location)
        {
            return Proxy.InstGameObject(SettingToLocalPath(location));
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void InstGameObject(string location, Transform parent, Action<GameObject> cb)
        {
            cb?.Invoke(await Proxy.InstGameObjectTask(SettingToLocalPath(location), parent));
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void InstGameObject(string location, Action<GameObject> cb)
        {
            cb?.Invoke(await Proxy.InstGameObjectTask(SettingToLocalPath(location)));
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<GameObject> InstGameObjectTask(string location, Transform parent)
        {
            return Proxy.InstGameObjectTask(SettingToLocalPath(location), parent);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<GameObject> InstGameObjectTask(string location)
        {
            return Proxy.InstGameObjectTask(SettingToLocalPath(location));
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator InstGameObjectCO(string location, Transform parent, Action<GameObject> cb)
        {
            return Proxy.InstGameObjectCO(SettingToLocalPath(location), cb, parent);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父级节点</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator InstGameObjectCO(string location, Transform parent)
        {
            return Proxy.InstGameObjectCO(SettingToLocalPath(location), _ => { }, parent);
        }

        /// <summary>
        /// 实例预制件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <returns><see cref="UnityEngine.GameObject"/></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator InstGameObjectCO(string location, Action<GameObject> cb)
        {
            return Proxy.InstGameObjectCO(SettingToLocalPath(location), cb);
        }
    }
}