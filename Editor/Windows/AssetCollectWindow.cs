using System.IO;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    ///     资源管理窗口
    /// </summary>
    [GWindow("资源管理器", "支持资源收集、资源管理、资源导出、资源打包等功能",
                IconResource = "Editor/Icon/Asset",
                Group = "Tools",
                Menu = "AIO/Window/Asset",
                MinSizeHeight = 650,
                MinSizeWidth = 1200
            )]
    public partial class AssetCollectWindow : GraphicWindow
    {
        private static AssetCollectWindow Instance;

        [LnkTools(
                     Tooltip = "AIO 资源管理工具",
                     IconResource = "Editor/Icon/Asset",
                     ShowMode = ELnkShowMode.Toolbar
                 )]
        public static void OpenWindow() { EditorApplication.ExecuteMenuItem("AIO/Window/Asset"); }

        [MenuItem("AIO/Asset/清空运行时缓存")]
        public static void ClearRuntimeCache()
        {
            var sandbox = Path.Combine(EHelper.Path.Project, ASConfig.GetOrCreate().RuntimeRootDirectory);
            if (Directory.Exists(sandbox))
                AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
        }

        [MenuItem("AIO/Asset/清空构建时缓存")]
        public static void ClearBuildCache()
        {
            var sandbox = Path.Combine(EHelper.Path.Project, "Bundles");
            if (Directory.Exists(sandbox))
                AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
        }

        partial void GCInit();

        partial void OnDrawHeaderBuildMode(Rect        rect);
        partial void OnDrawHeaderEditorMode(Rect       rect);
        partial void OnDrawHeaderConfigMode(Rect       rect);
        partial void OnDrawHeaderLookMode(Rect         rect);
        partial void OnDrawHeaderFirstPackageMode(Rect rect);
        partial void OnDrawHeaderTagsMode(Rect         rect);

        partial void OnDrawHeader(Rect rect)
        {
            rect.x     =  0;
            rect.y     =  0;
            rect.width -= 75;
            using (new GUI.GroupScope(rect))
            {
                rect.height = 20;
                switch (WindowMode)
                {
                    case Mode.Editor:
                        OnDrawHeaderEditorMode(rect);
                        break;
                    case Mode.Config:
                        OnDrawHeaderConfigMode(rect);
                        break;
                    case Mode.Look:
                        OnDrawHeaderLookMode(rect);
                        break;
                    case Mode.Build:
                        OnDrawHeaderBuildMode(rect);
                        break;
                    case Mode.LookTags:
                        OnDrawHeaderTagsMode(rect);
                        break;
                    case Mode.LookFirstPackage:
                        OnDrawHeaderFirstPackageMode(rect);
                        break;
                }
            }

            rect.x     = rect.width;
            rect.width = 75;
            WindowMode = (Mode)EditorGUI.EnumPopup(rect, WindowMode, GEStyle.PreDropDown);

            if (GUI.changed)
            {
                if (WindowMode == TempTable.GetOrDefault<Mode>(nameof(WindowMode))) return;
                GUI.FocusControl(null);
                UpdateData();
                TempTable[nameof(WindowMode)] = WindowMode;
            }
        }

        /// <summary>
        ///     更新数据
        /// </summary>
        private void UpdateData()
        {
            LookCurrentSelectAsset = null;
            switch (WindowMode)
            {
                case Mode.Config:
                    UpdateDataConfigMode();
                    break;
                case Mode.Look:
                    LookModeDisplayTypeIndex       = 0;
                    LookModeDisplayTagsIndex       = 0;
                    LookModeDisplayCollectorsIndex = 0;
                    UpdateDataLookMode();
                    break;
                case Mode.Build:
                    UpdateDataBuildMode();
                    break;
                case Mode.LookTags:
                    LookModeDisplayTypeIndex = 0;
                    LookModeDisplayTagsIndex = 0;
                    UpdateDataTagsMode();
                    break;
                case Mode.LookFirstPackage:
                    LookModeDisplayTypeIndex = 0;
                    LookModeDisplayTagsIndex = 0;
                    UpdateDataFirstPackageMode();
                    break;
            }
        }

        protected override void OnAwake()
        {
            Instance               = this;
            Data                   = AssetCollectRoot.GetOrCreate();
            Selection.activeObject = Data;
        }

        protected override void OnActivation()
        {
            Instance = this;
            Data     = AssetCollectRoot.GetOrCreate();
            Data.Refresh();
            Selection.activeObject = Data;

            Config      = ASConfig.GetOrCreate();
            BuildConfig = ASBuildConfig.GetOrCreate();

            GCInit();

            UpdateData();
        }

        protected override void OnDraw()
        {
            DrawRect.Set(0, 0, CurrentWidth, DrawHeaderHeight - 5);
            using (new GUI.GroupScope(DrawRect, GEStyle.INThumbnailShadow)) OnDrawHeader(DrawRect);
            DrawRect.y      = DrawHeaderHeight;
            DrawRect.height = CurrentHeight - DrawHeaderHeight;
            using (new GUI.GroupScope(DrawRect)) OnDrawBody(DrawRect);

            DrawVersion(Setting.Version);
            OnOpenEvent();
        }

        public override void EventMouseDown(in Event eventData)
        {
            switch (WindowMode)
            {
                case Mode.Editor:
                    ViewGroupList.ContainsDragStretch(eventData, ViewRect.DragStretchType.Horizontal);
                    ViewPackageList.ContainsDragStretch(eventData, ViewRect.DragStretchType.Horizontal);
                    break;
                case Mode.LookFirstPackage:
                case Mode.LookTags:
                case Mode.Look:
                    ViewDetailList.ContainsDragStretch(eventData, ViewRect.DragStretchType.Horizontal);
                    break;
                case Mode.Config:
                    ViewConfig.ContainsDragStretch(eventData, ViewRect.DragStretchType.Horizontal);
                    break;
                case Mode.Build:
                default:
                    break;
            }
        }

        public override void EventMouseDrag(in Event eventData)
        {
            switch (WindowMode)
            {
                case Mode.Editor:
                {
                    ViewGroupList.DraggingStretch(eventData, ViewRect.DragStretchType.Horizontal);
                    ViewPackageList.DraggingStretch(eventData, ViewRect.DragStretchType.Horizontal);
                    break;
                }
                case Mode.LookFirstPackage:
                case Mode.LookTags:
                case Mode.Look:
                {
                    ViewDetailList.DraggingStretch(eventData, ViewRect.DragStretchType.Horizontal);
                    break;
                }
                case Mode.Config:
                    ViewConfig.DraggingStretch(eventData, ViewRect.DragStretchType.Horizontal);
                    break;
                case Mode.Build:
                default:
                    break;
            }
        }

        public override void EventKeyDown(in Event eventData, in KeyCode keyCode)
        {
            switch (WindowMode)
            {
                case Mode.Config:
                    if (eventData.control && keyCode == KeyCode.S)
                    {
                        GUI.FocusControl(null);
                        Config.Save();
                        AssetDatabase.SaveAssets();
                        eventData.Use();
                    }

                    break;
                case Mode.Editor:

                    if (eventData.control && keyCode == KeyCode.S)
                    {
                        GUI.FocusControl(null);
                        Data.Save();
                        AssetDatabase.SaveAssets();
                        eventData.Use();
                    }

                    break;
                case Mode.Build:
                    if (eventData.control && keyCode == KeyCode.S)
                    {
                        GUI.FocusControl(null);
                        BuildConfig.Save();
                        AssetDatabase.SaveAssets();
                        eventData.Use();
                    }

                    break;
            }

            if (eventData.control)
                switch (keyCode)
                {
                    case KeyCode.Keypad1:
                    case KeyCode.Alpha1:
                        GUI.FocusControl(null);
                        WindowMode = Mode.Editor;
                        UpdateData();
                        eventData.Use();
                        break;
                    case KeyCode.Keypad2:
                    case KeyCode.Alpha2:
                        GUI.FocusControl(null);
                        WindowMode = Mode.Config;
                        UpdateData();
                        eventData.Use();
                        break;
                    case KeyCode.Keypad3:
                    case KeyCode.Alpha3:
                        GUI.FocusControl(null);
                        WindowMode = Mode.Look;
                        UpdateData();
                        eventData.Use();
                        break;
                    case KeyCode.Keypad4:
                    case KeyCode.Alpha4:
                        GUI.FocusControl(null);
                        WindowMode = Mode.LookTags;
                        UpdateData();
                        eventData.Use();
                        break;
                    case KeyCode.Keypad5:
                    case KeyCode.Alpha5:
                        GUI.FocusControl(null);
                        WindowMode = Mode.LookFirstPackage;
                        UpdateData();
                        eventData.Use();
                        break;
                    case KeyCode.Keypad6:
                    case KeyCode.Alpha6:
                        GUI.FocusControl(null);
                        WindowMode = Mode.Build;
                        UpdateData();
                        eventData.Use();
                        break;
                }
        }

        public override void EventMouseUp(in Event eventData)
        {
            ViewDetailList.CancelDragStretch();
            ViewConfig.CancelDragStretch();
            ViewGroupList.CancelDragStretch();
            ViewPackageList.CancelDragStretch();
            ViewSetting.CancelDragStretch();
        }

        #region 绘制

        private void OnDrawBody(Rect rect)
        {
            rect.x = 0;
            rect.y = 5;
            switch (WindowMode)
            {
                case Mode.Editor:
                    OnDrawEditorMode(rect);
                    break;
                case Mode.Config:
                    OnDrawConfigMode(rect);
                    break;
                case Mode.Look:
                    OnDrawLookMode(rect);
                    break;
                case Mode.Build:
                    OnDrawBuildMode(rect);
                    break;
                case Mode.LookTags:
                    OnDrawTagsMode(rect);
                    break;
                case Mode.LookFirstPackage:
                    OnDrawFirstPackageMode();
                    break;
            }
        }

        partial void OnDrawConfigMode(Rect rect);
        partial void OnDrawEditorMode(Rect rect);
        partial void OnDrawTagsMode(Rect   rect);
        partial void OnDrawBuildMode(Rect  rect);
        partial void OnDrawLookMode(Rect   rect);
        partial void OnDrawHeader(Rect     rect);
        partial void OnDrawSetting(Rect    rect);
        partial void OnDrawASConfig(Rect   rect);

        /// <summary>
        ///     绘制资源 阴影
        /// </summary>
        private static void OnDrawShading(Rect rect)
        {
            if (Mathf.FloorToInt((rect.y - 4f) / rect.height % 2f) != 0) return;
            var rect2 = new Rect(rect);
            rect2.width  += rect.x + rect.height;
            rect2.height += 1f;
            rect2.x      =  0f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
            rect2.height = 1f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
            rect2.y += rect.height;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
        }

        #endregion
    }
}