/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private void OnDrawPackageInfo()
        {
            using (new EditorGUILayout.VerticalScope(GEStyle.INThumbnailShadow))
            {
                Data.CurrentPackage.Name = GELayout.Field(
                    "Package Name", Data.CurrentPackage.Name);

                Data.CurrentPackage.Description = GELayout.Field(
                    "Package Description", Data.CurrentPackage.Description);
            }

            if (!Data.IsGroupValid()) return;

            EditorGUILayout.Space();
            using (new EditorGUILayout.VerticalScope(GEStyle.INThumbnailShadow))
            {
                Data.CurrentGroup.Name = GELayout.Field(
                    "Group Name", Data.CurrentGroup.Name);

                Data.CurrentGroup.Description = GELayout.Field(
                    "Group Description", Data.CurrentGroup.Description);

                Data.CurrentGroup.Tags = GELayout.Field(
                    "Group Tags", Data.CurrentGroup.Tags);
            }
        }

        private void OnDrawItem()
        {
            if (!Data.IsCollectValid()) return;
            using (var sp = new EditorGUILayout.ScrollViewScope(OnDrawItemListScroll))
            {
                OnDrawItemListScroll = sp.scrollPosition;
                for (var i = Data.CurrentGroup.Collectors.Length - 1;
                     i >= 0;
                     i--)
                {
                    if (CurrentCurrentCollectorsIndex == i)
                    {
                        using (new EditorGUILayout.VerticalScope(GEStyle.SelectionRect))
                        {
                            OnDrawItem(Data.CurrentGroup.Collectors[i], i);
                        }
                    }
                    else OnDrawItem(Data.CurrentGroup.Collectors[i], i);

                    EditorGUILayout.Space();
                }
            }
        }

        partial void OnDrawGroupList()
        {
            if (Data.Length <= 0) return;
            using (new EditorGUILayout.VerticalScope(GEStyle.GridList))
            {
                EditorGUILayout.Space();
                FoldoutPackageInfo = GELayout.VFoldoutHeaderGroupWithHelp(
                    OnDrawPackageInfo,
                    "Data Info",
                    FoldoutPackageInfo);

                EditorGUILayout.Space();
                var content = new GUIContent($"Collectors ({Data.CurrentGroup.Collectors.Length})");
                FoldoutCollectors = GELayout.VFoldoutHeaderGroupWithHelp(
                    OnDrawItem,
                    content,
                    FoldoutCollectors,
                    () =>
                    {
                        CurrentCurrentCollectorsIndex = Data.CurrentGroup.Collectors.Length;
                        OnDrawItemListScroll.y = 0;
                        Data.CurrentGroup.Collectors = Data.CurrentGroup.Collectors.Add(new AssetCollectItem());
                    },
                    0,
                    null,
                    new GUIContent("✚")
                );
            }
        }
    }
}