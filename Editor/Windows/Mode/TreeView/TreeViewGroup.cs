#region

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    internal class ViewTreeGroup : ViewTreeRowSingle
    {
        private readonly GUIContent GC_MENU_ADD         = new GUIContent("添加 : 收集器");
        private readonly GUIContent GC_MENU_ADD_GROUP   = new GUIContent("添加 : 资源组");
        private readonly GUIContent GC_MENU_ALL_DISABLE = new GUIContent("全部 : 禁用");
        private readonly GUIContent GC_MENU_ALL_ENABLE  = new GUIContent("全部 : 启用");
        private readonly GUIContent GC_MENU_DELETE      = new GUIContent("修改 : 删除");
        private readonly GUIContent GC_MENU_DISABLE     = new GUIContent("修改 : 禁用");
        private readonly GUIContent GC_MENU_ENABLE      = new GUIContent("修改 : 启用");
        private readonly GUIContent GC_MENU_RENAME_DESC = new GUIContent("修改 : 描述");
        private readonly GUIContent GC_MENU_RENAME_NAME = new GUIContent("修改 : 重命名");

        private AssetCollectRoot Config;
        private int              RenameIndex = -1;
        private ViewTreeGroup(TreeViewState state, MultiColumnHeader header) : base(state, header) { }

        public static ViewTreeGroup Create()
        {
            return new ViewTreeGroup(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("资源组", 150, 100)
            })));
        }

        protected override bool CanBeParent(TreeViewItem item) => true;

        protected override void OnInitialize()
        {
            Config = AssetCollectRoot.GetOrCreate();
        }

        protected override void OnDraw(Rect rect)
        {
            if (Config is null) return;
            if (Config.Count == 0) return;
            if (Config.CurrentPackage is null) return;
            if (Config.CurrentPackage.Count != 0) return;
            if (GELayout.Button(rect.center, new Vector2(100, 30), "创建资源组")) OP_CreateGroup();
        }

        protected override void OnEventKeyDown(KeyCode keyCode, TreeViewItem item)
        {
            switch (keyCode)
            {
                case KeyCode.F2:
                    OP_RenameGroupName(item);
                    break;
                case KeyCode.F3:
                    OP_RenameDescription(item);
                    break;
                case KeyCode.Delete:
                    OP_DeleteGroup(item);
                    break;
            }
        }

        protected override void OnRename(RenameEndedArgs args)
        {
            switch (RenameIndex)
            {
                case -1: return;
                case 1: // 重命名包名
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

                    break;
                }
                case 2: // 重命名描述
                {
                    Config.CurrentPackage[args.itemID - 1].Description = args.newName;
                    Reload();
                    break;
                }
            }

            RenameIndex = -1;
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
            for (var idxG = 0; idxG < Config.CurrentPackage?.Count; idxG++)
                root.AddChild(new TreeViewItemGroup(idxG + 1, Config.CurrentPackage[idxG]));
        }

        protected override void OnSelection(int id)
        {
            Config.CurrentGroupIndex = id - 1;
        }

        protected override void OnContextClicked(GenericMenu menu)
        {
            if (!Config.CurrentPackage.Enable) return;
            menu.AddItem(GC_MENU_ADD_GROUP, false, OP_CreateGroup);
            menu.AddItem(GC_MENU_ALL_DISABLE, false, () => { Config.CurrentPackage.ForEach(group => group.Enable = false); });
            menu.AddItem(GC_MENU_ALL_ENABLE, false, () => { Config.CurrentPackage.ForEach(group => group.Enable  = true); });
        }

        protected override void OnContextClicked(GenericMenu menu, TreeViewItem item)
        {
            if (!Config.CurrentPackage.Enable) return;
            if (Config.CurrentPackage[item.id - 1].Enable)
            {
                menu.AddItem(GC_MENU_ADD, false, OP_AddCollect, item);
                menu.AddItem(GC_MENU_DISABLE, false, OP_DisableGroup, item);
                menu.AddItem(GC_MENU_RENAME_NAME, false, OP_RenameGroupName, item);
                menu.AddItem(GC_MENU_RENAME_DESC, false, OP_RenameDescription, item);
            }
            else
            {
                menu.AddItem(GC_MENU_ENABLE, false, OP_EnableGroup, item);
            }

            menu.AddItem(GC_MENU_DELETE, false, OP_DeleteGroup, item);
        }


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
                    if (EditorUtility.DisplayDialog(
                            "删除资源组",
                            $"是否删除资源组 : {Config.CurrentPackage[item.id - 1].Name}",
                            "确定", "取消"))
                    {
                        Config.CurrentPackage.RemoveAt(item.id - 1);
                        ReloadAndSelect(Config.CurrentGroupIndex--);
                    }


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
            ReloadAndSelect(Config.CurrentPackage.Count);
        }

        private void OP_RenameGroupName(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    item.displayName = Config.CurrentPackage[item.id - 1].Name;
                    RenameIndex      = 1;
                    BeginRename(item, 0.1f);
                    ReloadAndSelect(item.id);
                    break;
            }
        }

        private void OP_RenameDescription(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    RenameIndex      = 2;
                    item.displayName = Config.CurrentPackage[item.id - 1].Description;
                    if (BeginRename(item, 0.1f))
                    {
                        ReloadAndSelect(item.id);
                        GUI.FocusControl(null);
                        HandleUtility.Repaint();
                    }

                    break;
            }
        }

        #region 工具函数

        private string GetOnlyName(string defName)
        {
            var index = Config.CurrentPackage.Count;
            for (var i = 0; i < 10; i++)
            {
                var name = $"{defName}_{index + i}";
                if (Config.CurrentPackage.All(group => group.Name != name)) return name;
            }

            return $"{defName}_{DateTimeOffset.Now.ToUnixTimeSeconds()}";
        }

        #endregion
    }
}