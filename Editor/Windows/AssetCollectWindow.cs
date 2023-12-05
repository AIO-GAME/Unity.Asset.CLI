﻿/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

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
        Menu = "AIO/Asset Window",
        MinSizeHeight = 500,
        MinSizeWidth = 1000
    )]
    public partial class AssetCollectWindow : GraphicWindow
    {
        /// <summary>
        /// 资源收集根节点
        /// </summary>
        public AssetCollectRoot Data;

        public ASConfig Config;

        private const int ButtonWidth = 75;

        protected override void OnAwake()
        {
            Data = AssetCollectRoot.GetOrCreate();
            Selection.activeObject = Data;
        }

        private GUIContent Content_ADD;
        private GUIContent Content_DEL;
        private GUIContent Content_OPEN;
        private GUIContent Content_REFRESH;
        private GUIContent Content_COPY;
        private GUIContent Content_SELECT;

        protected override void OnActivation()
        {
            if (Data is null) Data = AssetCollectRoot.GetOrCreate();
            Data.Save();
            
            if (Config is null) Config = ASConfig.GetOrCreate();
            Config.Save();
            
            AssetCollectSetting.Initialize();

            Content_SELECT = new GUIContent("☈", "选择指向指定资源");
            Content_ADD = new GUIContent("✚", "添加元素");
            Content_DEL = new GUIContent("✘", "删除元素");
            Content_REFRESH = new GUIContent("↺", "刷新内容");
            Content_OPEN = new GUIContent("☑", "打开资源管理界面");
            Content_COPY = new GUIContent("❒", "复制资源路径");
            
            if (_packages is null)
                _packages = Config.Packages is null
                    ? new List<AssetsPackageConfig>()
                    : Config.Packages.ToList();

            UpdateRecordQueue();
        }

        private int WidthOffset = 0;
        private int CurrentPackageIndex = 0;
        private int CurrentGroupIndex = 0;

        private int DrawListWidth = 400;
        private int DrawSettingWidth = 150;
        private int DrawPackageWidth = 150;
        private int DrawGroupWidth = 150;
        private int DrawHeaderHeight = 25;

        protected override void OnDraw()
        {
            GELayout.VHorizontal(OnDrawHeader, GEStyle.INThumbnailShadow, GTOption.Height(DrawHeaderHeight - 5));

            var height = CurrentHeight - DrawHeaderHeight;
            WidthOffset = 5;
            if (ShowSetting)
            {
                GULayout.BeginArea(new Rect(WidthOffset, DrawHeaderHeight, DrawSettingWidth - 5, height),
                    GEStyle.INThumbnailShadow);

                OnDrawSettingScroll = GELayout.VScrollView(OnDrawSetting, OnDrawSettingScroll, false, false);
                GULayout.EndArea();
                WidthOffset += DrawSettingWidth;
            }

            if (ShowPackage)
            {
                GULayout.BeginArea(new Rect(WidthOffset, DrawHeaderHeight, DrawPackageWidth - 5, height),
                    GEStyle.INThumbnailShadow);

                OnDrawPackageScroll = GELayout.VScrollView(OnDrawPackage, OnDrawPackageScroll, false, false);
                GULayout.EndArea();
                WidthOffset += DrawPackageWidth;
            }

            if (ShowGroup)
            {
                GULayout.BeginArea(new Rect(WidthOffset, DrawHeaderHeight, DrawGroupWidth - 5, height),
                    GEStyle.INThumbnailShadow);

                OnDrawGroupScroll = GELayout.VScrollView(OnDrawGroup, OnDrawGroupScroll, false, false);
                GULayout.EndArea();
                WidthOffset += DrawGroupWidth;
            }

            GULayout.BeginArea(new Rect(
                WidthOffset, DrawHeaderHeight,
                CurrentWidth - WidthOffset - 5 - (ShowList ? DrawListWidth : 0), height));
            OnDrawGroupListScroll = GELayout.VScrollView(OnDrawGroupList, OnDrawGroupListScroll, false, false);
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

                        if (GELayout.Button(Content_REFRESH, 24))
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
                            return;
                        }

                        if (GELayout.Button(Content_DEL, 24))
                        {
                            OnDrawCurrentItem = null;
                            return;
                        }
                    }

                    using (GELayout.VHorizontal())
                    {
                        GELayout.Label($"{OnDrawCurrentItem.CollectPath}", GEStyle.MiniLabel);
                    }

                    if (OnDrawCurrentItem.AssetDataInfos.Count > 20)
                    {
                        using (GELayout.VHorizontal())
                        {
                            GELayout.Label("Size", GTOption.Width(35)); // 设置页面数量
                            OnDrawCurrentItem.AssetDataInfos.PageSize = GELayout.Slider(
                                OnDrawCurrentItem.AssetDataInfos.PageSize,
                                20, 100);
                        }
                    }

                    if (OnDrawCurrentItem.AssetDataInfos.PageSize < OnDrawCurrentItem.AssetDataInfos.Count)
                    {
                        using (GELayout.VHorizontal())
                        {
                            GELayout.Label("Index", GTOption.Width(35)); // 设置页面滑动条

                            OnDrawCurrentItem.AssetDataInfos.PageIndex = GELayout.Slider(
                                OnDrawCurrentItem.AssetDataInfos.PageIndex,
                                0, OnDrawCurrentItem.AssetDataInfos.PageCount - 1);
                        }
                    }
                }

                OnDrawListScroll = GELayout.VScrollView(OnDrawList, OnDrawListScroll,
                    false, false);
                GULayout.EndArea();
            }

            DrawVersion(Setting.Version);
        }

        private Vector2 OnDrawSettingScroll = Vector2.zero;
        private Vector2 OnDrawPackageScroll = Vector2.zero;
        private Vector2 OnDrawGroupScroll = Vector2.zero;
        private Vector2 OnDrawGroupListScroll = Vector2.zero;
        private Vector2 OnDrawListScroll = Vector2.zero;

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