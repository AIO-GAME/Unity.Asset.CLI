/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// nameof(AssetCollectWindow_OnDrawList)
    /// </summary>
    public partial class AssetCollectWindow
    {
        private AssetCollectItem OnDrawCurrentItem;

        partial void OnDrawList()
        {
            using (GELayout.Vertical(GEStyle.GridList))
            {
                foreach (var data in OnDrawCurrentItem.AssetDataInfos.CurrentPage)
                {
                    using (GELayout.Vertical())
                    {
                        GELayout.HelpBox($"Location : {data.Value.Address} \nPath : {data.Key}");
                        using (GELayout.VHorizontal())
                        {
                            GELayout.Field(AssetDatabase.LoadAssetAtPath<Object>(data.Key));
                            GELayout.ButtonCopy("Copy", data.Value.Address, 50);
                            if (GELayout.Button("Select", 50))
                            {
                                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(data.Key);
                            }
                        }
                    }
                }
            }
        }
    }
}