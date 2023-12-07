/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Text;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        public enum Mode
        {
            [InspectorName("编辑模式")] Editor,
            [InspectorName("查询模式")] Look,
            [InspectorName("打包模式")] Build,
        }

        private Mode LookMode = Mode.Editor;

        public bool ShowGroup = false;
        public bool ShowPackage = true;
        public bool ShowSetting = false;

        public bool ShowList => OnDrawCurrentItem != null;
        private StringBuilder TempBuilder = new StringBuilder();

        private void OnDrawHeaderEditor()
        {
            if (!ShowSetting && GELayout.Button("⇘ Setting", GEStyle.TEtoolbarbutton, ButtonWidth, 20))
            {
                GUI.FocusControl(null);
                ShowSetting = true;
            }

            if (!ShowPackage && GELayout.Button("⇘ Package", GEStyle.TEtoolbarbutton, ButtonWidth, 20))
            {
                GUI.FocusControl(null);
                ShowPackage = true;
            }

            if (Data.Packages.Length <= CurrentPackageIndex || CurrentPackageIndex < 0 ||
                Data.Packages[CurrentPackageIndex] is null)
            {
                GUI.FocusControl(null);
                ShowGroup = false;
            }
            else if (!ShowGroup && GELayout.Button("⇘ Group", GEStyle.TEtoolbarbutton, ButtonWidth, 20))
            {
                GUI.FocusControl(null);
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
                if (!string.IsNullOrEmpty(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex]
                        .Description))
                {
                    TempBuilder.Append('(');
                    TempBuilder.Append(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex]
                        .Description);
                    TempBuilder.Append(')');
                }
            }

            GELayout.Label(TempBuilder.ToString(), GEStyle.HeaderLabel);
            EditorGUILayout.Separator();

#if SUPPORT_YOOASSET
            if (GELayout.Button("转换 Yoo", GEStyle.TEtoolbarbutton, ButtonWidth, 20))
            {
                GUI.FocusControl(null);
                ConvertYooAsset.Convert(Data);
            }
#endif

            if (GELayout.Button(EditorGUIUtility.IconContent("d_GameObject Icon"), GEStyle.TEtoolbarbutton, 30, 20))
            {
                GUI.FocusControl(null);
                Selection.activeObject = ASConfig.GetOrCreate();
            }

            if (GELayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, 30, 20))
            {
                GUI.FocusControl(null);
                Data.Save();
                EditorUtility.DisplayDialog("保存", "保存成功", "确定");
            }
        }

        partial void OnDrawHeaderBuild();

        partial void OnDrawHeader()
        {
            using (GELayout.VHorizontal())
            {
                switch (LookMode)
                {
                    case Mode.Editor:
                        OnDrawHeaderEditor();
                        break;
                    case Mode.Look:
                        OnDrawHeaderLook();
                        break;
                    case Mode.Build:
                        OnDrawHeaderBuild();
                        break;
                }

                LookMode = GELayout.Popup(LookMode, GEStyle.TEtoolbarbutton,
                    GTOption.Width(ButtonWidth), GTOption.Height(20));

                if (GUI.changed)
                {
                    GUI.FocusControl(null);
                    switch (LookMode)
                    {
                        default:
                        case Mode.Editor:
                            UpdateDataRecordQueue();
                            break;
                        case Mode.Look:
                            UpdateDataLook();
                            break;
                        case Mode.Build:
                            UpdateDataBuild();
                            break;
                    }
                }
            }
        }
    }
}