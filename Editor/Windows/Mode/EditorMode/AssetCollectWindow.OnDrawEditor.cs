namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawEditorMode()
        {
            DoEditorDrawRect.x      = 5;
            DoEditorDrawRect.height = CurrentHeight - DrawHeaderHeight;

            var temp = CurrentWidth - ViewCollectorsList.MinWidth;
            if (ViewPackageList.IsShow && ViewPackageList.IsAllowHorizontal) temp -= ViewPackageList.MinWidth;
            if (ViewGroupList.IsShow && ViewGroupList.IsAllowHorizontal) temp     -= ViewGroupList.MinWidth;

            DoEditorDrawRect.width = ViewPackageList.width;
            ViewPackageList.Draw(DoEditorDrawRect, () =>
            {
                ViewPackageList.MaxWidth = temp;
                ViewTreePackage.OnGUI(ViewPackageList);
                DoEditorDrawRect.x += ViewPackageList.width;
            }, GEStyle.INThumbnailShadow);

            DoEditorDrawRect.width = ViewGroupList.width;
            ViewGroupList.Draw(DoEditorDrawRect, () =>
            {
                ViewGroupList.MaxWidth = temp;
                ViewTreeGroup.OnGUI(ViewGroupList);
                DoEditorDrawRect.x += ViewGroupList.width;
            }, GEStyle.INThumbnailShadow);

            DoEditorDrawRect.width = CurrentWidth - DoEditorDrawRect.x - 5;
            ViewCollectorsList.Draw(DoEditorDrawRect, () =>
            {
                ViewCollectorsList.width = DoEditorDrawRect.width;
                ViewCurrentCollector.OnGUI(ViewCollectorsList);
            });
        }


        protected void UpdateDataEditorMode()
        {
            if (Data.CurrentGroup.Count > 0)
            {
                CurrentCurrentCollectorsIndex = Data.CurrentGroup.Count - 1;
                OnDrawItemListScroll.y        = 0;
            }
        }
    }
}