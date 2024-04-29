#region

using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    public class TreeViewItemQueryAsset : TreeViewItem, ITVItemDraw
    {
        /// <summary>
        ///     界面内容 - 首包 取消
        /// </summary>
        private static readonly GUIContent GC_FP_Cancel = GEContent.NewSettingCustom("Editor/Icon/Setting/cancel", "从首包列表删除");

        /// <summary>
        ///     界面内容 - 首包 确定
        /// </summary>
        private static readonly GUIContent GC_FP_OK = GEContent.NewSettingCustom("Editor/Icon/Setting/add-to-list", "添加进入首包列表");

        public AssetDataInfo data;

        public TreeViewItemQueryAsset(
            int           id,
            AssetDataInfo dataInfo) : base(id, 1)
        {
            data = dataInfo;
        }

        #region IGraphDraw

        /// <summary>
        ///    是否在首包中
        /// </summary>
        public bool IsFirstPackageResource { get; set; }

        /// <summary>
        ///   添加或删除首包资源
        /// </summary>
        public Action<AssetDataInfo, bool> OnFirstPackageResource { get; set; }

        public Action<int> Refresh { get; set; }

        public bool  AllowChangeExpandedState             => false;
        public bool  AllowRename                          => false;
        public float GetHeight()                          => 22;
        public Rect  GetRenameRect(Rect rowRect, int row) => rowRect;

        bool ITVItemDraw.MatchSearch(string search)
        {
            if (string.IsNullOrEmpty(search)) return true;
            return data.Address.Contains(search)
                || data.AssetPath.Contains(search)
                || data.Type.Contains(search);
        }

        void ITVItemDraw.OnDraw(Rect cell, int col, ref RowGUIArgs args)
        {
            if (string.IsNullOrEmpty(data.AssetPath))
            {
                if (col == 0)
                {
                    EditorGUI.DrawRect(args.rowRect, args.row % 2 == 0 ? TreeViewBasics.ColorAlternatingA : TreeViewBasics.ColorAlternatingB);
                    if (args.selected) GUI.Box(args.rowRect, string.Empty, GEStyle.SelectionRect);
                }

                return;
            }

            var rect = new Rect(cell.x + 10, cell.y, cell.width - 10, cell.height);
            switch (col)
            {
                case 0:
                    EditorGUI.DrawRect(args.rowRect, args.row % 2 == 0 ? TreeViewBasics.ColorAlternatingA : TreeViewBasics.ColorAlternatingB);
                    if (args.selected) GUI.Box(args.rowRect, string.Empty, GEStyle.SelectionRect);
                    rect.x     += 22;
                    rect.width -= 22;
                    EditorGUI.LabelField(rect, data.Address);
                    rect.x     = cell.x + 10;
                    rect.width = 20;
                    GUI.DrawTexture(rect, AssetDatabase.GetCachedIcon(data.AssetPath), ScaleMode.ScaleAndCrop);
                    cell.Set(cell.width - 1, args.rowRect.y, 1, args.rowRect.height - 2);
                    EditorGUI.DrawRect(cell, TreeViewBasics.ColorLine);
                    break;
                case 1: // 路径
                    EditorGUI.LabelField(rect, data.AssetPath);
                    cell.Set(cell.x + cell.width + 2, args.rowRect.y, 1, args.rowRect.height - 1);
                    EditorGUI.DrawRect(cell, TreeViewBasics.ColorLine);
                    break;
                case 2: // 类型
                    EditorGUI.LabelField(rect, data.Type);
                    cell.Set(cell.x + cell.width + 2, args.rowRect.y, 1, args.rowRect.height - 1);
                    EditorGUI.DrawRect(cell, TreeViewBasics.ColorLine);
                    break;
                case 3: // 大小
                    EditorGUI.LabelField(rect, data.Size.ToConverseStringFileSize());
                    cell.Set(cell.x + cell.width + 2, args.rowRect.y, 1, args.rowRect.height - 1);
                    EditorGUI.DrawRect(cell, TreeViewBasics.ColorLine);
                    break;
                case 4: // 时间
                    EditorGUI.LabelField(rect, data.GetLatestTime());
                    cell.Set(cell.x + cell.width + 2, args.rowRect.y, 1, args.rowRect.height - 1);
                    EditorGUI.DrawRect(cell, TreeViewBasics.ColorLine);
                    break;
                case 5: // 是否在首包中
                    rect.x = cell.x + cell.width / 5 - 1;
                    if (IsFirstPackageResource)
                    {
                        if (GUI.Button(rect, GC_FP_Cancel, GEStyle.IconButton))
                        {
                            OnFirstPackageResource?.Invoke(data, false);
                            Refresh?.Invoke(id);
                            HandleUtility.Repaint();
                            Event.current?.Use();
                        }
                    }
                    else
                    {
                        if (GUI.Button(rect, GC_FP_OK, GEStyle.IconButton))
                        {
                            OnFirstPackageResource?.Invoke(data, true);
                            Refresh?.Invoke(id);
                            HandleUtility.Repaint();
                            Event.current?.Use();
                        }
                    }

                    break;
            }
        }

        #endregion
    }
}