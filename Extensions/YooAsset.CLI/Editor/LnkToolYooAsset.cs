#if SUPPORT_YOOASSET
using UnityEditor;

namespace AIO.UEditor.CLI
{
    /// <summary>
    ///     LnkTool
    /// </summary>
    public static class LnkToolYooAsset
    {
        [LnkTools(
            Tooltip = "YooAsset Collector",
            IconResource = "Editor/Icon/Yooasset"
        )]
        public static void OpenWindow()
        {
            EditorApplication.ExecuteMenuItem("YooAsset/AssetBundle Collector");
        }
    }
}
#endif