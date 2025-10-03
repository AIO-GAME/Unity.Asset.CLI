#if SUPPORT_YOOASSET
using UnityEditor;
using UnityEngine;

[assembly: UnityAPICompatibilityVersion("2019.4.0", true)]

namespace AIO.UEditor.CLI
{
    /// <summary>
    ///     LnkTool
    /// </summary>
    public static class LnkToolYooAsset
    {
        [LnkTools(Tooltip = "YooAsset Collector", IconRelative = @"Packages\com.aio.cli.asset\Editor\Resources\Icon\Yooasset.png")]
        public static void OpenWindow() { EditorApplication.ExecuteMenuItem("YooAsset/AssetBundle Collector"); }
    }
}
#endif
