#region

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    internal sealed class TreeViewItemPackage : TreeViewItem, ITVItemDraw
    {
        private static Texture _MainIcon;
        private static Texture _Icon;

        public TreeViewItemPackage(int id, AssetCollectPackage package) : base(id, 1, package.Name) { Package = package; }

        private static Texture             Icon     => _Icon ?? (_Icon = Resources.Load<Texture>("Editor/Icon/Color/-school-bag"));
        private static Texture             MainIcon => _MainIcon ?? (_MainIcon = Resources.Load<Texture>("Editor/Icon/Color/-briefcase"));
        public         AssetCollectPackage Package  { get; }

        public override Texture2D icon => Package.Default ? MainIcon as Texture2D : Icon as Texture2D;

        #region IGraphDraw Members

        void ITVItemDraw.OnDraw(Rect cell, int col, ref RowGUIArgs args)
        {
            cell.x += 10;
            if (Package.Enable)
            {
                OnDrawContent(cell, ref args);
            }
            else
            {
                var oldColor = GUI.color;
                GUI.color = Color.gray;
                OnDrawContent(cell, ref args);
                GUI.color = oldColor;
            }
        }

        bool ITVItemDraw.MatchSearch(string search)
        {
            if (string.IsNullOrEmpty(search)) return true;
            return Package.Name.Contains(search) || Package.Description.Contains(search);
        }

        #endregion

        private void DrawStyle(Rect cellRect, ref RowGUIArgs args)
        {
            var rect = new Rect(args.rowRect.x, args.rowRect.y, args.rowRect.width - 1, args.rowRect.height - 1);
            GUI.Box(rect, string.Empty, args.selected ? GEStyle.SelectionRect : GEStyle.INThumbnailShadow);
            rect.Set(cellRect.x + 30, cellRect.y + 8, cellRect.width - 40, 1);
            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);
        }

        private void OnDrawContent(Rect cellRect, ref RowGUIArgs args)
        {
            DrawStyle(cellRect, ref args);

            var rect = new Rect(cellRect.x, cellRect.y + 1, cellRect.height - 1, cellRect.height - 1);
            GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);

            var width = cellRect.width - rect.width - rect.x;
            var x     = cellRect.x + rect.xMax + 1;

            rect.Set(x, cellRect.y - 9, width, cellRect.height); // 绘制名称
            TreeView.DefaultGUI.BoldLabel(rect, Package.Name, args.selected, args.focused);


            rect.Set(x, cellRect.y + 9, width, cellRect.height); // 绘制描述
            TreeView.DefaultGUI.Label(rect, Package.Description, args.selected, args.focused);
        }

        #region IGraphDraw

        bool ITVItemDraw. AllowChangeExpandedState              => false;
        bool ITVItemDraw. AllowRename                           => true;
        float ITVItemDraw.GetHeight()                           => 35;
        Rect ITVItemDraw. GetRenameRect(Rect cellRect, int row) => cellRect;

        #endregion
    }
}