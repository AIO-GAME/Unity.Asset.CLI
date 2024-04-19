using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewQueryAsset : ViewTreeRowSingle
    {
        private TreeViewQueryAsset(TreeViewState state, MultiColumnHeader header, PageList<AssetDataInfo> pageList)
            : base(state, header)
        {
            PageValues           = pageList;
            PageValues.PageIndex = 0;
        }

        /// <summary>
        ///     当前选择包索引
        /// </summary>
        private PageList<AssetDataInfo> PageValues;

        public static TreeViewQueryAsset Create(PageList<AssetDataInfo> pageValues)
        {
            return new TreeViewQueryAsset(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("可寻址路径", 150, 100),
                GetMultiColumnHeaderColumn("资源大小", 150, 100),
                GetMultiColumnHeaderColumn("修改时间", 150, 100)
            })), pageValues);
        }

        public int PageSize
        {
            get => PageValues.PageSize;
            set => PageValues.PageSize = value;
        }

        /// <inheritdoc />
        protected override void OnDraw(Rect rect)
        {
        }

        /// <inheritdoc />
        protected override void OnSorting(MultiColumnHeader header)
        {
            
        }

        protected override void OnInitialize()
        {
            // multiColumnHeader.state.maximumNumberOfSortedColumns = 3;
        }

        /// <inheritdoc />
        protected override void OnBuildRows(TreeViewItem root)
        {
            if (PageValues is null) return;
            if (root.children is null)
                root.children = new System.Collections.Generic.List<TreeViewItem>();
            else
                root.children.Clear();
            
            for (var i = 0; i < PageValues.CurrentPageValues.Length; i++)
            {
                var item = PageValues.CurrentPageValues[i];
                root.AddChild(new TreeViewItemQueryAsset(i, item));
            }
        }
    }
}