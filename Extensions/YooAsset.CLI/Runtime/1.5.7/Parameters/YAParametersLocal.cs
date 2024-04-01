#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    ///     离线模式
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
            var initParameters = new OfflinePlayModeParameters
            {
                DecryptionServices     = DecryptionServices,
                LoadingMaxTimeSlice    = LoadingMaxTimeSlice,
                DownloadFailedTryAgain = DownloadFailedTryAgain,
                BuildinRootDirectory   = BuildInRootDirectory,
                SandboxRootDirectory   = SandboxRootDirectory
            };
            return initParameters;
        }
    }
}
#endif