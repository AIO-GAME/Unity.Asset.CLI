/*|============|*|
|*|Author:     |*| xinan
|*|Date:       |*| 2024-01-03
|*|E-Mail:     |*| 1398581458@qq.com
|*|============|*/

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
            if (!config.Folded && config.isUploading) _builder.Append($"[上传中 ... ]");
            if (!string.IsNullOrEmpty(config.Name))
                _builder.Append(config.Name);

            if (!string.IsNullOrEmpty(config.Description))
                _builder.Append('(').Append(config.Description).Append(')');

            if (!string.IsNullOrEmpty(config.BUCKET_NAME))
            {
                _builder.Append('[');
                if (config.DirTreeFiled.IsValidity())
                {
                    _builder.Append(config.DirTreeFiled.GetFullPath()).Append(" -> ");
                }

                _builder.Append(config.BUCKET_NAME);
                if (string.IsNullOrEmpty(config.Description))
                {
                    _builder.Append(']');
                }
                else
                {
                    _builder.Append('/').Append(config.Description).Append(']');
                }
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
        }

        private void OnDrawBuildGCloud()
        {
            if (BuildConfig.GCloudConfigs is null || BuildConfig.GCloudConfigs.Length == 0) return;

            using (var scope = new EditorGUILayout.ScrollViewScope(OnDrawConfigGCScroll))
            {
                OnDrawConfigGCScroll = scope.scrollPosition;
                for (var i = BuildConfig.GCloudConfigs.Length - 1; i >= 0; i--)
                {
                    using (GELayout.Vertical(GEStyle.INThumbnailShadow))
                    {
                        using (GELayout.VHorizontal(GEStyle.Toolbar))
                        {
                            if (GELayout.Button(BuildConfig.GCloudConfigs[i].Folded ? GC_FOLDOUT : GC_FOLDOUT_ON,
                                    GEStyle.TEtoolbarbutton, GP_Width_30))
                            {
                                BuildConfig.GCloudConfigs[i].Folded = !BuildConfig.GCloudConfigs[i].Folded;
                                GUI.FocusControl(null);
                            }

                            GELayout.Label(GetGCItemDes(BuildConfig.GCloudConfigs[i], i),
                                GEStyle.HeaderLabel, GP_Width_EXPAND);

                            if (BuildConfig.GCloudConfigs[i].isUploading) GUI.enabled = false;

                            if (!string.IsNullOrEmpty(BuildConfig.GCloudConfigs[i].BUCKET_NAME))
                            {
                                if (!string.IsNullOrEmpty(BuildConfig.GCloudConfigs[i].DirTreeFiled.DirPath))
                                {
                                    if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                                    {
                                        if (GUILayout.Button("更新首包配置", GEStyle.toolbarbutton, GP_Width_100))
                                        {
                                            GUI.FocusControl(null);
                                            UpdateUploadFirstPack(BuildConfig.GCloudConfigs[i]);
                                        }
                                    }
                                }

                                if (GUILayout.Button(GC_REFRESH, GEStyle.toolbarbutton, GP_Width_20))
                                {
                                    GUI.FocusControl(null);
                                    BuildConfig.GCloudConfigs[i].DirTreeFiled.UpdateOption();
                                    return;
                                }
                            }

                            if (GUILayout.Button(GC_DEL, GEStyle.toolbarbutton, GP_Width_20))
                            {
                                GUI.FocusControl(null);
                                if (EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消"))
                                {
                                    BuildConfig.GCloudConfigs = BuildConfig.GCloudConfigs.RemoveAt(i);
                                    return;
                                }
                            }

                            if (BuildConfig.GCloudConfigs[i].isUploading) GUI.enabled = true;
                        }

                        OnDrawBuildGC(BuildConfig.GCloudConfigs[i]);
                    }
                }
            }
        }

        private void OnDrawBuildGC(ASBuildConfig.GCloudConfig config)
        {
            if (!config.Folded) return;
            if (config.isUploading) GUI.enabled = false;

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("名称:描述", GP_Width_100);
                config.Name = GELayout.FieldDelayed(config.Name);
                config.Description = GELayout.FieldDelayed(config.Description);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("环境 : gcloud local", GP_Width_100);
                config.GCLOUD_PATH = GELayout.FieldDelayed(config.GCLOUD_PATH);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("环境 : gsutil local", GP_Width_100);
                config.GSUTIL_PATH = GELayout.FieldDelayed(config.GSUTIL_PATH);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("储存桶", GP_Width_100);
                config.BUCKET_NAME = GELayout.FieldDelayed(config.BUCKET_NAME);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("元数据:[键:值]", GP_Width_100);
                config.MetaDataKey = GELayout.FieldDelayed(config.MetaDataKey);
                config.MetaDataValue = GELayout.FieldDelayed(config.MetaDataValue);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("本地路径", GP_Width_100);
                config.DirTreeFiled.OnDraw();

                if (config.isUploading) GUILayout.Label("上传中", GEStyle.toolbarbutton, GP_Width_50);
                else if (GUILayout.Button("上传", GEStyle.toolbarbutton, GP_Width_50))
                {
                    GUI.FocusControl(null);
                    _ = config.Upload();
                }
            }

            if (config.isUploading) GUI.enabled = true;
        }
    }
}