#if SUPPORT_YOOASSET
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor.CLI
{
    // [GWindow("YooAsset Tool", "YooAsset 工具集",
    //     Menu = "YooAsset/工具集",
    //     MinSizeWidth = 1000
    // )]
    public class YooAssetGraphWindow : GraphicWindow
    {
        private YooAssetGraphicRect SettingGraphicRect;

        protected override void OnActivation()
        {
            SettingGraphicRect = new YooAssetGraphicRect();
            SettingGraphicRect.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        protected override void OnDraw()
        {
            SettingGraphicRect.RectData = new Rect(0, 10, CurrentWidth, CurrentHeight - 30);
            SettingGraphicRect.Draw();
        }

        protected override void OnDispose()
        {
            SettingGraphicRect.Dispose();
        }
    }
}
#endif