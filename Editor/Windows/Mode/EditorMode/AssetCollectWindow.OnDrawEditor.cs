namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawEditorMode()
        {
            DoEditorDrawRect.x = 5;
            DoEditorDrawRect.height = CurrentHeight - DrawHeaderHeight;

            var temp = CurrentWidth - ViewCollectorsList.MinWidth;
            if (ViewPackageList.IsShow && ViewPackageList.IsAllowHorizontal) temp -= ViewPackageList.MinWidth;
            if (ViewGroupList.IsShow && ViewGroupList.IsAllowHorizontal) temp -= ViewGroupList.MinWidth;

            DoEditorDrawRect.width = ViewPackageList.width;
            ViewPackageList.Draw(DoEditorDrawRect, () =>
            {
                ViewPackageList.MaxWidth = temp;
                OnDrawPackageScroll = GELayout.VScrollView(OnDrawPackage, OnDrawPackageScroll);
                DoEditorDrawRect.x += ViewPackageList.width;
            }, GEStyle.INThumbnailShadow);

            DoEditorDrawRect.width = ViewGroupList.width;
            ViewGroupList.Draw(DoEditorDrawRect, () =>
            {
                ViewGroupList.MaxWidth = temp;
                OnDrawGroupScroll = GELayout.VScrollView(OnDrawGroup, OnDrawGroupScroll);
                DoEditorDrawRect.x += ViewGroupList.width;
            }, GEStyle.INThumbnailShadow);

            DoEditorDrawRect.width = CurrentWidth - DoEditorDrawRect.x - 5;
            ViewCollectorsList.Draw(DoEditorDrawRect,
                () => { OnDrawGroupListScroll = GELayout.VScrollView(OnDrawGroupList, OnDrawGroupListScroll); });
        }


        protected void UpdateDataEditorMode()
        {
            if (Data.CurrentGroup.Length > 0)
            {
                CurrentCurrentCollectorsIndex = Data.CurrentGroup.Length - 1;
                OnDrawItemListScroll.y = 0;
            }
        }
    }
}