/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections.Generic;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源管理窗口
    /// </summary>
    [GWindow("资源收集管理器", "支持资源收集、资源管理、资源导出、资源打包等功能",
        Group = "Tools",
        Menu = "AIO/Window/Asset",
        MinSizeHeight = 500,
        MinSizeWidth = 1000
    )]
    public partial class AssetCollectWindow : GraphicWindow
    {
        [LnkTools("Asset Window", "#00BFFF", "d_Folder Icon", LnkToolsMode.AllMode, 0)]
        public static void OpenWindow()
        {
            EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
        }

        protected override void OnAwake()
        {
            Data = AssetCollectRoot.GetOrCreate();
            Selection.activeObject = Data;
        }

        partial void GCInit();

        protected override void OnActivation()
        {
            AssetCollectSetting.Initialize();

            if (Data is null) Data = AssetCollectRoot.GetOrCreate();
            else Data.Save();

            if (Config is null) Config = ASConfig.GetOrCreate();
            else Config.Save();

            if (BuildConfig is null) BuildConfig = ASBuildConfig.GetOrCreate();
            else BuildConfig.Save();

            if (_packages is null)
                _packages = Config.Packages is null
                    ? new List<AssetsPackageConfig>()
                    : Config.Packages.ToList();

            GCInit();

            UpdateData();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        protected void UpdateData()
        {
            switch (WindowMode)
            {
                default:
                case Mode.Editor:
                    UpdateDataRecordQueue();
                    break;
                case Mode.Look:
                    UpdateDataLookMode();
                    break;
                case Mode.Build:
                    UpdateDataBuildMode();
                    break;
                case Mode.Tags:
                    UpdateDataTagsMode();
                    break;
            }
        }

        #region 绘制

        protected override void OnDraw()
        {
            using (new EditorGUILayout.HorizontalScope(
                       GEStyle.INThumbnailShadow, GTOption.Height(DrawHeaderHeight - 5)))
            {
                OnDrawHeader();
            }

            switch (WindowMode)
            {
                default:
                case Mode.Editor:
                    OnDrawEditorMode();
                    break;
                case Mode.Look:
                    OnDrawLookMode();
                    break;
                case Mode.Build:
                    OnDrawBuildMode();
                    break;
                case Mode.Tags:
                    OnDrawTagsMode();
                    break;
            }

            DrawVersion(Setting.Version);
            OnOpenEvent();
        }

        partial void OnDrawEditorMode();
        partial void OnDrawTagsMode();
        partial void OnDrawBuildMode();
        partial void OnDrawLookMode();
        partial void OnDrawHeader();
        partial void OnDrawSetting();
        partial void OnDrawPackage();
        partial void OnDrawGroup();
        partial void OnDrawGroupList();
        partial void OnDrawItem(AssetCollectItem item);
        partial void OnDrawASConfig();

        /// <summary>
        /// 绘制资源 阴影
        /// </summary>
        private static void OnDrawShading(Rect rect)
        {
            if (Mathf.FloorToInt((rect.y - 4f) / rect.height % 2f) != 0) return;
            var rect2 = new Rect(rect);
            rect2.width += rect.x + rect.height;
            rect2.height += 1f;
            rect2.x = 0f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
            rect2.height = 1f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
            rect2.y += rect.height;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
        }

        #endregion

        protected override void OnDisable()
        {
            Data.Save();
        }

        protected override void OnDispose()
        {
            Data.Save();
        }

        public override void EventMouseDown(in Event eventData)
        {
            switch (WindowMode)
            {
                case Mode.Editor:
                    ViewConfig.ContainsHorizontal(eventData);
                    ViewGroupList.ContainsHorizontal(eventData);
                    ViewPackageList.ContainsHorizontal(eventData);
                    ViewSetting.ContainsHorizontal(eventData);
                    break;
                case Mode.Tags:
                case Mode.Look:
                    ViewDetailList.ContainsHorizontal(eventData);
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
                    ViewConfig.DragHorizontal(eventData);
                    ViewGroupList.DragHorizontal(eventData);
                    ViewPackageList.DragHorizontal(eventData);
                    ViewSetting.DragHorizontal(eventData);
                    break;
                }
                case Mode.Tags:
                case Mode.Look:
                {
                    ViewDetailList.DragHorizontal(eventData);
                    break;
                }
                case Mode.Build:
                default:
                    break;
            }
        }

        public override void EventMouseUp(in Event eventData)
        {
            ViewDetailList.CancelHorizontal();
            ViewConfig.CancelHorizontal();
            ViewGroupList.CancelHorizontal();
            ViewPackageList.CancelHorizontal();
            ViewSetting.CancelHorizontal();
        }
    }
}