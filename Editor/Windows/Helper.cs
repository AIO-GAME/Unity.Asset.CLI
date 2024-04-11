using System;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

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

        private static void OnPostHeaderGUI(Editor editor)
        {
            if (editor.targets.Length == 1)
            {
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
                        result.Address = GELayout.Popup(result.Address, AssetCollectSetting.MapAddress.Displays,
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
                        if (ASConfig.GetOrCreate().LoadPathToLower)
                        {
                            GUI.enabled           = false;
                            result.LocationFormat = GELayout.Popup(EAssetLocationFormat.ToLower, GEStyle.PreDropDown);
                            GUI.enabled           = true;
                        }
                        else result.LocationFormat = GELayout.Popup(result.LocationFormat, GEStyle.PreDropDown);

                        if (ASConfig.GetOrCreate().HasExtension)
                        {
                            GUI.enabled         = false;
                            result.HasExtension = GELayout.ToggleLeft("后缀", true, GTOption.Width(42));
                            GUI.enabled         = true;
                        }
                        else result.HasExtension = GELayout.ToggleLeft("后缀", result.HasExtension, GTOption.Width(42));

                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        ShowRulePath(AssetCollectRoot.ObjToAddress(editor.target));
                    }
                }
                catch
                {
                    // ignored
                }
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