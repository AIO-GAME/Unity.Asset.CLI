using System;
using System.IO;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    internal class AssetPageEditBuild : IAssetPage
    {
        int IAssetPage.    Order => 3;
        private GUIContent GC_REPORT;
        private GUIContent GC_Select;
        private GUIContent GC_SAVE;

        private DriveInfo            Disk;        // 磁盘信息
        private AssetCollectRoot     Data;        // 资源数据
        private AssetBuildConfig     BuildConfig; // 构建配置
        private TreeViewBuildSetting TreeViewBuildSetting;

        public AssetPageEditBuild()
        {
            GC_REPORT            = new GUIContent("报", "资源报告工具");
            GC_Select            = GEContent.NewSetting("ic_Eyes", "选择资源配置文件");
            GC_SAVE              = GEContent.NewBuiltin("d_SaveAs", "保存");
            TreeViewBuildSetting = TreeViewBuildSetting.Create();
            Data                 = AssetCollectRoot.GetOrCreate();
            BuildConfig          = AssetBuildConfig.GetOrCreate();
        }

        public void Dispose()
        {
            TreeViewBuildSetting = null;
            BuildConfig          = null;
            Data                 = null;
        }

        #region IAssetWindow

        string IAssetPage.Title => "构建模式      [Ctrl + Number3]";

        bool IAssetPage.Shortcut(Event evt)
        {
            if (evt.type == EventType.KeyDown && evt.control)
                return evt.keyCode == KeyCode.Keypad6
                    || evt.keyCode == KeyCode.Alpha6
                    ;

            return false;
        }

        void IAssetPage.EventMouseDrag(in Event evt)                           { }
        void IAssetPage.EventMouseDown(in Event evt)                           { }
        void IAssetPage.EventMouseUp(in   Event evt)                           { }
        void IAssetPage.EventKeyDown(in   Event eventData, in KeyCode keyCode) { }
        void IAssetPage.EventKeyUp(in     Event eventData, in KeyCode keyCode) { }

        void IAssetPage.UpdateData()
        {
            try
            {
                Disk = new DriveInfo(EHelper.Path.Project); // 获取当前文件磁盘剩余空间
            }
            catch (Exception)
            {
                // ignored
            }

            BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            if (BuildConfig.BuildTarget == 0 ||
                BuildConfig.BuildTarget == BuildTarget.NoTarget
               ) BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        void IAssetPage.OnDrawHeader(Rect rect)
        {
            rect.x     = rect.width - 25;
            rect.width = 25;
            if (GUI.Button(rect, GC_SAVE, GEStyle.TEtoolbarbutton))
            {
                try
                {
                    GUI.FocusControl(null);
                    BuildConfig.Save();
#if UNITY_2021_1_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(BuildConfig);
#else
                    AssetDatabase.SaveAssets();
#endif
                    if (EditorUtility.DisplayDialog("保存", "保存成功", "确定")) AssetDatabase.Refresh();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            rect.x -= rect.width;
            if (GUI.Button(rect, GC_Select, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                Selection.activeObject = ASConfig.GetOrCreate();
            }

#if SUPPORT_YOOASSET
            rect.x -= rect.width;
            if (GUI.Button(rect, GC_REPORT, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                EditorApplication.ExecuteMenuItem("YooAsset/AssetBundle Reporter");
            }
#endif

            rect.x = 20;
            if (Disk != null)
            {
                rect.width = 150;
                EditorGUI.LabelField(rect, $"磁盘剩余空间:{Disk.AvailableFreeSpace.ToConverseStringFileSize()}",
                                     GEStyle.HeaderLabel);
            }

            rect.x     += rect.width;
            rect.width =  150;
            EditorGUI.LabelField(rect, $"资源包数量:{Data.Packages.Length}", GEStyle.HeaderLabel);
        }

        void IAssetPage.OnDrawContent(Rect rect)
        {
            rect.x     += 5;
            rect.width -= 10;
            GUI.Box(rect, string.Empty, GEStyle.INThumbnailShadow);
            TreeViewBuildSetting.OnGUI(rect);
        }

        #endregion
    }
}