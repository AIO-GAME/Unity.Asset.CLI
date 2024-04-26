using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewDependencies : TreeViewRowSingle
    {
        public static TreeViewDependencies Create(ICollection<AssetPageLook.DependenciesInfo> data, float width = 100, float min = 80, float max = 200)
        {
            return new TreeViewDependencies(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("资源", width, min, max, true), GetMultiColumnHeaderColumn("大小", 75, true),
                GetMultiColumnHeaderColumn("跳转", 35, false)
            })), data);
        }

        private IEnumerable<AssetPageLook.DependenciesInfo> data;

        private TreeViewDependencies(TreeViewState state, MultiColumnHeader header, IEnumerable<AssetPageLook.DependenciesInfo> list) : base(state, header)
        {
            data                          = list;
            showAlternatingRowBackgrounds = true;
        }

        protected override bool OnSorting(int col, bool ascending)
        {
            if (data is null) return false;
            switch (col)
            {
                case 0:
                    data = ascending ? data.OrderBy(x => x.Object?.name) : data.OrderByDescending(x => x.Object?.name);
                    break;
                case 1:
                    data = ascending ? data.OrderBy(x => x.Size) : data.OrderByDescending(x => x.Size);
                    break;
            }

            return true;
        }

        protected override void OnBuildRows(TreeViewItem root)
        {
            if (data is null) return;
            var idxG = 0;
            var size = 0L;
            foreach (var variable in data)
            {
                size += variable.Size;
                root.AddChild(new TreeViewItemDependencies(idxG++, variable));
            }

            multiColumnHeader.GetColumn(0).headerContent = EditorGUIUtility.TrTextContent($"资源数量 : {idxG} 合计大小 : {size.ToConverseStringFileSize()}");
        }

        public void Reload(ICollection<AssetPageLook.DependenciesInfo> list)
        {
            if (list is null) return;
            data = list;
            Reload();
        }
    }
}