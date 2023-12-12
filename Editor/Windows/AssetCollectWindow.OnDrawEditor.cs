/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawEditorMode()
        {
            DoDrawRect.x = 5;
            DoDrawRect.height = CurrentHeight - DrawHeaderHeight;

            var temp = CurrentWidth - ViewCollectorsList.MinWidth;
            if (ViewConfig.IsShow && ViewConfig.IsAllowHorizontal) temp -= ViewConfig.MinWidth;
            if (ViewSetting.IsShow && ViewSetting.IsAllowHorizontal) temp -= ViewSetting.MinWidth;
            if (ViewPackageList.IsShow && ViewPackageList.IsAllowHorizontal) temp -= ViewPackageList.MinWidth;
            if (ViewGroupList.IsShow && ViewGroupList.IsAllowHorizontal) temp -= ViewGroupList.MinWidth;

            DoDrawRect.width = ViewConfig.width;
            ViewConfig.Draw(DoDrawRect, () =>
            {
                ViewConfig.MaxWidth = temp;
                OnDrawConfigScroll = GELayout.VScrollView(OnDrawASConfig, OnDrawConfigScroll);
                DoDrawRect.x += ViewConfig.width;
            }, GEStyle.INThumbnailShadow);

            DoDrawRect.width = ViewSetting.width;
            ViewSetting.Draw(DoDrawRect, () =>
            {
                ViewSetting.MaxWidth = temp;
                OnDrawSettingScroll = GELayout.VScrollView(OnDrawSetting, OnDrawSettingScroll);
                DoDrawRect.x += ViewSetting.width;
            }, GEStyle.INThumbnailShadow);

            DoDrawRect.width = ViewPackageList.width;
            ViewPackageList.Draw(DoDrawRect, () =>
            {
                ViewPackageList.MaxWidth = temp;
                OnDrawPackageScroll = GELayout.VScrollView(OnDrawPackage, OnDrawPackageScroll);
                DoDrawRect.x += ViewPackageList.width;
            }, GEStyle.INThumbnailShadow);

            DoDrawRect.width = ViewGroupList.width;
            ViewGroupList.Draw(DoDrawRect, () =>
            {
                ViewGroupList.MaxWidth = temp;
                OnDrawGroupScroll = GELayout.VScrollView(OnDrawGroup, OnDrawGroupScroll);
                DoDrawRect.x += ViewGroupList.width;
            }, GEStyle.INThumbnailShadow);

            DoDrawRect.width = CurrentWidth - DoDrawRect.x - 5;
            ViewCollectorsList.Draw(DoDrawRect,
                () => { OnDrawGroupListScroll = GELayout.VScrollView(OnDrawGroupList, OnDrawGroupListScroll); });
        }
    }
}