using System.IO;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private void OnDrawBuildFTP(in Rect rect, out Rect outRect)
        {
            if (BuildConfig.FTPConfigs is null || BuildConfig.FTPConfigs.Length == 0)
            {
                outRect = rect;
                return;
            }

            var cell = new Rect(rect.x, rect.y, rect.width, 300);
            var view = new Rect(rect.x, rect.y, rect.width - 16, 20 * BuildConfig.FTPConfigs.Length);
            using (var scope = new GUI.ScrollViewScope(cell, OnDrawConfigFTPScroll, view))
            {
                OnDrawConfigFTPScroll = scope.scrollPosition;
                if (view.height > cell.height) cell.width -= 16;
                var rectItem = new Rect(rect.x, rect.y, rect.width - 16, 20);
                for (var i = BuildConfig.FTPConfigs.Length - 1; i >= 0; i--)
                {
                    rectItem.width = 20;
                    rectItem.x     = cell.width - rectItem.width;

                    if (GUI.Button(rectItem, GC_DEL, GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        if (EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消"))
                        {
                            BuildConfig.FTPConfigs = BuildConfig.FTPConfigs.RemoveAt(i).Exclude();
                        }

                        continue;
                    }

                    if (!string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].Server)
                     && !string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].User)
                     && !string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].Pass))
                    {
                        rectItem.width =  20;
                        rectItem.x     -= rectItem.width;
                        if (GUI.Button(rectItem, GC_REFRESH, GEStyle.toolbarbutton))
                        {
                            GUI.FocusControl(null);
                            BuildConfig.FTPConfigs[i].DirTreeFiled.UpdateOption();
                        }

                        if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                        {
                            rectItem.width =  100;
                            rectItem.x     -= rectItem.width;
                            if (GUI.Button(rectItem, "更新首包配置", GEStyle.toolbarbutton))
                            {
                                UpdateUploadFirstPack(BuildConfig.FTPConfigs[i]);
                                GUI.FocusControl(null);
                            }
                        }

                        rectItem.width =  50;
                        rectItem.x     -= rectItem.width;
                        if (GUI.Button(rectItem, "校验", GEStyle.toolbarbutton))
                        {
                            GUI.FocusControl(null);
                            ValidateFTP(BuildConfig.FTPConfigs[i]);
                        }
                    }

                    rectItem.width =  rectItem.x - 30;
                    rectItem.x     -= rectItem.width;
                    GUI.Label(rectItem, GetFTPItemDes(BuildConfig.FTPConfigs[i], i), GEStyle.HeaderLabel);

                    rectItem.width =  30;
                    rectItem.x     -= rectItem.width;
                    if (GUI.Button(rectItem, BuildConfig.FTPConfigs[i].Folded ? GC_FOLDOUT : GC_FOLDOUT_ON, GEStyle.TEtoolbarbutton))
                    {
                        BuildConfig.FTPConfigs[i].Folded = !BuildConfig.FTPConfigs[i].Folded;
                        GUI.FocusControl(null);
                    }

                    rectItem.y += rectItem.height;
                    if (!BuildConfig.FTPConfigs[i].Folded) continue;
                    rectItem.x     = cell.x;
                    rectItem.width = cell.width;
                    OnDrawBuildFTP(BuildConfig.FTPConfigs[i], rectItem, out rectItem);
                }
            }

            outRect = new Rect(rect.x, cell.y + cell.height, rect.width, cell.height);
        }

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
                if (config.DirTreeFiled.IsValidity()) _builder.Append(config.DirTreeFiled.GetFullPath()).Append(" -> ");

                _builder.Append(config.Server).Append(':').Append(config.Port);
                if (string.IsNullOrEmpty(config.RemotePath))
                    _builder.Append(']');
                else
                    _builder.Append('/').Append(config.RemotePath).Append(']');
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
        }

        private static void OnDrawBuildFTP(ASBuildConfig.FTPConfig config, in Rect rect, out Rect outRect)
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

                #region 地址:端口

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "地址:端口", GEStyle.HeaderLabel);

                cell.x        += cell.width;
                cell.width    =  rect.width - cell.x - 50;
                config.Server =  EditorGUI.DelayedTextField(cell, config.Server);

                cell.x      += cell.width;
                cell.width  =  50;
                config.Port =  EditorGUI.DelayedIntField(cell, config.Port);

                #endregion

                #region 账户:密码

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "账户:密码", GEStyle.HeaderLabel);

                cell.x      += cell.width;
                cell.width  =  (rect.width - cell.x) / 2;
                config.User =  EditorGUI.DelayedTextField(cell, config.User);

                cell.x      += cell.width;
                config.Pass =  EditorGUI.DelayedTextField(cell, config.Pass);

                #endregion

                #region 远程路径

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "远程路径", GEStyle.HeaderLabel);

                cell.x            += cell.width;
                cell.width        =  rect.width - cell.x - 50;
                config.RemotePath =  EditorGUI.DelayedTextField(cell, config.RemotePath);

                cell.x     += cell.width;
                cell.width =  50;
                if (GUI.Button(cell, "创建", GEStyle.toolbarbutton))
                {
                    GUI.FocusControl(null);
                    if (string.IsNullOrEmpty(config.RemotePath))
                        EditorUtility.DisplayDialog("提示", "远程路径不能为空", "确定");
                    else
                        CreateFTP(config);
                }

                #endregion

                #region 地址:端口

                cell.y += cell.height;
                if (config.isUploading)
                {
                    cell.x     = rect.x + 5;
                    cell.width = rect.width - cell.x - 50;
                    EditorGUI.LabelField(cell, $"上传进度... {config.UploadProgress}", GEStyle.CenteredLabel);

                    cell.x     += cell.width;
                    cell.width =  50;
                    if (GUI.Button(cell, "取消", GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        config.UploadOperation?.Cancel();
                    }
                }
                else
                {
                    cell.width = 0;
                    if (!string.IsNullOrEmpty(config.Server) &&
                        !string.IsNullOrEmpty(config.User) &&
                        !string.IsNullOrEmpty(config.Pass) &&
                        !string.IsNullOrEmpty(config.DirTreeFiled.DirPath)
                       )
                    {
                        cell.x     = rect.width - 50;
                        cell.width = 50;
                        if (GUI.Button(cell, "上传", GEStyle.toolbarbutton))
                        {
                            GUI.FocusControl(null);
                            config.Upload();
                        }
                    }

                    cell.x     = rect.x + 5 + 100;
                    cell.width = rect.width - cell.width - cell.x;
                    config.DirTreeFiled.OnDraw(cell);

                    cell.width =  100;
                    cell.x     -= cell.width;
                    EditorGUI.LabelField(cell, "本地路径", GEStyle.HeaderLabel);
                }

                #endregion

                outRect = new Rect(rect.x, cell.y + cell.height, rect.width, rect.height);
            }
        }
    }
}