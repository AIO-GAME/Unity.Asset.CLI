#region

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    public class TreeViewItemQueryAsset : TreeViewItem, IGraphDraw
    {
        private AssetDataInfo data;

        public TreeViewItemQueryAsset(int id, AssetDataInfo dataInfo) : base(id, 1)
        {
            data = dataInfo;
        }

        #region IGraphDraw

        public bool  AllowChangeExpandedState             => false;
        public bool  AllowRename                          => false;
        public float GetHeight()                          => 22;
        public Rect  GetRenameRect(Rect rowRect, int row) => rowRect;

        void IGraphDraw.OnDraw(Rect cellRect, ref RowGUIArgs args)
        {
            var rect = new Rect(cellRect.x, cellRect.y, cellRect.width, cellRect.height);
            GUI.Box(rect, string.Empty, args.selected ? GEStyle.SelectionRect : GEStyle.INThumbnailShadow);
            rect.Set(cellRect.x + 30, cellRect.y + 8, cellRect.width - 40, 1);
            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);
            rect.Set(cellRect.x + cellRect.width - 11, cellRect.y - 9, 1, cellRect.height + 16);
            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);

            rect.Set(cellRect.x + 5, cellRect.y, 150, cellRect.height);
            GUI.Label(rect, data.Address);
            rect.Set(cellRect.x + 155, cellRect.y, 150, cellRect.height);
            GUI.Label(rect, data.Size.ToString());
            rect.Set(cellRect.x + 305, cellRect.y, 150, cellRect.height);
            GUI.Label(rect, data.Size.ToString());
        }

        #endregion
    }
}