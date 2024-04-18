#region

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    /// <summary>
    ///     TreeEditorGroup
    /// </summary>
    public partial class ViewTreeGroup : ViewTreeRowSingle
    {
        private ViewTreeGroup(TreeViewState state, MultiColumnHeader header) : base(state, header) { }

        public static ViewTreeGroup Create()
        {
            return new ViewTreeGroup(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("资源组", 150, 100)
            })));
        }

        private AssetCollectRoot Config;

        private readonly GUIContent GC_MENU_ADD         = new GUIContent("添加 : 收集器");
        private readonly GUIContent GC_MENU_ADD_GROUP   = new GUIContent("添加 : 资源组");
        private readonly GUIContent GC_MENU_ALL_DISABLE = new GUIContent("全部 : 禁用");
        private readonly GUIContent GC_MENU_ALL_ENABLE  = new GUIContent("全部 : 启用");
        private readonly GUIContent GC_MENU_ENABLE      = new GUIContent("启用");
        private readonly GUIContent GC_MENU_DISABLE     = new GUIContent("禁用");
        private readonly GUIContent GC_MENU_DELETE      = new GUIContent("删除");
        private readonly GUIContent GC_MENU_RENAME      = new GUIContent("重命名");


        /// <inheritdoc />
        protected override bool CanBeParent(TreeViewItem item) => true;

        protected override void OnInitialize()
        {
            Config = AssetCollectRoot.GetOrCreate();
        }

        /// <inheritdoc />
        protected override void OnEventKeyDown(KeyCode keyCode, TreeViewItem item)
        {
            switch (keyCode)
            {
                case KeyCode.F2:
                    OP_RenameGroupName(item);
                    break;
                case KeyCode.Delete:
                    OP_DeleteGroup(item);
                    break;
            }
        }

        protected override void OnRename(RenameEndedArgs args)
        {
            if (Config.CurrentPackage.All(group => group.Name != args.newName))
            {
                var group = Config.CurrentPackage[args.itemID - 1];
                if (group.Name == args.originalName)
                    group.Name = args.newName;
                Reload();
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "名称已存在", "确定");
            }
        }

        protected override void OnSorting(MultiColumnHeader header)
        {
            var tempGroup = Config.CurrentGroup;
            Config.CurrentPackage.Sort(header.IsSortedAscending(header.sortedColumnIndex));
            var index = Config.CurrentPackage.IndexOf(tempGroup);
            Config.CurrentGroupIndex = index;
            SetSelection(new[] { index + 1 });
        }

        protected override void OnBuildRows(TreeViewItem root)
        {
            for (var idxG = 0; idxG < Config.CurrentPackage.Count; idxG++) root.AddChild(new TreeViewItemGroup(idxG + 1, Config.CurrentPackage[idxG]));
        }

        protected override void OnSelection(int id)
        {
            Config.CurrentGroupIndex = id - 1;
        }

        protected override void OnContextClicked(GenericMenu menu)
        {
            menu.AddItem(GC_MENU_ADD_GROUP, false, OP_CreateGroup);
            menu.AddItem(GC_MENU_ALL_DISABLE, false, () => { Config.CurrentPackage.ForEach(group => group.Enable = false); });
            menu.AddItem(GC_MENU_ALL_ENABLE, false, () => { Config.CurrentPackage.ForEach(group => group.Enable  = true); });
        }

        protected override void OnContextClicked(GenericMenu menu, TreeViewItem item)
        {
            menu.AddItem(GC_MENU_ADD, false, OP_AddCollect, item);
            menu.AddItem(GC_MENU_RENAME, false, OP_RenameGroupName, item);

            if (Config.CurrentPackage[item.id - 1].Enable)
                menu.AddItem(GC_MENU_DISABLE, false, OP_DisableGroup, item);
            else menu.AddItem(GC_MENU_ENABLE, false, OP_EnableGroup, item);

            menu.AddItem(GC_MENU_DELETE, false, OP_DeleteGroup, item);
        }

        /// <inheritdoc />
        protected override void OnDragSwapData(int from, int to)
        {
            (Config.CurrentPackage[from], Config.CurrentPackage[to]) = (Config.CurrentPackage[to], Config.CurrentPackage[from]);
        }

        private void OP_AddCollect(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    Config.CurrentPackage[item.id - 1].Add(new AssetCollectItem());
                    ReloadAndSelect(item.id);
                    break;
            }
        }

        private void OP_EnableGroup(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    Config.CurrentPackage[item.id - 1].Enable = true;
                    ReloadAndSelect(item.id);
                    break;
            }
        }

        private void OP_DisableGroup(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    Config.CurrentPackage[item.id - 1].Enable = false;
                    ReloadAndSelect(item.id);
                    break;
            }
        }

        private void OP_DeleteGroup(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    Config.CurrentPackage.RemoveAt(item.id - 1);
                    ReloadAndSelect(Config.Count);
                    break;
            }
        }

        private void OP_CreateGroup()
        {
            Config.CurrentPackage.Add(new AssetCollectGroup
            {
                Name        = GetOnlyName("Group"),
                Description = "新建资源组",
                Collectors  = Array.Empty<AssetCollectItem>()
            });
            ReloadAndSelect(Config.Count);
        }

        private void OP_RenameGroupName(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    BeginRename(item, 0.1f);
                    ReloadAndSelect(item.id);
                    break;
            }
        }

        #region 工具函数

        private string GetOnlyName(string defName)
        {
            var index = Config.Count;
            for (var i = 0; i < 10; i++)
            {
                var name = $"{defName}_{index + i}";
                if (Config.All(package => package.Name != name)) return name;
            }

            return $"{defName}_{DateTimeOffset.Now.ToUnixTimeSeconds()}";
        }

        #endregion
    }
}