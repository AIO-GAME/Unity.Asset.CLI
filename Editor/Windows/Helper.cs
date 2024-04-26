#region

using System;
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
    public class ResInspectorUI
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

        private static void OnPostHeaderGUI(Editor editor)
        {
            if (editor.targets.Length != 1) return;
            switch (editor.target)
            {
                case AssetCollectRoot _:
                case ASConfig _:
                case AssetBuildConfig _:
                case ConsoleWindowConfig _:
#if SUPPORT_YOOASSET
                case AssetBundleCollectorSetting _:
                case ShaderVariantCollectorSetting _:
#endif
                    return;
                default:

                    try
                    {
                        if (AssetCollectRoot.ObjToCollector(editor.target, out var result))
                        {
                            GUI.enabled = false;
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

                            GUI.enabled = true;

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
                                GUI.enabled           = false;
                                result.LocationFormat = GELayout.Popup(EAssetLocationFormat.ToLower, GEStyle.PreDropDown);
                                GUI.enabled           = true;
                            }
                            else
                            {
                                result.LocationFormat = GELayout.Popup(result.LocationFormat, GEStyle.PreDropDown);
                            }

                            if (Config.HasExtension)
                            {
                                GUI.enabled         = false;
                                result.HasExtension = GELayout.ToggleLeft("后缀", true, GTOptions.Width(42));
                                GUI.enabled         = true;
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

                    break;
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
            GUILayout.Label("Address", GUILayout.MaxWidth(50));
            GUI.enabled = false;
            EditorGUILayout.TextField(value);
            GUI.enabled = true;
            if (GUILayout.Button("Copy", GUILayout.Width(43))) GEHelper.CopyAction(value);
            GUILayout.EndHorizontal();
        }
    }
}