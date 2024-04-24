using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    partial class AssetCollectWindow
    {
        /// <summary>
        ///     更新 资源查询模式 数据
        /// </summary>
        private void UpdateDataFirstPackageMode()
        {
            if (!Config.EnableSequenceRecord) return;
            if (Config.SequenceRecord.ExistsLocal()) Config.SequenceRecord.UpdateLocal();
            UpdatePageValuesFirstPackageMode();
            ViewTreeQueryAsset.ReloadAndSelect(0);
        }

        /// <summary>
        ///     绘制 资源查询模式 导航栏
        /// </summary>
        partial void OnDrawHeaderFirstPackageMode(Rect rect)
        {
            var width = rect.width;
            rect.x     = 0;
            rect.width = 0;
            if (!Config.EnableSequenceRecord)
            {
                rect.width = 30;
                if (GUI.Button(rect, GC_Select_ASConfig, GEStyle.TEtoolbarbutton))
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
                if (GUI.Button(rect, GC_OPEN_FOLDER, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    EditorUtility.RevealInFinder(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                }

                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_DEL, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    AHelper.IO.DeleteFile(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                    ViewTreeQueryAsset.searchString = string.Empty;
                    LookModeDisplayTypeIndex        = 0;
                    Config.SequenceRecord.UpdateLocal();
                    UpdatePageValuesFirstPackageMode();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(Config.URL))
            {
                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_NET, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    Application.OpenURL(AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Config));
                }

                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_DOWNLOAD, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    SyncSequenceRecords();
                }
            }

            if (TagsModeDisplayTypes != null && TagsModeDisplayTypes.Length > 0)
            {
                rect.x     += rect.width;
                rect.width =  100;
                LookModeDisplayTypeIndex =
                    EditorGUI.MaskField(rect, LookModeDisplayTypeIndex, TagsModeDisplayTypes, GEStyle.PreDropDown);
            }

            rect.x                          += rect.width + 3;
            rect.width                      =  width - 190 - 30 - 30 - 30 - 30 - rect.x;
            ViewTreeQueryAsset.searchString =  GUI.TextField(rect, ViewTreeQueryAsset.searchString, GEStyle.SearchTextField);

            rect.x     += rect.width;
            rect.width =  30;
            if (GUI.Button(rect, GC_CLEAR, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                ViewTreeQueryAsset.searchString = string.Empty;
            }

            if (GUI.changed &&
                (TempTable.GetOrDefault<int>(nameof(LookModeDisplayTypeIndex)) != LookModeDisplayTypeIndex
              || TempTable.GetOrDefault<string>(nameof(ViewTreeQueryAsset.searchString)) != ViewTreeQueryAsset.searchString))
            {
                CurrentPageValues.Clear();
                CurrentPageValues.Add(CurrentTagValues.Where(data => !FirstPackageModeDataFilter(data)));
                CurrentPageValues.PageIndex                 = 0;
                LookModeCollectorsALLSize                   = CurrentPageValues.Sum(data => data.Size);
                TempTable[nameof(LookModeDisplayTypeIndex)] = LookModeDisplayTypeIndex;
                ViewTreeQueryAsset.Reload();
            }

            rect.x     += rect.width;
            rect.width =  190;
            OnDrawHeaderLookPageSetting(rect);

            rect.x     = width - 30;
            rect.width = 30;
            if (GUI.Button(rect, GC_SAVE, GEStyle.TEtoolbarbutton))
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
            if (GUI.Button(rect, GC_Select_ASConfig, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                Selection.activeObject = Config;
            }

            rect.x     -= rect.width;
            rect.width =  30;
            if (GUI.Button(rect, GC_REFRESH, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                ViewTreeQueryAsset.searchString = string.Empty;
                LookModeDisplayTypeIndex        = 0;
                Config.SequenceRecord.UpdateLocal();
                UpdatePageValuesFirstPackageMode();
            }
        }

        private async void SyncSequenceRecords()
        {
            ViewTreeQueryAsset.searchString = string.Empty;
            LookModeDisplayTypeIndex        = 0;
            await Config.SequenceRecord.DownloadTask(Config.URL);
            Config.SequenceRecord.UpdateLocal();
            UpdatePageValuesFirstPackageMode();
        }

        private void UpdatePageValuesFirstPackageMode()
        {
            CurrentTagValues.Clear();
            TagsModeDisplayTypes       = Array.Empty<string>();
            LookModeCollectorsALLSize  = 0;
            LookModeCollectorsPageSize = 0;
            var types = new List<string>();
            var asset = new Dictionary<string, AssetDataInfo>();
            for (var i = 0; i < Config.SequenceRecord.Count; i++)
            {
                var address = Config.SequenceRecord[i].AssetPath;
                if (string.IsNullOrEmpty(address)) continue;
                if (asset.ContainsKey(address)) continue;
                var tuple = AssetCollectRoot.AssetToAddress(address, Config.LoadPathToLower, Config.HasExtension);
                if (string.IsNullOrEmpty(tuple.Item1)) continue;
                Config.SequenceRecord[i].SetPackageName(tuple.Item1);
                var info = new AssetDataInfo
                {
                    AssetPath = Config.SequenceRecord[i].AssetPath, Address = tuple.Item3, Package = tuple.Item1, Group = "N/A",
                    Extension = Path.GetExtension(address)
                };
                asset[info.AssetPath] = info;
                CurrentTagValues.Add(info);
                types.Add(info.Type);
            }

            TagsModeDisplayTypes = types.Distinct().ToArray();
            CurrentPageValues.Clear();
            CurrentPageValues.Add(CurrentTagValues.Where(data => !FirstPackageModeDataFilter(data)));
            CurrentPageValues.PageIndex = 0;
            LookModeCollectorsALLSize   = CurrentPageValues.Sum(data => data.Size);
            LookModeDataPageValueSort(ESort.AssetName, true);
        }

        private void OnDrawFirstPackageMode()
        {
            if (!Config.EnableSequenceRecord)
            {
                var content = new GUIContent("请启用序列记录功能");
                var size    = GEStyle.HeaderButton.CalcSize(content) * 3;
                var rect    = new Rect(RectCenter.position - size / 2, size);
                if (GUI.Button(rect, content, GEStyle.HeaderButton))
                {
                    GUI.FocusControl(null);
                    Config.EnableSequenceRecord = true;
                    UpdateDataFirstPackageMode();
                }

                return;
            }

            ViewDetailList.height = CurrentHeight - DrawHeaderHeight;
            if (LookModeShowAssetDetail)
            {
                ViewDetailList.MaxWidth = CurrentWidth - ViewDetails.MinWidth - 10;
                ViewDetailList.MinWidth = CurrentWidth - ViewDetails.MaxWidth - 10;

                ViewDetails.IsShow = true;
                ViewDetails.y      = 0;
                ViewDetails.x      = ViewDetailList.width + ViewDetailList.x + 5;
                ViewDetails.width  = CurrentWidth - ViewDetails.x - 5;
                ViewDetails.height = ViewDetailList.height;
            }
            else
            {
                ViewDetails.IsShow   = false;
                ViewDetailList.width = CurrentWidth - 10;
            }

            ViewDetailList.Draw(ViewTreeQueryAsset.OnGUI, GEStyle.INThumbnailShadow);
            ViewDetails.Draw(OnDrawLookModeAssetDetail, GEStyle.INThumbnailShadow);
        }

        private bool FirstPackageModeDataFilter(AssetDataInfo data)
        {
            var filter = 0;
            if (IsFilterTypes(LookModeDisplayTypeIndex, data.AssetPath, TagsModeDisplayTypes))
                filter++;

            if (IsFilterSearch(ViewTreeQueryAsset.searchString, data))
                filter++;

            return filter != 2;
        }
    }
}