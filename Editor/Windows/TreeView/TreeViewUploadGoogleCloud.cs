using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewUploadGoogleCloud : TreeViewRowSingle
    {
        public static TreeViewUploadGoogleCloud Create(AssetBuildConfig.GCloudConfig[] list)
        {
            return new TreeViewUploadGoogleCloud(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("Google Cloud", 400, 400),
            })), list);
        }

        private TreeViewUploadGoogleCloud(TreeViewState state, MultiColumnHeader header, AssetBuildConfig.GCloudConfig[] list) : base(state, header)
        {
            showAlternatingRowBackgrounds = false;
            Data                          = list;
        }

        public AssetBuildConfig.GCloudConfig[] Data;

        protected override void OnBuildRows(TreeViewItem root)
        {
            if (Data is null) return;
            var index = 0;
            foreach (var item in Data)
            {
                if (item is null) continue;
                root.AddChild(new TreeViewItemUploadGoogleCloud(index++, item));
            }
        }

        protected override void OnDragSwapData(int from, int to) { Data.Swap(from, to); }

        protected override void OnDraw(Rect rect)
        {
            if (Data is null || Data.Length == 0)
                GUI.Label(rect, "没有数据");
        }

        protected override void OnContextClicked(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("新增配置"), false, () =>
            {
                Data = Data.Add(new AssetBuildConfig.GCloudConfig());
                Reload();
            });
        }

        protected override void OnContextClicked(GenericMenu menu, TreeViewItem item)
        {
            menu.AddItem(new GUIContent("删除配置"), false, () =>
            {
                GUI.FocusControl(null);
                if (!EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消")) return;
                Data = Data.RemoveAt(item.id);
                Reload();
            });
        }

        protected override bool OnSorting(int col, bool ascending)
        {
            Data = ascending
                ? Data.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture))
                : Data.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture));
            return true;
        }
    }
}