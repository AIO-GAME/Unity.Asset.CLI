#region

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    public class ViewTreeCollect : ViewTreeRowSingle
    {
        private readonly GUIContent GC_MENU_ADD         = new GUIContent("添加 : 收集器");
        private readonly GUIContent GC_MENU_ALL_DISABLE = new GUIContent("全部 : 禁用");
        private readonly GUIContent GC_MENU_ALL_ENABLE  = new GUIContent("全部 : 启用");
        private readonly GUIContent GC_MENU_SAVE        = new GUIContent("全部 : 保存");
        private readonly GUIContent GC_MENU_ALL_UNFOLD  = new GUIContent("全部 : 展开");
        private readonly GUIContent GC_MENU_ALL_FOLD    = new GUIContent("全部 : 折叠");
        private readonly GUIContent GC_MENU_ENABLE      = new GUIContent("修改 : 启用");
        private readonly GUIContent GC_MENU_DISABLE     = new GUIContent("修改 : 禁用");
        private readonly GUIContent GC_MENU_DELETE      = new GUIContent("修改 : 删除");
        private readonly GUIContent GC_MENU_UNFOLD      = new GUIContent("修改 : 展开");
        private readonly GUIContent GC_MENU_FOLD        = new GUIContent("修改 : 折叠");

        private ViewTreeCollect(TreeViewState state, MultiColumnHeader header) : base(state, header)
        {
            showAlternatingRowBackgrounds = false;
        }

        public static ViewTreeCollect Create(float width = 100, float min = 80, float max = 200)
        {
            return new ViewTreeCollect(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("收集器", width, min, max)
            })));
        }

        private AssetCollectRoot Config;

        protected override void OnInitialize()
        {
            Config = AssetCollectRoot.GetOrCreate();
        }

        protected override void OnSorting(MultiColumnHeader header)
        {
            var currentCollect = Config.CurrentCollect;
            Config.CurrentGroup.Sort(header.IsSortedAscending(header.sortedColumnIndex));
            var index = Config.CurrentGroup.IndexOf(currentCollect);
            Config.CurrentCollectIndex = index;
            SetSelection(new[] { index });
        }

        protected override void OnDraw(Rect rect)
        {
            if (Config is null) return;
            if (Config.Count == 0) return;
            if (Config.CurrentPackage is null) return;
            if (Config.CurrentPackage.Count == 0) return;
            if (Config.CurrentGroup is null) return;
            if (Config.CurrentGroup.Count != 0) return;
            if (GELayout.Button(rect.center, new Vector2(100, 30), "创建收集器")) OP_CreateCollect();
        }

        protected override void OnBuildRows(TreeViewItem root)
        {
            for (var idxG = 0; idxG < Config?.CurrentGroup?.Count; idxG++)
            {
                root.AddChild(new TreeViewItemCollect(idxG, Config.CurrentGroup[idxG])
                {
                    OnChangedFold = fold => Reload()
                });
            }
        }

        protected override void OnSelection(int id)
        {
            Config.CurrentCollectIndex = id;
        }

        protected override void OnContextClicked(GenericMenu menu)
        {
            if (!Config.CurrentGroup.Enable) return;
            menu.AddItem(GC_MENU_ADD, false, OP_CreateCollect);
            menu.AddItem(GC_MENU_ALL_DISABLE, false, () =>
            {
                Config.CurrentGroup.ForEach(group => group.Enable = false);
                Reload();
            });
            menu.AddItem(GC_MENU_ALL_ENABLE, false, () =>
            {
                Config.CurrentGroup.ForEach(group => group.Enable = true);
                Reload();
            });
            menu.AddItem(GC_MENU_ALL_FOLD, false, () =>
            {
                Config.CurrentGroup.ForEach(group => group.Folded = false);
                Reload();
            });
            menu.AddItem(GC_MENU_ALL_UNFOLD, false, () =>
            {
                Config.CurrentGroup.ForEach(group => group.Folded = group.Enable);
                Reload();
            });
            menu.AddItem(GC_MENU_SAVE, false, Config.Save);
        }

        protected override void OnContextClicked(GenericMenu menu, TreeViewItem item)
        {
            if (!Config.CurrentGroup.Enable) return;
            if (!(item is TreeViewItemCollect collect)) return;
            if (collect.Item.Enable)
            {
                menu.AddItem(GC_MENU_DISABLE, false, () =>
                {
                    collect.Item.Folded = collect.Item.Enable = false;
                    Reload();
                });
                if (collect.Item.Path)
                {
                    if (collect.Item.Folded)
                        menu.AddItem(GC_MENU_FOLD, false, () =>
                        {
                            collect.Item.Folded = false;
                            Reload();
                        });
                    else
                        menu.AddItem(GC_MENU_UNFOLD, false, () =>
                        {
                            collect.Item.Folded = collect.Item.Enable;
                            Reload();
                        });
                }
            }
            else menu.AddItem(GC_MENU_ENABLE, false, () => { collect.Item.Enable = true; });

            menu.AddItem(GC_MENU_DELETE, false, () =>
            {
                collect.OP_DEL();
                Reload();
            });
        }

        /// <inheritdoc />
        protected override void OnDragSwapData(int from, int to)
        {
            Config.CurrentGroup.Swap(from, to);
        }

        /// <inheritdoc />
        protected override void OnEventKeyDown(KeyCode keyCode, TreeViewItem item)
        {
            switch (item)
            {
                case TreeViewItemCollect collect:
                {
                    switch (keyCode)
                    {
                        case KeyCode.KeypadEnter:
                        case KeyCode.Return:
                            if (collect.Item.Path)
                            {
                                collect.Item.Folded = !collect.Item.Folded;
                                Reload();
                            }


                            break;
                        case KeyCode.Delete:
                            collect.OP_DEL();
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

                    break;
                }
            }
        }

        private void OP_CreateCollect()
        {
            Config.CurrentGroup.Add(new AssetCollectItem());
            Reload();
        }
    }
}