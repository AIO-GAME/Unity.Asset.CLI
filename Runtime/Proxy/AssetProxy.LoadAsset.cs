using System;
using System.Collections;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AIO.UEngine
{
    partial class ASProxy
    {
        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public abstract TObject LoadAssetSync<TObject>
            (string location) where TObject : Object;

        /// <summary>
        /// 协程加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadAssetCO<TObject>
            (string location, Action<TObject> cb) where TObject : Object;

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public abstract Task<TObject> LoadAssetTask<TObject>
            (string location) where TObject : Object;

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadAssetCO(string location, Type type, Action<Object> cb);

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public abstract Object LoadAssetSync(string location, Type type);

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public abstract Task<Object> LoadAssetTask(string location, Type type);
    }
}