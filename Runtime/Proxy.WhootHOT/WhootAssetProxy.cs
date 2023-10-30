using System.Collections;
using System.Threading.Tasks;
using AIO.UEngine;

#if SUPPORT_WHOOTHOT

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