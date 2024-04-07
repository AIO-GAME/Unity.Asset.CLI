using System;
using System.Collections;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AIO.UEngine
{
    partial class ASProxy
    {
        #region 子资源加载

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadSubAssetsCO<TObject>
            (string location, Action<TObject[]> cb) where TObject : Object;

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public abstract TObject[] LoadSubAssetsSync<TObject>
            (string location) where TObject : Object;

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public abstract Task<TObject[]> LoadSubAssetsTask<TObject>
            (string location) where TObject : Object;

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadSubAssetsCO
            (string location, Type type, Action<Object[]> cb);

        /// <summary>
        ///     同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public abstract Object[] LoadSubAssetsSync
            (string location, Type type);

        /// <summary>
        ///     异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public abstract Task<Object[]> LoadSubAssetsTask
            (string location, Type type);

        #endregion
    }
}