/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using UnityEngine;

namespace AIO
{
    public partial class AssetSystem
    {
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        public static void PreLoadSubAssets<TObject>(string location) where TObject : Object
        {
            Proxy.PreLoadSubAssets<TObject>(location);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static void PreLoadSubAssets(string location)
        {
            Proxy.PreLoadSubAssets<Object>(location);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        public static void PreLoadAsset<TObject>(string location) where TObject : Object
        {
            Proxy.PreLoadAsset<TObject>(location);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static void PreLoadAsset(string location)
        {
            Proxy.PreLoadAsset<Object>(location);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static void PreLoadRaw(string location)
        {
            Proxy.PreLoadRaw(location);
        }
    }
}
