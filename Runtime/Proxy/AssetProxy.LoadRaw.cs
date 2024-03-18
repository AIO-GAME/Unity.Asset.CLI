using System;
using System.Collections;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AIO.UEngine
{
    partial class ASProxy
    {
        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract string LoadRawFileTextSync(string location);

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract Task<string> LoadRawFileTextTask(string location);

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract byte[] LoadRawFileDataSync(string location);

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract Task<byte[]> LoadRawFileDataTask(string location);

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadRawFileDataCO(string location, Action<byte[]> cb);

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public abstract IEnumerator LoadRawFileTextCO(string location, Action<string> cb);
    }
}