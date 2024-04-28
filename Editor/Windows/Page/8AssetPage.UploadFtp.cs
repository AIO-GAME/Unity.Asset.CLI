using UnityEngine;

namespace AIO.UEditor
{
    internal class AssetPageUploadFtp : IAssetPage
    {
        private TreeViewUploadFtp TreeView;

        public AssetPageUploadFtp() { TreeView = TreeViewUploadFtp.Create(AssetBuildConfig.GetOrCreate().FTPConfigs); }

        public void   Dispose() { AssetBuildConfig.GetOrCreate().FTPConfigs = TreeView.Data; }
        public string Title     => "上传FTP         [Ctrl + Number8]";
        public int    Order     => 8;

        bool IAssetPage.Shortcut(Event evt) =>
            evt.control && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Keypad8 || evt.keyCode == KeyCode.Alpha8);

        public void OnDrawContent(Rect rect)
        {
            rect.x     += 5;
            rect.width -= 10;
            GUI.Box(rect, string.Empty, GEStyle.INThumbnailShadow);
            TreeView.OnGUI(rect);
        }

        public void UpdateData()                                     { TreeView.Reload(); }
        public void OnDrawHeader(Rect       rect)                    { }
        public void EventMouseDrag(in Event evt)                     { }
        public void EventMouseDown(in Event evt)                     { }
        public void EventMouseUp(in   Event evt)                     { }
        public void EventKeyDown(in   Event evt, in KeyCode keyCode) { }
        public void EventKeyUp(in     Event evt, in KeyCode keyCode) { }
    }
}