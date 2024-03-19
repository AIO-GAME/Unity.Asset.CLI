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
        /// 更新 资源查询模式 数据
        /// </summary>
        private void UpdateDataFirstPackageMode()
        {
            if (!Config.EnableSequenceRecord) return;
            if (Config.SequenceRecord.ExistsLocal()) Config.SequenceRecord.UpdateLocal();
            UpdatePageValuesFirstPackageMode();
        }

        /// <summary>
        /// 绘制 资源查询模式 导航栏
        /// </summary>
        private void OnDrawHeaderFirstPackageMode()
        {
            if (!Config.EnableSequenceRecord)
            {
                EditorGUILayout.Separator();
                if (GUILayout.Button(GC_Select_ASConfig, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
                {
                    GUI.FocusControl(null);
                    Selection.activeObject = Config;
                }

                return;
            }

            if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
            {
                if (GELayout.Button(GC_OPEN_FOLDER, GEStyle.TEtoolbarbutton, GP_Width_25))
                {
                    GUI.FocusControl(null);
                    EditorUtility.RevealInFinder(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                }

                if (GELayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, GP_Width_25))
                {
                    GUI.FocusControl(null);
                    AHelper.IO.DeleteFile(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                    SearchText = string.Empty;
                    LookModeDisplayTypeIndex = 0;
                    Config.SequenceRecord.UpdateLocal();
                    UpdatePageValuesFirstPackageMode();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(Config.URL))
            {
                if (GELayout.Button(GC_NET, GEStyle.TEtoolbarbutton, GP_Width_25))
                {
                    GUI.FocusControl(null);
                    Application.OpenURL(AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Config));
                }

                if (GELayout.Button(GC_DOWNLOAD, GEStyle.TEtoolbarbutton, GP_Width_25))
                {
                    GUI.FocusControl(null);
                    SyncSequenceRecords();
                }
            }

            SearchText = EditorGUILayout.TextField(SearchText, GEStyle.SearchTextField);
            if (GUILayout.Button(GC_CLEAR, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                GUI.FocusControl(null);
                SearchText = string.Empty;
            }

            if (TagsModeDisplayTypes != null && TagsModeDisplayTypes.Length > 0)
            {
                LookModeDisplayTypeIndex = EditorGUILayout.MaskField(LookModeDisplayTypeIndex,
                    TagsModeDisplayTypes, GEStyle.PreDropDown, GP_Width_100);
            }

            if (GUI.changed)
            {
                if (TempTable.GetOrDefault<int>(nameof(LookModeDisplayTypeIndex)) !=
                    LookModeDisplayTypeIndex ||
                    TempTable.GetOrDefault<string>(nameof(SearchText)) != SearchText)
                {
                    CurrentPageValues.Clear();
                    CurrentPageValues.Add(CurrentTagValues.Where(data => !FirstPackageModeDataFilter(data)));
                    CurrentPageValues.PageIndex = 0;
                    LookModeCollectorsALLSize = CurrentPageValues.Sum(data => data.Size);
                    TempTable[nameof(SearchText)] = SearchText;
                    TempTable[nameof(LookModeDisplayTypeIndex)] = LookModeDisplayTypeIndex;
                }
            }

            OnDrawHeaderLookPageSetting();

            if (GUILayout.Button(GC_REFRESH, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                GUI.FocusControl(null);
                SearchText = string.Empty;
                LookModeDisplayTypeIndex = 0;
                Config.SequenceRecord.UpdateLocal();
                UpdatePageValuesFirstPackageMode();
            }

            if (GUILayout.Button(GC_Select_ASConfig, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Selection.activeObject = Config;
            }

            if (GELayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                GUI.FocusControl(null);
                Config.SequenceRecord.Save();
                Config.Save();
#if UNITY_2021_1_OR_NEWER
                AssetDatabase.SaveAssetIfDirty(Config);
#else
                AssetDatabase.SaveAssets();
#endif
                if (EditorUtility.DisplayDialog("保存", "保存成功", "确定"))
                {
                    AssetDatabase.Refresh();
                }
            }
        }

        private async void SyncSequenceRecords()
        {
            SearchText = string.Empty;
            LookModeDisplayTypeIndex = 0;
            await Config.SequenceRecord.DownloadTask(Config.URL);
            Config.SequenceRecord.UpdateLocal();
            UpdatePageValuesFirstPackageMode();
        }

        private void UpdatePageValuesFirstPackageMode()
        {
            CurrentTagValues.Clear();
            LookModeCollectorsALLSize = 0;
            LookModeCollectorsPageSize = 0;
            TagsModeDisplayTypes = Array.Empty<string>();
            var types = new List<string>();
            var asset = new Dictionary<string, AssetDataInfo>();
            for (var i = 0; i < Config.SequenceRecord.Count; i++)
            {
                if (asset.ContainsKey(Config.SequenceRecord[i].AssetPath)) continue;
                var temp = AssetCollectRoot.AssetToAddress(
                    Config.SequenceRecord[i].AssetPath);
                Config.SequenceRecord[i].SetPackageName(temp.Item1);
                var info = new AssetDataInfo
                {
                    AssetPath = Config.SequenceRecord[i].AssetPath,
                    Address = temp.Item3,
                    Package = temp.Item1,
                    Group = "N/A",
                    Extension = Path.GetExtension(Config.SequenceRecord[i].AssetPath)
                };
                asset[info.AssetPath] = info;
                CurrentTagValues.Add(info);
                types.Add(info.Type);
            }

            TagsModeDisplayTypes = types.Distinct().ToArray();
            CurrentPageValues.Clear();
            CurrentPageValues.Add(CurrentTagValues.Where(data => !FirstPackageModeDataFilter(data)));
            CurrentPageValues.PageIndex = 0;
            LookModeCollectorsALLSize = CurrentPageValues.Sum(data => data.Size);
            LookModeDataPageValueSort(ESort.AssetName, true);
        }

        private void OnDrawFirstPackageMode()
        {
            if (!Config.EnableSequenceRecord)
            {
                var content = new GUIContent("请启用序列记录功能");
                var size = GEStyle.HeaderButton.CalcSize(content) * 3;
                var rect = new Rect(RectCenter.position - (size / 2), size);
                GELayout.Button(rect, content, () =>
                {
                    GUI.FocusControl(null);
                    Config.EnableSequenceRecord = true;
                    UpdateDataFirstPackageMode();
                }, GEStyle.HeaderButton);
                return;
            }

            if (EditorApplication.isPlaying)
            {
                var index = 0;
                foreach (var record in Config.SequenceRecord)
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        GELayout.Label(
                            $"{++index} : {record.PackageName} -> {record.Location} : {record.AssetPath} ");
                        GELayout.HelpBox(
                            $"{record.Time:yyyy-MM-dd HH:mm:ss} [Num : {record.Count}] [Size : {record.Bytes.ToConverseStringFileSize()}] ");
                    }
                }
            }
            else
            {
                if (!LookModeShowAssetDetail) ViewDetailList.width = CurrentWidth;
                else ViewDetailList.MaxWidth = CurrentWidth - 300;
                ViewDetailList.height = CurrentHeight - DrawHeaderHeight;
                ViewDetailList.IsAllowHorizontal = LookModeShowAssetDetail;
                ViewDetailList.Draw(OnDrawLookModeAssetList, GEStyle.Badge);

                if (LookModeShowAssetDetail)
                {
                    ViewDetails.IsShow = true;
                    ViewDetails.x = ViewDetailList.width;
                    ViewDetails.width = CurrentWidth - ViewDetails.x;
                    ViewDetails.height = ViewDetailList.height - 3;
                    ViewDetails.Draw(OnDrawLookModeAssetDetail, GEStyle.Badge);
                }
            }
        }

        private bool FirstPackageModeDataFilter(AssetDataInfo data)
        {
            var filter = 0;
            if (IsFilterTypes(LookModeDisplayTypeIndex, data.AssetPath, TagsModeDisplayTypes))
                filter++;

            if (IsFilterSearch(SearchText, data))
                filter++;

            return filter != 2;
        }
    }
}