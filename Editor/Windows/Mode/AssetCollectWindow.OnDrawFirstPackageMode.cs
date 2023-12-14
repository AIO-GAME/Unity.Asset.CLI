﻿/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-14
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AssetCollectWindow
    {
        private async void UpdateDataFirstPackageMode()
        {
            if (Config.EnableSequenceRecord)
            {
                if (SequenceRecords is null)
                    SequenceRecords = new AssetSystem.SequenceRecordQueue();
                if (SequenceRecords.ExistsLocal()) SequenceRecords.UpdateLocal();
                UpdatePageValuesFirstPackageMode();
            }
            else SequenceRecords?.Clear();
        }

        /// <summary>
        /// 绘制 资源查询模式 导航栏
        /// </summary>
        private async void OnDrawHeaderFirstPackageMode()
        {
            if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
            {
                if (GELayout.Button("Open", GEStyle.TEtoolbarbutton, GP_Width_50))
                {
                    Application.OpenURL(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                }

                if (GELayout.Button("Delete", GEStyle.TEtoolbarbutton, GP_Width_50))
                {
                    AHelper.IO.DeleteFile(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                    SearchText = string.Empty;
                    LookModeDisplayTypeIndex = 0;
                    SequenceRecords.UpdateLocal();
                    UpdatePageValuesFirstPackageMode();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(Config.URL))
            {
                if (GELayout.Button("Web", GEStyle.TEtoolbarbutton, GP_Width_50))
                    Application.OpenURL(AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Config));

                GELayout.Button("Sync", SyncSequenceRecords, GEStyle.TEtoolbarbutton, GP_Width_50);
            }

            SearchText = EditorGUILayout.TextField(SearchText, GEStyle.SearchTextField);
            if (GUILayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, GP_Width_25))
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
                    TempTable.GetOrDefault<string>(nameof(SearchText)) != SearchText
                   )
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

            GELayout.Button(GC_SAVE, () => { SequenceRecords.Save(); }, GEStyle.TEtoolbarbutton, GP_Width_25);
            GELayout.Button(GC_REFRESH, () =>
            {
                SearchText = string.Empty;
                LookModeDisplayTypeIndex = 0;
                SequenceRecords.UpdateLocal();
                UpdatePageValuesFirstPackageMode();
            }, GEStyle.TEtoolbarbutton, GP_Width_25);
        }

        private bool FirstPackageModeDataFilter(AssetDataInfo data)
        {
            var filter = 0;
            if (LookModeDisplayTypeIndex < 1) filter++;
            else
            {
                var objectType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath)?.FullName;
                if (string.IsNullOrEmpty(objectType)) objectType = "Unknown";
                var status = 1L;
                foreach (var display in TagsModeDisplayTypes)
                {
                    if ((LookModeDisplayTypeIndex & status) == status && objectType == display)
                    {
                        filter++;
                        break;
                    }

                    status *= 2;
                }
            }

            if (string.IsNullOrEmpty(SearchText)) filter++;
            else
            {
                if (data.AssetPath.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                    data.Address.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase))
                {
                    filter++;
                }
                else if (SearchText.Contains(data.AssetPath, StringComparison.CurrentCultureIgnoreCase) ||
                         SearchText.Contains(data.Address, StringComparison.CurrentCultureIgnoreCase))
                {
                    filter++;
                }
            }

            return filter != 2;
        }

        private async void SyncSequenceRecords()
        {
            SearchText = string.Empty;
            LookModeDisplayTypeIndex = 0;
            await SequenceRecords.DownloadTask(Config.URL);
            SequenceRecords.UpdateLocal();
            UpdatePageValuesFirstPackageMode();
        }

        private void UpdatePageValuesFirstPackageMode()
        {
            CurrentTagValues.Clear();
            LookModeCollectorsALLSize = 0;
            LookModeCollectorsPageSize = 0;
            TagsModeDisplayTypes = Array.Empty<string>();
            var types = new Dictionary<string, byte>();
            var asset = new Dictionary<string, AssetDataInfo>();
            foreach (var item in SequenceRecords)
            {
                if (asset.ContainsKey(item.AssetPath)) continue;
                var info = new AssetDataInfo
                {
                    AssetPath = item.AssetPath,
                    Address = item.Location,
                    Tags = "FirstPackage",
                    Extension = Path.GetExtension(item.AssetPath)
                };
                asset[info.AssetPath] = info;
                CurrentTagValues.Add(info);
                types[info.Type] = 0;
            }

            TagsModeDisplayTypes = types.Keys.ToArray();
            CurrentPageValues.Clear();
            CurrentPageValues.Add(CurrentTagValues.Where(data => !FirstPackageModeDataFilter(data)));
            LookModeCollectorsALLSize = CurrentPageValues.Sum(data => data.Size);
            CurrentPageValues.PageIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDrawFirstPackageMode()
        {
            if (!Config.EnableSequenceRecord)
            {
                var content = new GUIContent("请启用序列记录功能");
                var size = GEStyle.HeaderButton.CalcSize(content) * 3;
                var rect = new Rect(Center.position - size / 2, size);
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
                using (new EditorGUILayout.HorizontalScope(GEStyle.DropzoneStyle))
                {
                    GELayout.LabelPrefix("序列记录");
                    if (!string.IsNullOrEmpty(Config.URL))
                    {
                        if (GELayout.Button("Upload FTP"))
                        {
                            AHandle.FTP.Create("", "", "").UploadFile(
                                AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                        }
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
    }
}