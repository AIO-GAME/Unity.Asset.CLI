using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        /// <summary>
        ///     收集器搜索筛选
        /// </summary>
        private string ItemCollectorsSearch;

        /// <summary>
        ///     收集器搜索结果
        /// </summary>
        private readonly List<AssetCollectItem> ItemCollectorsSearchResult = new List<AssetCollectItem>();

        private bool ItemCollectorsSearching => !string.IsNullOrEmpty(ItemCollectorsSearch);

        private void OnDrawPackageInfo()
        {
            using (new EditorGUILayout.VerticalScope(GEStyle.INThumbnailShadow))
            {
                Data.CurrentPackage.Name = GELayout.Field(
                    "Package Name", Data.CurrentPackage.Name);

                Data.CurrentPackage.Description = GELayout.Field(
                    "Package Description",
                    Data.CurrentPackage.Description);
            }

            if (!Data.IsValidGroup()) return;

            EditorGUILayout.Space();
            using (new EditorGUILayout.VerticalScope(GEStyle.INThumbnailShadow))
            {
                Data.CurrentGroup.Name = GELayout.Field(
                    "Group Name", Data.CurrentGroup.Name);

                Data.CurrentGroup.Description = GELayout.Field(
                    "Group Description", Data.CurrentGroup.Description);

                Data.CurrentGroup.Tag = GELayout.Field(
                    "Group Tags", Data.CurrentGroup.Tag);
            }
        }

        private void OnDrawItem()
        {
            if (!Data.IsValidCollect()) return;

            if (Data.CurrentGroup.Collectors.Length > 1)
            {
                using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                {
                    if (GUILayout.Button(GC_FOLDOUT_ON, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
                        foreach (var item in Data.CurrentGroup.Collectors)
                            item.Folded = true;

                    if (GUILayout.Button(GC_FOLDOUT, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
                        foreach (var item in Data.CurrentGroup.Collectors)
                            item.Folded = false;

                    ItemCollectorsSearch = GELayout.FieldDelayed(ItemCollectorsSearch, GEStyle.SearchTextField);
                    if (GELayout.Button(GC_CLEAR, GEStyle.TEtoolbarbutton, 24))
                    {
                        ItemCollectorsSearch = string.Empty;
                        GUI.FocusControl(null);
                    }

                    if (GUI.changed)
                    {
                        ItemCollectorsSearchResult.Clear();
                        if (!string.IsNullOrEmpty(ItemCollectorsSearch))
                        {
                            var p2 = ItemCollectorsSearch.ToLower();
                            foreach (var item in Data.CurrentGroup.Collectors)
                            {
                                if (item.Path is null) continue;
                                var p1 = item.CollectPath.ToLower();
                                if (p1.ToLower().Contains(p2))
                                    ItemCollectorsSearchResult.Add(item);
                                else if (p2.Contains(p1)) ItemCollectorsSearchResult.Add(item);
                            }

                            if (CurrentCurrentCollectorsIndex >= ItemCollectorsSearchResult.Count)
                                CurrentCurrentCollectorsIndex = ItemCollectorsSearchResult.Count - 1;
                        }
                    }
                }

                EditorGUILayout.Space();
            }


            using (var sp = new EditorGUILayout.ScrollViewScope(OnDrawItemListScroll))
            {
                OnDrawItemListScroll = sp.scrollPosition;
                if (ItemCollectorsSearching)
                {
                    if (ItemCollectorsSearchResult.Count == 0)
                        EditorGUILayout.HelpBox("没有找到任何匹配的结果", MessageType.None);
                    else
                        for (var i = ItemCollectorsSearchResult.Count - 1; i >= 0; i--)
                        {
                            OnDrawItem(ItemCollectorsSearchResult[i], i);
                            if (CurrentCurrentCollectorsIndex == i)
                            {
                                var rect = GUILayoutUtility.GetLastRect();
                                GUI.Box(rect, string.Empty, GEStyle.SelectionRect);
                            }

                            EditorGUILayout.Space();
                        }
                }
                else
                {
                    for (var i = Data.CurrentGroup.Collectors.Length - 1; i >= 0; i--)
                    {
                        OnDrawItem(Data.CurrentGroup.Collectors[i], i);
                        if (CurrentCurrentCollectorsIndex == i)
                        {
                            var rect = GUILayoutUtility.GetLastRect();
                            GUI.Box(rect, string.Empty, GEStyle.SelectionRect);
                        }

                        EditorGUILayout.Space();
                    }
                }
            }
        }

        partial void OnDrawGroupList()
        {
            if (Data.Count <= 0) return;
            using (new EditorGUILayout.VerticalScope(GEStyle.GridList))
            {
                // EditorGUILayout.Space();
                // FoldoutPackageInfo = GELayout.VFoldoutHeaderGroupWithHelp(
                //                                                           OnDrawPackageInfo,
                //                                                           "Data Info",
                //                                                           FoldoutPackageInfo);

                EditorGUILayout.Space();
                var content = new GUIContent($"Collectors ({Data.CurrentGroup.Collectors.Length})");
                FoldoutCollectors = GELayout.VFoldoutHeaderGroupWithHelp(
                    OnDrawItem, content, FoldoutCollectors, () =>
                    {
                        OnDrawItemListScroll.y        = 0;
                        CurrentCurrentCollectorsIndex = Data.CurrentGroup.Collectors.Length;
                        Data.CurrentGroup.Collectors  = Data.CurrentGroup.Collectors.Add(new AssetCollectItem());
                    },
                    0, null, new GUIContent("✚")
                );
            }
        }
    }
}