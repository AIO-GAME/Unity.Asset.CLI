using System.Collections.Generic;

namespace AIO
{
    partial class AssetSystem
    {
        internal static readonly Dictionary<string, int> ReferenceHandleCount
            = new Dictionary<string, int>();

        internal static readonly Dictionary<string, IHandle> HandleDic
            = new Dictionary<string, IHandle>();
    }

}