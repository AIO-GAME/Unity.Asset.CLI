using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor.Test
{
    [GWindow("测试窗口", Menu = "AIO/测试窗口")]
    public class TestWindow : GraphicWindow
    {
        TreeView m_TreeEditor;

        /// <inheritdoc />
        protected override void OnActivation()
        {
            if (m_TreeEditor is null) m_TreeEditor = PackageTreeEditor.Create();
            else m_TreeEditor.Reload();
        }

        /// <inheritdoc />
        protected override void OnDraw()
        {
            m_TreeEditor.OnGUI(new Rect(0, 0, position.width, position.height));
        }
    }
}