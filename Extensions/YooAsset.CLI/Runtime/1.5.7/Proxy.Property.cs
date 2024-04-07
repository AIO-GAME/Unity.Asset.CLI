#if SUPPORT_YOOASSET
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private static MethodInfo _ReleaseInternal;

        /// <summary>
        ///     字典
        /// </summary>
        private Dictionary<string, ResPackage> Dic { get; set; }

        /// <summary>
        ///     主包
        /// </summary>
        private ResPackage DefaultPackage { get; set; }

        /// <summary>
        ///     主包
        /// </summary>
        private string DefaultPackageName { get; set; }

        /// <summary>
        ///     引用计数
        /// </summary>
        private Dictionary<string, OperationHandleBase> ReferenceOPHandle { get; set; }

        /// <summary>
        ///     初始化操作
        /// </summary>
        private List<InitializationOperation> InitializationOperations { get; set; }

        /// <summary>
        ///     下载操作
        /// </summary>
        private static Dictionary<string, DownloaderOperation> DownloaderOperations { get; set; }

        public override bool IsInitialize
        {
            get
            {
                if (InitializationOperations is null) return false;
                return InitializationOperations.All(operation => operation.Status == EOperationStatus.Succeed);
            }
        }

        private static MethodInfo ReleaseInternal
        {
            get
            {
                if (_ReleaseInternal is null)
                    _ReleaseInternal = typeof(OperationHandleBase).GetMethod("ReleaseInternal",
                                                                             BindingFlags.Instance |
                                                                             BindingFlags.NonPublic);

                return _ReleaseInternal;
            }
        }
    }
}
#endif