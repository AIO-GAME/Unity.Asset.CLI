/*|============|*|
|*|Author:     |*| xi nan
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
        #region FTP

        private string GetFTPItemDes(ASBuildConfig.FTPConfig config, int i)
        {
            _builder.Clear();
            if (!config.Folded && config.isUploading) _builder.Append($"[上传中:{config.UploadProgress.Progress}%]");
            if (!string.IsNullOrEmpty(config.Name))
                _builder.Append(config.Name);

            if (!string.IsNullOrEmpty(config.Description))
                _builder.Append('(').Append(config.Description).Append(')');

            if (!string.IsNullOrEmpty(config.Server))
            {
                _builder.Append('[');
                if (config.DirTreeFiled.IsValidity())
                {
                    _builder.Append(config.DirTreeFiled.GetFullPath()).Append(" -> ");
                }

                _builder.Append(config.Server).Append(':')
                    .Append(config.Port);
                if (string.IsNullOrEmpty(config.RemotePath))
                {
                    _builder.Append(']');
                }
                else
                {
                    _builder.Append('/').Append(config.RemotePath).Append(']');
                }
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
        }

        private void OnDrawBuildFTP()
        {
            if (BuildConfig.FTPConfigs is null || BuildConfig.FTPConfigs.Length == 0) return;
            using (var scope = new EditorGUILayout.ScrollViewScope(OnDrawConfigFTPScroll))
            {
                OnDrawConfigFTPScroll = scope.scrollPosition;
                for (var i = BuildConfig.FTPConfigs.Length - 1; i >= 0; i--)
                {
                    using (GELayout.Vertical(GEStyle.INThumbnailShadow))
                    {
                        using (GELayout.VHorizontal(GEStyle.Toolbar))
                        {
                            if (GELayout.Button(BuildConfig.FTPConfigs[i].Folded ? GC_FOLDOUT : GC_FOLDOUT_ON,
                                    GEStyle.TEtoolbarbutton, GP_Width_30))
                            {
                                BuildConfig.FTPConfigs[i].Folded = !BuildConfig.FTPConfigs[i].Folded;
                                GUI.FocusControl(null);
                            }

                            GELayout.Label(GetFTPItemDes(BuildConfig.FTPConfigs[i], i), GEStyle.HeaderLabel,
                                GP_Width_EXPAND);

                            if (!string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].Server) &&
                                !string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].User) &&
                                !string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].Pass))
                            {
                                if (GUILayout.Button("校验", GEStyle.toolbarbutton, GP_Width_50))
                                {
                                    GUI.FocusControl(null);
                                    ValidateFTP(BuildConfig.FTPConfigs[i]);
                                }

                                if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                                {
                                    if (GUILayout.Button("更新首包配置", GEStyle.toolbarbutton, GP_Width_100))
                                    {
                                        GUI.FocusControl(null);
                                        UpdateUploadFirstPack(BuildConfig.FTPConfigs[i]);
                                    }
                                }

                                if (GUILayout.Button(GC_REFRESH, GEStyle.toolbarbutton, GP_Width_20))
                                {
                                    GUI.FocusControl(null);
                                    BuildConfig.FTPConfigs[i].DirTreeFiled.UpdateOption();
                                    return;
                                }
                            }

                            if (GUILayout.Button(GC_DEL, GEStyle.toolbarbutton, GP_Width_20))
                            {
                                GUI.FocusControl(null);
                                if (EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消"))
                                {
                                    BuildConfig.FTPConfigs = BuildConfig.FTPConfigs.RemoveAt(i);
                                    return;
                                }
                            }
                        }

                        OnDrawBuildFTP(BuildConfig.FTPConfigs[i]);
                    }
                }
            }
        }

        private void OnDrawBuildFTP(ASBuildConfig.FTPConfig config)
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
                EditorGUILayout.LabelField("地址:端口", GP_Width_100);
                config.Server = GELayout.FieldDelayed(config.Server);
                config.Port = GELayout.FieldDelayed(config.Port, GP_Width_50);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("账户:密码", GP_Width_100);
                config.User = GELayout.FieldDelayed(config.User);
                config.Pass = GELayout.FieldDelayed(config.Pass);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("远程路径", GP_Width_100);
                config.RemotePath = GELayout.FieldDelayed(config.RemotePath);
                if (!string.IsNullOrEmpty(config.RemotePath))
                {
                    if (GUILayout.Button("创建", GEStyle.toolbarbutton, GP_Width_50))
                    {
                        GUI.FocusControl(null);
                        CreateFTP(config);
                        return;
                    }
                }
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                if (config.isUploading)
                {
                    EditorGUILayout.LabelField($"上传进度... {config.UploadProgress}",
                        GEStyle.CenteredLabel, GP_Width_EXPAND);

                    if (GUILayout.Button("取消", GEStyle.toolbarbutton, GP_Width_50))
                    {
                        GUI.FocusControl(null);
                        config.UploadOperation?.Cancel();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("本地路径", GP_Width_100);
                    config.DirTreeFiled.OnDraw();
                    if (!string.IsNullOrEmpty(config.Server) &&
                        !string.IsNullOrEmpty(config.User) &&
                        !string.IsNullOrEmpty(config.Pass) &&
                        !string.IsNullOrEmpty(config.DirTreeFiled.DirPath)
                       )
                    {
                        if (GUILayout.Button("上传", GEStyle.toolbarbutton, GP_Width_50))
                        {
                            GUI.FocusControl(null);
                            config.Upload();
                        }
                    }
                }
            }

            if (config.isUploading) GUI.enabled = true;
        }

        #endregion
    }
}