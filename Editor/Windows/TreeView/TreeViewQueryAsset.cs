using System;
using System.Collections.Generic;
using AIO.UEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewQueryAsset : ViewTreeRowSingle
    {
        private TreeViewQueryAsset(TreeViewState state, MultiColumnHeader header, PageList<AssetDataInfo> pageList)
            : base(state, header)
        {
            AllowDrag                     = false;
            showAlternatingRowBackgrounds = true;
            PageValues                    = pageList;
            PageValues.PageIndex          = 0;
        }

        /// <summary>
        ///     当前选择包索引
        /// </summary>
        public PageList<AssetDataInfo> PageValues;

        public void Reload(PageList<AssetDataInfo> dataInfos)
        {
            var index = PageValues.PageIndex;
            PageValues           = dataInfos;
            PageValues.PageIndex = index;
            PageValues.Sort();
            ReloadAndSelect(0);
        }

        public static TreeViewQueryAsset Create(PageList<AssetDataInfo> pageValues)
        {
            return new TreeViewQueryAsset(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("可寻址路径", 400, 400),
                GetMultiColumnHeaderColumn("真实地址", 350, 300, 400),
                GetMultiColumnHeaderColumn("资源类型", 175, 150, 200),
                GetMultiColumnHeaderColumn("资源大小", 75, 75, 75),
                GetMultiColumnHeaderColumn("修改时间", 75, 75, 75),
                GetMultiColumnHeaderColumn("首包", 30, 30, 30, false),
            })), pageValues);
        }

        public int PageSize
        {
            get => PageValues.PageSize;
            set => PageValues.PageSize = value;
        }

        /// <inheritdoc />
        protected override void OnDraw(Rect rect)
        {
            if (PageValues is null || PageValues.Count == 0)
            {
                GUI.Label(rect, "没有数据", GEStyle.CenteredLabel);
            }
        }

        /// <inheritdoc />
        protected override void OnSorting(MultiColumnHeader header)
        {
            switch (header.sortedColumnIndex)
            {
                case 0:
                    if (header.IsSortedAscending(header.sortedColumnIndex))
                        PageValues.Sort((a, b) => string.Compare(a.Address, b.Address, StringComparison.CurrentCulture));
                    else
                        PageValues.Sort((a, b) => string.Compare(b.Address, a.Address, StringComparison.CurrentCulture));
                    break;
                case 1:
                    if (header.IsSortedAscending(header.sortedColumnIndex))
                        PageValues.Sort((a, b) => string.Compare(a.AssetPath, b.AssetPath, StringComparison.CurrentCulture));
                    else
                        PageValues.Sort((a, b) => string.Compare(b.AssetPath, a.AssetPath, StringComparison.CurrentCulture));
                    break;
                case 2:
                    if (header.IsSortedAscending(header.sortedColumnIndex))
                        PageValues.Sort((a, b) => string.Compare(a.Type, b.Type, StringComparison.CurrentCulture));
                    else
                        PageValues.Sort((a, b) => string.Compare(b.Type, a.Type, StringComparison.CurrentCulture));
                    break;
                case 3:
                    if (header.IsSortedAscending(header.sortedColumnIndex))
                        PageValues.Sort((a, b) => a.Size.CompareTo(b.Size));
                    else
                        PageValues.Sort((a, b) => b.Size.CompareTo(a.Size));
                    break;
                case 4:
                    if (header.IsSortedAscending(header.sortedColumnIndex))
                        PageValues.Sort((a, b) => string.Compare(a.GetLatestTime(), b.GetLatestTime(), StringComparison.CurrentCulture));
                    else
                        PageValues.Sort((a, b) => string.Compare(b.GetLatestTime(), a.GetLatestTime(), StringComparison.CurrentCulture));
                    break;
            }
        }

        protected override void OnInitialize() { }

        /// <inheritdoc />
        protected override void OnBuildRows(TreeViewItem root)
        {
            if (PageValues is null) return;
            if (root.children is null)
                root.children = new List<TreeViewItem>();
            else
                root.children.Clear();

            for (var i = 0; i < PageValues.CurrentPageValues.Length; i++)
            {
                root.AddChild(new TreeViewItemQueryAsset(i, PageValues.CurrentPageValues[i], IsFirstPackageResource, OnFirstPackageResource)
                {
                    Refresh = ReloadAndSelect
                });
            }
        }

        /// <summary>
        ///    是否在首包中
        /// </summary>
        public event Func<string, bool> IsFirstPackageResource;

        /// <summary>
        ///   添加或删除首包资源
        /// </summary>
        public event Action<AssetDataInfo, bool> OnFirstPackageResource;

        /// <inheritdoc />
        protected override void OnSelection(int id)
        {
            InvokeSelectionChanged(id);
        }

        /// <inheritdoc />
        protected override void OnContextClicked(GenericMenu menu, TreeViewItem item)
        {
            AssetDataInfo data;
            switch (item)
            {
                case TreeViewItemQueryAsset asset:
                    data = asset.data;
                    break;
                default:
                    return;
            }

            menu.AddItem(new GUIContent("打开 : 资源所在文件夹"), false, () => { EditorUtility.RevealInFinder(data.AssetPath); });
            if (AHelper.IO.ExistsFile(data.AssetPath))
            {
                menu.AddItem(new GUIContent("打开 : 使用默认程序打开"), false, () => { EditorUtility.OpenWithDefaultApp(data.AssetPath); });
                menu.AddItem(new GUIContent("选择 : 资源"), false, () => { Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(data.AssetPath); });
            }

            menu.AddItem(new GUIContent("复制 : 可寻址路径"), false, () => { GUIUtility.systemCopyBuffer = data.Address; });
            menu.AddItem(new GUIContent("复制 : 资源路径"), false, () => { GUIUtility.systemCopyBuffer  = data.AssetPath; });
            menu.AddItem(new GUIContent("复制 : GUID"), false, () => { GUIUtility.systemCopyBuffer  = data.GUID; });
            menu.AddItem(new GUIContent("复制 : 资源类型"), false, () => { GUIUtility.systemCopyBuffer  = data.Type; });
            if (!string.IsNullOrEmpty(data.Tags))
                menu.AddItem(new GUIContent("复制 : 标签列表"), false, () => { GUIUtility.systemCopyBuffer = data.Tags; });
            if (AssetCollectWindow.WindowMode == AssetCollectWindow.Mode.LookFirstPackage) return;
            if (!ASConfig.GetOrCreate().EnableSequenceRecord) return;
            if (!IsFirstPackageResource?.Invoke(data.GUID) ?? false)
                menu.AddItem(new GUIContent("添加 : 首包列表"), false, () => { OnFirstPackageResource?.Invoke(data, true); });
            else
                menu.AddItem(new GUIContent("移除 : 首包列表"), false, () => { OnFirstPackageResource?.Invoke(data, false); });
        }

        /// <inheritdoc />
        protected override void OnEventKeyDown(KeyCode keyCode, TreeViewItem item)
        {
            switch (keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    if (item is TreeViewItemQueryAsset asset)
                        OnFirstPackageResource?.Invoke(asset.data, !IsFirstPackageResource?.Invoke(asset.data.GUID) ?? false);
                    break;

                case KeyCode.LeftArrow: // 数字键盘 右键
                    if (PageValues.PageIndex > 0)
                        PageValues.PageIndex  -= 1;
                    else PageValues.PageIndex =  PageValues.PageCount - 1;
                    ReloadAndSelect(0);
                    break;

                case KeyCode.RightArrow: // 数字键盘 左键 
                    if (PageValues.PageIndex < PageValues.PageCount - 1)
                        PageValues.PageIndex  += 1;
                    else PageValues.PageIndex =  0;
                    ReloadAndSelect(0);
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
    }
}