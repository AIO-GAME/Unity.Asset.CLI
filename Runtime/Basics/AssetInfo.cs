/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

namespace AIO
{
    /// <summary>
    /// nameof(AssetInfo)
    /// </summary>
    public class AssetDataInfo
    {
        public string AssetPath;

        public string Address;

        public string Extension => System.IO.Path.GetExtension(AssetPath).Replace(".", "");

        public string Name => System.IO.Path.GetFileNameWithoutExtension(AssetPath);
    }
}