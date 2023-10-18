#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// 离线模式
    /// </summary>
    internal class YAssetParametersOfflinePlayMode : YAssetParameters
    {
        public YAssetParametersOfflinePlayMode() : base(EPlayMode.OfflinePlayMode)
        {
            Parameters = GetParameters();
        }

        protected sealed override InitializeParameters GetParameters()
        {
            var initParameters = new OfflinePlayModeParameters();
            initParameters.DecryptionServices = DecryptionServices;
            initParameters.LoadingMaxTimeSlice = LoadingMaxTimeSlice;
            initParameters.DownloadFailedTryAgain = DownloadFailedTryAgain;
            initParameters.BuildinRootDirectory = BuildinRootDirectory;
            initParameters.SandboxRootDirectory = SandboxRootDirectory;
            initParameters.DecryptionServices = DecryptionServices;
            initParameters.LoadingMaxTimeSlice = LoadingMaxTimeSlice;
            return initParameters;
        }
    }
}
#endif