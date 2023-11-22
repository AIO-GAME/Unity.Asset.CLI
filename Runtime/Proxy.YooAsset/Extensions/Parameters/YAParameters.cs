#if SUPPORT_YOOASSET
using System;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    public abstract class YAssetParameters
    {
        /// <summary>
        /// 文件解密服务接口
        /// </summary>
        public IDecryptionServices DecryptionServices { get; set; } = null;

        /// <summary>
        /// 资源加载的最大数量
        /// </summary>
        public int LoadingMaxTimeSlice { get; set; } = 2048;

        /// <summary>
        /// 资源加载模式
        /// </summary>
        public EPlayMode Mode { get; protected set; }

        /// <summary>
        /// 初始化参数
        /// </summary>
        public InitializeParameters Parameters { get; protected set; }

        /// <summary>
        /// 内置文件的根路径
        /// 注意：当参数为空的时候会使用默认的根目录。
        /// </summary>
        public string BuildInRootDirectory { get; set; } = string.Empty;

        /// <summary>
        /// 沙盒文件的根路径
        /// 注意：当参数为空的时候会使用默认的根目录。
        /// </summary>
        public string SandboxRootDirectory { get; set; } = string.Empty;

        /// <summary>
        /// 下载失败尝试次数
        /// 注意：默认值为MaxValue
        /// </summary>
        public int DownloadFailedTryAgain { get; set; } = 10;

        protected YAssetParameters(EPlayMode mode, ASConfig config) : this(mode)
        {
            DownloadFailedTryAgain = config.DownloadFailedTryAgain;
            LoadingMaxTimeSlice = config.LoadingMaxTimeSlice;
        }

        protected YAssetParameters(EPlayMode mode)
        {
            Mode = mode;
#if UNITY_EDITOR
            switch (Mode)
            {
                case EPlayMode.EditorSimulateMode:
                    Debug.Log("Asset System : 编辑器资源管理");
                    break;
                case EPlayMode.OfflinePlayMode:
                    Debug.Log("Asset System : 离线资源管理");
                    break;
                case EPlayMode.HostPlayMode:
                    Debug.Log("Asset System : 联网资源管理");
                    break;
                case EPlayMode.WebPlayMode:
                    Debug.Log("Asset System : Web资源管理");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#endif
        }

        public void UpdateParameters()
        {
            Parameters = GetParameters();
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        protected abstract InitializeParameters GetParameters();
    }
}
#endif