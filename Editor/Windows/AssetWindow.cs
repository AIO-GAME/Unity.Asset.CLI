using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    ///     资源管理窗口
    /// </summary>
    [GWindow("资源管理器", "支持资源收集、资源管理、资源导出、资源打包等功能",
                IconResource = "Editor/Icon/Asset",
                Group = "Tools",
                Menu = "AIO/Window/Asset",
                MinSizeHeight = 650,
                MinSizeWidth = 1200
            )]
    public partial class AssetWindow : GraphicWindow
    {
        private const  float       DrawHeaderHeight = 25;
        private static AssetWindow Instance;

        #region override

        protected override void OnAwake()
        {
            Instance               = this;
            Selection.activeObject = AssetCollectRoot.GetOrCreate();
        }

        protected override void OnActivation()
        {
            Instance               = this;
            Selection.activeObject = AssetCollectRoot.GetOrCreate();

            if (Pages is null)
            {
                Pages = new List<IAssetPage>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.FullName.Contains("Editor")) continue;
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsAbstract || type.IsInterface) continue;
                        if (!typeof(IAssetPage).IsAssignableFrom(type)) continue;
                        if (!(Activator.CreateInstance(type) is IAssetPage page)) continue;
                        Pages.Add(page);
                    }
                }

                Pages.Sort((a, b) => b.Order.CompareTo(a.Order));
            }

            PageNames = Pages.Select(p => p.Title).ToArray();
            PageIndex = 0;
        }

        private Rect DrawRect;

        protected override void OnDraw()
        {
            DrawRect.Set(0, 0, CurrentWidth, DrawHeaderHeight - 5);
            using (new GUI.GroupScope(DrawRect, GEStyle.INThumbnailShadow))
            {
                DrawRect.x     =  0;
                DrawRect.y     =  0;
                DrawRect.width -= 75;

                using (new GUI.GroupScope(DrawRect))
                {
                    DrawRect.height = 20;
                    CurrentPage?.OnDrawHeader(DrawRect);
                }

                DrawRect.x     = DrawRect.width;
                DrawRect.width = 75;
                PageIndex      = EditorGUI.Popup(DrawRect, PageIndex, PageNames, GEStyle.PreDropDown);
            }

            DrawRect.y      = DrawHeaderHeight;
            DrawRect.height = CurrentHeight - DrawRect.y;
            DrawRect.x      = 0;
            DrawRect.width  = CurrentWidth;
            using (new GUI.GroupScope(DrawRect))
            {
                DrawRect.x      =  0;
                DrawRect.y      =  5;
                DrawRect.height -= 10;
                CurrentPage?.OnDrawContent(DrawRect);
            }

            DrawVersion(Setting.Version);
            OnOpenEvent();
        }

        public override void EventMouseDown(in Event eventData) => CurrentPage?.EventMouseDown(eventData);
        public override void EventMouseDrag(in Event eventData) => CurrentPage?.EventMouseDrag(eventData);
        public override void EventMouseUp(in   Event eventData) { CurrentPage?.EventMouseUp(eventData); }

        public override void EventKeyDown(in Event eventData, in KeyCode keyCode)
        {
            CurrentPage?.EventKeyDown(eventData, keyCode);
            for (var i = 0; i < Pages.Count; i++)
            {
                if (!Pages[i].Shortcut(eventData)) continue;
                PageIndex = i;
                GUI.FocusControl(null);
                eventData.Use();
                break;
            }
        }

        #endregion

        #region Page

        private IAssetPage        CurrentPage => Pages[PageIndex];
        private IList<IAssetPage> Pages;
        private string[]          PageNames;
        private int               _PageIndex = -1;

        /// <summary>
        ///    当前界面
        /// </summary>
        public int PageIndex
        {
            get => _PageIndex;
            set
            {
                if (value < 0 || value >= Pages.Count || value == _PageIndex) return;
                _PageIndex = value;
                Pages[PageIndex]?.UpdateData();
            }
        }

        public static T OpenPage<T>() where T : IAssetPage
        {
            if (!Instance) EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
            Instance.PageIndex = Instance.Pages.IndexOf(Instance.Pages.FirstOrDefault(p => p is T));
            if (Instance.PageIndex == -1) throw new Exception($"未找到页面 {typeof(T).Name}");
            return (T)Instance.Pages[Instance.PageIndex];
        }

        public static bool IsOpenPage<T>() where T : IAssetPage
        {
            if (!Instance) return false;
            return Instance.Pages[Instance.PageIndex] is T;
        }

        #endregion
    }
}