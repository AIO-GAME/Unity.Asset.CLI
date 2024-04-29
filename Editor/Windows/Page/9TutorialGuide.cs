using UnityEngine;

namespace AIO.UEditor
{
    internal class TutorialGuide : IAssetPage
    {
        public TutorialGuide() { }

        public void   Dispose() { }
        public string Title     => "教程指南      [Ctrl + Number9]";
        public int    Order     => 9;

        bool IAssetPage.Shortcut(Event evt) =>
            evt.control && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Keypad9 || evt.keyCode == KeyCode.Alpha9);

        public void OnDrawContent(Rect rect)
        {
            rect.x     += 5;
            rect.width -= 10;
            GUI.Box(rect, string.Empty, GEStyle.INThumbnailShadow);
        }

        public void UpdateData()                                     { }
        public void OnDrawHeader(Rect       rect)                    { }
        public void EventMouseDrag(in Event evt)                     { }
        public void EventMouseDown(in Event evt)                     { }
        public void EventMouseUp(in   Event evt)                     { }
        public void EventKeyDown(in   Event evt, in KeyCode keyCode) { }
        public void EventKeyUp(in     Event evt, in KeyCode keyCode) { }
    }
}