#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    /// <summary>
    ///     SingleRowTreeEditor
    /// </summary>
    public abstract class ViewTreeRowSingle : TreeViewBasics
    {
        public event Action<int> OnSelectionChanged;
        protected int            ContentID;

        protected void InvokeSelectionChanged(int id) => OnSelectionChanged?.Invoke(id);

        private readonly string FullName;

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
        ///     初始化
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        ///     构建行
        /// </summary>
        /// <param name="root">根节点</param>
        protected abstract void OnBuildRows(TreeViewItem root);

        /// <summary>
        ///     重命名完成
        /// </summary>
        /// <param name="args">重命名参数</param>
        protected virtual void OnRename(RenameEndedArgs args)
        {
        }

        /// <summary>
        ///     排序
        /// </summary>
        /// <param name="header">多列头</param>
        protected virtual void OnSorting(MultiColumnHeader header)
        {
        }

        /// <summary>
        ///     绘制
        /// </summary>
        protected virtual void OnDraw(Rect rect)
        {
        }

        /// <summary>
        ///     选择
        /// </summary>
        /// <param name="id">ID</param>
        protected virtual void OnSelection(int id)
        {
        }

        /// <summary>
        ///     拖拽交换数据
        /// </summary>
        /// <param name="from">源</param>
        /// <param name="to">目标</param>
        protected virtual void OnDragSwapData(int from, int to)
        {
        }

        /// <summary>
        ///     右键点击空白区域
        /// </summary>
        /// <param name="menu">菜单</param>
        protected virtual void OnContextClicked(GenericMenu menu)
        {
        }

        /// <summary>
        ///     右键点击Item区域
        /// </summary>
        /// <param name="menu">菜单</param>
        /// <param name="item">选中组件</param>
        protected virtual void OnContextClicked(GenericMenu menu, TreeViewItem item)
        {
        }

        /// <summary>
        ///     按键按下
        /// </summary>
        /// <param name="keyCode"> 按键 </param>
        /// <param name="item"> 选中组件 </param>
        protected virtual void OnEventKeyDown(KeyCode keyCode, TreeViewItem item)
        {
        }

        /// <summary>
        ///     按键抬起
        /// </summary>
        /// <param name="keyCode"> 按键 </param>
        /// <param name="item"> 选中组件 </param>
        protected virtual void OnEventKeyUp(KeyCode keyCode, TreeViewItem item)
        {
        }

        #endregion

        #region 工具函数

        private void SortingChanged(MultiColumnHeader header)
        {
            OnSorting(header);
            Reload();
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
            switch (args.item)
            {
                case ITVItemDraw item:
                {
                    var count = args.GetNumVisibleColumns() - 1;
                    for (var i = 0; i <= count; i++)
                    {
                        EditorGUI.BeginChangeCheck();
                        var cellRect = args.GetCellRect(i);
                        CenterRectUsingSingleLineHeight(ref cellRect);
                        try
                        {
                            var cast = Cast(args);
                            item.OnDraw(cellRect, i, ref cast);
                        }
                        catch (Exception)
                        {
                            GUIUtility.ExitGUI();
                        }

                        if (i == count)
                        {
                            cellRect.Set(cellRect.width + cellRect.x + count - 1, args.rowRect.y, 1,
                                         args.rowRect.height - 1);
                            EditorGUI.DrawRect(cellRect, ColorLine);
                        }

                        if (EditorGUI.EndChangeCheck()) Reload();
                    }

                    break;
                }
            }
        }

        public sealed override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            ContentID = GUIUtility.GetControlID(FocusType.Passive, rect);
            multiColumnHeader.state.AutoWidth(rect.width);

            OnDraw(rect);
            if (Event.current.type == EventType.MouseDown
             && Event.current.button == 0
             && rect.Contains(Event.current.mousePosition)
               ) SetSelection(state.selectedIDs, TreeViewSelectionOptions.FireSelectionChanged);

            rect.height = 26;
            EditorGUI.DrawRect(rect, ColorLine);
        }

        /// <summary>
        ///     更改名称完成
        /// </summary>
        protected sealed override void RenameEnded(RenameEndedArgs args)
        {
            if (!args.acceptedRename ||
                string.IsNullOrEmpty(args.newName) ||
                args.newName == args.originalName
               ) return;
            OnRename(args);
            EndRename();
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
            if (item is ITVItemDraw draw) return draw.AllowRename && state.lastClickedID == item.id;
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
            if (item is ITVItemDraw draw) return draw.GetRenameRect(rowRect, row);
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
            if (item is ITVItemDraw draw) return draw.GetHeight();
            return base.GetCustomRowHeight(row, item);
        }

        /// <summary>
        ///     是否能改变展开状态
        /// </summary>
        /// <param name="item">选中组件</param>
        /// <returns>Ture:能 False:不能</returns>
        protected sealed override bool CanChangeExpandedState(TreeViewItem item)
        {
            if (item is ITVItemDraw draw) return draw.AllowChangeExpandedState;
            return base.CanChangeExpandedState(item);
        }

        /// <summary>
        ///     组件是否匹配搜索
        /// </summary>
        /// <param name="item">组件</param>
        /// <param name="search">搜索内容</param>
        /// <returns>Ture:匹配 False:不匹配</returns>
        protected sealed override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            if (item is ITVItemDraw draw) return draw.MatchSearch(search);
            return base.DoesItemMatchSearch(item, search);
        }

        /// <summary>
        ///    搜索改变
        /// </summary>
        /// <param name="newSearch">新搜索</param>
        protected override void SearchChanged(string newSearch)
        {
            if (string.IsNullOrEmpty(newSearch))
            {
                Reload();
                return;
            }

            var search = newSearch.ToLower();
            rootItem.children = rootItem.children.Where(item => DoesItemMatchSearch(item, search)).ToList();
            SetupDepthsFromParentsAndChildren(rootItem);
            BuildRows(rootItem);
        }

        /// <summary>
        ///     多选
        /// </summary>
        /// <param name="id">ID</param>
        protected sealed override void DoubleClickedItem(int id)
        {
        }

        protected sealed override void KeyEvent()
        {
            if (state.selectedIDs.Count != 1) return;
            var code = Event.current.keyCode;
            if (code == KeyCode.None) return;
            switch (Event.current.type)
            {
                case EventType.KeyDown:
                {
                    OnEventKeyDown(code, rootItem.children[state.selectedIDs[0]]);
                    Event.current.Use();
                    break;
                }
                case EventType.KeyUp:
                {
                    OnEventKeyUp(code, rootItem.children[state.selectedIDs[0]]);
                    Event.current.Use();
                    break;
                }
            }
        }

        #region 拖拽事件

        /// <summary>
        ///     设置拖拽
        /// </summary>
        /// <param name="args"></param>
        protected sealed override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
        }

        /// <summary>
        ///     处理拖拽
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
                    OnDragSwapData(dragArgs.draggedItem.id, args.parentItem.id);
                    ReloadAndSelect(new[] { args.parentItem.id });
                    DragAndDrop.PrepareStartDrag();
                    HandleUtility.Repaint();
                }

                return DragAndDropVisualMode.Move;
            }

            return DragAndDropVisualMode.None;
        }

        protected bool AllowDrag { get; set; } = true;

        /// <summary>
        ///     是否能开始拖拽
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected sealed override bool CanStartDrag(CanStartDragArgs args)
        {
            if (!AllowDrag || !GUI.enabled) return false;
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
            if (rootItem.children.Count > selectedIds.Count)
                SetSelection(selectedIds, TreeViewSelectionOptions.RevealAndFrame);
            OnSelection(id);
            OnSelectionChanged?.Invoke(id);
        }

        /// <inheritdoc />
        protected sealed override void SingleClickedItem(int id)
        {
            if (state.lastClickedID == id) return;
            SelectionClick(rootItem.children[id], false);
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
        protected sealed override void ContextClickedItem(int id)
        {
            ReloadAndSelect(id);
            var menu = new GenericMenu();
            OnContextClicked(menu, rootItem.children[id]);
            if (menu.GetItemCount() == 0) return;
            menu.ShowAsContext();
            Event.current.Use();
        }

        #endregion

        #endregion
    }
}