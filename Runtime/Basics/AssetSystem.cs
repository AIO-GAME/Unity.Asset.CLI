/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AIO.UEngine;

namespace AIO
{
    /// <summary>
    /// 资源管理系统
    /// </summary>
    public static partial class AssetSystem
    {
        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(ASConfig config) where T : AssetProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), config);
        }

        public static ICollection<string> GetAssetInfos(string tag)
        {
            return Proxy.GetAssetInfos(tag);
        }

        public static ICollection<string> GetAssetInfos(IEnumerable<string> tag)
        {
            return Proxy.GetAssetInfos(tag);
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize(ASConfig config)
        {
#if SUPPORT_YOOASSET
            Proxy = new YAssetProxy();
#else
            throw new Exception("Not Found Other Asset Proxy! Please Input Asset Proxy!");
#endif
            yield return Initialize(Proxy, config);
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(T proxy) where T : AssetProxy
        {
            return Initialize(proxy, ASConfig.GetOrCreate());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>() where T : AssetProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), ASConfig.GetOrCreate());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(T proxy, ASConfig config) where T : AssetProxy
        {
            IsInitialized = false;
            Parameter = config;
            Proxy = proxy;
            if (Parameter.ASMode == EASMode.Remote)
                yield return Parameter.UpdatePackageRemote();
            else Parameter.UpdatePackage();

            yield return Proxy.Initialize();
            SequenceRecords = new SequenceRecordQueue(Parameter.EnableSequenceRecord);
            yield return SequenceRecords.LoadCo();
            IsInitialized = true;
        }

        /// <summary>
        /// 获取序列记录
        /// </summary>
        /// <param name="record">记录</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void AddSequenceRecord(SequenceRecord record)
        {
            SequenceRecords.Add(record);
        }

        /// <summary>
        /// 销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task DestroyTask()
        {
            Destroy();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DestroyCO()
        {
            Destroy();
            yield break;
        }

        /// <summary>
        /// 销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void Destroy()
        {
            Proxy.Dispose();
            SequenceRecords.Dispose();
        }
        
        /// <summary>
        /// 清理缓存资源 (清空之后需要重新下载资源)
        /// </summary>
        public static void CleanCache(Action<bool> cb = null)
        {
            Proxy.CleanCache(cb);
        }
    }
}