/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Text;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        public bool ShowGroup = false;
        public bool ShowPackage = true;
        public bool ShowSetting = false;
        public bool ShowList => OnDrawCurrentItem != null;
        private StringBuilder TempBuilder = new StringBuilder();

        partial void OnDrawHeader()
        {
            using (GELayout.VHorizontal())
            {
                if (!ShowSetting && GELayout.Button("Setting", GEStyle.RLFooter, ButtonWidth, 20))
                {
                    ShowSetting = true;
                }

                if (!ShowPackage && GELayout.Button("Package", GEStyle.RLFooter, ButtonWidth, 20))
                {
                    ShowPackage = true;
                }

                if (Data.Packages.Length <= CurrentPackageIndex || CurrentPackageIndex < 0 ||
                    Data.Packages[CurrentPackageIndex] is null)
                {
                    ShowGroup = false;
                }
                else if (!ShowGroup && GELayout.Button("Group", GEStyle.RLFooter, ButtonWidth, 20))
                {
                    ShowGroup = true;
                }

                EditorGUILayout.Separator();
                TempBuilder.Clear();
                if (CurrentPackageIndex >= 0 && Data.Packages.Length > CurrentPackageIndex)
                {
                    TempBuilder.Append(Data.Packages[CurrentPackageIndex].Name);
                    if (!string.IsNullOrEmpty(Data.Packages[CurrentPackageIndex].Description))
                    {
                        TempBuilder.Append('(');
                        TempBuilder.Append(Data.Packages[CurrentPackageIndex].Description);
                        TempBuilder.Append(')');
                    }
                }

                if (CurrentGroupIndex >= 0 && Data.Packages.Length > CurrentPackageIndex &&
                    Data.Packages[CurrentPackageIndex].Groups.Length > CurrentGroupIndex)
                {
                    TempBuilder.Append(" / ");
                    TempBuilder.Append(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Name);
                    if (!string.IsNullOrEmpty(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Description))
                    {
                        TempBuilder.Append('(');
                        TempBuilder.Append(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Description);
                        TempBuilder.Append(')');
                    }
                }

                GELayout.Label(TempBuilder.ToString(), GEStyle.HeaderLabel);
                EditorGUILayout.Separator();

                if (GELayout.Button("转换", GEStyle.RLFooter, ButtonWidth, 20))
                {
                }

                if (GELayout.Button("保存", GEStyle.RLFooter, ButtonWidth, 20))
                {
                    Data.Save();
                }
            }
        }
    }
}