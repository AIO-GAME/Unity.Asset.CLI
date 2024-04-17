using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor.Test
{
    [GWindow("测试窗口", Menu = "AIO/测试窗口")]
    public class TestWindow : GraphicWindow
    {
        private Rect            TempRect;
        private ViewTreePackage VT_Package;
        private ViewTreeGroup   VT_Group;

        /// <inheritdoc />
        protected override void OnActivation()
        {
            if (VT_Package is null)
            {
                VT_Package                    =  ViewTreePackage.Create();
                VT_Package.OnSelectionChanged += id => { VT_Group.Reload(); };
            }
            else VT_Package.Reload();

            if (VT_Group is null) VT_Group = ViewTreeGroup.Create();
            else VT_Group.Reload();
        }

        /// <inheritdoc />
        protected override void OnDraw()
        {
            TempRect = new Rect(0, 0, position.width / 4, position.height);
            VT_Package.OnGUI(TempRect);
            TempRect.x += TempRect.width;
            VT_Group.OnGUI(TempRect);
        }
    }
}