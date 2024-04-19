namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawEditorMode()
        {
            DoEditorDrawRect.x   = 5;
            ViewGroupList.height = ViewCollectorsList.height = ViewPackageList.height = DoEditorDrawRect.height = CurrentHeight - DrawHeaderHeight;
            var temp = CurrentWidth - ViewCollectorsList.MinWidth;
            if (ViewPackageList.IsShow && ViewPackageList.IsAllowHorizontal) temp -= ViewPackageList.MinWidth;
            if (ViewGroupList.IsShow && ViewGroupList.IsAllowHorizontal) temp     -= ViewGroupList.MinWidth;
            
            ViewGroupList.y      = ViewCollectorsList.y      = ViewPackageList.y      = DoEditorDrawRect.y;

            DoEditorDrawRect.width   = ViewPackageList.width;
            ViewPackageList.MaxWidth = temp;
            ViewPackageList.x        = DoEditorDrawRect.x;
            ViewPackageList.Draw(ViewTreePackage.OnGUI, GEStyle.INThumbnailShadow);
            
            DoEditorDrawRect.x += ViewPackageList.width;
            DoEditorDrawRect.width = ViewGroupList.width;
            
            ViewGroupList.MaxWidth = temp;
            ViewGroupList.x        = DoEditorDrawRect.x;
            ViewGroupList.Draw(ViewTreeGroup.OnGUI, GEStyle.INThumbnailShadow);
            
            DoEditorDrawRect.x       += ViewGroupList.width;
            DoEditorDrawRect.width   =  CurrentWidth - DoEditorDrawRect.x - 5;
            
            ViewCollectorsList.width =  DoEditorDrawRect.width;
            ViewCollectorsList.x     =  DoEditorDrawRect.x;
            ViewCollectorsList.Draw(ViewTreeCollector.OnGUI);
        }

        protected void UpdateDataEditorMode() { }
    }
}