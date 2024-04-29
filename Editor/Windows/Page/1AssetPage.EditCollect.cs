using System;
using System.Text;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public class AssetPageEditCollect : IAssetPage
    {
        int IAssetPage.   Order => 1;
        string IAssetPage.Title => "编辑模式      [Ctrl + Number1]";

        void IAssetPage.EventMouseDown(in Event evt)
        {
            if (ViewGroupList.IsShow) ViewGroupList.ContainsDragStretch(evt, ViewRect.DragStretchType.Horizontal);
            if (ViewPackageList.IsShow) ViewPackageList.ContainsDragStretch(evt, ViewRect.DragStretchType.Horizontal);
        }

        void IAssetPage.EventMouseUp(in Event evt)
        {
            ViewGroupList.CancelDragStretch();
            ViewPackageList.CancelDragStretch();
        }

        void IAssetPage.EventKeyDown(in Event evt, in KeyCode keyCode)
        {
            if (!evt.control || keyCode != KeyCode.S) return;
            GUI.FocusControl(null);
            Data.Save();
            AssetDatabase.SaveAssets();
            evt.Use();
        }

        void IAssetPage.EventKeyUp(in Event evt, in KeyCode keyCode) { }

        void IAssetPage.EventMouseDrag(in Event evt)
        {
            ViewGroupList.DraggingStretch(evt, ViewRect.DragStretchType.Horizontal);
            ViewPackageList.DraggingStretch(evt, ViewRect.DragStretchType.Horizontal);
        }

        bool IAssetPage.Shortcut(Event evt)
        {
            if (evt.type == EventType.KeyDown && evt.control)
                return evt.keyCode == KeyCode.Keypad1
                    || evt.keyCode == KeyCode.Alpha1
                    ;

            return false;
        }

        void IAssetPage.UpdateData()
        {
            Data = AssetCollectRoot.GetOrCreate();
            TreeViewPackage.Reload();
            TreeViewGroup.Reload();
            TreeViewCollector.Reload();
        }

        private AssetCollectRoot Data;

        private TreeViewGroup   TreeViewGroup;
        private TreeViewPackage TreeViewPackage;
        private TreeViewCollect TreeViewCollector;

        private ViewRect ViewPackageList;
        private ViewRect ViewGroupList;
        private ViewRect ViewCollectorsList;

        private StringBuilder TempBuilder;
        private GUIContent    GC_Select;
        private GUIContent    GC_MERGE;
        private GUIContent    GC_ToConvert;
        private GUIContent    GC_SAVE;
        private GUIContent    GC_REFRESH;

        public AssetPageEditCollect()
        {
            GC_MERGE     = new GUIContent("合并配置", "合并当前资源包的所有组和收集器");
            GC_ToConvert = new GUIContent("转换配置", "转换为第三方配置文件");
            GC_SAVE      = EditorGUIUtility.IconContent("d_SaveAs", "保存");
            GC_REFRESH   = EditorGUIUtility.IconContent("Refresh", "刷新");
            GC_Select    = GEContent.NewSetting("ic_Eyes", "选择资源配置文件");
            TempBuilder  = new StringBuilder();

            Data = AssetCollectRoot.GetOrCreate();

            ViewPackageList    = new ViewRect();
            ViewGroupList      = new ViewRect();
            ViewCollectorsList = new ViewRect();


            ViewPackageList = new ViewRect(120, 1)
            {
                IsShow = true, IsAllowDragStretchHorizontal = true, DragStretchHorizontalWidth = 5, width = 150,
            };

            ViewGroupList = new ViewRect(120, 1)
            {
                IsShow = true, IsAllowDragStretchHorizontal = true, DragStretchHorizontalWidth = 5, width = 150,
            };

            ViewCollectorsList = new ViewRect(700, 1)
            {
                IsShow = true, IsAllowDragStretchHorizontal = false, width = 750,
            };

            TreeViewPackage   = TreeViewPackage.Create();
            TreeViewGroup     = TreeViewGroup.Create();
            TreeViewCollector = TreeViewCollect.Create(300, ViewCollectorsList.MinWidth, ViewCollectorsList.MaxWidth);
            TreeViewPackage.OnSingleSelectionChanged += id =>
            {
                Data.CurrentPackageIndex = id;
                Data.CurrentGroupIndex   = 0;
                TreeViewGroup.Reload();
                TreeViewCollector.Reload();
            };
            TreeViewGroup.OnSingleSelectionChanged += id =>
            {
                Data.CurrentGroupIndex   = id;
                Data.CurrentCollectIndex = 0;
                TreeViewCollector.Reload();
            };
        }

        public void Dispose()
        {
            TreeViewPackage   = null;
            TreeViewGroup     = null;
            TreeViewCollector = null;
        }

        public static void OpenCollectItem(AssetCollectItem item)
        {
            if (item.Type == EAssetCollectItemType.MainAssetCollector)
            {
                var list = AssetCollectRoot.GetOrCreate().CurrentGroup.Collectors.GetDisPlayNames();
                if (list.Length > 31)
                {
                    AssetPageLook.DisplayCollectorsIndex = 0;
                    for (var i = 0; i < list.Length; i++)
                        if (list[i] == item.CollectPath)
                        {
                            AssetPageLook.DisplayCollectorsIndex = i + 1;
                            break;
                        }
                }
                else
                {
                    var status = 1;
                    foreach (var collector in list)
                    {
                        if (collector != item.CollectPath)
                        {
                            status *= 2;
                            continue;
                        }

                        AssetPageLook.DisplayCollectorsIndex = status;
                        break;
                    }
                }

                AssetWindow.OpenPage<AssetPageLook.Collect>();
                return;
            }

            EditorUtility.DisplayDialog("打开", "只有动态资源才能查询", "确定");
        }

        void IAssetPage.OnDrawContent(Rect rect)
        {
            ViewGroupList.y      = ViewPackageList.y         = ViewCollectorsList.y   = rect.y;
            ViewGroupList.height = ViewCollectorsList.height = ViewPackageList.height = rect.height;

            var temp = rect.width - ViewCollectorsList.MinWidth;
            if (ViewPackageList.IsShow && ViewPackageList.IsAllowDragStretchHorizontal)
                temp -= ViewPackageList.MinWidth;
            if (ViewGroupList.IsShow && ViewGroupList.IsAllowDragStretchHorizontal)
                temp -= ViewGroupList.MinWidth;

            ViewGroupList.MaxWidth   = temp;
            ViewPackageList.MaxWidth = temp;

            ViewCollectorsList.x = 5;
            if (ViewPackageList.IsShow)
            {
                ViewPackageList.x = ViewCollectorsList.x;
                ViewPackageList.Draw(TreeViewPackage.OnGUI, GEStyle.INThumbnailShadow);
                ViewCollectorsList.x = ViewPackageList.width + ViewPackageList.x;
            }

            if (ViewGroupList.IsShow)
            {
                ViewGroupList.x = ViewCollectorsList.x;
                ViewGroupList.Draw(TreeViewGroup.OnGUI, GEStyle.INThumbnailShadow);
                ViewCollectorsList.x = ViewGroupList.width + ViewGroupList.x;
            }

            ViewCollectorsList.width = rect.width - ViewCollectorsList.x - 5;
            ViewCollectorsList.Draw(TreeViewCollector.OnGUI);
        }

        public void OnDrawHeader(Rect rect)
        {
            var width = rect.width;
            rect.height = 20;
            rect.x      = rect.width - 30;
            rect.width  = 30;
            if (GUI.Button(rect, GC_SAVE, GEStyle.TEtoolbarbutton))
            {
                try
                {
                    GUI.FocusControl(null);
                    Data.Save();
#if UNITY_2021_1_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(Data);
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

            rect.width =  30;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_REFRESH, GEStyle.TEtoolbarbutton))
            {
                Data.Refresh();
            }

            rect.width =  30;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_Select, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                Selection.activeObject = ASConfig.GetOrCreate();
            }

            rect.width =  75;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_ToConvert, GEStyle.TEtoolbarbutton))
            {
                try
                {
                    GUI.FocusControl(null);
                    AssetProxyEditor.ConvertConfig(Data, false);
                    EditorUtility.DisplayDialog("转换", "转换 YooAsset 成功", "确定");
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            rect.width =  75;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_MERGE, GEStyle.TEtoolbarbutton))
            {
                if (EditorUtility.DisplayDialog("合并", "确定合并当前资源包的所有组和收集器?", "确定", "取消"))
                    Data.MergeCollector(Data.CurrentPackage.Name);
            }

            rect.x     = 0;
            rect.width = ViewPackageList.width + ViewPackageList.DragStretchHorizontalWidth / 2;
            if (GUI.Button(rect, $"{(ViewPackageList.IsShow ? "⇘" : "⇗")} Package", GEStyle.TEtoolbarbutton))
            {
                ViewPackageList.IsShow = !ViewPackageList.IsShow;
            }

            rect.x     += rect.width;
            rect.width =  ViewGroupList.width + ViewGroupList.DragStretchHorizontalWidth / 2;
            if (GUI.Button(rect, $"{(ViewGroupList.IsShow ? "⇘" : "⇗")} Group", GEStyle.TEtoolbarbutton))
            {
                ViewGroupList.IsShow = !ViewGroupList.IsShow;
            }

            TempBuilder.Clear();
            if (Data.IsValidPackage())
            {
                TempBuilder.Append(Data.CurrentPackage.Name);
                if (!string.IsNullOrEmpty(Data.CurrentPackage.Description))
                {
                    TempBuilder.Append('(').Append(Data.CurrentPackage.Description).Append(')');
                }
            }
            else ViewGroupList.IsShow = false;

            if (Data.IsValidGroup())
            {
                TempBuilder.Append(" / ").Append(Data.CurrentGroup.Name);
                if (!string.IsNullOrEmpty(Data.CurrentGroup.Description))
                {
                    TempBuilder.Append('(').Append(Data.CurrentGroup.Description).Append(')');
                }
            }

            rect.x     = 0;
            rect.width = width;
            EditorGUI.DropShadowLabel(rect, TempBuilder.ToString());
        }
    }
}