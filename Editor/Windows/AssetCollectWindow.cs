/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

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

        private const int ButtonWidth = 75;

        protected override void OnAwake()
        {
            Data = AssetCollectRoot.GetOrCreate();
        }

        protected override void OnActivation()
        {
            if (Data is null) Data = AssetCollectRoot.GetOrCreate();
            Data.Save();
        }

        private int WidthOffset = 0;
        private int CurrentPackageIndex = 0;
        private int CurrentGroupIndex = 0;
        private const int DrawSettingWidth = 200;
        private const int DrawPackageWidth = 200;
        private const int DrawGroupWidth = 200;
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

            GULayout.BeginArea(new Rect(WidthOffset, DrawHeaderHeight, CurrentWidth - WidthOffset - 5, height));
            OnDrawGroupListScroll = GELayout.VScrollView(OnDrawGroupList, OnDrawGroupListScroll, false, false);
            GULayout.EndArea();

            DrawVersion(Setting.Version);
        }

        private Vector2 OnDrawSettingScroll = Vector2.zero;
        private Vector2 OnDrawPackageScroll = Vector2.zero;
        private Vector2 OnDrawGroupScroll = Vector2.zero;
        private Vector2 OnDrawGroupListScroll = Vector2.zero;

        partial void OnDrawHeader();
        partial void OnDrawSetting();
        partial void OnDrawPackage();
        partial void OnDrawGroup();
        partial void OnDrawGroupList();
        partial void OnDrawItem(AssetCollectItem item);

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