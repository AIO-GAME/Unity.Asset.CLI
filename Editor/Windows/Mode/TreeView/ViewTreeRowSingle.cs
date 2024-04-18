#region

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    /// <summary>
    ///     SingleRowTreeEditor
    /// </summary>
    public abstract class ViewTreeRowSingle : TreeView
    {
        public event Action<int> OnSelectionChanged;

        #region 静态函数

        protected static readonly Color ColorLine         = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        protected static readonly Color ColorAlternatingA = new Color(63 / 255f, 63 / 255f, 63 / 255f, 1); //#3F3F3F
        protected static readonly Color ColorAlternatingB = new Color(56 / 255f, 56 / 255f, 56 / 255f, 1); //#383838

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

        #endregion

        protected interface IGraphDraw
        {
            /// <summary>
            ///    绘制
            /// </summary>
            /// <param name="cellRect"> 单元格矩形 </param>
            /// <param name="args"> 行参数 </param>
            void OnDraw(Rect cellRect, ref RowGUIArgs args);

            /// <summary>
            ///   是否允许改变展开状态
            /// </summary>
            bool AllowChangeExpandedState { get; }

            /// <summary>
            ///  是否允许重命名
            /// </summary>
            bool AllowRename { get; }

            /// <summary>
            ///   高度
            /// </summary>
            float GetHeight();

            /// <summary>
            ///  获取重命名矩形
            /// </summary>
            Rect GetRenameRect(Rect rowRect, int row);
        }

        private readonly string FullName;
        protected        int    ContentID;

        protected ViewTreeRowSingle(TreeViewState state, MultiColumnHeader header) : base(state, header)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            OnInitialize();
            FullName                      = GetType().FullName;
            showBorder                    = false;
            showAlternatingRowBackgrounds = true;
            useScrollView                 = true;
            baseIndent                    = 10;
            extraSpaceBeforeIconAndLabel  = 20;
            multiColumnHeader.SetSorting(0, false);
            multiColumnHeader.sortingChanged += SortingChanged;
            Reload();
            SetFocus();
        }

        #region 虚函数

        /// <summary>
        ///    初始化
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        ///   构建行
        /// </summary>
        /// <param name="root">根节点</param>
        protected abstract void OnBuildRows(TreeViewItem root);

        /// <summary>
        ///   重命名完成
        /// </summary>
        /// <param name="args">重命名参数</param>
        protected virtual void OnRename(RenameEndedArgs args) { }

        /// <summary>
        ///    排序
        /// </summary>
        /// <param name="header">多列头</param>
        protected virtual void OnSorting(MultiColumnHeader header) { }

        /// <summary>
        ///    绘制
        /// </summary>
        protected virtual void OnDraw() { }

        /// <summary>
        ///     选择
        /// </summary>
        /// <param name="id">ID</param>
        protected virtual void OnSelection(int id) { }

        /// <summary>
        ///    拖拽交换数据
        /// </summary>
        /// <param name="from">源</param>
        /// <param name="to">目标</param>
        protected virtual void OnDragSwapData(int from, int to) { }

        /// <summary>
        ///    右键点击空白区域
        /// </summary>
        /// <param name="menu">菜单</param>
        protected virtual void OnContextClicked(GenericMenu menu) { }

        /// <summary>
        ///   右键点击Item区域
        /// </summary>
        /// <param name="menu">菜单</param>
        /// <param name="item">选中组件</param>
        protected virtual void OnContextClicked(GenericMenu menu, TreeViewItem item) { }

        /// <summary>
        ///   按键按下
        /// </summary>
        /// <param name="keyCode"> 按键 </param>
        /// <param name="item"> 选中组件 </param>
        protected virtual void OnEventKeyDown(KeyCode keyCode, TreeViewItem item) { }

        /// <summary>
        ///   按键抬起
        /// </summary>
        /// <param name="keyCode"> 按键 </param>
        /// <param name="item"> 选中组件 </param>
        protected virtual void OnEventKeyUp(KeyCode keyCode, TreeViewItem item) { }

        #endregion

        #region 工具函数

        private void SortingChanged(MultiColumnHeader header)
        {
            OnSorting(header);
            Reload();
        }

        protected void ReloadAndSelect()             => ReloadAndSelect(Array.Empty<int>());
        protected void ReloadAndSelect(int hashCode) => ReloadAndSelect(new[] { hashCode });

        protected void ReloadAndSelect(IList<int> hashCodes)
        {
            Reload();
            SetSelection(hashCodes, TreeViewSelectionOptions.RevealAndFrame);
            SelectionChanged(hashCodes);
            SetFocus();
        }

        #endregion

        #region 接口实现

        /// <inheritdoc />
        protected sealed override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (root.children is null) root.children = new List<TreeViewItem>();
            else root.children.Clear();
            OnBuildRows(root);
            SetupDepthsFromParentsAndChildren(root);
            return base.BuildRows(root);
        }

        /// <summary>
        ///     绘制行
        /// </summary>
        protected sealed override void RowGUI(RowGUIArgs args)
        {
            if (args.item is IGraphDraw item)
            {
                var cellRect = args.GetCellRect(0);
                cellRect.x += 10;
                CenterRectUsingSingleLineHeight(ref cellRect);
                EditorGUI.BeginChangeCheck();
                item.OnDraw(cellRect, ref args);
                if (EditorGUI.EndChangeCheck()) Reload();
            }
        }

        public sealed override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            ContentID = GUIUtility.GetControlID(FocusType.Passive, rect);
            multiColumnHeader.state.AutoWidth(rect.width);
            if (Event.current.type == EventType.MouseDown
             && Event.current.button == 0
             && rect.Contains(Event.current.mousePosition)
               ) SetSelection(state.selectedIDs, TreeViewSelectionOptions.FireSelectionChanged);

            OnDraw();
        }

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

        /// <summary>
        ///     更改名称完成
        /// </summary>
        protected sealed override void RenameEnded(RenameEndedArgs args)
        {
            if (string.IsNullOrEmpty(args.newName) || args.newName == args.originalName)
            {
                args.acceptedRename = false;
                return;
            }

            OnRename(args);
        }

        /// <summary>
        ///     是否能多选
        /// </summary>
        /// <param name="item">选中组件</param>
        /// <returns>Ture:能 False:不能</returns>
        protected sealed override bool CanMultiSelect(TreeViewItem item) => false;

        /// <summary>
        ///     能否重新命名
        /// </summary>
        /// <param name="item">选中组件</param>
        /// <returns>Ture:能 False:不能</returns>
        protected sealed override bool CanRename(TreeViewItem item)
        {
            if (item is IGraphDraw draw) return draw.AllowRename && state.lastClickedID == item.id;
            return base.CanRename(item);
        }

        /// <summary>
        ///     获取重命名矩形
        /// </summary>
        /// <param name="rowRect">行的矩形</param>
        /// <param name="row">行数</param>
        /// <param name="item">选中组件</param>
        /// <returns></returns>
        protected sealed override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            if (item is IGraphDraw draw) return draw.GetRenameRect(rowRect, row);
            return base.GetRenameRect(rowRect, row, item);
        }

        /// <summary>
        ///     获取行高
        /// </summary>
        /// <param name="row">行数</param>
        /// <param name="item">选中组件</param>
        /// <returns>行高</returns>
        protected sealed override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            if (item is IGraphDraw draw) return draw.GetHeight();
            return base.GetCustomRowHeight(row, item);
        }

        /// <summary>
        ///     是否能改变展开状态
        /// </summary>
        /// <param name="item">选中组件</param>
        /// <returns>Ture:能 False:不能</returns>
        protected sealed override bool CanChangeExpandedState(TreeViewItem item)
        {
            if (item is IGraphDraw draw) return draw.AllowChangeExpandedState;
            return base.CanChangeExpandedState(item);
        }

        /// <summary>
        ///     组件是否匹配搜索
        /// </summary>
        /// <param name="item">组件</param>
        /// <param name="search">搜索内容</param>
        /// <returns>Ture:匹配 False:不匹配</returns>
        protected sealed override bool DoesItemMatchSearch(TreeViewItem item, string search)
            => item.displayName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        ///    多选
        /// </summary>
        /// <param name="id">ID</param>
        protected sealed override void DoubleClickedItem(int id) { }

        protected sealed override void KeyEvent()
        {
            if (state.selectedIDs.Count != 1) return;
            var code = Event.current.keyCode;
            if (code == KeyCode.None) return;
            switch (Event.current.type)
            {
                case EventType.KeyDown:
                {
                    OnEventKeyDown(code, FindItem(state.selectedIDs[0], rootItem));
                    break;
                }
                case EventType.KeyUp:
                {
                    OnEventKeyUp(code, FindItem(state.selectedIDs[0], rootItem));
                    break;
                }
            }
        }

        #region 拖拽事件

        /// <summary>
        ///   设置拖拽
        /// </summary>
        /// <param name="args"></param>
        protected sealed override void SetupDragAndDrop(SetupDragAndDropArgs args) { }

        /// <summary>
        ///   处理拖拽
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected sealed override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (!GUI.enabled) return DragAndDropVisualMode.None;
            if (DragAndDrop.activeControlID == ContentID)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                return DragAndDropVisualMode.None;
            }

            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.BetweenItems:
                    return DragAndDropVisualMode.Rejected;
                case DragAndDropPosition.OutsideItems:
                    return DragAndDropVisualMode.None;
            }

            if (DragAndDrop.GetGenericData(FullName) is CanStartDragArgs dragArgs && dragArgs.draggedItem != null)
            {
                if (args.parentItem == null)
                    return DragAndDropVisualMode.Rejected;
                if (dragArgs.draggedItem.id == args.parentItem.id)
                    return DragAndDropVisualMode.Rejected;

                if (args.performDrop)
                {
                    DragAndDrop.AcceptDrag();
                    OnDragSwapData(dragArgs.draggedItem.id - 1, args.parentItem.id - 1);
                    ReloadAndSelect(new[] { args.parentItem.id });
                    DragAndDrop.PrepareStartDrag();
                    HandleUtility.Repaint();
                }

                return DragAndDropVisualMode.Move;
            }

            return DragAndDropVisualMode.None;
        }

        /// <summary>
        ///  是否能开始拖拽
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected sealed override bool CanStartDrag(CanStartDragArgs args)
        {
            if (!GUI.enabled) return false;
            if (args.draggedItemIDs.Count != 1 || args.draggedItem == null) return false;

            if (!IsSelected(args.draggedItem.id))
            {
                OnSelection(args.draggedItem.id);
                OnSelectionChanged?.Invoke(args.draggedItem.id);
                return false;
            }

            DragAndDrop.activeControlID = ContentID;
            if (HasSelection() && isDragging)
            {
                DragAndDrop.SetGenericData(FullName, args);
                DragAndDrop.StartDrag(FullName);
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                Event.current?.Use();
                return true;
            }

            DragAndDrop.PrepareStartDrag();
            Event.current?.Use();
            return true;
        }

        #endregion

        #region 点击事件

        /// <inheritdoc />
        protected sealed override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count == 0) return;
            var id = selectedIds[0];
            OnSelection(id);
            OnSelectionChanged?.Invoke(id);
        }

        /// <inheritdoc />
        protected sealed override void SingleClickedItem(int id)
        {
            if (state.lastClickedID == id) return;
            SelectionClick(FindItem(id, rootItem), false);
            state.selectedIDs.Clear();
            state.selectedIDs.Add(id);
            state.lastClickedID = id;
            OnSelection(id);
            OnSelectionChanged?.Invoke(id);
            Event.current?.Use();
        }

        #endregion

        #region 右键事件

        /// <summary>
        ///     右键点击 空白区域
        /// </summary>
        protected sealed override void ContextClicked()
        {
            ReloadAndSelect();
            var menu = new GenericMenu();
            OnContextClicked(menu);
            if (menu.GetItemCount() == 0) return;
            menu.ShowAsContext();
            Event.current?.Use();
        }

        /// <summary>
        ///     右键点击 Item区域
        /// </summary>
        protected override void ContextClickedItem(int id)
        {
            ReloadAndSelect(id);
            var item = FindItem(id, rootItem);
            var menu = new GenericMenu();
            OnContextClicked(menu, item);
            if (menu.GetItemCount() == 0) return;
            menu.ShowAsContext();
            Event.current.Use();
        }

        #endregion

        #endregion
    }
}