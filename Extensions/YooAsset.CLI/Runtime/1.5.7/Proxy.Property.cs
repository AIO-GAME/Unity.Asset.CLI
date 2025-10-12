#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Scripting;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        /// <summary>
        ///     字典
        /// </summary>
        [Preserve]
        private Dictionary<string, ResPackage> Dic { get; set; }

        /// <summary>
        ///     主包
        /// </summary>
        [Preserve]
        private ResPackage DefaultPackage { get; set; }

        /// <summary>
        ///     主包
        /// </summary>
        [Preserve]
        private string DefaultPackageName { get; set; }

        /// <summary>
        ///     引用计数
        /// </summary>
        [Preserve]
        private Dictionary<string, OperationHandleBase> ReferenceOPHandle { get; set; }

        /// <summary>
        ///     初始化操作
        /// </summary>
        [Preserve]
        private List<InitializationOperation> InitializationOperations { get; set; }

        /// <summary>
        ///     下载操作
        /// </summary>
        [Preserve]
        private static Dictionary<string, DownloaderOperation> DownloaderOperations { get; set; }

        [Preserve]
        public override bool IsInitialize
        {
            get { return InitializationOperations.Count != 0 && InitializationOperations.All(operation => operation.Status == EOperationStatus.Succeed); }
        }

        [Preserve]
        private static MethodInfo ReleaseInternal => ReleaseInternalLazy.Value;

        [Preserve]
        private static Lazy<MethodInfo> ReleaseInternalLazy = new Lazy<MethodInfo>
            (() => typeof(OperationHandleBase).GetMethod("ReleaseInternal", BindingFlags.Instance | BindingFlags.NonPublic));
    }
}
#endif