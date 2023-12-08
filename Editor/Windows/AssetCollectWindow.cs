/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源管理窗口
    /// </summary>
    [GWindow("资源收集管理器", "支持资源收集、资源管理、资源导出、资源打包等功能",
        Group = "Tools",
        Menu = "AIO/Window/Asset",
        MinSizeHeight = 500,
        MinSizeWidth = 1000
    )]
    public partial class AssetCollectWindow : GraphicWindow
    {
        [LnkTools("Asset Window", "#00BFFF", "d_Folder Icon", LnkToolsMode.AllMode, -5)]
        public static void OpenWindow()
        {
            EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
        }


        /// <summary>
        /// 资源收集根节点
        /// </summary>
        public AssetCollectRoot Data;

        /// <summary>
        /// 资源系统配置
        /// </summary>
        public ASConfig Config;

        /// <summary>
        /// 资源系统打包配置
        /// </summary>
        public ASBuildConfig BuildConfig;

        private const int ButtonWidth = 75;

        protected override void OnAwake()
        {
            Data = AssetCollectRoot.GetOrCreate();
            Selection.activeObject = Data;
        }

        private GUIContent GC_ADD;
        private GUIContent GC_DEL;
        private GUIContent GC_OPEN;
        private GUIContent GC_REFRESH;
        private GUIContent GC_COPY;
        private GUIContent GC_SELECT;
        private GUIContent GC_SAVE;

        protected override void OnActivation()
        {
            if (Data is null) Data = AssetCollectRoot.GetOrCreate();
            Data.Save();

            for (var i = 0; i < Data.Packages.Length; i++)
            {
                if (Data.Packages[CurrentPackageIndex] is null) continue;
                if (Data.Packages[CurrentPackageIndex].Groups is null) continue;
                for (var j = 0; j < Data.Packages[CurrentPackageIndex].Groups.Length; j++)
                {
                    // 根据Group的Name进行排序 从小到大
                    Data.Packages[CurrentPackageIndex].Groups = Data.Packages[CurrentPackageIndex].Groups.OrderByDescending(
                        group => group.Name).ToArray();
                }
            }

            Data.Packages = Data.Packages.OrderByDescending(package => package.Name).ToArray();

            if (Config is null) Config = ASConfig.GetOrCreate();
            Config.Save();

            if (BuildConfig is null) BuildConfig = ASBuildConfig.GetOrCreate();
            BuildConfig.Save();

            AssetCollectSetting.Initialize();

            GC_SELECT = new GUIContent("☈", "选择指向指定资源");
            GC_ADD = EditorGUIUtility.IconContent("d_CreateAddNew");
            GC_DEL = new GUIContent("✘", "删除元素");
            GC_REFRESH = new GUIContent("↺", "刷新数据");
            GC_OPEN = new GUIContent("☑", "打开");
            GC_COPY = new GUIContent("❒", "复制资源路径");
            GC_SAVE = EditorGUIUtility.IconContent("d_SaveAs");

            if (_packages is null)
                _packages = Config.Packages is null
                    ? new List<AssetsPackageConfig>()
                    : Config.Packages.ToList();

            switch (LookMode)
            {
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

        private int WidthOffset = 0;
        private int CurrentPackageIndex = 0;
        private int CurrentGroupIndex = 0;

        private int DrawListWidth = 400;
        private int DrawSettingWidth = 150;
        private int DrawPackageWidth = 150;
        private int DrawGroupWidth = 150;
        private int DrawHeaderHeight = 25;

        partial void OnDrawBuild();
        partial void OnDrawLook();

        protected void OnDrawNoLook()
        {
            var height = CurrentHeight - DrawHeaderHeight;
            WidthOffset = 5;
            if (ShowSetting)
            {
                GULayout.BeginArea(new Rect(WidthOffset, DrawHeaderHeight, DrawSettingWidth - 5, height),
                    GEStyle.INThumbnailShadow);

                OnDrawSettingScroll = GELayout.VScrollView(OnDrawSetting, OnDrawSettingScroll);
                GULayout.EndArea();
                WidthOffset += DrawSettingWidth;
            }

            if (ShowPackage)
            {
                GULayout.BeginArea(new Rect(WidthOffset, DrawHeaderHeight, DrawPackageWidth - 5, height),
                    GEStyle.INThumbnailShadow);

                OnDrawPackageScroll = GELayout.VScrollView(OnDrawPackage, OnDrawPackageScroll);
                GULayout.EndArea();
                WidthOffset += DrawPackageWidth;
            }

            if (ShowGroup)
            {
                GULayout.BeginArea(new Rect(WidthOffset, DrawHeaderHeight, DrawGroupWidth - 5, height),
                    GEStyle.INThumbnailShadow);

                OnDrawGroupScroll = GELayout.VScrollView(OnDrawGroup, OnDrawGroupScroll);
                GULayout.EndArea();
                WidthOffset += DrawGroupWidth;
            }

            GULayout.BeginArea(new Rect(
                WidthOffset, DrawHeaderHeight,
                CurrentWidth - WidthOffset - 5 - (ShowList ? DrawListWidth : 0), height));
            OnDrawGroupListScroll = GELayout.VScrollView(OnDrawGroupList, OnDrawGroupListScroll);
            GULayout.EndArea();

            if (ShowList)
            {
                GULayout.BeginArea(new Rect(CurrentWidth - 5 - DrawListWidth,
                    DrawHeaderHeight, DrawListWidth, height), GEStyle.INThumbnailShadow);
                using (GELayout.Vertical(GEStyle.INThumbnailShadow))
                {
                    using (GELayout.VHorizontal())
                    {
                        GELayout.Label($"[{OnDrawCurrentItem.Type}] Count: {OnDrawCurrentItem.AssetDataInfos.Count}");
                        GELayout.Separator();

                        if (GELayout.Button(GC_REFRESH, 24))
                        {
                            if (Data.Packages[CurrentPackageIndex] is null ||
                                Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex] is null)
                            {
                                OnDrawCurrentItem = null;
                                return;
                            }

                            OnDrawCurrentItem.CollectAsset(
                                Data.Packages[CurrentPackageIndex].Name,
                                Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Name);
                            GUI.FocusControl(null);
                            return;
                        }

                        if (GELayout.Button(GC_DEL, 24))
                        {
                            OnDrawCurrentItem = null;
                            GUI.FocusControl(null);
                            return;
                        }
                    }

                    using (GELayout.VHorizontal())
                    {
                        GELayout.Label(OnDrawCurrentItem.CollectPath, GEStyle.MiniLabel);
                    }

                    if (OnDrawCurrentItem.AssetDataInfos.Count > 20)
                    {
                        using (GELayout.VHorizontal())
                        {
                            GELayout.Label("显示数量", GTOption.Width(35)); // 设置页面数量
                            OnDrawCurrentItem.AssetDataInfos.PageSize = GELayout.Slider(
                                OnDrawCurrentItem.AssetDataInfos.PageSize,
                                20, 100);
                        }
                    }

                    if (OnDrawCurrentItem.AssetDataInfos.PageSize < OnDrawCurrentItem.AssetDataInfos.Count)
                    {
                        using (GELayout.VHorizontal())
                        {
                            GELayout.Label("当前页数", GTOption.Width(35)); // 设置页面滑动条

                            OnDrawCurrentItem.AssetDataInfos.PageIndex = GELayout.Slider(
                                OnDrawCurrentItem.AssetDataInfos.PageIndex,
                                0, OnDrawCurrentItem.AssetDataInfos.PageCount - 1);
                        }
                    }
                }

                OnDrawListScroll = GELayout.VScrollView(OnDrawList, OnDrawListScroll);
                GULayout.EndArea();
            }
        }

        protected override void OnDraw()
        {
            GELayout.VHorizontal(OnDrawHeader, GEStyle.INThumbnailShadow, GTOption.Height(DrawHeaderHeight - 5));
            switch (LookMode)
            {
                case Mode.Editor:
                    OnDrawNoLook();
                    break;
                case Mode.Look:
                    OnDrawLook();
                    break;
                case Mode.Build:
                    OnDrawBuild();
                    break;
            }

            DrawVersion(Setting.Version);
            OnOpenEvent();
        }

        private Vector2 OnDrawSettingScroll = Vector2.zero;
        private Vector2 OnDrawPackageScroll = Vector2.zero;
        private Vector2 OnDrawGroupScroll = Vector2.zero;
        private Vector2 OnDrawGroupListScroll = Vector2.zero;
        private Vector2 OnDrawListScroll = Vector2.zero;
        private Vector2 OnDrawLookDataScroll = Vector2.zero;

        partial void OnDrawHeader();
        partial void OnDrawSetting();
        partial void OnDrawPackage();
        partial void OnDrawGroup();
        partial void OnDrawGroupList();
        partial void OnDrawItem(AssetCollectItem item);
        partial void OnDrawList();
        partial void OnDrawASConfig();

        protected override void OnDisable()
        {
            Data.Save();
        }

        protected override void OnDispose()
        {
            Data.Save();
        }
    }
}