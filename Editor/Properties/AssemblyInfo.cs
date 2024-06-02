#region namespace

using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

[assembly: InternalsVisibleTo("AIO.CLI.YooAsset.Editor")]

#endregion

namespace AIO.UEditor
{
    public static class AssetsEditorSetting
    {
        public const string MENU_ROOT         = "AIO/Asset/";
        public const string MENU_ROOT_SETTING = MENU_ROOT + "Setting/";

        /// <summary>
        ///     名称
        /// </summary>
        public const string Name = "AIO.Asset";

        /// <summary>
        ///     版本
        /// </summary>
        public static string Version { get; private set; }

        #region AutoConversionConfig

        /// <summary>
        /// 是否自动转换配置
        /// </summary>
        public static bool AutoConversionConfig;

        private const string MENU_PATH_AUTO_CONVERSION_CONFIG = MENU_ROOT_SETTING + "自动转换配置";

        [MenuItem(MENU_PATH_AUTO_CONVERSION_CONFIG, true)]
        public static bool MenuAutoConversionConfigValidate()
        {
            Menu.SetChecked(MENU_PATH_AUTO_CONVERSION_CONFIG, AutoConversionConfig);
            return true;
        }

        [MenuItem(MENU_PATH_AUTO_CONVERSION_CONFIG)]
        public static void MenuAutoConversionConfig()
        {
            AutoConversionConfig = !AutoConversionConfig;
            EditorPrefs.SetBool(MENU_PATH_AUTO_CONVERSION_CONFIG, AutoConversionConfig);
            if (AutoConversionConfig) Editor.finishedDefaultHeaderGUI += ResInspectorUI.OnPostHeaderGUI;
            else Editor.finishedDefaultHeaderGUI                      -= ResInspectorUI.OnPostHeaderGUI;
        }

        #endregion

        #region ShowInspectorAddress

        /// <summary>
        /// 显示检视器地址
        /// </summary>
        public static bool ShowInspectorAddress;

        [MenuItem(MENU_PATH_SHOW_INSPECTOR_ADDRESS, true)]
        public static bool MenuShowInspectorAddressValidate()
        {
            Menu.SetChecked(MENU_PATH_SHOW_INSPECTOR_ADDRESS, ShowInspectorAddress);
            return true;
        }

        private const string MENU_PATH_SHOW_INSPECTOR_ADDRESS = MENU_ROOT_SETTING + "显示检视器地址";

        [MenuItem(MENU_PATH_SHOW_INSPECTOR_ADDRESS)]
        public static void MenuShowInspectorAddress()
        {
            ShowInspectorAddress = !ShowInspectorAddress;
            EditorPrefs.SetBool(MENU_PATH_SHOW_INSPECTOR_ADDRESS, ShowInspectorAddress);
            if (ShowInspectorAddress) Editor.finishedDefaultHeaderGUI += ResInspectorUI.OnPostHeaderGUI;
            else Editor.finishedDefaultHeaderGUI                      -= ResInspectorUI.OnPostHeaderGUI;
        }

        #endregion

        [AInit(EInitAttrMode.Both, int.MaxValue)]
        private static void Initialize()
        {
            ShowInspectorAddress = EditorPrefs.GetBool(MENU_PATH_SHOW_INSPECTOR_ADDRESS, true);
            AutoConversionConfig = EditorPrefs.GetBool(MENU_PATH_AUTO_CONVERSION_CONFIG, true);

            var package     = PackageInfo.FindForAssembly(typeof(AssetsEditorSetting).Assembly);
            var packageJson = AHelper.IO.ReadJsonUTF8<JObject>(string.Concat(package.resolvedPath, "/package.json"));
            Version = packageJson.Value<string>("version");
        }

        private static SettingsProvider provider;

        /// <summary>
        /// 创建设置提供者
        /// </summary>
        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            if (provider != null) return provider;

            provider       = new GraphicSettingsProvider("AIO/Asset System", SettingsScope.User);
            provider.label = "Asset System";
            provider.guiHandler = delegate
            {
                GUILayout.BeginVertical();
                GUILayout.Space(10);
                GUILayout.Label("Settings", EditorStyles.boldLabel);
                GUILayout.Space(5);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("显示检视器地址");
                    if (GUILayout.Button(ShowInspectorAddress ? "开启" : "关闭", EditorStyles.miniButton, GUILayout.Width(50)))
                        MenuShowInspectorAddress();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("自动转换配置");
                    if (GUILayout.Button(AutoConversionConfig ? "开启" : "关闭", EditorStyles.miniButton, GUILayout.Width(50)))
                        MenuAutoConversionConfig();
                }

                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField($"Version {Version}", EditorStyles.centeredGreyMiniLabel);
            };
            return provider;
        }
    }
}