#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// 离线模式
    /// </summary>
    internal class YAParametersLocal : YAssetParameters
    {
        public YAParametersLocal() : base(EPlayMode.OfflinePlayMode)
        {
            Parameters = GetParameters();
        }

        public YAParametersLocal(ASConfig config) : base(EPlayMode.OfflinePlayMode, config)
        {
            Parameters = GetParameters();
        }

        protected sealed override InitializeParameters GetParameters()
        {
            var initParameters = new OfflinePlayModeParameters();
            initParameters.DecryptionServices = DecryptionServices;
            initParameters.LoadingMaxTimeSlice = LoadingMaxTimeSlice;
            initParameters.DownloadFailedTryAgain = DownloadFailedTryAgain;
            initParameters.BuildinRootDirectory = BuildInRootDirectory;
            initParameters.SandboxRootDirectory = SandboxRootDirectory;
            initParameters.DecryptionServices = DecryptionServices;
            initParameters.LoadingMaxTimeSlice = LoadingMaxTimeSlice;
            return initParameters;
        }
    }
}
#endif