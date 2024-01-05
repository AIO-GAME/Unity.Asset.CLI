#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal class YAssetParametersRemote : YAssetParameters
    {
        /// <summary>
        /// 内置资源查询服务接口
        /// </summary>
        public IBuildinQueryServices QueryServices { get; set; }

        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        public IRemoteServices RemoteServices { get; set; }

        /// <summary>
        /// 分发资源查询服务接口
        /// </summary>
        public IDeliveryQueryServices DeliveryQueryServices { get; set; }

        public YAssetParametersRemote() : base(MODE)
        {
            Parameters = GetParameters();
        }

        public YAssetParametersRemote(ASConfig config) : base(MODE, config)
        {
            Parameters = GetParameters();
        }

#if UNITY_WEBGL
        private const EPlayMode MODE = EPlayMode.WebPlayMode;

        protected sealed override InitializeParameters GetParameters()
        {
            var initParameters = new WebPlayModeParameters
            {
                DecryptionServices = DecryptionServices,
                LoadingMaxTimeSlice = LoadingMaxTimeSlice,
                BuildinQueryServices = QueryServices,
                RemoteServices = RemoteServices
            };
            return initParameters;
        }
#else
        private const EPlayMode MODE = EPlayMode.HostPlayMode;

        protected sealed override InitializeParameters GetParameters()
        {
            var initParameters = new HostPlayModeParameters
            {
                DecryptionServices = DecryptionServices,
                LoadingMaxTimeSlice = LoadingMaxTimeSlice,
                DownloadFailedTryAgain = DownloadFailedTryAgain,
                BuildinQueryServices = QueryServices,
                DeliveryQueryServices = DeliveryQueryServices,
                BuildinRootDirectory = BuildInRootDirectory,
                SandboxRootDirectory = SandboxRootDirectory,
                RemoteServices = RemoteServices
            };
            return initParameters;
        }
#endif
    }
}
#endif