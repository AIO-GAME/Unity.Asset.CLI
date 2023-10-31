#if SUPPORT_YOOASSET
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor.Build
{
    [GWindow("YooAsset Tool", "YooAsset 工具集")]
    public class YooAssetGraphWindow : GraphicWindow
    {
        [MenuItem("YooAsset/工具集")]
        public static void Open()
        {
            Open<YooAssetGraphWindow>();
        }

        private YooAssetGraphicRect SettingGraphicRect;

        protected override void OnAwake()
        {
        }

        protected override void OnActivation()
        {
            SettingGraphicRect = new YooAssetGraphicRect();
            SettingGraphicRect.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        protected override void OnGUI()
        {
            SettingGraphicRect.RectData = new Rect(0, 10, CurrentWidth, CurrentHeight - 30);
            SettingGraphicRect.Draw();
            SettingGraphicRect.OnOpenEvent();
            DrawVersion(Setting.Version);
        }
    }
}
#endif