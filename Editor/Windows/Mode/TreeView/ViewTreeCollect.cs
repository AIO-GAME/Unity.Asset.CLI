﻿using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class ViewTreeCollect : ViewTreeRowSingle
    {
        private readonly GUIContent GC_MENU_ADD         = new GUIContent("添加 : 收集器");
        private readonly GUIContent GC_MENU_ALL_DISABLE = new GUIContent("全部 : 禁用");
        private readonly GUIContent GC_MENU_ALL_ENABLE  = new GUIContent("全部 : 启用");
        private readonly GUIContent GC_MENU_ALL_UNFOLD  = new GUIContent("全部 : 展开");
        private readonly GUIContent GC_MENU_ALL_FOLD    = new GUIContent("全部 : 折叠");
        private readonly GUIContent GC_MENU_ENABLE      = new GUIContent("启用");
        private readonly GUIContent GC_MENU_DISABLE     = new GUIContent("禁用");
        private readonly GUIContent GC_MENU_DELETE      = new GUIContent("删除");

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
            SetSelection(new[] { index + 1 });
        }

        protected override void OnBuildRows(TreeViewItem root)
        {
            for (var idxG = 0; idxG < Config.CurrentGroup.Count; idxG++)
            {
                var collect = new TreeViewItemCollect(idxG + 1, Config.CurrentGroup[idxG]);
                collect.OnChangedFold += fold => Reload();
                root.AddChild(collect);
            }
        }

        protected override void OnSelection(int id)
        {
            Config.CurrentCollectIndex = id - 1;
        }

        protected override void OnContextClicked(GenericMenu menu)
        {
            menu.AddItem(GC_MENU_ALL_DISABLE, false, () => { Config.CurrentGroup.ForEach(group => group.Enable = false); });
            menu.AddItem(GC_MENU_ALL_ENABLE, false, () => { Config.CurrentGroup.ForEach(group => group.Enable  = true); });
        }

        /// <inheritdoc />
        protected override void OnDragSwapData(int from, int to)
        {
            (Config.CurrentGroup[from], Config.CurrentGroup[to]) = (Config.CurrentGroup[to], Config.CurrentGroup[from]);
        }
    }
}