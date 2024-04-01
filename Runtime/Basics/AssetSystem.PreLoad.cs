using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        public static Task PreLoadSubAssets<TObject>(string location) where TObject : Object
        {
            return Proxy.PreLoadSubAssetsTask<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        public static Task PreLoadSubAssets(string location)
        {
            return Proxy.PreLoadSubAssetsTask<Object>(SettingToLocalPath(location));
        }

        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        public static Task PreLoadAsset<TObject>(string location) where TObject : Object
        {
            return Proxy.PreLoadAssetTask<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        public static Task PreLoadAsset(string location, Type type)
        {
            return Proxy.PreLoadAssetTask(SettingToLocalPath(location), type);
        }

        /// <summary>
        ///     预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        public static Task PreLoadRaw(string location)
        {
            return Proxy.PreLoadRawTask(SettingToLocalPath(location));
        }
    }
}