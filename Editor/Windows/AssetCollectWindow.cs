/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源管理窗口
    /// </summary>
    [GWindow("资源收集管理器", "支持资源收集、资源管理、资源导出、资源打包等功能",
        IconResource = "Editor/Icon/Asset",
        Group = "Tools",
        Menu = "AIO/Window/Asset",
        MinSizeHeight = 650,
        MinSizeWidth = 1200
    )]
    public partial class AssetCollectWindow : GraphicWindow
    {
        [LnkTools(
            Tooltip = "AIO 资源管理工具",
            IconResource = "Editor/Icon/Asset"
        )]
        public static void OpenWindow()
        {
            EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
        }

        partial void GCInit();

        partial void OnDrawHeader()
        {
            using (new GUILayout.HorizontalScope())
            {
                switch (WindowMode)
                {
                    case Mode.Editor:
                        OnDrawHeaderEditorMode();
                        break;
                    case Mode.Config:
                        OnDrawHeaderConfigMode();
                        break;
                    case Mode.Look:
                        OnDrawHeaderLookMode();
                        break;
                    case Mode.Build:
                        OnDrawHeaderBuildMode();
                        break;
                    case Mode.LookTags:
                        OnDrawHeaderTagsMode();
                        break;
                    case Mode.LookFirstPackage:
                        OnDrawHeaderFirstPackageMode();
                        break;
                }

                WindowMode = GELayout.Popup(WindowMode, GEStyle.PreDropDown, GP_Width_75, GP_Height_20);

                if (GUI.changed)
                {
                    if (WindowMode == TempTable.GetOrDefault<Mode>(nameof(WindowMode))) return;
                    GUI.FocusControl(null);
                    UpdateData();
                    TempTable[nameof(WindowMode)] = WindowMode;
                }
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        private void UpdateData()
        {
            switch (WindowMode)
            {
                case Mode.Config:
                    UpdateDataConfigMode();
                    break;
                case Mode.Editor:
                    UpdateDataEditorMode();
                    break;
                case Mode.Look:
                    LookModeDisplayTypeIndex = 0;
                    LookModeDisplayTagsIndex = 0;
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
            Data = AssetCollectRoot.GetOrCreate();
            Selection.activeObject = Data;
        }

        protected override void OnActivation()
        {
            AssetCollectSetting.Initialize();

            Data = AssetCollectRoot.GetOrCreate();
            Data.Refresh();
            Selection.activeObject = Data;

            Config = ASConfig.GetOrCreate();
            BuildConfig = ASBuildConfig.GetOrCreate();


            GCInit();

            UpdateData();
        }

        protected override void OnDraw()
        {
            using (new GUILayout.HorizontalScope(
                       GEStyle.INThumbnailShadow, GTOption.Height(DrawHeaderHeight - 5)))
            {
                OnDrawHeader();
            }

            switch (WindowMode)
            {
                case Mode.Editor:
                    OnDrawEditorMode();
                    break;
                case Mode.Config:
                    OnDrawConfigMode();
                    break;
                case Mode.Look:
                    OnDrawLookMode();
                    break;
                case Mode.Build:
                    OnDrawBuildMode();
                    break;
                case Mode.LookTags:
                    OnDrawTagsMode();
                    break;
                case Mode.LookFirstPackage:
                    OnDrawFirstPackageMode();
                    break;
            }

            DrawVersion(Setting.Version);
            OnOpenEvent();
        }

        // 关闭自动更新
        // protected override void OnDisable()
        // {
        //     if (Data != null) Data.Save();
        //     if (Config != null) Config.Save();
        //     if (BuildConfig != null) BuildConfig.Save();
        //     AssetDatabase.SaveAssets();
        // }

        public override void EventMouseDown(in Event eventData)
        {
            switch (WindowMode)
            {
                case Mode.Editor:
                    ViewGroupList.ContainsHorizontal(eventData);
                    ViewPackageList.ContainsHorizontal(eventData);
                    break;
                case Mode.LookFirstPackage:
                case Mode.LookTags:
                case Mode.Look:
                    ViewDetailList.ContainsHorizontal(eventData);
                    break;
                case Mode.Config:
                    ViewConfig.ContainsHorizontal(eventData);
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
                    ViewGroupList.DragHorizontal(eventData);
                    ViewPackageList.DragHorizontal(eventData);
                    break;
                }
                case Mode.LookFirstPackage:
                case Mode.LookTags:
                case Mode.Look:
                {
                    ViewDetailList.DragHorizontal(eventData);
                    break;
                }
                case Mode.Config:
                    ViewConfig.DragHorizontal(eventData);
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
                case Mode.LookTags:
                case Mode.Look:
                case Mode.LookFirstPackage:
                {
                    switch (keyCode)
                    {
                        // 判断ESC
                        case KeyCode.Escape:
                            GUI.FocusControl(null);
                            CancelCurrentSelectAsset();
                            eventData.Use();
                            break;

                        case KeyCode.LeftArrow: // 数字键盘 右键
                            if (CurrentPageValues.PageIndex > 0)
                            {
                                GUI.FocusControl(null);
                                CurrentSelectAssetIndex = 0;
                                CurrentPageValues.PageIndex -= 1;
                                eventData.Use();
                            }

                            break;

                        case KeyCode.RightArrow: // 数字键盘 左键 
                            if (CurrentPageValues.PageIndex < CurrentPageValues.PageCount - 1)
                            {
                                GUI.FocusControl(null);
                                CurrentSelectAssetIndex = 0;
                                CurrentPageValues.PageIndex += 1;
                                eventData.Use();
                            }

                            break;

                        case KeyCode.UpArrow: // 数字键盘 上键
                            if (CurrentSelectAssetIndex >= 0)
                            {
                                GUI.FocusControl(null);
                                CurrentSelectAssetIndex -= 1;
                                if (CurrentSelectAssetIndex < 0)
                                {
                                    if (CurrentPageValues.CurrentPageValues.Length > 0)
                                    {
                                        CurrentSelectAssetIndex = CurrentPageValues.CurrentPageValues.Length - 1;
                                    }
                                    else CurrentSelectAssetIndex = 0;
                                }

                                UpdateCurrentSelectAsset(CurrentSelectAssetIndex);
                                eventData.Use();
                            }

                            break;

                        case KeyCode.DownArrow: // 数字键盘 下键
                            if (CurrentPageValues.CurrentPageValues is null) break;
                            if (CurrentSelectAssetIndex < CurrentPageValues.CurrentPageValues.Length)
                            {
                                GUI.FocusControl(null);
                                CurrentSelectAssetIndex += 1;
                                if (CurrentSelectAssetIndex >= CurrentPageValues.CurrentPageValues.Length)
                                    CurrentSelectAssetIndex = 0;
                                UpdateCurrentSelectAsset(CurrentSelectAssetIndex);
                                eventData.Use();
                            }

                            break;
                    }

                    break;
                }
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
                    switch (keyCode)
                    {
                        case KeyCode.UpArrow: // 数字键盘 上键
                            if (ItemCollectorsSearching)
                            {
                                if (ItemCollectorsSearchResult.Count <= 0) break;
                                if (CurrentCurrentCollectorsIndex >= 0)
                                {
                                    CurrentCurrentCollectorsIndex += 1;
                                    if (CurrentCurrentCollectorsIndex < 0)
                                    {
                                        CurrentCurrentCollectorsIndex = ItemCollectorsSearchResult.Count - 1;
                                        OnDrawItemListScroll.y = 0;
                                    }
                                    else OnDrawItemListScroll.y -= 27;

                                    GUI.FocusControl(null);
                                    eventData.Use();
                                }
                            }
                            else
                            {
                                if (Data.CurrentGroup.Length <= 0) break;
                                if (CurrentCurrentCollectorsIndex < Data.CurrentGroup.Length)
                                {
                                    CurrentCurrentCollectorsIndex += 1;
                                    if (CurrentCurrentCollectorsIndex >= Data.CurrentGroup.Length)
                                    {
                                        CurrentCurrentCollectorsIndex = 0;
                                        OnDrawItemListScroll.y = 27 * Data.CurrentGroup.Length - 1;
                                    }
                                    else OnDrawItemListScroll.y -= 27;

                                    GUI.FocusControl(null);
                                    eventData.Use();
                                }
                            }

                            break;

                        case KeyCode.DownArrow: // 数字键盘 下键
                            if (ItemCollectorsSearching)
                            {
                                if (ItemCollectorsSearchResult.Count <= 0) break;
                                if (CurrentCurrentCollectorsIndex >= 0)
                                {
                                    CurrentCurrentCollectorsIndex -= 1;
                                    if (CurrentCurrentCollectorsIndex < 0)
                                    {
                                        CurrentCurrentCollectorsIndex = ItemCollectorsSearchResult.Count - 1;
                                        OnDrawItemListScroll.y = 0;
                                    }
                                    else OnDrawItemListScroll.y += 27;

                                    GUI.FocusControl(null);
                                    eventData.Use();
                                }
                            }
                            else
                            {
                                if (Data.CurrentGroup.Length <= 0) break;
                                if (CurrentCurrentCollectorsIndex >= 0)
                                {
                                    CurrentCurrentCollectorsIndex -= 1;
                                    if (CurrentCurrentCollectorsIndex < 0)
                                    {
                                        CurrentCurrentCollectorsIndex = Data.CurrentGroup.Length - 1;
                                        OnDrawItemListScroll.y = 0;
                                    }
                                    else OnDrawItemListScroll.y += 27;

                                    GUI.FocusControl(null);
                                    eventData.Use();
                                }
                            }

                            break;
                        // 判断回车
                        case KeyCode.Return:
                        case KeyCode.KeypadEnter:
                            if (ItemCollectorsSearching)
                            {
                                if (CurrentCurrentCollectorsIndex >= 0)
                                {
                                    GUI.FocusControl(null);
                                    ItemCollectorsSearchResult[CurrentCurrentCollectorsIndex].Folded =
                                        !ItemCollectorsSearchResult[CurrentCurrentCollectorsIndex].Folded;
                                    eventData.Use();
                                }
                            }
                            else
                            {
                                if (CurrentCurrentCollectorsIndex >= 0)
                                {
                                    GUI.FocusControl(null);
                                    Data.CurrentGroup.Collectors[CurrentCurrentCollectorsIndex].Folded =
                                        !Data.CurrentGroup.Collectors[CurrentCurrentCollectorsIndex].Folded;
                                    eventData.Use();
                                }
                            }

                            break;
                    }

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
            {
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
        }

        public override void EventMouseUp(in Event eventData)
        {
            ViewDetailList.CancelHorizontal();
            ViewConfig.CancelHorizontal();
            ViewGroupList.CancelHorizontal();
            ViewPackageList.CancelHorizontal();
            ViewSetting.CancelHorizontal();
        }

        #region 绘制

        partial void OnDrawConfigMode();
        partial void OnDrawEditorMode();
        partial void OnDrawTagsMode();
        partial void OnDrawBuildMode();
        partial void OnDrawLookMode();
        partial void OnDrawHeader();
        partial void OnDrawSetting();
        partial void OnDrawPackage();
        partial void OnDrawGroup();
        partial void OnDrawGroupList();
        partial void OnDrawASConfig();

        /// <summary>
        /// 绘制资源 阴影
        /// </summary>
        private static void OnDrawShading(Rect rect)
        {
            if (Mathf.FloorToInt(((rect.y - 4f) / rect.height) % 2f) != 0) return;
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
    }
}