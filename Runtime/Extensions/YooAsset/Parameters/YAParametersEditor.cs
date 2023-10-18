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
            var initParameters = new EditorSimulateModeParameters();
            initParameters.LoadingMaxTimeSlice = LoadingMaxTimeSlice;
            initParameters.SimulateManifestFilePath = string.Empty;
            initParameters.DecryptionServices = DecryptionServices;
            initParameters.BuildinRootDirectory = string.Empty;
            initParameters.SandboxRootDirectory = string.Empty;
            return initParameters;
        }
    }
}
#endif