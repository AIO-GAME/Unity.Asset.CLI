
#if SUPPORT_WHOOTHOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rol.Game
{
	public class AssetCache<TKey, TAsset> : IAssetCache<TKey, TAsset> where TAsset : UnityEngine.Object
	{
		private readonly Dictionary<TKey, TAsset> map = new Dictionary<TKey, TAsset>();

		public TAsset this [TKey index]
		{
			get
			{
				map.TryGetValue(index, out var asset);
				return asset;
			}
		}

		public void PushInto(TKey key, TAsset asset)
		{
			if (asset == null)
			{
				return;
			}
			else
			{
				map.Add(key, asset);
			}
		}

		public void Clear()
		{
			map.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return map.ContainsKey(key);
		}

		public IEnumerator<KeyValuePair<TKey, TAsset>> GetEnumerator()
		{
			return map.GetEnumerator();
		}

		public TAsset PopOut(TKey key)
		{
			if (map.TryGetValue(key, out var asset))
			{
				map.Remove(key);
				return asset;
			}
			return null;
		}

		public bool TryGetAsset(TKey key, out TAsset asset)
		{
			return map.TryGetValue(key, out asset);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return map.GetEnumerator();
		}

        
    }

    public static class AssetCacheUtils
    {
        public static IEnumerator ReleaseAssetCache<T>(IAssetCache<string, T> cache) where T : UnityEngine.Object
        {
            //ResourceManager.ReleaseAssets(cache);
            yield return null;
            // yield return Resources.UnloadUnusedAssets();
        }

        public static IEnumerator ReleaseAssetsIds(ICollection<string> addresses)
        {
            //ResourceManager.ReleaseAssets(addresses);
            yield return null;
            // yield return Resources.UnloadUnusedAssets();
            //addresses.Clear();
        }

        public static IEnumerator ReleaseAssetsId(string addresse)
        {
            //ResourceManager.ReleaseAsset(addresse);
            yield return null;
            // yield return Resources.UnloadUnusedAssets();
        }
    }
}
#endif