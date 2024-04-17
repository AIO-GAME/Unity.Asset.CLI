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
    internal partial class ViewTreePackage : ViewTreeRowSingle
    {
        public static ViewTreePackage Create()
        {
            return new ViewTreePackage(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("资源包", 150, 100)
            })));
        }

        private AssetCollectRoot Config;

        private readonly GUIContent GC_MENU_ADD_PACKAGE = new GUIContent("添加 : 资源包");
        private readonly GUIContent GC_MENU_ADD_GROUP   = new GUIContent("添加 : 资源组");
        private readonly GUIContent GC_MENU_ALL_DISABLE = new GUIContent("全部 : 禁用");
        private readonly GUIContent GC_MENU_ALL_ENABLE  = new GUIContent("全部 : 启用");
        private readonly GUIContent GC_MENU_SET_PACKAGE = new GUIContent("设为 : 附加包");
        private readonly GUIContent GC_MENU_SET_MAIN    = new GUIContent("设为 : 默认包");
        private readonly GUIContent GC_MENU_ENABLE      = new GUIContent("启用");
        private readonly GUIContent GC_MENU_DISABLE     = new GUIContent("禁用");
        private readonly GUIContent GC_MENU_DELETE      = new GUIContent("删除");
        private readonly GUIContent GC_MENU_RENAME      = new GUIContent("重命名");

        private ViewTreePackage(TreeViewState state, MultiColumnHeader header) : base(state, header) { }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Config = AssetCollectRoot.GetOrCreate();
        }

        /// <inheritdoc />
        protected override void OnSorting(MultiColumnHeader header)
        {
            var temp = Config.CurrentPackage;
            Config.Sort(header.IsSortedAscending(header.sortedColumnIndex));
            var index = Config.IndexOf(temp);
            Config.CurrentPackageIndex = index;
            SetSelection(new[] { index + 1 });
        }

        /// <inheritdoc />
        protected override void OnSelection(int id)
        {
            Config.CurrentPackageIndex = id - 1;
        }

        /// <inheritdoc />
        protected override void OnBuildRows(TreeViewItem root)
        {
            var isDefault = false;
            for (var idxP = 0; idxP < Config.Count; idxP++)
            {
                root.AddChild(new TreeViewItemPackage(idxP + 1, Config[idxP]));
                if (!Config[idxP].Default) continue;

                if (isDefault) Config[idxP].Default = false;
                else isDefault                      = true;
            }

            if (!isDefault && Config.Count > 0) Config[0].Default = true;
        }

        #region 事件回调

        /// <inheritdoc />
        protected override void OnRename(RenameEndedArgs args)
        {
            if (Config.All(package => package.Name != args.newName))
            {
                var package = Config[args.itemID - 1];
                if (package.Name == args.originalName)
                    package.Name = args.newName;
                Reload();
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "名称已存在", "确定");
            }
        }

        /// <inheritdoc />
        protected override void OnDragSwapData(int from, int to)
        {
            Config.Swap(from, to);
        }


        #region 右键事件

        /// <inheritdoc />
        protected override void OnContextClicked(GenericMenu menu)
        {
            menu.AddItem(GC_MENU_ADD_PACKAGE, false, CreatePackage);
            menu.AddItem(GC_MENU_ALL_DISABLE, false, () => { Config.ForEach(package => package.Enable = false); });
            menu.AddItem(GC_MENU_ALL_ENABLE, false, () => { Config.ForEach(package => package.Enable  = true); });
        }

        /// <inheritdoc />
        protected override void OnContextClicked(GenericMenu menu, TreeViewItem item)
        {
            var index = item.id - 1;
            if (Config[index].Enable)
            {
                menu.AddItem(GC_MENU_ADD_GROUP, false, CreateGroup, item);
                if (!Config[index].Default) menu.AddItem(GC_MENU_SET_MAIN, false, ChangeDefPackage, item);
            }

            if (!Config[index].Default)
            {
                var contentEnable = Config[index].Enable ? GC_MENU_DISABLE : GC_MENU_ENABLE;
                menu.AddItem(contentEnable, false, ChangeEnable, item);
            }

            menu.AddItem(GC_MENU_RENAME, false, RenameGroupName, item);
            menu.AddItem(GC_MENU_DELETE, false, DeletePackages, item.id);
        }

        #endregion

        protected override void KeyEvent()
        {
            if (Event.current.keyCode == KeyCode.Delete
             && GetSelection().Count > 0) DeletePackages(GetSelection());
        }

        private void ChangeDefPackage(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                {
                    foreach (var package in Config.Where(package => package.Default))
                        package.Default = false;

                    Config[item.id - 1].Default = true;
                    ReloadAndSelect(item.id);
                    break;
                }
            }
        }

        private void ChangeEnable(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    var index = item.id - 1;
                    Config[index].Enable = !Config[index].Enable;
                    ReloadAndSelect(item.id);
                    break;
            }
        }

        private void CreatePackage()
        {
            var package = new AssetCollectPackage
            {
                Name   = GetOnlyName("Package"),
                Enable = true,
                Groups = Array.Empty<AssetCollectGroup>()
            };
            Config.Add(package);
            ReloadAndSelect(Config.Count);
        }

        private void CreateGroup(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                {
                    var newObj = new AssetCollectGroup
                    {
                        Name       = GetOnlyName("Group"),
                        Collectors = Array.Empty<AssetCollectItem>()
                    };
                    var index = item.id - 1;
                    Config[index].Groups = Config[index].Groups is null
                        ? new[] { newObj }
                        : Config[index].Groups.Add(newObj);

                    ReloadAndSelect(item.id);
                    break;
                }
            }
        }

        private void DeletePackages(object obj)
        {
            switch (obj)
            {
                case IList<int> indexes:
                {
                    if (EditorUtility.DisplayDialog(
                            "Delete Package",
                            "Are you sure you want to delete packages?",
                            "Yes", "No"))
                    {
                        foreach (var item in indexes) Config.RemoveAt(item - 1);
                        ReloadAndSelect();
                    }

                    GUI.FocusControl(null);
                    break;
                }
                case int index:
                {
                    if (EditorUtility.DisplayDialog(
                            "Delete Package",
                            $"Are you sure you want to delete {Config[index - 1]} packages?",
                            "Yes", "No"))
                    {
                        Config.RemoveAt(index - 1);
                        ReloadAndSelect();
                    }

                    GUI.FocusControl(null);
                    break;
                }
            }
        }

        private void RenameGroupName(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    BeginRename(item, 0.1f);
                    ReloadAndSelect(item.id);
                    break;
            }
        }

        #endregion

        #region 工具方法

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