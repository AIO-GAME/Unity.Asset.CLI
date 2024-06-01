#region namespace

using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

#endregion

namespace AIO.UEngine
{
    partial class ASProxy
    {
        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public abstract Task PreLoadSubAssetsTask(string location, Type type);

        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public abstract Task PreLoadAssetTask(string location, Type type);

        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract Task PreLoadRawTask(string location);

        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        public Task PreLoadSubAssetsTask<TObject>(string location)
        where TObject : Object
        {
            return PreLoadSubAssetsTask(location, typeof(TObject));
        }

        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        public Task PreLoadAssetTask<TObject>(string location)
        where TObject : Object
        {
            return PreLoadAssetTask(location, typeof(TObject));
        }
    }
}