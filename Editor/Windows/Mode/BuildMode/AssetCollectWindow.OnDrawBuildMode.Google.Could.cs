using System.IO;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private string GetGCItemDes(ASBuildConfig.GCloudConfig config, int i)
        {
            _builder.Clear();
            if (!config.Folded && config.isUploading) _builder.Append("[上传中 ... ]");
            if (!string.IsNullOrEmpty(config.Name))
                _builder.Append(config.Name);

            if (!string.IsNullOrEmpty(config.Description))
                _builder.Append('(').Append(config.Description).Append(')');

            if (!string.IsNullOrEmpty(config.BUCKET_NAME))
            {
                _builder.Append('[');
                if (config.DirTreeFiled.IsValidity()) _builder.Append(config.DirTreeFiled.GetFullPath()).Append(" -> ");

                _builder.Append(config.BUCKET_NAME);
                if (!string.IsNullOrEmpty(config.Description)) _builder.Append('/').Append(config.Description);
                _builder.Append(']');
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
        }

        private void OnDrawBuildGCloud(in Rect rect, out Rect outRect)
        {
            if (BuildConfig.GCloudConfigs is null || BuildConfig.GCloudConfigs.Length == 0)
            {
                outRect = rect;
                return;
            }

            var cell = new Rect(rect.x, rect.y, rect.width, 300);
            var view = new Rect(rect.x, rect.y, rect.width - 16, 20 * BuildConfig.FTPConfigs.Length);
            using (var scope = new GUI.ScrollViewScope(cell, OnDrawConfigGCScroll, view))
            {
                OnDrawConfigGCScroll = scope.scrollPosition;
                if (view.height > cell.height + cell.y) cell.width -= 16;
                var rectItem = new Rect(rect.x, rect.y, rect.width - 16, 20);
                for (var i = BuildConfig.GCloudConfigs.Length - 1; i >= 0; i--)
                {
                    rectItem.width = 20;
                    rectItem.x     = cell.width - rectItem.width;

                    if (GUI.Button(rectItem, GC_DEL, GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        if (EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消"))
                        {
                            BuildConfig.GCloudConfigs = BuildConfig.GCloudConfigs.RemoveAt(i).Exclude();
                        }
                    }

                    using (new EditorGUI.DisabledScope(BuildConfig.GCloudConfigs[i].isUploading))
                    {
                        if (!string.IsNullOrEmpty(BuildConfig.GCloudConfigs[i].BUCKET_NAME))
                        {
                            rectItem.width =  20;
                            rectItem.x     -= rectItem.width;
                            if (GUI.Button(rectItem, GC_REFRESH, GEStyle.toolbarbutton))
                            {
                                GUI.FocusControl(null);
                                BuildConfig.GCloudConfigs[i].DirTreeFiled.UpdateOption();
                            }

                            if (!string.IsNullOrEmpty(BuildConfig.GCloudConfigs[i].DirTreeFiled.DirPath)
                             && File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                            {
                                rectItem.width =  100;
                                rectItem.x     -= rectItem.width;
                                if (GUI.Button(rectItem, "更新首包配置", GEStyle.toolbarbutton))
                                {
                                    GUI.FocusControl(null);
                                    UpdateUploadFirstPack(BuildConfig.GCloudConfigs[i]);
                                }
                            }
                        }
                    }

                    rectItem.width =  rectItem.x - 30;
                    rectItem.x     -= rectItem.width;
                    GUI.Label(rectItem, GetGCItemDes(BuildConfig.GCloudConfigs[i], i), GEStyle.HeaderLabel);

                    rectItem.width =  30;
                    rectItem.x     -= rectItem.width;
                    if (GUI.Button(rectItem, BuildConfig.GCloudConfigs[i].Folded ? GC_FOLDOUT : GC_FOLDOUT_ON, GEStyle.TEtoolbarbutton))
                    {
                        BuildConfig.GCloudConfigs[i].Folded = !BuildConfig.GCloudConfigs[i].Folded;
                        GUI.FocusControl(null);
                    }

                    rectItem.y += rectItem.height;
                    if (!BuildConfig.GCloudConfigs[i].Folded) continue;
                    rectItem.x     = cell.x;
                    rectItem.width = cell.width;
                    OnDrawBuildGC(BuildConfig.GCloudConfigs[i], rectItem, out rectItem);
                }
            }

            outRect = new Rect(rect.x, cell.y + cell.height, rect.width, cell.height);
        }

        private void OnDrawBuildGC(ASBuildConfig.GCloudConfig config, in Rect rect, out Rect outRect)
        {
            using (new EditorGUI.DisabledScope(config.isUploading))
            {
                #region 名称:描述

                var cell = new Rect(rect.x + 5, rect.y, 100, 20);
                EditorGUI.LabelField(cell, "名称:描述", GEStyle.HeaderLabel);

                cell.x      += cell.width;
                cell.width  =  (rect.width - cell.x) / 2;
                config.Name =  EditorGUI.DelayedTextField(cell, config.Name);

                cell.x             += cell.width;
                config.Description =  EditorGUI.DelayedTextField(cell, config.Description);

                #endregion

                #region 环境 : gcloud local

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "环境 : gcloud local", GEStyle.HeaderLabel);

                cell.x             += cell.width;
                cell.width         =  rect.width - cell.x;
                config.GCLOUD_PATH =  EditorGUI.DelayedTextField(cell, config.GCLOUD_PATH);

                #endregion

                #region 环境 gsutil local

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "环境 : gcloud local", GEStyle.HeaderLabel);

                cell.x             += cell.width;
                cell.width         =  rect.width - cell.x;
                config.GSUTIL_PATH =  EditorGUI.DelayedTextField(cell, config.GSUTIL_PATH);

                #endregion

                #region 储存桶

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "储存桶", GEStyle.HeaderLabel);

                cell.x             += cell.width;
                cell.width         =  rect.width - cell.x;
                config.BUCKET_NAME =  EditorGUI.DelayedTextField(cell, config.BUCKET_NAME);

                #endregion


                #region 元数据:[键:值]

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "元数据:[键:值]", GEStyle.HeaderLabel);

                cell.x             += cell.width;
                cell.width         =  (rect.width - cell.x) / 2;
                config.MetaDataKey =  EditorGUI.DelayedTextField(cell, config.MetaDataKey);

                cell.x               += cell.width;
                config.MetaDataValue =  EditorGUI.DelayedTextField(cell, config.MetaDataValue);

                #endregion

                #region 本地路径

                using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
                {
                    cell.y     += cell.height;
                    cell.x     =  rect.x + 5;
                    cell.width =  100;
                    EditorGUI.LabelField(cell, "本地路径", GEStyle.HeaderLabel);

                    cell.x     += cell.width;
                    cell.width =  rect.width - cell.x - 50;
                    config.DirTreeFiled.OnDraw(cell);

                    cell.x     += cell.width;
                    cell.width =  50;
                    if (config.isUploading)
                    {
                        EditorGUI.LabelField(cell, "上传中", GEStyle.toolbarbutton);
                    }
                    else if (GUI.Button(cell, "上传", GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        _ = config.Upload();
                    }
                }

                #endregion

                outRect = new Rect(rect.x, cell.y + cell.height, rect.width, rect.height);
            }
        }
    }
}