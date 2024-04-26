using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewItemUploadGoogleCloud : TreeViewItem, ITVItemDraw
    {
        public TreeViewItemUploadGoogleCloud(int id, AssetBuildConfig.GCloudConfig config) : base(id, 1) { Config = config; }

        #region ITVItemDraw

        public bool  AllowChangeExpandedState             => false;
        public bool  AllowRename                          => false;
        public float GetHeight()                          => Config?.Folded ?? false ? 22 * 7 : 22 + 3;
        public bool  MatchSearch(string search)           => false;
        public Rect  GetRenameRect(Rect rowRect, int row) => rowRect;

        #endregion

        private GUIContent                 GC_FOLDOUT    = GEContent.NewSetting("quanping-shouqi-xian", "收缩");
        private GUIContent                 GC_FOLDOUT_ON = GEContent.NewSetting("quanping-zhankai-xian", "展开");
        public  AssetBuildConfig.GCloudConfig Config;

        #region Google Cloud

        private static string GetItemDes(AssetBuildConfig.GCloudConfig Config, int i)
        {
            var _builder = new StringBuilder();
            if (!Config.Folded && Config.isUploading) _builder.Append("[上传中 ... ]");
            if (!string.IsNullOrEmpty(Config.Name))
                _builder.Append(Config.Name);

            if (!string.IsNullOrEmpty(Config.Description))
                _builder.Append('(').Append(Config.Description).Append(')');

            if (!string.IsNullOrEmpty(Config.BUCKET_NAME))
            {
                _builder.Append('[');
                if (Config.DirTreeFiled.IsValidity()) _builder.Append(Config.DirTreeFiled.GetFullPath()).Append(" -> ");

                _builder.Append(Config.BUCKET_NAME);
                if (!string.IsNullOrEmpty(Config.Description)) _builder.Append('/').Append(Config.Description);
                _builder.Append(']');
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
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
            OnDrawBuildGC(rectItem);
        }

        private void OnDrawHeader(Rect rectItem)
        {
            using (new EditorGUI.DisabledScope(Config.isUploading))
            {
                if (!string.IsNullOrEmpty(Config.BUCKET_NAME))
                {
                    rectItem.width =  20;
                    rectItem.x     -= rectItem.width;
                    if (GUI.Button(rectItem, EditorGUIUtility.IconContent("Refresh").SetTooltips("刷新"), GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        Config.DirTreeFiled.UpdateOption();
                    }

                    if (!string.IsNullOrEmpty(Config.DirTreeFiled.DirPath)
                     && File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                    {
                        rectItem.width =  100;
                        rectItem.x     -= rectItem.width;
                        if (GUI.Button(rectItem, "更新首包配置", GEStyle.toolbarbutton))
                        {
                            GUI.FocusControl(null);
                            AssetWindow.UpdateUploadFirstPack(Config);
                        }
                    }
                }
            }

            rectItem.width =  rectItem.x - 30;
            rectItem.x     -= rectItem.width;
            GUI.Label(rectItem, GetItemDes(Config, id + 1), GEStyle.HeaderLabel);

            rectItem.width =  30;
            rectItem.x     -= rectItem.width;
            if (GUI.Button(rectItem, Config.Folded ? GC_FOLDOUT : GC_FOLDOUT_ON, GEStyle.TEtoolbarbutton))
            {
                Config.Folded = !Config.Folded;
                GUI.FocusControl(null);
            }
        }

        private void OnDrawBuildGC(Rect rect)
        {
            using (new EditorGUI.DisabledScope(Config.isUploading))
            {
                #region 名称:描述

                var cell = new Rect(rect.x + 5, rect.y, 100, 20);
                EditorGUI.LabelField(cell, "名称:描述", GEStyle.HeaderLabel);

                cell.x      += cell.width;
                cell.width  =  (rect.width - cell.x) / 2;
                Config.Name =  EditorGUI.DelayedTextField(cell, Config.Name);

                cell.x             += cell.width;
                Config.Description =  EditorGUI.DelayedTextField(cell, Config.Description);

                #endregion

                #region 环境 : gcloud local

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "环境 : gcloud local", GEStyle.HeaderLabel);

                cell.x             += cell.width;
                cell.width         =  rect.width - cell.x;
                Config.GCLOUD_PATH =  EditorGUI.DelayedTextField(cell, Config.GCLOUD_PATH);

                #endregion

                #region 环境 gsutil local

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "环境 : gcloud local", GEStyle.HeaderLabel);

                cell.x             += cell.width;
                cell.width         =  rect.width - cell.x;
                Config.GSUTIL_PATH =  EditorGUI.DelayedTextField(cell, Config.GSUTIL_PATH);

                #endregion

                #region 储存桶

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "储存桶", GEStyle.HeaderLabel);

                cell.x             += cell.width;
                cell.width         =  rect.width - cell.x;
                Config.BUCKET_NAME =  EditorGUI.DelayedTextField(cell, Config.BUCKET_NAME);

                #endregion


                #region 元数据:[键:值]

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "元数据:[键:值]", GEStyle.HeaderLabel);

                cell.x             += cell.width;
                cell.width         =  (rect.width - cell.x) / 2;
                Config.MetaDataKey =  EditorGUI.DelayedTextField(cell, Config.MetaDataKey);

                cell.x               += cell.width;
                Config.MetaDataValue =  EditorGUI.DelayedTextField(cell, Config.MetaDataValue);

                #endregion

                #region 本地路径

                cell.y     += cell.height;
                cell.x     =  rect.x + 5;
                cell.width =  100;
                EditorGUI.LabelField(cell, "本地路径", GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x - 50;
                Config.DirTreeFiled.OnDraw(cell);

                cell.x     += cell.width;
                cell.width =  50;
                if (Config.isUploading)
                {
                    EditorGUI.LabelField(cell, "上传中", GEStyle.toolbarbutton);
                }
                else if (GUI.Button(cell, "上传", GEStyle.toolbarbutton))
                {
                    GUI.FocusControl(null);
                    _ = Config.Upload();
                }

                #endregion
            }
        }

        #endregion
    }
}