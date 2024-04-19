using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public struct RowGUIArgs
    {
        /// <summary>
        ///   <para>Item for the current row being handled in TreeView.RowGUI.</para>
        /// </summary>
        public TreeViewItem item;

        /// <summary>
        ///   <para>Label used for text rendering of the item displayName. Note this is an empty string when isRenaming == true.</para>
        /// </summary>
        public string label;

        /// <summary>
        ///   <para>Row rect for the current row being handled.</para>
        /// </summary>
        public Rect rowRect;

        /// <summary>
        ///   <para>Row index into the list of current rows.</para>
        /// </summary>
        public int row;

        /// <summary>
        ///   <para>This value is true when the current row's item is part of the current selection.</para>
        /// </summary>
        public bool selected;

        /// <summary>
        ///   <para>This value is true only when the TreeView has keyboard focus and the TreeView's window has focus.</para>
        /// </summary>
        public bool focused;

        /// <summary>
        ///   <para>This value is true when the ::item is currently being renamed.</para>
        /// </summary>
        public bool isRenaming;
    }

    public interface IGraphDraw
    {
        /// <summary>
        ///     绘制
        /// </summary>
        /// <param name="cellRect"> 单元格矩形 </param>
        /// <param name="args"> 行参数 </param>
        void OnDraw(Rect cellRect, ref RowGUIArgs args);

        /// <summary>
        ///     是否允许改变展开状态
        /// </summary>
        bool AllowChangeExpandedState { get; }

        /// <summary>
        ///     是否允许重命名
        /// </summary>
        bool AllowRename { get; }

        /// <summary>
        ///     高度
        /// </summary>
        float GetHeight();

        /// <summary>
        ///     获取重命名矩形
        /// </summary>
        Rect GetRenameRect(Rect rowRect, int row);
    }

    public abstract class TreeViewBasics : TreeView
    {
        public static readonly Color ColorLine         = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        public static readonly Color ColorAlternatingA = new Color(63 / 255f, 63 / 255f, 63 / 255f, 1); //#3F3F3F
        public static readonly Color ColorAlternatingB = new Color(56 / 255f, 56 / 255f, 56 / 255f, 1); //#383838

        protected static MultiColumnHeaderState.Column GetMultiColumnHeaderColumn(
            string name,
            float  width = 100,
            float  min   = 80,
            float  max   = 200
        ) => new MultiColumnHeaderState.Column
        {
            headerContent         = new GUIContent(name),
            minWidth              = min,
            maxWidth              = max,
            width                 = width,
            sortedAscending       = true,
            headerTextAlignment   = TextAlignment.Center,
            sortingArrowAlignment = TextAlignment.Center,
            canSort               = true,
            autoResize            = true,
            allowToggleVisibility = false
        };

        protected TreeViewBasics(TreeViewState state, MultiColumnHeader header) : base(state, header) { }

        #region 工具函数

        /// <summary>
        /// 重载并选中
        /// </summary>
        protected void ReloadAndSelect() => ReloadAndSelect(Array.Empty<int>());

        /// <summary>
        /// 重载并选中
        /// </summary>
        protected void ReloadAndSelect(int hc) => ReloadAndSelect(new[] { hc });

        /// <summary>
        /// 重载并选中
        /// </summary>
        protected void ReloadAndSelect(int hc1, int hc2) => ReloadAndSelect(new[] { hc1, hc2 });

        /// <summary>
        /// 重载并选中
        /// </summary>
        protected void ReloadAndSelect(IList<int> hashCodes)
        {
            Reload();
            if (hashCodes.Count > 0) SelectionChanged(hashCodes);
            SetFocus();
        }
        
        protected static AIO.UEditor.RowGUIArgs Cast(RowGUIArgs args)
        {
            var RowGUIArgs = new AIO.UEditor.RowGUIArgs
            {
                item       = args.item,
                label      = args.label,
                rowRect    = args.rowRect,
                row        = args.row,
                selected   = args.selected,
                focused    = args.focused,
                isRenaming = args.isRenaming
            };
            return RowGUIArgs;
        }

        #endregion

        #region sealed override

        /// <summary>
        ///     构建根节点
        /// </summary>
        /// <returns>根节点</returns>
        protected sealed override TreeViewItem BuildRoot() => new TreeViewItem
        {
            id          = 0,
            depth       = -1,
            displayName = "root",
            children    = new List<TreeViewItem>()
        };

        #endregion
    }
}