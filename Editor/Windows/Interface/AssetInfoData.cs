/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;

namespace AIO
{
    /// <summary>
    /// nameof(AssetInfo)
    /// </summary>
    public struct AssetDataInfo
    {
        /// <summary>
        /// Asset Path
        /// </summary>
        public string AssetPath;

        /// <summary>
        /// Asset GUID
        /// </summary>
        public string GUID;

        /// <summary>
        /// Asset Address
        /// </summary>
        public string Address;

        /// <summary>
        /// Asset Extension
        /// </summary>
        public string Extension;

        /// <summary>
        /// Asset Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Asset Collect Path
        /// </summary>
        public string CollectPath;

        /// <summary>
        /// Asset Size
        /// </summary>
        public long Size;

        /// <summary>
        /// Asset Tags
        /// </summary>
        public string Tags;

        /// <summary>
        /// Asset Last Imported
        /// </summary>
        public DateTime LastWriteTime;
        
        /// <summary>
        /// Asset Type
        /// </summary>
        public string Type;
    }

    public static class ExtensionAssetDataInfo
    {
        public static string GetLatestTime(this AssetDataInfo data)
        {
            // 当前时间 - 最后修改时间 = 距离现在的时间
            // 小于1分钟 = 刚刚
            // 小于1小时 = {}分钟前
            // 小于1天 = {}小时前
            // 小于1周 = {}天前
            // 小于1月 = {}周前
            // 小于1年 = {}月前
            // 大于1年 = {}年前
            var time = DateTime.Now - data.LastWriteTime;
            if (time.TotalMinutes < 1)
            {
                return "刚刚";
            }

            if (time.TotalHours < 1)
            {
                return $"{time.Minutes}分钟前";
            }

            if (time.TotalDays < 1)
            {
                return $"{time.Hours}小时前";
            }

            if (time.TotalDays < 7)
            {
                return $"{time.Days}天前";
            }

            if (time.TotalDays < 30)
            {
                return $"{time.Days / 7}周前";
            }

            if (time.TotalDays < 365)
            {
                return $"{time.Days / 30}月前";
            }

            return $"{time.Days / 365}年前";
        }
    }
}