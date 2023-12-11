/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
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
        [LnkTools("Asset Window", "#00BFFF", "d_Folder Icon", LnkToolsMode.AllMode, -5)]
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

            Data.Packages = Data.Packages.OrderByDescending(package => package.Name).ToArray();
            for (var i = 0; i < Data.Packages.Length; i++)
            {
                if (Data.Packages[CurrentPackageIndex]?.Groups is null) continue;
                for (var j = 0; j < Data.Packages[CurrentPackageIndex].Groups.Length; j++)
                {
                    Data.Packages[CurrentPackageIndex].Groups = Data.Packages[CurrentPackageIndex].Groups
                        .OrderByDescending(group => group.Name).ToArray();
                }
            }

            if (_packages is null)
                _packages = Config.Packages is null
                    ? new List<AssetsPackageConfig>()
                    : Config.Packages.ToList();

            GCInit();

            UpdateData();
        }

        protected void UpdateData()
        {
            switch (WindowMode)
            {
                case Mode.Editor:
                    UpdateDataRecordQueue();
                    break;
                case Mode.Look:
                    UpdateDataLook();
                    break;
                case Mode.Build:
                    UpdateDataBuild();
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
                    OnDrawNoLook();
                    break;
                case Mode.Look:
                    OnDrawLookMode();
                    break;
                case Mode.Build:
                    OnDrawBuild();
                    break;
            }

            DrawVersion(Setting.Version);
            OnOpenEvent();
        }

        protected void OnDrawNoLook()
        {
            DoDrawRect.x = 5;
            DoDrawRect.height = CurrentHeight - DrawHeaderHeight;
            if (ShowSetting)
            {
                DoDrawRect.width = DrawWidthSettingView - 5;
                GULayout.BeginArea(DoDrawRect, GEStyle.INThumbnailShadow);
                OnDrawSettingScroll = GELayout.VScrollView(OnDrawSetting, OnDrawSettingScroll);
                GULayout.EndArea();
                DoDrawRect.x += DrawWidthSettingView;

                OnDragRectSettingView = new Rect(DoDrawRect.x - 5, DoDrawRect.y, 5, DoDrawRect.height);
                EditorGUIUtility.AddCursorRect(OnDragRectSettingView, MouseCursor.ResizeHorizontal);
            }

            if (ShowPackage)
            {
                DoDrawRect.width = DrawWidthPackageList - 5;
                GULayout.BeginArea(DoDrawRect, GEStyle.INThumbnailShadow);
                OnDrawPackageScroll = GELayout.VScrollView(OnDrawPackage, OnDrawPackageScroll);
                GULayout.EndArea();
                DoDrawRect.x += DrawWidthPackageList;

                OnDragRectPackageList = new Rect(DoDrawRect.x - 5, DoDrawRect.y, 5, DoDrawRect.height);
                EditorGUIUtility.AddCursorRect(OnDragRectPackageList, MouseCursor.ResizeHorizontal);
            }

            if (ShowGroup)
            {
                DoDrawRect.width = DrawWidthGroupList - 5;
                GULayout.BeginArea(DoDrawRect, GEStyle.INThumbnailShadow);
                OnDrawGroupScroll = GELayout.VScrollView(OnDrawGroup, OnDrawGroupScroll);
                GULayout.EndArea();
                DoDrawRect.x += DrawWidthGroupList;

                OnDragRectGroupList = new Rect(DoDrawRect.x - 5, DoDrawRect.y, 5, DoDrawRect.height);
                EditorGUIUtility.AddCursorRect(OnDragRectGroupList, MouseCursor.ResizeHorizontal);
            }

            DoDrawRect.width = CurrentWidth - DoDrawRect.x - 5;
            GULayout.BeginArea(DoDrawRect);
            OnDrawGroupListScroll = GELayout.VScrollView(OnDrawGroupList, OnDrawGroupListScroll);
            GULayout.EndArea();
        }

        partial void OnDrawBuild();
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
                    if (OnDragRectSettingView.Contains(eventData.mousePosition))
                    {
                        OnDragRectSettingViewing = true;
                    }
                    else if (OnDragRectPackageList.Contains(eventData.mousePosition))
                    {
                        OnDragRectPackageListViewing = true;
                    }
                    else if (OnDragRectGroupList.Contains(eventData.mousePosition))
                    {
                        OnDragRectGroupListViewing = true;
                    }

                    break;
                case Mode.Look:
                    if (OnDragRectDetailsView.Contains(eventData.mousePosition))
                    {
                        OnDragRectDetailsViewing = true;
                    }

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
                    if (OnDragRectSettingViewing)
                    {
                        var temp = DrawWidthSettingView + eventData.delta.x;
                        if (temp < 100)
                        {
                            DrawWidthSettingView = 100;
                            eventData.Use();
                            break;
                        }

                        var t = CurrentWidth - temp;
                        if (ShowGroup) t -= DrawWidthGroupList;
                        if (ShowPackage) t -= DrawWidthPackageList;
                        if (t < 100) break;
                        DrawWidthSettingView = temp;
                        eventData.Use();
                    }

                    else if (OnDragRectPackageListViewing)
                    {
                        var temp = DrawWidthPackageList + eventData.delta.x;
                        if (temp < 100)
                        {
                            DrawWidthPackageList = 100;
                            eventData.Use();
                            break;
                        }

                        var t = CurrentWidth - temp;
                        if (ShowSetting) t -= DrawWidthSettingView;
                        if (ShowGroup) t -= DrawWidthGroupList;
                        if (t < 100) break;
                        DrawWidthPackageList = temp;
                        eventData.Use();
                    }

                    else if (OnDragRectGroupListViewing)
                    {
                        var temp = DrawWidthGroupList + eventData.delta.x;
                        if (temp < 100)
                        {
                            DrawWidthGroupList = 100;
                            eventData.Use();
                            break;
                        }

                        var t = CurrentWidth - temp;
                        if (ShowSetting) t -= DrawWidthSettingView;
                        if (ShowPackage) t -= DrawWidthPackageList;
                        if (t < 100) break;
                        DrawWidthGroupList = temp;
                        eventData.Use();
                    }

                    break;
                }
                case Mode.Look:
                {
                    if (!OnDragRectDetailsViewing) return;
                    var temp = LookModeShowAssetListWidth + eventData.delta.x;
                    if (temp < LookModeShowAssetListMinWidth) break;
                    if (CurrentWidth - temp < LookModeShowAssetDetailMinWidth) break;
                    LookModeShowAssetListWidth = temp;
                    eventData.Use();
                    break;
                }
                case Mode.Build:
                default:
                    break;
            }
        }

        public override void EventMouseUp(in Event eventData)
        {
            OnDragRectSettingViewing = OnDragRectGroupListViewing =
                OnDragRectPackageListViewing = OnDragRectDetailsViewing = false;
        }
    }
}