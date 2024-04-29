#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal sealed class YAParametersEditor : YAssetParameters
    {
        public YAParametersEditor() : base(EPlayMode.EditorSimulateMode)
        {
            Parameters = GetParameters();
        }

        public YAParametersEditor(ASConfig mode) : base(EPlayMode.EditorSimulateMode, mode)
        {
            Parameters = GetParameters();
        }

        protected override InitializeParameters GetParameters()
        {
            var initParameters = new EditorSimulateModeParameters
            {
                LoadingMaxTimeSlice      = LoadingMaxTimeSlice,
                DecryptionServices       = DecryptionServices,
                SimulateManifestFilePath = string.Empty,
                BuildinRootDirectory     = string.Empty,
                SandboxRootDirectory     = string.Empty
            };
            return initParameters;
        }
    }
}
#endif