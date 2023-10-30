
#if SUPPORT_WHOOTHOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rol.Game
{
    public interface IResourceCache
    {
        bool IsAddressCached(string address);
        bool TryGetAsset<T>(string address, out T asset) where T : Object;
    }
}
#endif