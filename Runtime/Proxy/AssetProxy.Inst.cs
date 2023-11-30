/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace AIO.UEngine
{
    public partial class AssetProxy
    {
        /// <summary>
        /// 同步实例化GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="parent">父位置</param>
        /// <returns><see cref="GameObject"/></returns>
        public abstract GameObject InstGameObject(string location, Transform parent = null);

        /// <summary>
        /// Task实例化GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="parent">父位置</param>
        /// <returns><see cref="GameObject"/></returns>
        public abstract Task<GameObject> InstGameObjectTask(string location, Transform parent = null);

        /// <summary>
        /// 同步GameObject
        /// </summary>
        /// <param name="location">唯一路径</param>
        /// <param name="cb">回调函数</param>
        /// <param name="parent">父位置</param>
        /// <returns><see cref="GameObject"/></returns>
        public abstract IEnumerator InstGameObjectCO(string location, Action<GameObject> cb, Transform parent = null);
    }
}
