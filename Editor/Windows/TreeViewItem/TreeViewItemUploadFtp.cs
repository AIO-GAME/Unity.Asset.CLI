using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewItemUploadFtp : TreeViewItem, ITVItemDraw
    {
        public TreeViewItemUploadFtp(int id, ASBuildConfig.FTPConfig config) : base(id, 1) { Config = config; }

        #region ITVItemDraw

        public bool  AllowChangeExpandedState             => false;
        public bool  AllowRename                          => false;
        public float GetHeight()                          => Config.Folded ? 22 * 6 : 22 + 3;
        public bool  MatchSearch(string search)           => GetFtpItemDes(Config, id + 1).Contains(search);
        public Rect  GetRenameRect(Rect rowRect, int row) => rowRect;

        #endregion

        private GUIContent              GC_FOLDOUT    = GEContent.NewSetting("quanping-shouqi-xian", "收缩");
        private GUIContent              GC_FOLDOUT_ON = GEContent.NewSetting("quanping-zhankai-xian", "展开");
        public  ASBuildConfig.FTPConfig Config;

        private void OnDrawHeader(Rect rect)
        {
            if (!string.IsNullOrEmpty(Config.Server)
             && !string.IsNullOrEmpty(Config.User)
             && !string.IsNullOrEmpty(Config.Pass))
            {
                rect.width =  20;
                rect.x     -= rect.width;
                if (GUI.Button(rect, EditorGUIUtility.IconContent("Refresh").SetTooltips("刷新"), GEStyle.toolbarbutton))
                {
                    GUI.FocusControl(null);
                    Config.DirTreeFiled.UpdateOption();
                }

                if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                {
                    rect.width =  100;
                    rect.x     -= rect.width;
                    if (GUI.Button(rect, "更新首包配置", GEStyle.toolbarbutton))
                    {
                        AssetWindow.UpdateUploadFirstPack(Config);
                        GUI.FocusControl(null);
                    }
                }

                rect.width =  50;
                rect.x     -= rect.width;
                if (GUI.Button(rect, "校验", GEStyle.toolbarbutton))
                {
                    GUI.FocusControl(null);
                    ValidateFTP(Config);
                }
            }

            rect.width =  rect.x - 30;
            rect.x     -= rect.width;
            GUI.Label(rect, GetFtpItemDes(Config, id + 1), GEStyle.HeaderLabel);

            rect.width =  30;
            rect.x     -= rect.width;
            if (GUI.Button(rect, Config.Folded ? GC_FOLDOUT : GC_FOLDOUT_ON, GEStyle.TEtoolbarbutton))
            {
                Config.Folded = !Config.Folded;
                GUI.FocusControl(null);
            }
        }

        public void OnDraw(Rect cell, int col, ref RowGUIArgs args)
        {
            EditorGUI.DrawRect(args.rowRect, args.row % 2 == 0 ? TreeViewBasics.ColorAlternatingA : TreeViewBasics.ColorAlternatingB);

            var rectItem = new Rect(args.rowRect.x, args.rowRect.y, args.rowRect.width, 22);
            GUI.Box(rectItem, string.Empty, GEStyle.INThumbnailShadow);
            if (args.selected) GUI.Box(rectItem, string.Empty, GEStyle.SelectionRect);

            rectItem.x = cell.width - 1;
            OnDrawHeader(rectItem);
            if (!Config.Folded) return;

            rectItem.y      += rectItem.height;
            rectItem.x      =  cell.x;
            rectItem.width  =  cell.width;
            rectItem.height =  args.rowRect.height - rectItem.height - 3;
            GUI.Box(rectItem, string.Empty, GEStyle.InnerShadowBg);
            rectItem.y      += 3;
            rectItem.height =  22;
            OnDrawBuildFTP(Config, rectItem);
        }

        private static void OnDrawBuildFTP(ASBuildConfig.FTPConfig config, in Rect rect)
        {
            using (new EditorGUI.DisabledScope(config.isUploading))
            {
                #region 名称:描述

                var cell = new Rect(rect.x + 10, rect.y, 100, 20);
                EditorGUI.LabelField(cell, "名称:描述", GEStyle.HeaderLabel);

                cell.x      += cell.width;
                cell.width  =  (rect.width - cell.x) / 2;
                config.Name =  EditorGUI.DelayedTextField(cell, config.Name);

                cell.x             += cell.width;
                config.Description =  EditorGUI.DelayedTextField(cell, config.Description);

                #endregion

                #region 地址:端口

                cell.y     += cell.height;
                cell.x     =  rect.x + 10;
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
                cell.x     =  rect.x + 10;
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
                cell.x     =  rect.x + 10;
                cell.width =  100;
                EditorGUI.LabelField(cell, "远程路径", GEStyle.HeaderLabel);

                cell.x            += cell.width;
                cell.width        =  rect.width - cell.x - 50;
                config.RemotePath =  EditorGUI.DelayedTextField(cell, config.RemotePath);

                cell.x     += cell.width;
                cell.width =  50;
                if (GUI.Button(cell, "创建", GEStyle.toolbarbuttonRight))
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
                    cell.x     = rect.x + 10;
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
                        if (GUI.Button(cell, "上传", GEStyle.toolbarbuttonRight))
                        {
                            GUI.FocusControl(null);
                            config.Upload();
                        }
                    }

                    cell.x     = rect.x + 10 + 100;
                    cell.width = rect.width - cell.width - cell.x;
                    config.DirTreeFiled.OnDraw(cell);

                    cell.width =  100;
                    cell.x     -= cell.width;
                    EditorGUI.LabelField(cell, "本地路径", GEStyle.HeaderLabel);
                }

                #endregion
            }
        }

        private static string GetFtpItemDes(ASBuildConfig.FTPConfig config, int i)
        {
            var _builder = new StringBuilder();
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

        /// <summary>
        ///     创建 FTP
        /// </summary>
        private static async void CreateFTP(ASBuildConfig.FTPConfig config)
        {
            using (var handle = AHandle.FTP.Create(config.Server,
                                                   config.Port,
                                                   config.User,
                                                   config.Pass,
                                                   config.RemotePath))
            {
                EditorUtility.DisplayDialog("提示", await handle.InitAsync()
                                                ? $"创建成功 {handle.URI}"
                                                : $"创建失败 {handle.URI}", "确定");
            }
        }

        /// <summary>
        ///     验证 FTP 是否有效
        /// </summary>
        private static async void ValidateFTP(ASBuildConfig.FTPConfig config)
        {
            EditorUtility.DisplayDialog("提示", await config.Validate() ? "连接成功" : "连接失败", "确定");
        }
    }
}