using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// PackageTreeDraw
    /// </summary>
    internal class PackageTreeEditor : TreeView
    {
        public static PackageTreeEditor Create()
        {
            var header = new MultiColumnHeader(CreateDefaultMultiColumnHeaderState());
            return new PackageTreeEditor(new TreeViewState(), header);
        }

        public static PackageTreeEditor Create(TreeViewState state, MultiColumnHeaderState headerState)
        {
            var header = new MultiColumnHeader(headerState) { canSort = true };
            return new PackageTreeEditor(state, header);
        }

        private static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            return new MultiColumnHeaderState(GetColumns());
        }

        private static MultiColumnHeaderState.Column GetMultiColumnHeaderColumn(
            string name,
            int    width = 100,
            int    min   = 80,
            int    max   = 200
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
            allowToggleVisibility = false,
        };

        private static MultiColumnHeaderState.Column[] GetColumns()
        {
            return new[] { GetMultiColumnHeaderColumn("资源包") };
        }

        /// <summary>
        /// 头部状态
        /// </summary>
        public MultiColumnHeaderState HeaderState { get; }

        private TreeViewState State { get; }

        private PackageTreeEditor(TreeViewState state, MultiColumnHeader header) : base(state, header)
        {
            State                         =  state;
            showBorder                    =  true;
            HeaderState                   =  header.state;
            header.sortingChanged         += SortingChanged;
            showAlternatingRowBackgrounds =  true;
            Refresh();
            Reload();
        }

        private void SortingChanged(MultiColumnHeader header)
        {
            if (header.sortedColumnIndex == -1) return;
            Config.Packages = header.IsSortedAscending(header.sortedColumnIndex)
                ? Config.Packages.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture))
                : Config.Packages.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture));
            Config.Save();
            Reload();
        }

        private AssetCollectRoot      Config;
        private AssetCollectPackage[] Packages => Config.Packages;
        private TreeViewItem          mRoot;

        protected override TreeViewItem BuildRoot()
        {
            if (mRoot is null)
            {
                mRoot = new TreeViewItem
                {
                    id       = -1,
                    depth    = -1,
                    children = new List<TreeViewItem>()
                };
            }
            else mRoot.children.Clear();

            Config = AssetCollectRoot.GetOrCreate();
            if (Config.Packages is null)
                Config.Packages = Array.Empty<AssetCollectPackage>();

            var isDefault = false;
            for (var index = 0; index < Config.Packages.Length; index++)
            {
                mRoot.AddChild(new TreeViewItemPackage(index + 1, Config.Packages[index]));
                if (!Config.Packages[index].Default) continue;
                if (isDefault)
                    Config.Packages[index].Default = false;
                else
                    isDefault = true;
            }

            if (!isDefault && Config.Packages.Length > 0) Config.Packages[0].Default = true;

            return mRoot;
        }

        internal void Refresh()
        {
            SelectionChanged(GetSelection());
        }

        private void ReloadAndSelect(IList<int> hashCodes)
        {
            Reload();
            SetSelection(hashCodes, TreeViewSelectionOptions.RevealAndFrame);
            SelectionChanged(hashCodes);
        }

        #region 绘制

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            HeaderState.AutoWidth(rect.width);
            if (Event.current.type == EventType.MouseDown
             && Event.current.button == 0
             && rect.Contains(Event.current.mousePosition)
               ) SetSelection(State.selectedIDs, TreeViewSelectionOptions.FireSelectionChanged);
        }

        /// <summary>
        /// 绘制行
        /// </summary>
        /// <param name="args"></param>
        protected override void RowGUI(RowGUIArgs args)
        {
            for (var i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                if (!(args.item is TreeViewItemPackage item)) continue;
                if (item.Package.Enable) CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
                else
                {
                    var oldColor = GUI.color;
                    GUI.color = Color.gray;
                    CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
                    GUI.color = oldColor;
                }
            }
        }

        private void CellGUI(Rect cellRect, TreeViewItemPackage item, int column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            if (column != 0) return;
            var iconRect = new Rect(cellRect.x + 4, cellRect.y + 1, cellRect.height - 2, cellRect.height - 2);
            GUI.DrawTexture(iconRect, item.icon, ScaleMode.ScaleToFit);
            var nameRect = new Rect(cellRect.x + iconRect.xMax + 1, cellRect.y,
                                    cellRect.width - iconRect.width, cellRect.height);
            switch (item.Package.Default)
            {
                case true:
                    DefaultGUI.BoldLabel(nameRect, item.displayName, args.selected, args.focused);
                    break;
                default:
                    DefaultGUI.Label(nameRect, item.displayName, args.selected, args.focused);
                    break;
            }
        }

        #endregion

        #region 接口实现

        /// <summary>
        /// 是否能多选
        /// </summary>
        /// <param name="item">选中组件</param>
        /// <returns>Ture:能 False:不能</returns>
        protected override bool CanMultiSelect(TreeViewItem item) => false;

        /// <summary>
        /// 能否重新命名
        /// </summary>
        /// <param name="item">选中组件</param>
        /// <returns>Ture:能 False:不能</returns>
        protected override bool CanRename(TreeViewItem item) => item.displayName.Length > 0;

        /// <summary>
        /// 获取重命名矩形
        /// </summary>
        /// <param name="rowRect">行的矩形</param>
        /// <param name="row">行数</param>
        /// <param name="item">选中组件</param>
        /// <returns></returns>
        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item) => rowRect;

        /// <summary>
        /// 获取行高
        /// </summary>
        /// <param name="row">行数</param>
        /// <param name="item">选中组件</param>
        /// <returns>行高</returns>
        protected override float GetCustomRowHeight(int row, TreeViewItem item) => 30;

        #endregion

        #region 事件回调

        /// <summary>
        /// 更改名称完成
        /// </summary>
        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (string.IsNullOrEmpty(args.newName) || args.newName == args.originalName)
            {
                args.acceptedRename = false;
                return;
            }

            if (Packages.All(package => package.Name != args.newName))
            {
                var package = Packages[args.itemID - 1];
                if (package.Name == args.originalName)
                    package.Name = args.newName;
                Reload();
            }
            else EditorUtility.DisplayDialog("错误", "名称已存在", "确定");
        }

        #endregion

        #region 菜单事件

        /// <summary>
        /// 点击的时候
        /// </summary>
        protected override void ContextClicked()
        {
            var menu = new GenericMenu();
            menu.AddItem(GC_MENU_ADD_PACKAGE, false, CreatePackage);
            menu.ShowAsContext();

            State.selectedIDs   = new List<int>();
            State.lastClickedID = -1;
            Event.current.Use();
        }

        private readonly GUIContent GC_MENU_ADD_PACKAGE      = new GUIContent("添加包");
        private readonly GUIContent GC_MENU_ADD_GROUP        = new GUIContent("添加组");
        private readonly GUIContent GC_MENU_SET_PACKAGE      = new GUIContent("设为附加包");
        private readonly GUIContent GC_MENU_SET_MAIN_PACKAGE = new GUIContent("设为默认包");
        private readonly GUIContent GC_MENU_ENABLE           = new GUIContent("启用");
        private readonly GUIContent GC_MENU_DISABLE          = new GUIContent("禁用");
        private readonly GUIContent GC_MENU_DELETE           = new GUIContent("删除");
        private readonly GUIContent GC_MENU_RENAME           = new GUIContent("重命名");

        #region Drag

        /// <inheritdoc />
        protected override void SetupDragAndDrop(SetupDragAndDropArgs args) { }

        /// <inheritdoc />
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (DragAndDrop.GetGenericData("Package") is CanStartDragArgs dragArgs && dragArgs.draggedItemIDs.Count > 0)
            {
                if (args.insertAtIndex <= 0) args.insertAtIndex = 1;
                var swapA = dragArgs.draggedItemIDs[0] - 1;
                var swapB = args.insertAtIndex - 1;
                if (swapA == swapB)
                {
                    DragAndDrop.SetGenericData("Package", null);
                    DragAndDrop.PrepareStartDrag();
                    return DragAndDropVisualMode.Rejected;
                }

                if (args.performDrop)
                {
                    Debug.Log($"HandleDragAndDrop {swapA} {swapB}");
                    Packages.Swap(swapA, swapB);
                    DragAndDrop.AcceptDrag();
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.SetGenericData("Package", null);
                    ReloadAndSelect(new[] { args.insertAtIndex });
                    dragArgs.draggedItemIDs.Clear();
                }

                return DragAndDropVisualMode.Move;
            }

            DragAndDrop.PrepareStartDrag();
            return DragAndDropVisualMode.None;
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            if (args.draggedItemIDs.Count <= 0) return false;
            DragAndDrop.SetGenericData("Package", args);
            DragAndDrop.StartDrag("Package");
            return true;
        }

        #endregion


        protected override void ContextClickedItem(int id)
        {
            State.selectedIDs   = new List<int> { id };
            State.lastClickedID = id;
            lock (Packages)
            {
                var index = id - 1;
                var menu = new GenericMenu();
                if (Packages[index].Enable)
                {
                    menu.AddItem(GC_MENU_ADD_GROUP, false, CreateGroup, index);
                    if (!Packages[index].Default)
                    {
                        menu.AddItem(GC_MENU_SET_MAIN_PACKAGE, false, ChangeDefPackage, index);
                    }
                }

                if (!Packages[index].Default)
                {
                    var contentEnable = Packages[index].Enable ? GC_MENU_DISABLE : GC_MENU_ENABLE;
                    menu.AddItem(contentEnable, false, ChangeEnable, index);
                }

                menu.AddItem(GC_MENU_RENAME, false, RenameGroupName, index);
                menu.AddItem(GC_MENU_DELETE, false, DeleteGroups, index);
                menu.ShowAsContext();

                Event.current.Use();
            }
        }

        protected override void KeyEvent()
        {
            if (Event.current.keyCode == KeyCode.Delete
             && GetSelection().Count > 0
               ) DeleteGroups(GetSelection());
        }

        private void ChangeDefPackage(object obj)
        {
            if (obj is int index)
            {
                foreach (var package in Packages)
                {
                    if (package.Default) package.Default = false;
                }

                Packages[index].Default = true;
            }
        }

        private void ChangeEnable(object obj)
        {
            if (obj is IList<int> indexes)
            {
                foreach (var item in indexes)
                {
                    var index = item - 1;
                    if (Packages.Length <= index || index < 0) continue;
                    Packages[index].Enable = !Packages[index].Enable;
                }

                ReloadAndSelect(Array.Empty<int>());
            }
            else if (obj is int index)
            {
                Packages[index].Enable = !Packages[index].Enable;
            }
        }

        private void CreatePackage()
        {
            var package = new AssetCollectPackage
            {
                Name   = GetOnlyName(),
                Enable = true,
                Groups = Array.Empty<AssetCollectGroup>()
            };
            Packages.Add(package);
            ReloadAndSelect(new[] { Packages.Length });
        }

        private void CreateGroup(object obj)
        {
            if (obj is int index)
            {
                if (Packages[index].Groups is null)
                {
                    Packages[index].Groups = Array.Empty<AssetCollectGroup>();
                }

                Packages[index].Groups = Packages[index].Groups.Add(new AssetCollectGroup
                {
                    Name       = GetOnlyName(),
                    Collectors = Array.Empty<AssetCollectItem>()
                });
                ReloadAndSelect(Array.Empty<int>());
            }
        }

        private void DeleteGroups(object obj)
        {
            if (obj is IList<int> indexes)
            {
                foreach (var item in indexes)
                {
                    Packages.RemoveAt(item - 1);
                }

                ReloadAndSelect(Array.Empty<int>());
            }
            else if (obj is int index)
            {
                Packages.RemoveAt(index);
                ReloadAndSelect(Array.Empty<int>());
            }
        }

        private void RenameGroupName(object obj)
        {
            if (obj is int index)
            {
                BeginRename(FindItem(index + 1, rootItem), 0.1f);
            }
        }

        #endregion

        #region 工具方法

        private string GetOnlyName()
        {
            var index = Packages.Length;
            for (var i = 0; i < 10; i++)
            {
                var name = $"Package_{index + i}";
                if (Packages.All(package => package.Name != name)) return name;
            }

            return $"Package_{DateTimeOffset.Now.ToUnixTimeSeconds()}";
        }

        #endregion
    }
}