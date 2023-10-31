
#if SUPPORT_WHOOTHOT
using System.Collections;
using AIO.UEngine;
using UnityEngine.AddressableAssets;

namespace Rol.Game
{
    public partial class WhootAssetProxy : AssetProxy
    {
        public override IEnumerator Initialize()
        {
            yield return ResourceManager.Initialize();
        }

        public override void Dispose()
        {
        }
    }
}
#endif