using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewQueryAsset : TreeViewRowSingle
    {
        private TreeViewQueryAsset(TreeViewState state, MultiColumnHeader header, PageList<AssetDataInfo> pageList)
            : base(state, header)
        {
            AllowDrag                     = false;
            showAlternatingRowBackgrounds = true;
            PageValues                    = pageList;
            PageValues.PageIndex          = 0;
            AllowMultiSelect              = true;
        }

        internal PageList<AssetDataInfo>         PageValues;
        public event Func<string, bool>          IsFirstPackageResource;
        public event Action<AssetDataInfo, bool> OnFirstPackageResource;
        private string                           AllSize;
        private Task                             SortingTask;
        private CancellationTokenSource          CancellationTokenSource;

        public void Reload(PageList<AssetDataInfo> dataInfos)
        {
            var index = PageValues.PageIndex;
            PageValues           = dataInfos;
            PageValues.PageIndex = index;
            AllSize              = PageValues.Sum(a => a.Size).ToConverseStringFileSize();
            if (rootItem.children.Count == 0)
                Reload();
            else
            {
                Refresh();
                SetSelection(new[] { 0 }, TreeViewSelectionOptions.RevealAndFrame);
                InvokeSelectionChanged(0);
            }
        }

        public static TreeViewQueryAsset Create(PageList<AssetDataInfo> pageValues)
        {
            return new TreeViewQueryAsset(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("可寻址路径", 400, 400), GetMultiColumnHeaderColumn("真实地址", 350, 300, 400),
                GetMultiColumnHeaderColumn("资源类型", 175, 150, 200), GetMultiColumnHeaderColumn("资源大小", 75, 75, 75),
                GetMultiColumnHeaderColumn("修改时间", 75, 75, 75), GetMultiColumnHeaderColumn("首包", 30, 30, 30, false)
            })), pageValues);
        }

        protected override void OnDraw(Rect rect)
        {
            if (PageValues is null || PageValues.Count == 0)
            {
                GUI.Label(rect, "没有数据", GEStyle.CenteredLabel);
            }
        }

        private void Sorting(int columnIndex, bool ascending)
        {
            switch (columnIndex)
            {
                case 0:
                    PageValues.Sort(ascending ? AssetDataInfo.ComparerAddressAsc : AssetDataInfo.ComparerAddress);
                    break;
                case 1:
                    PageValues.Sort(ascending ? AssetDataInfo.ComparerAssetPathAsc : AssetDataInfo.ComparerAssetPath);
                    break;
                case 2:
                    PageValues.Sort(ascending ? AssetDataInfo.ComparerTypeAsc : AssetDataInfo.ComparerType);
                    break;
                case 3:
                    PageValues.Sort(ascending ? AssetDataInfo.ComparerSizeAsc : AssetDataInfo.ComparerSize);
                    break;
                case 4:
                    PageValues.Sort(ascending ? AssetDataInfo.ComparerTimeAsc : AssetDataInfo.ComparerTime);
                    break;
            }
        }

        private void Sorting(int index, int count, int columnIndex, bool ascending)
        {
            switch (columnIndex)
            {
                case 0:
                    PageValues.Sort(index, count, ascending ? AssetDataInfo.ComparerAddressAsc : AssetDataInfo.ComparerAddress);
                    break;
                case 1:
                    PageValues.Sort(index, count, ascending ? AssetDataInfo.ComparerAssetPathAsc : AssetDataInfo.ComparerAssetPath);
                    break;
                case 2:
                    PageValues.Sort(index, count, ascending ? AssetDataInfo.ComparerTypeAsc : AssetDataInfo.ComparerType);
                    break;
                case 3:
                    PageValues.Sort(index, count, ascending ? AssetDataInfo.ComparerSizeAsc : AssetDataInfo.ComparerSize);
                    break;
                case 4:
                    PageValues.Sort(index, count, ascending ? AssetDataInfo.ComparerTimeAsc : AssetDataInfo.ComparerTime);
                    break;
            }
        }

        private int LastSorting;

        protected override bool OnSorting(int columnIndex, bool ascending)
        {
            if (LastSorting == columnIndex)
            {
                PageValues.Reverse();
                Refresh();
                InvokeSelectionChanged(0);
                SetSelection(new[] { 0 }, TreeViewSelectionOptions.RevealAndFrame);
                return false;
            }

            if (PageValues.Count >= 200)
            {
                LastSorting          = -1;
                PageValues.PageIndex = 0;
                Sorting(0, PageValues.CurrentPageValues.Length, columnIndex, ascending);
                Refresh();
                InvokeSelectionChanged(0);
                SetSelection(new[] { 0 }, TreeViewSelectionOptions.RevealAndFrame);
                CancellationTokenSource?.Cancel();
                CancellationTokenSource = new CancellationTokenSource();
                SortingTask = new Task(() =>
                {
                    Sorting(columnIndex, ascending);
                    LastSorting = columnIndex;
                    Refresh();
                    InvokeSelectionChanged(0);
                    SetSelection(new[] { 0 }, TreeViewSelectionOptions.RevealAndFrame);
                }, CancellationTokenSource.Token);
                SortingTask.Start(TaskScheduler.Default); // 使用后台线程进行排序
            }
            else
            {
                Sorting(columnIndex, ascending);
                Refresh();
                InvokeSelectionChanged(0);
                SetSelection(new[] { 0 }, TreeViewSelectionOptions.RevealAndFrame);
                LastSorting = columnIndex;
            }

            return false;
        }

        protected override void OnBuildRows(TreeViewItem root)
        {
            if (PageValues is null || PageValues.Count == 0) return;
            var index = 0;
            var size  = 0L;
            PageValues.PageIndex = PageValues.PageIndex;
            if (IsFirstPackageResource is null)
            {
                foreach (var info in PageValues.CurrentPageValues)
                {
                    root.AddChild(new TreeViewItemQueryAsset(index++, info)
                    {
                        OnFirstPackageResource = OnFirstPackageResource,
                        Refresh                = Select
                    });
                    size += info.Size;
                }
            }
            else
            {
                foreach (var info in PageValues.CurrentPageValues)
                {
                    root.AddChild(new TreeViewItemQueryAsset(index++, info)
                    {
                        IsFirstPackageResource = IsFirstPackageResource.Invoke(info.GUID),
                        OnFirstPackageResource = OnFirstPackageResource,
                        Refresh                = Select
                    });
                    size += info.Size;
                }
            }

            AllSize = PageValues.Sum(a => a.Size).ToConverseStringFileSize();
            multiColumnHeader.GetColumn(1).headerContent = EditorGUIUtility
                .TrTextContent(PageValues.PageCount <= 1
                                   ? $"数量 : {PageValues.Count} 总计 : {AllSize}"
                                   : $"数量 : {PageValues.Count} 总计 : {AllSize} 当前 : {size.ToConverseStringFileSize()}");
        }

        protected override void OnSelection(int id) => InvokeSelectionChanged(id);

        protected override void OnContextClicked(GenericMenu menu, IList<TreeViewItem> item)
        {
            menu.AddItem(new GUIContent("复制 : 可寻址路径"), false, () =>
            {
                var str = string.Empty;
                foreach (var treeViewItem in item)
                {
                    if (!(treeViewItem is TreeViewItemQueryAsset asset)) continue;
                    str += asset.data.Address + "\n";
                }

                GUIUtility.systemCopyBuffer = str;
            });

            menu.AddItem(new GUIContent("复制 : 资源路径"), false, () =>
            {
                var str = string.Empty;
                foreach (var treeViewItem in item)
                {
                    if (!(treeViewItem is TreeViewItemQueryAsset asset)) continue;
                    str += asset.data.AssetPath + "\n";
                }

                GUIUtility.systemCopyBuffer = str;
            });

            menu.AddItem(new GUIContent("复制 : GUID"), false, () =>
            {
                var str = string.Empty;
                foreach (var treeViewItem in item)
                {
                    if (!(treeViewItem is TreeViewItemQueryAsset asset)) continue;
                    str += asset.data.GUID + "\n";
                }

                GUIUtility.systemCopyBuffer = str;
            });

            if (IsFirstPackageResource != null
             && OnFirstPackageResource != null
             && ASConfig.GetOrCreate().EnableSequenceRecord)
            {
                if (AssetWindow.IsOpenPage<AssetPageLook.FirstPackage>())
                {
                    menu.AddItem(new GUIContent("添加 : 首包列表"), false, () =>
                    {
                        foreach (var treeViewItem in item)
                        {
                            if (!(treeViewItem is TreeViewItemQueryAsset asset)) continue;
                            if (!IsFirstPackageResource.Invoke(asset.data.GUID))
                                OnFirstPackageResource.Invoke(asset.data, true);
                        }
                    });
                }

                menu.AddItem(new GUIContent("移除 : 首包列表"), false, () =>
                {
                    foreach (var treeViewItem in item)
                    {
                        if (!(treeViewItem is TreeViewItemQueryAsset asset)) continue;
                        if (IsFirstPackageResource.Invoke(asset.data.GUID))
                            OnFirstPackageResource.Invoke(asset.data, false);
                    }
                });
            }
        }

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

            if (!string.IsNullOrEmpty(data.Tag))
                menu.AddItem(new GUIContent("复制 : 标签列表"), false, () => { GUIUtility.systemCopyBuffer = data.Tag; });

            if (AssetWindow.IsOpenPage<AssetPageLook.FirstPackage>()) return;
            if (!ASConfig.GetOrCreate().EnableSequenceRecord) return;

            if (!IsFirstPackageResource?.Invoke(data.GUID) ?? false)
                menu.AddItem(new GUIContent("添加 : 首包列表"), false, () => { OnFirstPackageResource?.Invoke(data, true); });
            else
                menu.AddItem(new GUIContent("移除 : 首包列表"), false, () => { OnFirstPackageResource?.Invoke(data, false); });
        }

        private void Refresh()
        {
            if (rootItem.children.Count != PageValues.CurrentPageValues.Length)
            {
                Reload();
                return;
            }

            var size = 0L;
            for (var i = rootItem.children.Count - 1; i >= 0; i--)
            {
                if (rootItem.children[i] is TreeViewItemQueryAsset item)
                {
                    if (i >= PageValues.CurrentPageValues.Length)
                    {
                        item.data = AssetDataInfo.Empty;
                        rootItem.children.Remove(item);
                        continue;
                    }

                    item.data                   =  PageValues.CurrentPageValues[i];
                    item.IsFirstPackageResource =  IsFirstPackageResource?.Invoke(item.data.GUID) ?? false;
                    size                        += item.data.Size;
                }
            }

            var title = PageValues.PageCount <= 1
                ? $"数量 : {PageValues.Count} 总计 : {AllSize}"
                : $"数量 : {PageValues.Count} 总计 : {AllSize} 当前 : {size.ToConverseStringFileSize()}";
            multiColumnHeader.GetColumn(1).headerContent = EditorGUIUtility.TrTextContent(title);
        }

        protected override void OnEventKeyDown(Event evt, TreeViewItem item)
        {
            if (PageValues.Count == 0) return;
            switch (evt.keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                {
                    if (item is TreeViewItemQueryAsset asset)
                        OnFirstPackageResource?.Invoke(asset.data, !IsFirstPackageResource?.Invoke(asset.data.GUID) ?? false);
                    break;
                }
                case KeyCode.A:
                {
                    if (evt.control) // 全选
                    {
                        if (PageValues.CurrentPageValues.Length == 0) return;
                        state.selectedIDs.Clear();
                        foreach (var treeViewItem in rootItem.children)
                            state.selectedIDs.Add(treeViewItem.id);
                        if (state.selectedIDs.Count > 0) state.lastClickedID = state.selectedIDs.Last();
                        InvokeSelectionChanged(-1);
                    }

                    break;
                }
                case KeyCode.Escape:
                {
                    if (PageValues.CurrentPageValues.Length == 0) return;
                    state.selectedIDs.Clear();
                    state.lastClickedID = -1;
                    InvokeSelectionChanged(-1);
                    break;
                }
                case KeyCode.LeftArrow: // 数字键盘 右键
                {
                    if (PageValues.PageCount <= 1) return;
                    PageValues.PageIndex = PageValues.PageIndex > 0 ? PageValues.PageIndex - 1 : PageValues.PageCount - 1;
                    Refresh();
                    SetSelection(new[] { 0 }, TreeViewSelectionOptions.RevealAndFrame);
                    InvokeSelectionChanged(0);
                    break;
                }
                case KeyCode.RightArrow: // 数字键盘 左键
                {
                    if (PageValues.PageCount <= 1) return;
                    PageValues.PageIndex = PageValues.PageIndex < PageValues.PageCount - 1 ? PageValues.PageIndex + 1 : 0;
                    Refresh();
                    SetSelection(new[] { 0 }, TreeViewSelectionOptions.RevealAndFrame);
                    InvokeSelectionChanged(0);
                    break;
                }
                case KeyCode.DownArrow: // 数字键盘 下键
                {
                    if (PageValues.CurrentPageValues.Length == 0) return;
                    var temp = item.id + 1;
                    var id   = temp >= Count ? 0 : temp;
                    SetSelection(new[] { id }, TreeViewSelectionOptions.RevealAndFrame);
                    InvokeSelectionChanged(id);
                    break;
                }
                case KeyCode.UpArrow: // 数字键盘 上键
                {
                    if (PageValues.CurrentPageValues.Length == 0) return;
                    var temp = item.id - 1;
                    var id   = temp < 0 ? Count - 1 : temp;
                    SetSelection(new[] { id }, TreeViewSelectionOptions.RevealAndFrame);
                    InvokeSelectionChanged(id);
                    break;
                }
            }
        }
    }
}