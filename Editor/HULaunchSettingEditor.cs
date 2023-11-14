/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if UNITY_EDITOR && SUPPORT_YOOASSET

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using AIO.UEngine;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    [CustomEditor(typeof(HULaunchSetting))]
    public class HULaunchSettingEditor : Editor
    {
        [MenuItem("Setting/Create/Hot Update Setting")]
        public static void Create()
        {
            if (!AssetDatabase.IsMainAssetAtPathLoaded(HULaunchSetting.Path))
            {
                var setting = CreateInstance<HULaunchSetting>();
                AssetDatabase.CreateAsset(setting, HULaunchSetting.Path);
                Selection.activeObject = setting;
            }
            else
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<HULaunchSetting>(HULaunchSetting.Path);
            }
        }

        public static HULaunchSetting Get()
        {
            HULaunchSetting setting = null;
            foreach (var item in Resources.LoadAll<HULaunchSetting>(""))
            {
                setting = item;
                if (setting != null) break;
            }

            if (setting is null)
            {
                Debug.LogErrorFormat("未读取到热更配置 系统已自动创建配置 -> {0}", HULaunchSetting.Path);
                setting = CreateInstance<HULaunchSetting>();
                // setting.ConfigList = new List<AssetsPackageConfig>();
                // foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
                // {
                //     var ismain = package.PackageName == "main";
                //     setting.ConfigList.Add(new AssetsPackageConfig { Name = package.PackageName, Version = "0.0.1", IsDefault = ismain });
                // }


                foreach (var item in Resources.LoadAll<HUpdatePanel>(""))
                {
                    if (item is null) continue;
                    setting.HotUpdatePanelPath = AssetDatabase.GetAssetPath(item);
                    if (!string.IsNullOrEmpty(setting.HotUpdatePanelPath))
                    {
                        setting.HotUpdatePanelPath = setting.HotUpdatePanelPath.Replace("Assets/Resources/", "").Replace(".prefab", "");
                        break;
                    }
                }

                AssetDatabase.CreateAsset(setting, HULaunchSetting.Path);
            }

            return setting;
        }

        private HULaunchSetting setting;
        private Object hotUpdatePanel;
        private SystemLanguage language;
        private Dictionary<EASEventType, string> infokeys;

        private void OnEnable()
        {
            language = Application.systemLanguage;
            setting = serializedObject.targetObject as HULaunchSetting;
            if (!string.IsNullOrEmpty(setting.EventLanguageDictionaryJson))
                setting.EventLanguageDictionary = JsonConvert.DeserializeObject<Dictionary<SystemLanguage, Dictionary<EASEventType, string>>>(setting.EventLanguageDictionaryJson);
            if (setting.EventLanguageDictionary is null)
                setting.EventLanguageDictionary = new Dictionary<SystemLanguage, Dictionary<EASEventType, string>>();

            // if (setting.ConfigList is null)
            //     setting.ConfigList = new List<AssetsPackageConfig>();

            infokeys = new Dictionary<EASEventType, string>();
            foreach (EASEventType value in Enum.GetValues(typeof(EASEventType)))
            {
                var de = value.GetAttribute<DescriptionAttribute>();
                infokeys.Add(value, de.Description);
            }
        }

        private static readonly string[] modenames = new string[] { "编译器模式", "远端资源模式", "离线本地模式" };

        public override void OnInspectorGUI()
        {
            GELayout.Label("热更新设置", GEStyle.DDHeaderStyle, GTOption.Height(25));

            GELayout.Space();

            GELayout.VHorizontal(() =>
            {
                GELayout.Label("热更新资源模式", GTOption.Width(100));
                setting.EnableHotUpdate = GELayout.Popup(setting.EnableHotUpdate, modenames);
            });

            if (setting.EnableHotUpdate == 1)
            {
                setting.UrlAppendTimestamp = GELayout.ToggleLeft("URL请求附加时间搓", setting.UrlAppendTimestamp);
                setting.EnableSidePlayWithDownload = GELayout.ToggleLeft("开启边玩边下", setting.EnableSidePlayWithDownload);
                setting.DownloadingMaxNumber = GELayout.Slider("资源同时最大下载数量", setting.DownloadingMaxNumber, 1, 100);
                setting.FailedTryAgain = GELayout.Slider("资源下载失败重试次数", setting.FailedTryAgain, 1, 10);
                setting.Timeout = GELayout.Slider("资源下载超时时间", setting.Timeout, 1, 180);

                GELayout.Space();

                GELayout.Label("资源包地址");
                GELayout.VHorizontal(() =>
                {
                    setting.URL = GELayout.Field(setting.URL);
                    if (GELayout.Button("打开", GTOption.Width(40)))
                    {
                        Process.Start(setting.URL);
                    }
                });
            }

            if (setting.EnableHotUpdate != 1)
            {
                setting.UrlAppendTimestamp = false;
                setting.EnableSidePlayWithDownload = false;

                GELayout.Space();

                // GELayout.Field("资源包列表", setting.ConfigList, data =>
                // {
                //     data.Name = GELayout.Field(data.Name);
                //     data.Version = GELayout.Field(data.Version, GTOption.Width(40));
                //     data.IsDefault = GELayout.Field(data.IsDefault, GTOption.Width(15));
                //     if (data.IsDefault)
                //     {
                //         foreach (var item in setting.ConfigList
                //                      .Where(item => item.Name != data.Name))
                //             item.IsDefault = false;
                //     }
                // }, () => new AssetsPackageConfig());
            }

            GELayout.Space();

            hotUpdatePanel = EditorGUILayout.ObjectField("预制件面板", hotUpdatePanel, typeof(HUpdatePanel), false);
            if (hotUpdatePanel != null) setting.HotUpdatePanelPath = AssetDatabase.GetAssetPath(hotUpdatePanel);
            if (!string.IsNullOrEmpty(setting.HotUpdatePanelPath))
            {
                setting.HotUpdatePanelPath = setting.HotUpdatePanelPath.Replace("Assets/Resources/", "").Replace(".prefab", "");
                EditorGUILayout.LabelField("预制件路径", setting.HotUpdatePanelPath);
            }

            GELayout.Space();

            GELayout.Vertical(() =>
            {
                GELayout.VHorizontal(() => { GELayout.Label("事件类型", GEStyle.DropzoneStyle); });
                GELayout.Space();
                language = GELayout.Popup("选择语言", language);
                if (setting.EventLanguageDictionary is null)
                    setting.EventLanguageDictionary = new Dictionary<SystemLanguage, Dictionary<EASEventType, string>>();

                if (!setting.EventLanguageDictionary.ContainsKey(language))
                {
                    var dictionary = new Dictionary<EASEventType, string>();
                    foreach (EASEventType enums in Enum.GetValues(typeof(EASEventType)))
                    {
                        dictionary.Add(enums, "");
                    }

                    setting.EventLanguageDictionary.Add(language, dictionary);
                }

                GELayout.Space();
                foreach (var pair in infokeys)
                {
                    setting.EventLanguageDictionary[language][pair.Key] =
                        GELayout.Field(pair.Value, setting.EventLanguageDictionary[language][pair.Key]);
                }
            }, GEStyle.DDHeaderStyle);
            serializedObject.SetIsDifferentCacheDirty();
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();
        }

        private void OnValidate()
        {
            Save();
        }

        private void OnDisable()
        {
            Save();
        }

        private void OnDestroy()
        {
            Save();
        }

        private void Save()
        {
            serializedObject.FindProperty("EventLanguageDictionaryJson").stringValue = setting.EventLanguageDictionaryJson = JsonConvert.SerializeObject(setting.EventLanguageDictionary);
            serializedObject.SetIsDifferentCacheDirty();
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();
            EditorUtility.SetDirty(setting);
        }
    }
}
#endif
