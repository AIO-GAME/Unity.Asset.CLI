﻿#region

using Config;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    internal partial class ViewTreePackage
    {
        protected sealed class TreeViewItemPackage : TreeViewItem, IGraphDraw
        {
            private static Texture _MainIcon;
            private static Texture _Icon;

            private static Texture Icon     => _Icon ?? (_Icon = Resources.Load<Texture>("Editor/Icon/Color/-school-bag"));
            private static Texture MainIcon => _MainIcon ?? (_MainIcon = Resources.Load<Texture>("Editor/Icon/Color/-briefcase"));

            public AssetCollectPackage Package { get; }


            public TreeViewItemPackage(int id, AssetCollectPackage package) : base(id, 1, package.Name)
            {
                Package = package;
            }

            public override Texture2D icon        => Package.Default ? MainIcon as Texture2D : Icon as Texture2D;
            public override string    displayName => Package.Name;
            public override string    ToString()  => Package.ToString();

            #region IGraphDraw

            float IGraphDraw.Height                                                   => 35;
            bool IGraphDraw. AllowChangeExpandedState                                 => false;
            bool IGraphDraw. AllowRename                                              => true;
            Rect IGraphDraw. GetRenameRect(Rect cellRect, int row, TreeViewItem item) => cellRect;

            #endregion

            public void OnDraw(Rect cellRect, ref RowGUIArgs args)
            {
                if (Package.Enable)
                {
                    OnDrawContent(cellRect, ref args);
                }
                else
                {
                    var oldColor = GUI.color;
                    GUI.color = Color.gray;
                    OnDrawContent(cellRect, ref args);
                    GUI.color = oldColor;
                }
            }


            private static readonly Color ColorLine = new Color(0.5f, 0.5f, 0.5f, 0.5f);


            private void DrawStyle(Rect cellRect, ref RowGUIArgs args)
            {
                var rect = new Rect(args.rowRect.x, args.rowRect.y, args.rowRect.width - 1, args.rowRect.height - 1);
                GUI.Box(rect, string.Empty, args.selected ? GEStyle.SelectionRect : GEStyle.INThumbnailShadow);
                rect.Set(cellRect.x + 30, cellRect.y + 8, cellRect.width - 40, 1);
                EditorGUI.DrawRect(rect, ColorLine);
                rect.Set(cellRect.x + cellRect.width - 11, cellRect.y - 9, 1, cellRect.height + 16);
                EditorGUI.DrawRect(rect, ColorLine);
            }

            private void OnDrawContent(Rect cellRect, ref RowGUIArgs args)
            {
                DrawStyle(cellRect, ref args);

                var iconRect = new Rect(cellRect.x, cellRect.y + 1, cellRect.height - 1, cellRect.height - 1);
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);

                // 绘制名称
                var nameRect = new Rect(cellRect.x + iconRect.xMax + 1, cellRect.y - 9, cellRect.width - iconRect.width - iconRect.x, cellRect.height);
                DefaultGUI.BoldLabel(nameRect, displayName, args.selected, args.focused);

                // 绘制描述
                var descRect = new Rect(cellRect.x + iconRect.xMax + 1, cellRect.y + 9, cellRect.width - iconRect.width, cellRect.height);
                DefaultGUI.Label(descRect, Package.Description, args.selected, args.focused);
            }
        }
    }
}