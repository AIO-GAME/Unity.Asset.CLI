/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.IO;
using UnityEditor;

namespace AIO
{
    /// <summary>
    /// 
    /// </summary>
    public struct AssetDataInfo
    {
        /// <summary>
        /// Asset Path
        /// </summary>
        public string AssetPath;

        /// <summary>
        /// Asset Address
        /// </summary>
        public string Address;

        /// <summary>
        /// Asset Extension
        /// </summary>
        public string Extension;

        /// <summary>
        /// Asset Tags
        /// </summary>
        public string Tags;

        /// <summary>
        /// Asset Collect Path
        /// </summary>
        public string CollectPath;

        /// <summary>
        /// 组名
        /// </summary>
        public string Group;

        /// <summary>
        /// 包名
        /// </summary>
        public string Package;

        /// <summary>
        /// Asset Size
        /// </summary>
        public long Size
        {
            get
            {
                if (_size == default)
                {
                    if (File.Exists(AssetPath))
                    {
                        _size = AHelper.IO.GetFileLength(AssetPath);
                    }
                    else if (Directory.Exists(AssetPath))
                    {
                        _size = AHelper.IO.GetDirLength(AssetPath);
                    }
                    else _size = -1;
                }

                return _size;
            }
        }

        public string SizeStr
        {
            get
            {
                if (string.IsNullOrEmpty(_sizeStr)) _sizeStr = Size.ToConverseStringFileSize();
                return _sizeStr;
            }
        }

        /// <summary>
        /// Asset GUID
        /// </summary>
        public string GUID
        {
            get
            {
                if (string.IsNullOrEmpty(_guid))
                {
                    _guid = AssetDatabase.AssetPathToGUID(AssetPath);
                }

                return _guid;
            }
        }

        /// <summary>
        /// Asset Last Imported
        /// </summary>
        public DateTime LastWriteTime
        {
            get
            {
                if (_lastWriteTime == default)
                {
                    if (File.Exists(AssetPath))
                        _lastWriteTime = File.GetLastWriteTime(AssetPath);
                    else if (Directory.Exists(AssetPath))
                        _lastWriteTime = Directory.GetLastWriteTime(AssetPath);
                    else _lastWriteTime = DateTime.MinValue;
                }

                return _lastWriteTime;
            }
        }

        /// <summary>
        /// Asset Name
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = Path.GetFileNameWithoutExtension(AssetPath);
                }

                return _name;
            }
        }

        /// <summary>
        /// Asset Type
        /// </summary>
        public string Type
        {
            get
            {
                if (string.IsNullOrEmpty(_type))
                {
                    _type = AssetDatabase.GetMainAssetTypeAtPath(AssetPath)?.FullName;
                    if (string.IsNullOrEmpty(_type)) _type = "Unknown";
                }

                return _type;
            }
        }

        [NonSerialized] private string _sizeStr;
        [NonSerialized] private string _name;
        [NonSerialized] private DateTime _lastWriteTime;
        [NonSerialized] private long _size;
        [NonSerialized] private string _type;
        [NonSerialized] private string _guid;
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