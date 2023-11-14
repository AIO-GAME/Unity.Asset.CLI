#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal sealed class YAssetHandleEditor : YAssetParameters
    {
        public YAssetHandleEditor() : base(EPlayMode.EditorSimulateMode)
        {
            Parameters = GetParameters();
        }

        protected override InitializeParameters GetParameters()
        {
            var initParameters = new EditorSimulateModeParameters
            {
                LoadingMaxTimeSlice = LoadingMaxTimeSlice,
                SimulateManifestFilePath = string.Empty,
                DecryptionServices = DecryptionServices,
                BuildinRootDirectory = string.Empty,
                SandboxRootDirectory = string.Empty
            };
            return initParameters;
        }
    }
}
#endif