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
                    using (GELayout.Vertical(GEStyle.HelpBox))
                    {
                        using (GELayout.VHorizontal())
                        {
                            GELayout.Label(data.Value.Address, GEStyle.MiniLabel);
                            GELayout.ButtonCopy(GC_COPY, data.Value.Address, 24);
                        }

                        using (GELayout.VHorizontal())
                        {
                            GELayout.Label(data.Key, GEStyle.MiniLabel);
                            GELayout.Field(AssetDatabase.LoadAssetAtPath<Object>(data.Key), GTOption.Width(80));
                            if (GELayout.Button(GC_SELECT, 24))
                            {
                                GUI.FocusControl(null);
                                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(data.Key);
                            }
                        }
                    }
                }
            }
        }
    }
}