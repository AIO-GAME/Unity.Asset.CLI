#region

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    internal sealed class TreeViewItemGroup : TreeViewItem, ITVItemDraw
    {
        private static Texture _Icon;

        public TreeViewItemGroup(int id, AssetCollectGroup group) : base(id, 2, group.Name)
        {
            Group = group;
        }

        private static Texture Icon => _Icon ?? (_Icon = Resources.Load<Texture>("Editor/Icon/Color/-tutorial-"));

        public AssetCollectGroup Group { get; }

        public override Texture2D icon => Icon as Texture2D;

        #region IGraphDraw Members

        void ITVItemDraw.OnDraw(Rect cellRect, int col, ref RowGUIArgs args)
        {
            cellRect.x += 10;
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

        bool ITVItemDraw.MatchSearch(string search)
        {
            if (string.IsNullOrEmpty(search)) return true;
            return Group.Name.Contains(search) || Group.Description.Contains(search);
        }

        #endregion

        private static Rect GetRectLine(Rect cellRect)
        {
            return new Rect(cellRect.x + 30, cellRect.y + 8, cellRect.width - 40, 1);
        }


        private void DrawStyle(Rect cellRect, ref RowGUIArgs args)
        {
            GUI.Box(args.rowRect, GUIContent.none, args.selected ? GEStyle.SelectionRect : GEStyle.INThumbnailShadow);

            var rect = new Rect(cellRect.x + 30, args.rowRect.y + args.rowRect.height / 2f - 1, cellRect.width - 40, 1);
            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);
        }

        private void OnDrawContent(Rect cellRect, ref RowGUIArgs args)
        {
            DrawStyle(cellRect, ref args);

            var rect = new Rect(cellRect.x, cellRect.y + 1, cellRect.height - 1, cellRect.height - 1);
            GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);

            var width = cellRect.width - rect.width - rect.x;
            var x = cellRect.x + rect.xMax + 1;

            rect.Set(x, cellRect.y - 9, width, cellRect.height); // 绘制名称
            TreeView.DefaultGUI.BoldLabel(rect, Group.Name, args.selected, args.focused);


            rect.Set(x, cellRect.y + 9, width, cellRect.height); // 绘制描述
            TreeView.DefaultGUI.Label(rect, Group.Description, args.selected, args.focused);
        }


        #region IGraphDraw

        float ITVItemDraw.GetHeight()                           => 35;
        bool ITVItemDraw. AllowChangeExpandedState              => false;
        bool ITVItemDraw. AllowRename                           => true;
        Rect ITVItemDraw. GetRenameRect(Rect cellRect, int row) => cellRect;

        #endregion
    }
}