/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-12
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public struct ViewRect
    {
        public float MinWidth;

        public float MaxWidth;

        public float MinHeight;

        public float MaxHeight;

        public ViewRect(float minWidth, float minHeight)
        {
            MinWidth = minWidth;
            MaxWidth = minWidth;
            MinHeight = minHeight;
            MaxHeight = minWidth;
            IsAllowHorizontal = false;
            IsAllowVertical = false;
            DragHorizontalWidth = 1;
            DragVerticalHeight = 1;
            IsDragHorizontal = false;
            IsDragVertical = false;
            IsShow = true;
            Current = new Rect
            {
                width = minWidth,
                height = minHeight,
            };
            RectDragHorizontal = new Rect();
            RectDragVertical = new Rect();
        }

        public ViewRect(float minWidth, float maxWidth, float minHeight, float maxHeight)
        {
            MinWidth = minWidth;
            MaxWidth = maxWidth;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
            IsAllowHorizontal = false;
            IsAllowVertical = false;
            DragHorizontalWidth = 1;
            DragVerticalHeight = 1;
            IsDragHorizontal = false;
            IsDragVertical = false;
            IsShow = true;
            Current = new Rect
            {
                width = (minWidth + maxWidth) / 2,
                height = (minHeight + maxHeight) / 2,
            };
            RectDragHorizontal = new Rect();
            RectDragVertical = new Rect();
        }

        private Rect Current;

        /// <summary>
        /// 是否允许 横向拖拽
        /// </summary>
        public bool IsAllowHorizontal;

        /// <summary>
        /// 是否允许 竖向拖拽
        /// </summary>
        public bool IsAllowVertical;

        public float DragHorizontalWidth;

        public float DragVerticalHeight;

        public float width
        {
            get => Current.width;
            set => Current.width = value;
        }

        public float height
        {
            get => Current.height;
            set => Current.height = value;
        }

        public float x
        {
            get => Current.x;
            set => Current.x = value;
        }

        public float y
        {
            get => Current.y;
            set => Current.y = value;
        }

        private bool IsDragHorizontal;

        private bool IsDragVertical;

        /// <summary>
        /// 是否拖拽
        /// </summary>
        public bool IsDragging => IsDragHorizontal || IsDragVertical;

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow;

        /// <summary>
        /// 拖拽区域
        /// </summary>
        private Rect RectDragHorizontal;

        /// <summary>
        /// 拖拽区域
        /// </summary>
        private Rect RectDragVertical;

        public void ContainsHorizontal(Event e)
        {
            if (!IsShow || !IsAllowHorizontal)
            {
                if (IsDragHorizontal) IsDragHorizontal = false;
                return;
            }

            IsDragHorizontal = RectDragHorizontal.Contains(e.mousePosition);
        }

        public void ContainsVertical(Event e)
        {
            if (!IsShow || !IsAllowVertical)
            {
                if (IsDragVertical) IsDragVertical = false;
                return;
            }

            IsDragVertical = RectDragVertical.Contains(e.mousePosition);
        }

        public void CancelHorizontal()
        {
            IsDragHorizontal = false;
        }

        public void CancelVertical()
        {
            IsDragVertical = false;
        }

        public void DragHorizontal(Event e)
        {
            if (!IsShow || !IsAllowHorizontal || !IsDragHorizontal) return;
            var temp = Current.width + e.delta.x;
            if (temp < MinWidth) Current.width = MinWidth;
            else if (temp > MaxWidth) Current.width = MaxWidth;
            else Current.width = temp;
            e.Use();
        }

        public void DragVertical(Event e)
        {
            if (!IsShow || !IsAllowVertical || !IsDragVertical) return;
            var temp = Current.height + e.delta.y;
            if (temp < MinHeight) Current.height = MinHeight;
            else if (temp > MaxHeight) Current.height = MaxHeight;
            else Current.height = temp;
            e.Use();
        }

        public void Draw(Action onDraw, GUIStyle style = null)
        {
            if (!IsShow) return;
            Draw(Current, onDraw, style);
        }

        public void Draw(Rect rect, Action onDraw, GUIStyle style = null)
        {
            if (!IsShow) return;
            if (IsAllowVertical)
            {
                rect.height -= DragVerticalHeight;
                RectDragVertical = new Rect(rect.x, rect.y + rect.height,
                    rect.width, DragVerticalHeight);
                EditorGUIUtility.AddCursorRect(RectDragVertical, MouseCursor.ResizeVertical);
            }

            if (IsAllowHorizontal)
            {
                rect.width -= DragHorizontalWidth;
                RectDragHorizontal = new Rect(rect.x + rect.width, rect.y,
                    DragHorizontalWidth, rect.height);
                EditorGUIUtility.AddCursorRect(RectDragHorizontal, MouseCursor.ResizeHorizontal);
            }

            if (style is null) GUILayout.BeginArea(rect);
            else GUILayout.BeginArea(rect, style);
            onDraw?.Invoke();
            GUILayout.EndArea();
        }
    }
}