using System;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    ///   资源页面
    /// </summary>
    public interface IAssetPage : IDisposable
    {
        string Title { get; }

        /// <summary>
        ///    排序
        /// </summary>
        int Order { get; }

        /// <summary>
        ///    快捷键
        /// </summary>
        /// <param name="evt"> 事件参数 </param>
        /// <returns> True: 触发快捷键 </returns>
        bool Shortcut(Event evt);

        /// <summary>
        ///   绘制标题栏
        /// </summary>
        /// <param name="rect"> 区域 </param>
        void OnDrawHeader(Rect rect);

        /// <summary>
        ///   绘制内容
        /// </summary>
        /// <param name="rect"> 区域 </param>
        void OnDrawContent(Rect rect);

        /// <summary>
        ///  更新数据
        /// </summary>
        void UpdateData();

        /// <summary>
        ///  鼠标拖拽
        /// </summary>
        /// <param name="evt"> 事件参数 </param>
        void EventMouseDown(in Event evt);

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="evt"> 事件参数 </param>
        void EventMouseUp(in Event evt);

        /// <summary>
        /// 按下按键
        /// </summary>
        /// <param name="evt"> 事件参数 </param>
        /// <param name="keyCode"> 按键 </param>
        void EventKeyDown(in Event evt, in KeyCode keyCode);

        /// <summary>
        /// 松开按键
        /// </summary>
        /// <param name="evt"> 事件参数 </param>
        /// <param name="keyCode"> 按键 </param>
        void EventKeyUp(in Event evt, in KeyCode keyCode);

        /// <summary>
        /// 鼠标拖拽
        /// </summary>
        /// <param name="evt"> 事件参数 </param>
        void EventMouseDrag(in Event evt);
    }
}