#region

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    internal class TreeViewPackage : TreeViewRowSingle
    {
        private readonly GUIContent GC_MENU_ADD_GROUP   = new GUIContent("添加 : 资源组");
        private readonly GUIContent GC_MENU_ADD_PACKAGE = new GUIContent("添加 : 资源包");
        private readonly GUIContent GC_MENU_ALL_DISABLE = new GUIContent("全部 : 禁用");
        private readonly GUIContent GC_MENU_ALL_ENABLE  = new GUIContent("全部 : 启用");
        private readonly GUIContent GC_MENU_DELETE      = new GUIContent("修改 : 删除");
        private readonly GUIContent GC_MENU_DISABLE     = new GUIContent("修改 : 禁用");
        private readonly GUIContent GC_MENU_ENABLE      = new GUIContent("修改 : 启用");
        private readonly GUIContent GC_MENU_RENAME_DESC = new GUIContent("修改 : 描述");
        private readonly GUIContent GC_MENU_RENAME_NAME = new GUIContent("修改 : 包名");
        private readonly GUIContent GC_MENU_SET_MAIN    = new GUIContent("设为 : 默认包");

        private int RenameIndex { get; set; } = -1;

        private AssetCollectRoot Config;
        private TreeViewPackage(TreeViewState state, MultiColumnHeader header) : base(state, header) { }

        public static TreeViewPackage Create()
        {
            return new TreeViewPackage(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("资源包", 150, 100)
            })));
        }

        protected override void OnInitialize()
        {
            Config = AssetCollectRoot.GetOrCreate();
            foreach (var group in Config.Where(package => package != null).SelectMany(package => package))
                group?.Refresh();
        }

        protected override bool OnSorting(int columnIndex, bool ascending)
        {
            var temp = Config.CurrentPackage;
            Config.Sort(ascending);
            var index = Config.IndexOf(temp);
            Config.CurrentPackageIndex = index;
            SetSelection(new[] { index });
            return true;
        }

        protected override void OnSelection(int id) { Config.CurrentPackageIndex = id; }

        protected override void OnBuildRows(TreeViewItem root)
        {
            if (!Config) return;
            var isDefault = false;
            for (var idxP = 0; idxP < Config.Count; idxP++)
            {
                root.AddChild(new TreeViewItemPackage(idxP, Config[idxP]));
                if (!Config[idxP].Default) continue;

                if (isDefault) Config[idxP].Default = false;
                else isDefault                      = true;
            }

            if (!isDefault && Config.Count > 0) Config[0].Default = true;
        }

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

        #region 事件回调

        protected override void OnRename(RenameEndedArgs args)
        {
            switch (RenameIndex)
            {
                case -1: return;
                case 1: // 重命名包名
                {
                    if (Config.All(package => package.Name != args.newName))
                    {
                        var package = Config[args.itemID];
                        if (package.Name == args.originalName)
                            package.Name = args.newName;
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
                    var package = Config[args.itemID];
                    package.Description = args.newName;
                    Reload();
                    break;
                }
            }

            RenameIndex = -1;
        }

        protected override void OnDragSwapData(int from, int to) { Config.Swap(from, to); }

        #region 右键事件

        protected override void OnContextClicked(GenericMenu menu)
        {
            menu.AddItem(GC_MENU_ADD_PACKAGE, false, OP_CreatePackage);
            menu.AddItem(GC_MENU_ALL_DISABLE, false, () => { Config.ForEach(package => { package.Enable = package.Default; }); });
            menu.AddItem(GC_MENU_ALL_ENABLE, false, () => { Config.ForEach(package => package.Enable = true); });
        }

        protected override void OnContextClicked(GenericMenu menu, TreeViewItem item)
        {
            var index = item.id;
            if (Config.Count <= index) return;
            if (Config[index].Enable)
            {
                menu.AddItem(GC_MENU_ADD_GROUP, false, CreateGroup, item);
                menu.AddItem(GC_MENU_RENAME_NAME, false, OP_RenamePackageName, item);
                menu.AddItem(GC_MENU_RENAME_DESC, false, OP_RenameDescription, item);
            }

            if (Config[index].Default) return;
            if (Config[index].Enable)
            {
                menu.AddItem(GC_MENU_SET_MAIN, false, ChangeDefPackage, item);
                menu.AddItem(GC_MENU_DISABLE, false, ChangeEnable, item);
            }
            else
            {
                menu.AddItem(GC_MENU_ENABLE, false, ChangeEnable, item);
            }

            menu.AddItem(GC_MENU_DELETE, false, OP_DeletePackages, item.id);
        }

        #endregion

        protected override void OnEventKeyDown(Event evt, TreeViewItem item)
        {
            switch (evt.keyCode)
            {
                case KeyCode.F2:
                    OP_RenamePackageName(item);
                    break;
                case KeyCode.F3:
                    OP_RenameDescription(item);
                    break;
                case KeyCode.Delete:
                    OP_DeletePackages(item.id);
                    break;
                case KeyCode.DownArrow: // 数字键盘 下键
                {
                    var temp = item.id + 1;
                    ReloadAndSelect(temp >= Count ? 0 : temp);
                    break;
                }
                case KeyCode.UpArrow: // 数字键盘 上键
                {
                    var temp = item.id - 1;
                    ReloadAndSelect(temp < 0 ? Count - 1 : temp);
                    break;
                }
            }
        }

        private void ChangeDefPackage(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                {
                    foreach (var package in Config.Where(package => package.Default)) package.Default = false;
                    Config[item.id].Default = true;
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
                    var index = item.id;
                    Config[index].Enable = !Config[index].Enable;
                    ReloadAndSelect(index);
                    break;
            }
        }

        private void OP_CreatePackage()
        {
            Config.Add(new AssetCollectPackage
            {
                Name        = GetOnlyName("Package"),
                Enable      = true,
                Description = string.Empty,
                Groups      = Array.Empty<AssetCollectGroup>()
            });
            Reload();
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
                    var index = item.id;
                    Config[index].Groups = Config[index].Groups is null
                        ? new[] { newObj }
                        : Config[index].Groups.Add(newObj);

                    ReloadAndSelect(item.id);
                    break;
                }
            }
        }

        private void OP_DeletePackages(object obj)
        {
            switch (obj)
            {
                case int index:
                {
                    if (Config[index].Default)
                    {
                        EditorUtility.DisplayDialog("错误", "默认包不能删除", "确定");
                        return;
                    }

                    if (EditorUtility.DisplayDialog("删除",
                                                    $"确定是否删除 {Config[index].Name} 资源包? 【删除后不可恢复】",
                                                    "确定", "取消"))
                    {
                        Config.RemoveAt(index);
                        ReloadAndSelect(--Config.CurrentPackageIndex);
                    }

                    GUI.FocusControl(null);
                    break;
                }
            }
        }

        private void OP_RenamePackageName(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    RenameIndex      = 1;
                    item.displayName = Config[item.id].Name;
                    if (BeginRename(item, 0.1f))
                    {
                        ReloadAndSelect(item.id);
                        GUI.FocusControl(null);
                        HandleUtility.Repaint();
                    }

                    break;
            }
        }

        private void OP_RenameDescription(object obj)
        {
            switch (obj)
            {
                case TreeViewItem item:
                    RenameIndex      = 2;
                    item.displayName = Config[item.id].Description;
                    if (BeginRename(item, 0.1f))
                    {
                        ReloadAndSelect(item.id);
                        GUI.FocusControl(null);
                        HandleUtility.Repaint();
                    }

                    break;
            }
        }

        protected override void OnDraw(Rect rect)
        {
            if (!Config) return;
            if (Config.Count != 0) return;
            if (GELayout.Button(rect.center, new Vector2(100, 30), "创建资源包"))
            {
                OP_CreatePackage();
            }
        }

        #endregion
    }
}