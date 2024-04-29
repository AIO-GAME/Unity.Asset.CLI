using UnityEngine;

namespace AIO.UEditor
{
    internal class AssetPageUploadGoogleCloud : IAssetPage
    {
        private TreeViewUploadGoogleCloud TreeViewUpload;

        public AssetPageUploadGoogleCloud() { TreeViewUpload = TreeViewUploadGoogleCloud.Create(AssetBuildConfig.GetOrCreate().GCloudConfigs); }

        public void   Dispose() { AssetBuildConfig.GetOrCreate().GCloudConfigs = TreeViewUpload.Data; }
        public string Title     => "上传谷歌云  [Ctrl + Number7]";
        public int    Order     => 7;

        bool IAssetPage.Shortcut(Event evt) =>
            evt.control && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Keypad7 || evt.keyCode == KeyCode.Alpha7);

        public void OnDrawContent(Rect rect)
        {
            rect.x     += 5;
            rect.width -= 10;
            GUI.Box(rect, string.Empty, GEStyle.INThumbnailShadow);
            TreeViewUpload.OnGUI(rect);
        }

        public void UpdateData()                                     { TreeViewUpload.Reload(); }
        public void OnDrawHeader(Rect       rect)                    { }
        public void EventMouseDrag(in Event evt)                     { }
        public void EventMouseDown(in Event evt)                     { }
        public void EventMouseUp(in   Event evt)                     { }
        public void EventKeyDown(in   Event evt, in KeyCode keyCode) { }
        public void EventKeyUp(in     Event evt, in KeyCode keyCode) { }
    }
}