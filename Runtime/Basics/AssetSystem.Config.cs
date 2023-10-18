/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

namespace AIO.UEngine
{
    public enum EASMode
    {
        /// <summary>
        /// 编辑器模式
        /// </summary>
        Editor,

        /// <summary>
        /// 远端模式
        /// </summary>
        Remote,

        /// <summary>
        /// 本地模式
        /// </summary>
        Local,
    }

    public class ASConfig
    {
        /// <summary>
        /// 资源加载模式
        /// </summary>
        public EASMode ASMode = EASMode.Local;

        /// <summary>
        /// 热更新资源包服务器地址
        /// </summary>
        public string URL = "";

        /// <summary>
        /// 自动激活清单
        /// </summary>
        public bool AutoSaveVersion = true;

        /// <summary>
        /// URL请求附加时间搓
        /// </summary>
        public bool AppendTimeTicks = true;

        /// <summary>
        /// 加载路径转小写
        /// </summary>
        public bool LoadPathToLower = true;

        /// <summary>
        /// 输出日志
        /// </summary>
        public bool OutputLog = true;
    }
}
