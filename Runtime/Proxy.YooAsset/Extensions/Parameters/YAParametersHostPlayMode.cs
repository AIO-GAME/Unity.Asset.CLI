#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal class YAssetParametersHostPlayMode : YAssetParameters
    {
        /// <summary>
        /// 内置资源查询服务接口
        /// </summary>
        public IQueryServices QueryServices { get; set; } = null;

        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        public IRemoteServices RemoteServices { get; set; } = null;

        public YAssetParametersHostPlayMode() : base(EPlayMode.HostPlayMode)
        {
            Parameters = GetParameters();
        }

        protected sealed override InitializeParameters GetParameters()
        {
            var initParameters = new HostPlayModeParameters();
            initParameters.DecryptionServices = DecryptionServices;
            initParameters.LoadingMaxTimeSlice = LoadingMaxTimeSlice;
            initParameters.DownloadFailedTryAgain = DownloadFailedTryAgain;
            initParameters.QueryServices = QueryServices;
            initParameters.BuildinRootDirectory = BuildInRootDirectory;
            initParameters.SandboxRootDirectory = SandboxRootDirectory;
            initParameters.RemoteServices = RemoteServices;
            return initParameters;
        }
    }
}
#endif