
#if SUPPORT_WHOOTHOT
using System.Collections.Generic;

namespace Rol.Game
{
	public interface IAssetCache<TKey, TAsset> : IEnumerable<KeyValuePair<TKey, TAsset>> where TAsset : UnityEngine.Object
	{
		TAsset this [TKey index] { get; }
		bool ContainsKey(TKey key);
		bool TryGetAsset(TKey key, out TAsset asset);
		void PushInto(TKey key, TAsset asset);
		TAsset PopOut(TKey key);
		void Clear();
	}
}
#endif