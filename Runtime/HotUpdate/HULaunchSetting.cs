/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIO.UEngine
{
    /// <summary>
    /// 热更新启动器配置
    /// </summary>
    public class HULaunchSetting : ScriptableObject
    {
        public const string Path = "Assets/Resources/" + nameof(HULaunchSetting) + ".asset";

        public static HULaunchSetting Get()
        {
            HULaunchSetting setting = null;
            foreach (var item in Resources.LoadAll<HULaunchSetting>(nameof(HULaunchSetting)))
            {
                if (item is null) continue;
                setting = item;
                break;
            }

            return setting;
        }

        /// <summary>
        /// 热更新资源包 是否从服务器下载
        /// 0:编译器模式
        /// 1:远端模式
        /// 2:本地模式
        /// </summary>
        public int EnableHotUpdate = 1;

        /// <summary>
        /// 启用边玩边下
        /// </summary>
        public bool EnableSidePlayWithDownload = false;

        /// <summary>
        /// 热更新资源包配置 列表
        /// </summary>
        public List<AssetsPackageConfig> ConfigList;

        /// <summary>
        /// 热更新资源包服务器地址
        /// </summary>
        public string URL = "";

        /// <summary>
        /// 热更新资源同时最大下载数量
        /// </summary>
        public int DownloadingMaxNumber = 50;

        /// <summary>
        /// 热更新资源下载失败重试次数
        /// </summary>
        public int FailedTryAgain = 3;

        /// <summary>
        /// 热更新资源下载超时时间
        /// </summary>
        public int Timeout = 10;

        /// <summary>
        /// 自动激活清单
        /// </summary>
        public bool AutoSaveVersion;

        /// <summary>
        /// URL请求附加时间搓
        /// </summary>
        public bool UrlAppendTimestamp = true;

        /// <summary>
        /// 热更新预制件地址
        /// </summary>
        public string HotUpdatePanelPath = "";

        /// <summary>
        /// 多国事件语言字典
        /// </summary>
        [NonSerialized] public Dictionary<SystemLanguage, Dictionary<EASEventType, string>> EventLanguageDictionary;

        public string EventLanguageDictionaryJson;
    }
}
