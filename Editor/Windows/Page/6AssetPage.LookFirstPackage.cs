using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    partial class AssetPageLook
    {
        internal class FirstPackage : IAssetPage
        {
            #region IAssetPage

            int IAssetPage.   Order => 6;
            string IAssetPage.Title => "查询首包      [Ctrl + Number6]";

            void IAssetPage.EventMouseDown(in Event evt) { Instance.ViewDetailList.ContainsDragStretch(evt, ViewRect.DragStretchType.Horizontal); }

            void IAssetPage.EventMouseUp(in Event evt) { Instance.ViewDetailList.CancelDragStretch(); }

            void IAssetPage.EventKeyDown(in Event evt, in KeyCode keyCode) { }
            void IAssetPage.EventKeyUp(in   Event evt, in KeyCode keyCode) { }

            void IAssetPage.EventMouseDrag(in Event evt)
            {
                if (Instance.ShowAssetDetail) Instance.ViewDetailList.DraggingStretch(evt, ViewRect.DragStretchType.Horizontal);
            }

            bool IAssetPage.Shortcut(Event evt) =>
                evt.control && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Keypad5 || evt.keyCode == KeyCode.Alpha5);

            #endregion

            public void UpdateData()
            {
                if (!Config.EnableSequenceRecord) return;
                if (Config.SequenceRecord.ExistsLocal()) Config.SequenceRecord.UpdateLocal();
                UpdateDataAll();
            }

            public void OnDrawHeader(Rect rect)
            {
                var width = rect.width;
                rect.x     = 0;
                rect.width = 0;
                if (!Config.EnableSequenceRecord)
                {
                    rect.width = 30;
                    if (GUI.Button(rect, Instance.GC_Select, GEStyle.TEtoolbarbutton))
                    {
                        GUI.FocusControl(null);
                        Selection.activeObject = Config;
                    }

                    return;
                }

                if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                {
                    rect.x     += rect.width;
                    rect.width =  30;
                    if (GUI.Button(rect, Instance.GC_OPEN_FOLDER, GEStyle.TEtoolbarbutton))
                    {
                        GUI.FocusControl(null);
                        EditorUtility.RevealInFinder(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                    }

                    rect.x     += rect.width;
                    rect.width =  30;
                    if (GUI.Button(rect, Instance.GC_DEL, GEStyle.TEtoolbarbutton))
                    {
                        GUI.FocusControl(null);
                        AHelper.IO.DeleteFile(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                        TreeViewQueryAsset.searchString = string.Empty;
                        DisplayTypeIndex                = 0;
                        Config.SequenceRecord.UpdateLocal();
                        UpdateDataAll();
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(Config.URL))
                {
                    rect.x     += rect.width;
                    rect.width =  30;
                    if (GUI.Button(rect, Instance.GC_NET, GEStyle.TEtoolbarbutton)) // 打开网络路径
                    {
                        GUI.FocusControl(null);
                        Application.OpenURL(AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Config));
                    }

                    rect.x     += rect.width;
                    rect.width =  30;
                    if (GUI.Button(rect, Instance.GC_DOWNLOAD, GEStyle.TEtoolbarbutton)) // 下载网络路径
                    {
                        GUI.FocusControl(null);
                        SyncSequenceRecords();
                    }
                }

                if (DisplayTypes != null && DisplayTypes.Length > 0)
                {
                    EditorGUI.BeginChangeCheck();
                    rect.x           += rect.width;
                    rect.width       =  100;
                    DisplayTypeIndex =  EditorGUI.MaskField(rect, DisplayTypeIndex, DisplayTypes, GEStyle.PreDropDown);
                    if (EditorGUI.EndChangeCheck())
                    {
                        PageValues.Clear();
                        PageValues.Add(Values.Where(data => !DataFilter(data)));
                        PageValues.PageIndex = 0;
                        TreeViewQueryAsset.Reload(PageValues);
                    }
                }

                rect.x                          += rect.width + 3;
                rect.width                      =  width - 30 - 30 - 30 - 30 - rect.x - (PageValues.Count <= 0 ? 0 : 190);
                TreeViewQueryAsset.searchString =  GUI.TextField(rect, TreeViewQueryAsset.searchString, GEStyle.SearchTextField);

                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, Instance.GC_CLEAR, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    TreeViewQueryAsset.searchString = string.Empty;
                }

                rect.x     += rect.width;
                rect.width =  190;
                Instance.OnDrawPageSetting(rect);

                rect.x     = width - 30;
                rect.width = 30;
                if (GUI.Button(rect, Instance.GC_SAVE, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    Config.SequenceRecord.Save();
                    Config.Save();
#if UNITY_2021_1_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(Config);
#else
                    AssetDatabase.SaveAssets();
#endif
                    if (EditorUtility.DisplayDialog("保存", "保存成功", "确定")) AssetDatabase.Refresh();
                }

                rect.x     -= rect.width;
                rect.width =  30;
                if (GUI.Button(rect, Instance.GC_Select, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    Selection.activeObject = Config;
                }

                rect.x     -= rect.width;
                rect.width =  30;
                if (GUI.Button(rect, Instance.GC_REFRESH, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    TreeViewQueryAsset.searchString = string.Empty;
                    DisplayTypeIndex                = 0;
                    Config.SequenceRecord.UpdateLocal();
                    UpdateDataAll();
                }
            }

            private async void SyncSequenceRecords()
            {
                TreeViewQueryAsset.searchString = string.Empty;
                DisplayTypeIndex                = 0;
                await Config.SequenceRecord.DownloadTask(Config.URL);
                Config.SequenceRecord.UpdateLocal();
                UpdateDataAll();
            }

            private void AddAsset(AssetSystem.SequenceRecord record, IList<string> types, IDictionary<string, AssetDataInfo> assets)
            {
                if (string.IsNullOrEmpty(record.AssetPath)
                 || string.IsNullOrEmpty(record.PackageName)
                 || assets.ContainsKey(record.AssetPath)) return;
                var tuple = AssetCollectRoot.AssetToAddress(record.AssetPath, Config.LoadPathToLower, Config.HasExtension, record.PackageName);
                if (string.IsNullOrEmpty(tuple.Item1)) return;
                var info = new AssetDataInfo
                {
                    AssetPath = record.AssetPath,
                    Extension = Path.GetExtension(record.AssetPath),
                    Address   = tuple.Item3,
                    Package   = record.PackageName,
                    Group     = "N/A",
                };
                assets[info.AssetPath] = info;
                Values.Add(info);
                types.Add(info.Type);
            }

            private void UpdateDataAll()
            {
                Values.Clear();
                DisplayTypes = Array.Empty<string>();
                var types = new List<string>();
                var asset = new Dictionary<string, AssetDataInfo>();

                var count = Config.SequenceRecord.Count <= PageValues.PageSize ? Config.SequenceRecord.Count : PageValues.PageSize;
                for (var i = 0; i < count; i++) AddAsset(Config.SequenceRecord[i], types, asset);

                DisplayTypes = types.Distinct().ToArray();
                PageValues.Clear();
                PageValues.Add(Values.Where(data => !DataFilter(data)));
                PageValues.PageIndex = 0;
                TreeViewQueryAsset.Reload(PageValues);
                TreeViewQueryAsset.Select(0);

                if (Config.SequenceRecord.Count > PageValues.PageSize)
                {
                    Runner.StartCoroutine(() =>
                    {
                        for (var i = count; i < Config.SequenceRecord.Count - count; i++)
                            AddAsset(Config.SequenceRecord[i], types, asset);

                        DisplayTypes = types.Distinct().ToArray();
                        PageValues.Clear();
                        PageValues.Add(Values.Where(data => !DataFilter(data)));
                        TreeViewQueryAsset.Reload(PageValues);
                    });
                }
            }

            void IAssetPage.OnDrawContent(Rect rect)
            {
                if (!Config.EnableSequenceRecord)
                {
                    var content = new GUIContent("请启用序列记录功能");
                    var size    = GEStyle.HeaderButton.CalcSize(content) * 3;
                    if (GUI.Button(new Rect(rect.position - size / 2, size), content, GEStyle.HeaderButton))
                    {
                        GUI.FocusControl(null);
                        Config.EnableSequenceRecord = true;
                        UpdateData();
                    }

                    return;
                }

                Instance.ViewDetailList.x      = rect.x + 5;
                Instance.ViewDetailList.height = rect.height + 5;
                if (Instance.ShowAssetDetail)
                {
                    Instance.ViewDetailList.MaxWidth = rect.width - Instance.ViewDetails.MinWidth - 10;

                    Instance.ViewDetails.IsShow = true;
                    Instance.ViewDetails.y      = 0;
                    Instance.ViewDetails.x      = Instance.ViewDetailList.x + Instance.ViewDetailList.width;
                    Instance.ViewDetails.width  = rect.width - Instance.ViewDetails.x - 5;
                    if (Instance.ViewDetails.width < Instance.ViewDetails.MinWidth)
                    {
                        Instance.ViewDetails.width    = Instance.ViewDetails.MinWidth - 5;
                        Instance.ViewDetailList.width = rect.width - Instance.ViewDetails.width - 10;
                    }

                    Instance.ViewDetails.height = Instance.ViewDetailList.height;
                }
                else
                {
                    Instance.ViewDetails.IsShow   = false;
                    Instance.ViewDetailList.width = rect.width - 5;
                }

                Instance.ViewDetailList.Draw(TreeViewQueryAsset.OnGUI, GEStyle.INThumbnailShadow);
                Instance.ViewDetails.Draw(Instance.OnDrawAssetDetail, GEStyle.INThumbnailShadow);
            }

            private static bool DataFilter(AssetDataInfo data)
            {
                var filter = 0;
                if (IsFilterTypes(DisplayTypeIndex, data, DisplayTypes))
                    filter++;

                return filter != 1;
            }

            public void Dispose() { }
        }
    }
}