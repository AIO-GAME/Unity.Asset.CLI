using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace AIO.UEditor
{
    public class TreeViewDependencies : TreeViewRowSingle
    {
        public static TreeViewDependencies Create(ICollection<AssetCollectWindow.DependenciesInfo> data, float width = 100, float min = 80, float max = 200)
        {
            return new TreeViewDependencies(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("资源", width, min, max, true), GetMultiColumnHeaderColumn("大小", 75, true), 
                GetMultiColumnHeaderColumn("跳转", 35, false)
            })), data);
        }

        private ICollection<AssetCollectWindow.DependenciesInfo> data;

        private TreeViewDependencies(TreeViewState state, MultiColumnHeader header, ICollection<AssetCollectWindow.DependenciesInfo> list) : base(state, header)
        {
            data                          = list;
            showAlternatingRowBackgrounds = true;
        }

        protected override void OnBuildRows(TreeViewItem root)
        {
            if (data is null) return;
            var idxG = 0;
            foreach (var variable in data)
            {
                root.AddChild(new TreeViewItemDependencies(idxG++, variable));
            }
        }

        public void Reload(ICollection<AssetCollectWindow.DependenciesInfo> list)
        {
            if (list is null) return;
            data = list;
            Reload();
        }
    }
}