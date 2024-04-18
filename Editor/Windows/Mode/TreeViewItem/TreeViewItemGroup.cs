#region

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    partial class ViewTreeGroup
    {
        protected sealed class TreeViewItemGroup : TreeViewItem, IGraphDraw
        {
            private static Texture _Icon;

            private static Texture Icon => _Icon ?? (_Icon = Resources.Load<Texture>("Editor/Icon/Color/-tutorial-"));

            public AssetCollectGroup Group { get; }

            public TreeViewItemGroup(int id, AssetCollectGroup group) : base(id, 2, group.Name)
            {
                Group = group;
            }

            public override Texture2D icon => Icon as Texture2D;

            private static Rect GetRectLine(Rect cellRect)
            {
                return new Rect(cellRect.x + 30, cellRect.y + 8, cellRect.width - 40, 1);
            }

            public void OnDraw(Rect cellRect, ref RowGUIArgs args)
            {
                if (Group.Enable)
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


            #region IGraphDraw

            float IGraphDraw.GetHeight()                           => 35;
            bool IGraphDraw. AllowChangeExpandedState              => false;
            bool IGraphDraw. AllowRename                           => true;
            Rect IGraphDraw. GetRenameRect(Rect cellRect, int row) => cellRect;

            #endregion


            private void DrawStyle(Rect cellRect, ref RowGUIArgs args)
            {
                var rect = new Rect(args.rowRect.x, args.rowRect.y, args.rowRect.width - 1, args.rowRect.height - 1);
                GUI.Box(rect, string.Empty, args.selected ? GEStyle.SelectionRect : GEStyle.INThumbnailShadow);
                rect.Set(cellRect.x + 30, cellRect.y + 8, cellRect.width - 40, 1);
                EditorGUI.DrawRect(rect, ColorLine);
                rect.Set(cellRect.x + cellRect.width - 11, cellRect.y - 9, 1, args.rowRect.height);
                EditorGUI.DrawRect(rect, ColorLine);
            }

            private void OnDrawContent(Rect cellRect, ref RowGUIArgs args)
            {
                DrawStyle(cellRect, ref args);

                var rect = new Rect(cellRect.x, cellRect.y + 1, cellRect.height - 1, cellRect.height - 1);
                GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);

                var width = cellRect.width - rect.width - rect.x;
                var x = cellRect.x + rect.xMax + 1;

                rect.Set(x, cellRect.y - 9, width, cellRect.height); // 绘制名称
                DefaultGUI.BoldLabel(rect, Group.Name, args.selected, args.focused);


                rect.Set(x, cellRect.y + 9, width, cellRect.height); // 绘制描述
                DefaultGUI.Label(rect, Group.Description, args.selected, args.focused);
            }
        }
    }
}