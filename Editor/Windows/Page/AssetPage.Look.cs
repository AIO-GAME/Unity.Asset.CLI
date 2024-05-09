using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    public partial class AssetPageLook
    {
        #region Instance

        static AssetPageLook() { _Instance = new AssetPageLook(); }

        private static AssetPageLook _Instance;
        private static AssetPageLook Instance => _Instance ?? (_Instance = new AssetPageLook());

        #endregion

        private static AssetCollectRoot   Data;   // 资源数据
        private static ASConfig           Config; // 配置文件
        private static TreeViewQueryAsset TreeViewQueryAsset;

        private static TreeViewDependencies TreeViewDependencies;

        private static PageList<AssetDataInfo> PageValues; // 当前页资源列表
        private static List<AssetDataInfo>     Values;     // 当前所有资源

        private static string[] DisplayPackages;   // 列表:包
        private static string[] DisplayTags;       // 列表:标签
        private static string[] DisplayCollectors; // 列表:收集器
        private static string[] DisplayTypes;      // 列表:类型

        public static int DisplayTypeIndex;            // 当前选择包类型索引
        public static int DisplayCollectorsIndex = -1; // 当前选择收集器索引
        public static int DisplayTagsIndex;            // 当前标签列表索引

        private readonly GUIContent GC_CLEAR;
        private readonly GUIContent GC_DEL;
        private readonly GUIContent GC_DOWNLOAD;
        private readonly GUIContent GC_NET;
        private readonly GUIContent GC_OPEN_FOLDER;
        private readonly GUIContent GC_REFRESH;
        private readonly GUIContent GC_SAVE;
        private readonly GUIContent GC_COPY;
        private readonly GUIContent GC_Select;

        private readonly GUIContent GC_Detail_IsSubAsset;
        private readonly GUIContent GC_Detail_Size;
        private readonly GUIContent GC_Detail_GUID;
        private readonly GUIContent GC_Detail_Type;
        private readonly GUIContent GC_Detail_Path;
        private readonly GUIContent GC_Detail_LastWriteTime;
        private readonly GUIContent GC_Detail_Tags;

        private readonly GUIContent GC_LookMode_Page_MaxLeft;  // 界面内容 - 第一页
        private readonly GUIContent GC_LookMode_Page_Left;     // 界面内容 - 左边一页
        private readonly GUIContent GC_LookMode_Page_MaxRight; // 界面内容 - 最后一页
        private readonly GUIContent GC_LookMode_Page_Right;    // 界面内容 - 右边一页
        private readonly GUIContent GC_LookMode_Page_Size;     // 界面内容 - 页面大小

        private ViewRect ViewDetailList; // 界面 - 资源列表
        private ViewRect ViewDetails;    // 界面 - 详情界面

        #region Dependencies

        public class DependenciesInfo
        {
            private string _Type;

            public string Name => !Object ? "Obj is Null" : Object.name;

            public string AssetPath;

            public Object Object;

            [Description("大小")]
            public long Size;

            public string Type
            {
                get
                {
                    if (string.IsNullOrEmpty(_Type)) return _Type;
                    _Type = Object is null ? "Unknown" : Object.GetType().FullName;
                    return _Type;
                }
            }
        }

        private Object        SelectAsset;         // 用户当前选择的资源实体
        private AssetDataInfo SelectAssetDataInfo; // 选择的资源实体配置
        private long          DependenciesSize;    // 依赖资源大小

        private bool ShowAssetDetail => !string.IsNullOrEmpty(SelectAssetDataInfo.GUID) && SelectAsset; //是否显示资源详情

        private readonly Dictionary<string, DependenciesInfo> Dependencies = new Dictionary<string, DependenciesInfo>(); // 依赖资源

        private void OnQueryAsseChanged(int id)
        {
            Runner.StopCoroutine(OnSelectionChangedRef);
            if (id < 0)
            {
                SelectAsset = null;
                return;
            }

            SelectAssetDataInfo = PageValues.CurrentPageValues[id];
            Runner.StartCoroutine(OnSelectionChangedRef);
        }

        private IEnumerator OnSelectionChangedRef()
        {
            Dependencies.Clear();
            yield return Runner.WaitForSeconds(0.5f);
            var assetPath = SelectAssetDataInfo.AssetPath;
            foreach (var dependency in AssetDatabase.GetDependencies(assetPath))
            {
                if (assetPath == dependency) continue;
                if (Dependencies.ContainsKey(dependency)) continue;
                var info = new DependenciesInfo
                {
                    AssetPath = dependency, Object = AssetDatabase.LoadAssetAtPath<Object>(dependency)
                };
                if (!info.Object) continue;
                DependenciesSize += info.Size = new FileInfo(dependency).Length;
                Dependencies.Add(dependency, info);
            }

            SelectAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            TreeViewDependencies.Reload(Dependencies.Values);
        }

        private static bool IsFirstPackageResource(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return false;
            return Config.EnableSequenceRecord && Config.SequenceRecord.ContainsGUID(guid);
        }

        private static void OnFirstPackageResource(AssetDataInfo data, bool isAdd)
        {
            if (isAdd)
            {
                var record = new AssetSystem.SequenceRecord
                {
                    AssetPath = data.AssetPath, Location = data.Address, PackageName = data.Package, Bytes = data.Size, Count = 1, Time = DateTime.MinValue
                };
                record.SetGUID(data.GUID);
                Config.SequenceRecord.Add(record);
                Config.SequenceRecord.Save();
                Config.SequenceRecord.UpdateLocal();
                if (AssetWindow.IsOpenPage<FirstPackage>())
                {
                    PageValues.Add(data);
                    TreeViewQueryAsset.Reload(PageValues);
                }
            }
            else
            {
                Config.SequenceRecord.UpdateLocal();
                Config.SequenceRecord.RemoveAssetPath(data.AssetPath);
                Config.SequenceRecord.Save();
                Config.SequenceRecord.UpdateLocal();
                if (AssetWindow.IsOpenPage<FirstPackage>())
                {
                    for (var i = 0; i < PageValues.Count; i++)
                    {
                        if (PageValues[i].GUID != data.GUID) continue;
                        PageValues.RemoveAt(i);
                        TreeViewQueryAsset.Reload(PageValues);
                        break;
                    }
                }
            }

            Event.current?.Use();
        }

        #endregion

        private AssetPageLook()
        {
            Config = ASConfig.GetOrCreate();
            Data   = AssetCollectRoot.GetOrCreate();
            Values = new List<AssetDataInfo>();

            GC_CLEAR       = GEContent.NewSetting("cancel", "清空");
            GC_DEL         = GEContent.NewBuiltin("d_TreeEditor.Trash", "删除");
            GC_DOWNLOAD    = GEContent.NewBuiltin("d_UnityEditor.ConsoleWindow", "下载");
            GC_NET         = GEContent.NewBuiltin("d_preAudioLoopOff", "云端");
            GC_OPEN_FOLDER = GEContent.NewBuiltin("d_Folder Icon", "打开文件夹");
            GC_REFRESH     = GEContent.NewBuiltin("d_Refresh", "刷新");
            GC_SAVE        = GEContent.NewBuiltin("d_SaveAs", "保存");
            GC_Select      = GEContent.NewSetting("ic_Eyes", "选择资源配置文件");
            GC_COPY        = GEContent.NewSetting("ic_copy", "复制资源路径");

            GC_Detail_IsSubAsset    = EditorGUIUtility.TrTextContent("是否被引用", "资源是否构成了其他资源的一部分？");
            GC_Detail_Size          = EditorGUIUtility.TrTextContent("资源大小", "文件大小");
            GC_Detail_GUID          = EditorGUIUtility.TrTextContent("GUID", "资源GUID");
            GC_Detail_Type          = EditorGUIUtility.TrTextContent("资源类型", "资源类型");
            GC_Detail_Path          = EditorGUIUtility.TrTextContent("资源路径", "资源文件路径");
            GC_Detail_LastWriteTime = EditorGUIUtility.TrTextContent("最后修改时间", "最后写入时间");
            GC_Detail_Tags          = EditorGUIUtility.TrTextContent("资源标签", "资源标签");

            GC_LookMode_Page_MaxLeft  = EditorGUIUtility.IconContent("d_scrollleft").SetTooltips("跳转到第一页");
            GC_LookMode_Page_Left     = EditorGUIUtility.IconContent("ArrowNavigationLeft").SetTooltips("上一页");
            GC_LookMode_Page_MaxRight = EditorGUIUtility.IconContent("d_scrollright").SetTooltips("跳转到最后一页");
            GC_LookMode_Page_Right    = EditorGUIUtility.IconContent("ArrowNavigationRight").SetTooltips("下一页");
            GC_LookMode_Page_Size     = EditorGUIUtility.IconContent("d_Preset.Context").SetTooltips("设置页面大小");

            ViewDetails = new ViewRect(300, 1)
            {
                IsShow = false, IsAllowDragStretchHorizontal = false, width = 400, y = ViewDetailList.y + 3
            };

            ViewDetailList = new ViewRect(850, 1)
            {
                IsShow = true, IsAllowDragStretchHorizontal = true, DragStretchHorizontalWidth = 5, width = 850, x = 5,
            };

            if (PageValues is null)
            {
                PageValues                                  =  new PageList<AssetDataInfo> { PageSize = 25 };
                TreeViewQueryAsset                          =  TreeViewQueryAsset.Create(PageValues);
                TreeViewQueryAsset.IsFirstPackageResource   += IsFirstPackageResource;
                TreeViewQueryAsset.OnFirstPackageResource   += OnFirstPackageResource;
                TreeViewQueryAsset.OnSingleSelectionChanged += OnQueryAsseChanged;
            }

            ControlID            = GUIUtility.GetControlID(FocusType.Passive).ToString();
            TreeViewDependencies = TreeViewDependencies.Create(Dependencies.Values);
            UpdatePageSizeMenu();
        }

        #region GenericMenu

        private          GenericMenu PageSizeMenu;                        // 资源展示模式 当前页数量选项
        private readonly int[]       PageSizeValues = { 25, 30, 40, 50 }; // 资源展示模式 当前页数量选项

        private void UpdatePageSizeMenu()
        {
            PageSizeMenu = new GenericMenu();
            foreach (var size in PageSizeValues)
            {
                PageSizeMenu.AddItem(new GUIContent(size.ToString()), PageValues.PageSize == size, data =>
                {
                    PageValues.PageSize = (int)data;
                    UpdatePageSizeMenu();
                    TreeViewQueryAsset.Reload(PageValues);
                }, size);
            }
        }

        #endregion

        private static string ControlID;

        private static void SearchAssetText(Rect rect)
        {
            rect.y      += 2;
            rect.height -= 2;

            using (new GUI.GroupScope(rect, GEStyle.ToolbarSeachTextField))
            {
                var cell = new Rect(12, -1, rect.width, rect.height + 1);
                if (!string.IsNullOrEmpty(TreeViewQueryAsset.searchString)) cell.width -= cell.x + 15;
                else cell.width                                                        -= cell.x;
                EditorGUIUtility.AddCursorRect(cell, MouseCursor.Text);
                GUI.SetNextControlName(ControlID);
                TreeViewQueryAsset.searchString = GUI.TextField(cell, TreeViewQueryAsset.searchString, GEStyle.MiniBoldLabel);

                // 按下 Ctrl + F 时，自动聚焦到搜索框
                if (Event.current != null && Event.current.keyCode == KeyCode.F && (Event.current.control || Event.current.command))
                {
                    Debug.Log("Focus");
                    EditorGUIUtility.editingTextField = true;
                    GUI.FocusControl(ControlID);
                    Event.current.Use();
                }

                if (string.IsNullOrEmpty(TreeViewQueryAsset.searchString)) return;

                cell.width  = 15;
                cell.y      = 0;
                cell.x      = rect.width - cell.width;
                cell.height = rect.height;
                EditorGUIUtility.AddCursorRect(cell, MouseCursor.Arrow);
                if (!GUI.Button(cell, GUIContent.none, GEStyle.ToolbarSeachCancelButton)) return;

                GUI.FocusControl(null);
                TreeViewQueryAsset.searchString = string.Empty;
            }
        }

        #region IsFilter

        /// <summary>
        ///     是否过滤 收集器
        /// </summary>
        private static bool IsFilterCollectors(int index, string collectPath, IList<string> displays)
        {
            if (index < 1) return true;
            if (displays is null || displays.Count == 0) return false;

            if (displays.Count >= 31)
            {
                if (displays.Count <= index) index = displays.Count - 1;
                if (displays[index].EndsWith(collectPath))
                    return true;
            }
            else if (displays.Count > 15)
            {
                var status = 1L;
                collectPath = collectPath.Replace('/', '\\').TrimEnd('\\');
                foreach (var display in displays)
                {
                    if ((index & status) == status && display.EndsWith(collectPath)) return true;
                    status *= 2;
                }
            }
            else
            {
                var status = 1L;
                collectPath = collectPath.Replace('/', '\\').TrimEnd('\\');
                foreach (var display in displays)
                {
                    if ((index & status) == status && display == collectPath) return true;
                    status *= 2;
                }
            }

            return false;
        }

        /// <summary>
        ///     是否过滤 指定类型
        /// </summary>
        private static bool IsFilterTypes(int index, AssetDataInfo data, ICollection<string> displays)
        {
            if (index < 1) return true;
            if (displays is null || displays.Count == 0) return false;
            var objectType = data.Type;
            var status     = 1L;
            foreach (var item in displays)
            {
                if ((index & status) == status && objectType == item) return true;
                status *= 2;
            }

            return false;
        }

        /// <summary>
        ///     是否过滤 指定标签
        /// </summary>
        private static bool IsFilterTags(int index, ICollection<string> tags, ICollection<string> displays)
        {
            if (index < 1) return true;
            if (displays is null || displays.Count == 0) return false;
            if (tags is null || tags.Count == 0) return false;

            var status = 1L;
            foreach (var display in displays)
            {
                if ((index & status) == status && tags.Contains(display)) return true;
                status *= 2;
            }

            return false;
        }

        #endregion

        #region OnDraw

        private void OnDrawAssetDetail(Rect rect)
        {
            var cell = new Rect(10, rect.y, rect.width - 20, 20);

            EditorGUI.LabelField(cell, SelectAssetDataInfo.Address, GEStyle.HeaderLabel);

            {
                cell.y     += cell.height + 5;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, "资源包", GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, SelectAssetDataInfo.Package);
            }

            if (AssetWindow.IsOpenPage<FirstPackage>())
            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, "资源组", GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, SelectAssetDataInfo.Group);
            }

            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, GC_Detail_IsSubAsset, GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, $"{AssetDatabase.IsSubAsset(SelectAsset)}");
            }

            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, GC_Detail_Size, GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, SelectAssetDataInfo.SizeStr);
            }

            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, GC_Detail_GUID, GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, SelectAssetDataInfo.GUID);
            }

            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, GC_Detail_Type, GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, SelectAssetDataInfo.Type);
            }

            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, GC_Detail_Path, GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                if (GUI.Button(cell, SelectAssetDataInfo.AssetPath, GEStyle.LinkLabel))
                {
                    EditorUtility.RevealInFinder(SelectAssetDataInfo.AssetPath);
                }
            }

            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, GC_Detail_LastWriteTime, GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, SelectAssetDataInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss"));
            }

            if (!string.IsNullOrEmpty(SelectAssetDataInfo.Tag))
            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, GC_Detail_Tags, GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x - 16;
                EditorGUI.LabelField(cell, SelectAssetDataInfo.Tag);

                cell.x     += cell.width;
                cell.width =  16;
                if (GUI.Button(cell, GC_COPY, GEStyle.IconButton))
                {
                    GEHelper.CopyAction(SelectAssetDataInfo.Tag);
                }
            }

            if (Config.EnableSequenceRecord)
            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  100;
                EditorGUI.LabelField(cell, "首包资源", GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, Config.SequenceRecord.ContainsGUID(SelectAssetDataInfo.GUID)
                                         ? "是"
                                         : "否");
            }

            if (Dependencies.Count > 0)
            {
                cell.y     += cell.height;
                cell.x     =  10;
                cell.width =  rect.width - cell.x;
                EditorGUI.LabelField(cell, $"Dependencies({Dependencies.Count})[{DependenciesSize.ToConverseStringFileSize()}]", GEStyle.HeaderLabel);

                cell.y     += cell.height;
                cell.width =  0;
                if (!string.IsNullOrEmpty(TreeViewDependencies.searchString))
                {
                    cell.width = 21;
                    cell.x     = rect.width - cell.width - 10;
                    if (GUI.Button(cell, "✘", GEStyle.toolbarbuttonLeft))
                    {
                        GUI.FocusControl(null);
                        TreeViewDependencies.searchString = string.Empty;
                    }
                }

                cell.width                        = rect.width - cell.width - 20;
                cell.x                            = 10;
                TreeViewDependencies.searchString = EditorGUI.TextField(cell, TreeViewDependencies.searchString, GEStyle.SearchTextField);

                cell.y += cell.height;
                TreeViewDependencies.OnGUI(new Rect(0, cell.y, rect.width, rect.height - cell.y));
            }

            EditorGUI.DrawRect(new Rect(8, 0, 1, cell.y), TreeViewBasics.ColorLine);
        }

        private void OnDrawPageSetting(Rect rect)
        {
            if (PageValues.Count <= 0) return;
            using (new EditorGUI.DisabledScope(PageValues.PageIndex <= 0))
            {
                rect.width = 30;
                if (GUI.Button(rect, GC_LookMode_Page_MaxLeft, GEStyle.TEtoolbarbutton))
                {
                    PageValues.PageIndex = 0;
                    TreeViewQueryAsset.Reload(PageValues);
                }
            }

            using (new EditorGUI.DisabledScope(PageValues.PageIndex == 0))
            {
                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_LookMode_Page_Left, GEStyle.TEtoolbarbutton))
                {
                    PageValues.PageIndex -= 1;
                    TreeViewQueryAsset.Reload(PageValues);
                }
            }

            rect.x     += rect.width;
            rect.width =  40;

            var temp = rect;
            using (new EditorGUI.DisabledScope(PageValues.PageIndex + 1 >= PageValues.PageCount))
            {
                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_LookMode_Page_Right, GEStyle.TEtoolbarbutton))
                {
                    PageValues.PageIndex += 1;
                    TreeViewQueryAsset.Reload(PageValues);
                }

                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_LookMode_Page_MaxRight, GEStyle.TEtoolbarbutton))
                {
                    PageValues.PageIndex = PageValues.PageCount - 1;
                    TreeViewQueryAsset.Reload(PageValues);
                }
            }

            rect.x     += rect.width;
            rect.width =  30;
            if (GUI.Button(rect, GC_LookMode_Page_Size, GEStyle.TEtoolbarbutton))
            {
                PageSizeMenu.ShowAsContext();
            }

            GUI.Label(temp, $"{PageValues.PageIndex + 1}/{PageValues.PageCount}", GEStyle.MeTimeLabel);
        }

        #endregion

        private static string[] GetGroupDisPlayNames(ICollection<AssetCollectGroup> groups)
        {
            var page = groups.Count > 15;
            return (from t in groups
                    select t.Name
                    into groupName
                    where !string.IsNullOrEmpty(groupName)
                    select page
                        ? string.Concat(char.ToUpper(groupName[0]), '/', groupName)
                        : groupName).ToArray();
        }

        private static string[] GetCollectorDisPlayNames(IList<string> collectors)
        {
            if (collectors.Count >= 31)
            {
                var temp = new string[collectors.Count + 1];
                temp[0] = "All";
                for (var index = 0; index < collectors.Count; index++) temp[index + 1] = collectors[index];
                return temp;
            }

            if (collectors.Count > 15)
                for (var index = 0; index < collectors.Count; index++)
                    collectors[index] = string.Concat(char.ToUpper(collectors[index][0]), '/',
                                                      collectors[index].Replace('/', '\\').TrimEnd('\\'));
            else
                for (var index = 0; index < collectors.Count; index++)
                    collectors[index] = collectors[index].Replace('/', '\\').TrimEnd('\\');

            return collectors.ToArray();
        }
    }
}