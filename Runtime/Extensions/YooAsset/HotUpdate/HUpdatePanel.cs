/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using UnityEngine;

namespace AIO.UEngine
{
    public abstract class HUpdatePanel : MonoBehaviour
    {
        /// <summary>
        /// 进度
        /// </summary>
        public abstract double Progress { get; set; }

        /// <summary>
        /// 进度文本
        /// </summary>
        public abstract string ProgressText { get; set; }

        /// <summary>
        /// 更新进度
        /// </summary>
        public abstract void OnUpdate();
    }
}
