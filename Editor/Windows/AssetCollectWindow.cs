/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
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

        private const int ButtonWidth = 75;

        protected override void OnAwake()
        {
        }

        protected override void OnActivation()
        {
            if (Data is null) Data = AssetCollectRoot.GetOrCreate();
            Data.Save();
            AssetCollectSetting.Initialize();
        }

        private int WidthOffset = 0;
        private int CurrentPackageIndex = 0;
        private int CurrentGroupIndex = 0;
        private const int DrawListWidth = 400;
        private const int DrawSettingWidth = 150;
        private const int DrawPackageWidth = 150;
        private const int DrawGroupWidth = 150;
        private const int DrawHeaderHeight = 25;

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
                using (GELayout.VHorizontal(GEStyle.DDHeaderStyle))
                {
                    GELayout.Label(
                        $"[{OnDrawCurrentItem.Type}] Collector : {OnDrawCurrentItem.CollectorPath} Count: {OnDrawCurrentItem.AssetDataInfos.Count}");
                    if (GELayout.Button("刷新", 50))
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

                    if (GELayout.Button("关闭", 50))
                    {
                        OnDrawCurrentItem = null;
                        return;
                    }
                }

                using (GELayout.VHorizontal()) // 设置页面滑动条
                {
                    GELayout.Label("Page Index : ");
                    OnDrawCurrentItem.AssetDataInfos.PageIndex = GELayout.Slider(
                        OnDrawCurrentItem.AssetDataInfos.PageIndex,
                        1, OnDrawCurrentItem.AssetDataInfos.PageCount - 1);
                }

                using (GELayout.VHorizontal()) // 设置页面数量
                {
                    GELayout.Label("Page Count : ");
                    OnDrawCurrentItem.AssetDataInfos.PageSize = GELayout.Slider(
                        OnDrawCurrentItem.AssetDataInfos.PageSize,
                        20, 100);
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