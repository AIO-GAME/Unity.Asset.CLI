/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;

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
        /// Asset Last Imported
        /// </summary>
        public DateTime LastWriteTime;
    }
}