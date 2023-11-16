/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System.Diagnostics;
using System.IO;

namespace AIO.UEngine
{
    [DebuggerNonUserCode]
    public class ASConfig
    {
        /// <summary>
        /// 资源加载模式
        /// </summary>
        public EASMode ASMode { get; set; }

        /// <summary>
        /// 热更新资源包服务器地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 自动激活清单
        /// </summary>
        public bool AutoSaveVersion { get; set; }

        /// <summary>
        /// URL请求附加时间搓
        /// </summary>
        public bool AppendTimeTicks { get; set; }

        /// <summary>
        /// 加载路径转小写
        /// </summary>
        public bool LoadPathToLower { get; set; }

        /// <summary>
        /// 输出日志
        /// </summary>
        public bool OutputLog { get; set; }

        public ASConfig(string url, EASMode mode = EASMode.Local)
        {
            URL = url;
            ASMode = mode;
            AutoSaveVersion = true;
            AppendTimeTicks = true;
            LoadPathToLower = true;
            OutputLog = true;
        }

        public ASConfig()
        {
            URL = string.Empty;
            ASMode = EASMode.Local;
            AutoSaveVersion = true;
            AppendTimeTicks = true;
            LoadPathToLower = true;
            OutputLog = true;
        }

        /// <summary>
        /// 获取远程资源包地址
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="package">包名</param>
        /// <param name="version">版本</param>
        public string GetRemoteURL(string fileName, string package, string version)
        {
            return Path.Combine(URL, AssetSystem.PlatformNameStr, package, version, fileName);
        }
    }
}