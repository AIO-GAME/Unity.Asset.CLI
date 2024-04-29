using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace AIO
{
    /// <summary>
    ///     资源数据信息
    /// </summary>
    public struct AssetDataInfo
    {
        #region IComparer

        public static readonly IComparer<AssetDataInfo> ComparerSize    = new XComparerSize();
        public static readonly IComparer<AssetDataInfo> ComparerSizeAsc = new XComparerSize { Ascending = true };

        private class XComparerSize : IComparer<AssetDataInfo>
        {
            public bool Ascending;
            public int  Compare(AssetDataInfo x, AssetDataInfo y) => Ascending ? x.Size.CompareTo(y.Size) : y.Size.CompareTo(x.Size);
        }

        public static readonly IComparer<AssetDataInfo> ComparerTime    = new XComparerTime();
        public static readonly IComparer<AssetDataInfo> ComparerTimeAsc = new XComparerTime { Ascending = true };

        private class XComparerTime : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) =>
                Ascending ? x.LastWriteTime.CompareTo(y.LastWriteTime) : y.LastWriteTime.CompareTo(x.LastWriteTime);
        }

        public static readonly IComparer<AssetDataInfo> ComparerName    = new XComparerName();
        public static readonly IComparer<AssetDataInfo> ComparerNameAsc = new XComparerName { Ascending = true };

        private class XComparerName : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) => Ascending
                ? string.Compare(x.Name, y.Name, StringComparison.CurrentCulture)
                : string.Compare(y.Name, x.Name, StringComparison.CurrentCulture);
        }

        public static readonly IComparer<AssetDataInfo> ComparerType    = new XComparerType();
        public static readonly IComparer<AssetDataInfo> ComparerTypeAsc = new XComparerType { Ascending = true };

        private class XComparerType : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) => Ascending
                ? string.Compare(x.Type, y.Type, StringComparison.CurrentCulture)
                : string.Compare(y.Type, x.Type, StringComparison.CurrentCulture);
        }

        public static readonly IComparer<AssetDataInfo> ComparerGUID    = new XComparerGUID();
        public static readonly IComparer<AssetDataInfo> ComparerGUIDAsc = new XComparerGUID { Ascending = true };

        private class XComparerGUID : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) => Ascending
                ? string.Compare(x.GUID, y.GUID, StringComparison.CurrentCulture)
                : string.Compare(y.GUID, x.GUID, StringComparison.CurrentCulture);
        }

        public static readonly IComparer<AssetDataInfo> ComparerAddress    = new XComparerAddress();
        public static readonly IComparer<AssetDataInfo> ComparerAddressAsc = new XComparerAddress { Ascending = true };

        private class XComparerAddress : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) => Ascending
                ? string.Compare(x.Address, y.Address, StringComparison.CurrentCulture)
                : string.Compare(y.Address, x.Address, StringComparison.CurrentCulture);
        }

        public static readonly IComparer<AssetDataInfo> ComparerExtension    = new XComparerExtension();
        public static readonly IComparer<AssetDataInfo> ComparerExtensionAsc = new XComparerExtension { Ascending = true };

        private class XComparerExtension : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) => Ascending
                ? string.Compare(x.Extension, y.Extension, StringComparison.CurrentCulture)
                : string.Compare(y.Extension, x.Extension, StringComparison.CurrentCulture);
        }

        public static readonly IComparer<AssetDataInfo> ComparerAssetPath    = new XComparerAssetPath();
        public static readonly IComparer<AssetDataInfo> ComparerAssetPathAsc = new XComparerAssetPath { Ascending = true };

        private class XComparerAssetPath : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) => Ascending
                ? string.Compare(x.AssetPath, y.AssetPath, StringComparison.CurrentCulture)
                : string.Compare(y.AssetPath, x.AssetPath, StringComparison.CurrentCulture);
        }

        public static readonly IComparer<AssetDataInfo> ComparerCollectPath    = new XComparerCollectPath();
        public static readonly IComparer<AssetDataInfo> ComparerCollectPathAsc = new XComparerCollectPath { Ascending = true };

        private class XComparerCollectPath : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) => Ascending
                ? string.Compare(x.CollectPath, y.CollectPath, StringComparison.CurrentCulture)
                : string.Compare(y.CollectPath, x.CollectPath, StringComparison.CurrentCulture);
        }

        public static readonly IComparer<AssetDataInfo> ComparerTags    = new XComparerTags();
        public static readonly IComparer<AssetDataInfo> ComparerTagsAsc = new XComparerTags { Ascending = true };

        private class XComparerTags : IComparer<AssetDataInfo>
        {
            public bool Ascending;

            public int Compare(AssetDataInfo x, AssetDataInfo y) => Ascending
                ? string.Compare(x.Tag, y.Tag, StringComparison.CurrentCulture)
                : string.Compare(y.Tag, x.Tag, StringComparison.CurrentCulture);
        }

        #endregion

        public static AssetDataInfo Empty = new AssetDataInfo
        {
            AssetPath   = string.Empty,
            Address     = string.Empty,
            Extension   = string.Empty,
            Tag         = string.Empty,
            CollectPath = string.Empty,
            Group       = string.Empty,
            Package     = string.Empty
        };

        /// <summary>
        ///     Asset Path
        /// </summary>
        public string AssetPath;

        /// <summary>
        ///     Asset Address
        /// </summary>
        public string Address;

        /// <summary>
        ///     Asset Extension
        /// </summary>
        public string Extension;

        /// <summary>
        ///     Asset Tag
        /// </summary>
        public string Tag;

        /// <summary>
        ///     Asset Tags
        /// </summary>
        public string[] Tags => string.IsNullOrEmpty(Tag) ? Array.Empty<string>() : Tag.Split(';', ',', ' ');

        /// <summary>
        ///     Asset Collect Path
        /// </summary>
        public string CollectPath;

        /// <summary>
        ///     组名
        /// </summary>
        public string Group;

        /// <summary>
        ///     包名
        /// </summary>
        public string Package;

        /// <summary>
        ///     Asset Size
        /// </summary>
        public long Size
        {
            get
            {
                if (_size == default)
                {
                    if (File.Exists(AssetPath))
                        _size = AHelper.IO.GetFileLength(AssetPath);
                    else if (Directory.Exists(AssetPath))
                        _size  = AHelper.IO.GetDirLength(AssetPath);
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
        ///     Asset GUID
        /// </summary>
        public string GUID
        {
            get
            {
                if (string.IsNullOrEmpty(_guid)) _guid = AssetDatabase.AssetPathToGUID(AssetPath);
                return _guid;
            }
        }

        /// <summary>
        ///     Asset Last Imported
        /// </summary>
        public DateTime LastWriteTime
        {
            get
            {
                if (_lastWriteTime != default(DateTime)) return _lastWriteTime;
                if (File.Exists(AssetPath))
                    _lastWriteTime = File.GetLastWriteTime(AssetPath);
                else if (Directory.Exists(AssetPath))
                    _lastWriteTime  = Directory.GetLastWriteTime(AssetPath);
                else _lastWriteTime = DateTime.MinValue;
                return _lastWriteTime;
            }
        }

        /// <summary>
        ///     Asset Name
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) _name = Path.GetFileNameWithoutExtension(AssetPath);

                return _name;
            }
        }

        /// <summary>
        ///     Asset Type
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

        [NonSerialized]
        private string _sizeStr;

        [NonSerialized]
        private string _name;

        [NonSerialized]
        private DateTime _lastWriteTime;

        [NonSerialized]
        private long _size;

        [NonSerialized]
        private string _type;

        [NonSerialized]
        private string _guid;
    }

    public static class ExtensionAssetDataInfo
    {
        public static string GetLatestTime(this AssetDataInfo data)
        {
            var time = DateTime.Now - data.LastWriteTime;
            if (time.TotalMinutes < 1) return "刚刚";
            if (time.TotalHours < 1) return $"{time.Minutes}分钟前";
            if (time.TotalDays < 1) return $"{time.Hours}小时前";
            if (time.TotalDays < 7) return $"{time.Days}天前";
            if (time.TotalDays < 30) return $"{time.Days / 7}周前";
            if (time.TotalDays < 365) return $"{time.Days / 30}月前";
            return $"{time.Days / 365}年前";
        }
    }
}