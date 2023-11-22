﻿/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AIO.UEngine;
using Object = UnityEngine.Object;

namespace AIO
{
    public partial class AssetSystem
    {
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task PreLoadSubAssets<TObject>(string location) where TObject : Object
        {
            return Proxy.PreLoadSubAssets<TObject>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task PreLoadSubAssets(string location)
        {
            return Proxy.PreLoadSubAssets<Object>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task PreLoadAsset<TObject>(string location) where TObject : Object
        {
            return Proxy.PreLoadAsset<TObject>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task PreLoadAsset(string location, Type type)
        {
            return Proxy.PreLoadAsset(Parameter.LoadPathToLower ? location.ToLower() : location, type);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task PreLoadRaw(string location)
        {
            return Proxy.PreLoadRaw(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 预加载记录
        /// </summary>
        public static async Task DownloadPreRecord(ProgressArgs progressArgs = default)
        {
            if (Parameter.ASMode != EASMode.Remote) return;
            var handle = GetDownloader();
            var flow = await handle.UpdatePackageManifestTask();
            if (flow) flow = await handle.UpdatePackageVersionTask();
            if (flow) await Proxy.PreRecord(SequenceRecordQueue, progressArgs);
        }
    }
}