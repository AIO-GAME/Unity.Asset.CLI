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
        Group = "Tools",
        Menu = "AIO/Window/Asset",
        MinSizeHeight = 550,
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

            Data = AssetCollectRoot.GetOrCreate();
            Config = ASConfig.GetOrCreate();
            BuildConfig = ASBuildConfig.GetOrCreate();

            GCInit();

            UpdateData();
        }

        partial void OnDrawHeader()
        {
            using (GELayout.VHorizontal())
            {
                switch (WindowMode)
                {
                    default:
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

                if (!GUI.changed) return;
                if (WindowMode == TempTable.GetOrDefault<Mode>(nameof(WindowMode))) return;
                GUI.FocusControl(null);
                UpdateData();

                TempTable[nameof(WindowMode)] = WindowMode;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        protected void UpdateData()
        {
            switch (WindowMode)
            {
                case Mode.Config:
                    UpdateDataConfigMode();
                    break;
                default:
                case Mode.Editor:

                    break;
                case Mode.Look:
                    UpdateDataLookMode();
                    break;
                case Mode.Build:
                    UpdateDataBuildMode();
                    break;
                case Mode.LookTags:
                    UpdateDataTagsMode();
                    break;
                case Mode.LookFirstPackage:
                    UpdateDataFirstPackageMode();
                    break;
            }
        }

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
        partial void OnDrawItem(AssetCollectItem item);
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

        protected override void OnDisable()
        {
            Data.Save();
            Config.Save();
            BuildConfig.Save();
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if UNITY_2021_1_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(Data);
            AssetDatabase.SaveAssetIfDirty(Config);
            AssetDatabase.SaveAssetIfDirty(BuildConfig);
#else
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
#endif
        }

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
                        case KeyCode.LeftArrow: // 数字键盘 右键
                            if (CurrentPageValues.PageIndex > 0)
                            {
                                CurrentSelectAssetIndex = 0;
                                CurrentPageValues.PageIndex -= 1;
                                eventData.Use();
                            }

                            break;

                        case KeyCode.RightArrow: // 数字键盘 左键 
                            if (CurrentPageValues.PageIndex < CurrentPageValues.PageCount - 1)
                            {
                                CurrentSelectAssetIndex = 0;
                                CurrentPageValues.PageIndex += 1;
                                eventData.Use();
                            }

                            break;

                        case KeyCode.UpArrow: // 数字键盘 上键
                            if (CurrentSelectAssetIndex >= 0)
                            {
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
                        Config.Save();
                        eventData.Use();
#if UNITY_2021_1_OR_NEWER
                        AssetDatabase.SaveAssetIfDirty(Config);
#else
                        AssetDatabase.SaveAssets();
#endif
                    }

                    break;
                case Mode.Editor:
                    if (eventData.control && keyCode == KeyCode.S)
                    {
                        Data.Save();
                        eventData.Use();
#if UNITY_2021_1_OR_NEWER
                        AssetDatabase.SaveAssetIfDirty(Data);
#else
                        AssetDatabase.SaveAssets();
#endif
                    }

                    break;
                case Mode.Build:
                    if (eventData.control && keyCode == KeyCode.S)
                    {
                        BuildConfig.Save();
                        eventData.Use();
#if UNITY_2021_1_OR_NEWER
                        AssetDatabase.SaveAssetIfDirty(BuildConfig);
#else
                        AssetDatabase.SaveAssets();
#endif
                    }

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