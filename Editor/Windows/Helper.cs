#region

using System;
using System.Collections.Generic;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;
#if SUPPORT_YOOASSET
using YooAsset.Editor;
#endif

#endregion

namespace AIO.UEditor
{
    [InitializeOnLoad]
    internal class ResInspectorUI
    {
        static ResInspectorUI()
        {
            Editor.finishedDefaultHeaderGUI -= OnPostHeaderGUI;
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }

        private static ASConfig Config
        {
            get
            {
                if (!_Config) _Config = ASConfig.GetOrCreate();
                return _Config;
            }
        }

        private static ASConfig _Config;

        private static readonly List<string> WhiteList = new List<string>
        {
            typeof(AssetCollectRoot).FullName,
            typeof(ASConfig).FullName,
            typeof(AssetBuildConfig).FullName,
            typeof(ConsoleWindowConfig).FullName,
#if SUPPORT_YOOASSET
            typeof(AssetBundleCollectorSetting).FullName,
            typeof(ShaderVariantCollectorSetting).FullName,
#endif
        };

        private static void OnPostHeaderGUI(Editor editor)
        {
            if (editor.targets == null) return;
            if (editor.targets.Length != 1) return;
            if (WhiteList.Contains(editor.target.GetType().FullName)) return; // 判断资源类型
            try
            {
                if (AssetCollectRoot.ObjToCollector(editor.target, out var result))
                {
                    using (new EditorGUI.DisabledScope(false))
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("包名", GUILayout.MaxWidth(65));
                        EditorGUILayout.TextField(result.PackageName, GUILayout.MinWidth(30));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("组名", GUILayout.MaxWidth(65));
                        EditorGUILayout.TextField(result.GroupName, GUILayout.MinWidth(30));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("收集模式", GUILayout.MaxWidth(65));
                        result.Type = GELayout.Popup(result.Type, GEStyle.PreDropDown);
                        GUILayout.EndHorizontal();
                    }

                    if (result.Type != EAssetCollectItemType.MainAssetCollector) return;

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("寻址规则", GUILayout.MaxWidth(65));
                    result.AddressIndex = GELayout.Popup(result.AddressIndex, AssetCollectSetting.MapAddress.Displays,
                                                         GEStyle.PreDropDown);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("打包规则", GUILayout.MaxWidth(65));
                    result.RulePackIndex = GELayout.Popup(result.RulePackIndex, AssetCollectSetting.MapPacks.Displays,
                                                          GEStyle.PreDropDown);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("打包模式", GUILayout.MaxWidth(65));
                    result.LoadType = GELayout.Popup(result.LoadType, GEStyle.PreDropDown);
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("寻址命名", GUILayout.MaxWidth(65));
                    if (Config.LoadPathToLower)
                    {
                        using (new EditorGUI.DisabledScope(false))
                            result.LocationFormat = GELayout.Popup(EAssetLocationFormat.ToLower, GEStyle.PreDropDown);
                    }
                    else
                    {
                        result.LocationFormat = GELayout.Popup(result.LocationFormat, GEStyle.PreDropDown);
                    }

                    if (Config.HasExtension)
                    {
                        using (new EditorGUI.DisabledScope(false))
                            result.HasExtension = GELayout.ToggleLeft("后缀", true, GTOptions.Width(42));
                    }
                    else
                    {
                        result.HasExtension = GELayout.ToggleLeft("后缀", result.HasExtension, GTOptions.Width(42));
                    }

                    GUILayout.EndHorizontal();
                }
                else
                {
                    ShowRulePath(AssetCollectRoot.ObjToAddress(editor.target, Config.LoadPathToLower, Config.HasExtension));
                }
            }
            catch
            {
                // ignored
            }
        }

        private static void ShowRulePath(Tuple<string, string, string> ruleAsset)
        {
            if (string.IsNullOrEmpty(ruleAsset.Item1)) return;
            if (string.IsNullOrEmpty(ruleAsset.Item2)) return;
            if (string.IsNullOrEmpty(ruleAsset.Item3)) return;
            DrawAddressPath(ruleAsset.Item3);
        }

        private static void DrawAddressPath(string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Address", GTOptions.MaxWidth(50));
            using (new EditorGUI.DisabledScope(false))
            {
                EditorGUILayout.TextField(value);
            }

            if (GUILayout.Button("Copy", GTOptions.Width(43))) GEHelper.CopyAction(value);
            GUILayout.EndHorizontal();
        }
    }
}