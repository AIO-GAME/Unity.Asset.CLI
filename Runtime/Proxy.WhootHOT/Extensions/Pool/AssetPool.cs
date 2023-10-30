
#if SUPPORT_WHOOTHOT
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rol.Game
{
	public class AssetPoolEntry<TKey, TAsset> : IAssetPoolEntry<TAsset> where TAsset : Object
	{
		private IAssetFactory<TKey, TAsset> factory;
		private TKey key;
		private Queue<TAsset> cacheQueue = new Queue<TAsset>();
		private List<TAsset> borrowList = new List<TAsset>();
		private int _capacity = int.MaxValue;

		public AssetPoolEntry(TKey key, IAssetFactory<TKey, TAsset> factory)
		{
			this.key = key;
			this.factory = factory;
		}

		public int capacity
		{
			get
			{
				return _capacity;
			}

			set
			{
				_capacity = Math.Max(0, value);
				if (_capacity < cacheCount)
				{
					cacheCount = _capacity;
				}
			}
		}

		public int cacheCount
		{
			get
			{
				return cacheQueue.Count;
			}
			set
			{
				value = Math.Max(0, value);
				value = Math.Min(value, capacity);
				while (cacheQueue.Count > value)
				{
					var asset = cacheQueue.Dequeue();
					factory.OnDestroy(key, asset);
				}

				while (cacheQueue.Count < value)
				{
					var asset = factory.OnCreate(key);
					if (null == asset)
					{
						break;
					}
					factory.OnReturn(key, asset);
					cacheQueue.Enqueue(asset);
				}
			}
		}

		public int borrowCount => borrowList.Count;

		public TAsset Borrow()
		{
			TAsset asset = null;
			if (cacheQueue.Count > 0)
			{
				asset = cacheQueue.Dequeue();
				borrowList.Add(asset);
				factory.OnBorrow(key, asset);
			}
			else
			{
				asset = factory.OnCreate(key);
				if (null != asset)
				{
					borrowList.Add(asset);
				}
			}
			return asset;
		}

		public void Clear(bool destroyBorrow = false)
		{
			TAsset asset = null;
			while (cacheQueue.Count > 0)
			{
				asset = cacheQueue.Dequeue();
				factory.OnDestroy(key, asset);
			}

			if (destroyBorrow)
			{
				foreach (var ast in borrowList)
				{
					factory.OnDestroy(key, ast);
				}
				borrowList.Clear();
			}
		}

		public bool Return(TAsset obj, bool destroy = false)
		{
			if (null != obj)
			{
				int index = borrowList.IndexOf(obj);
				if (index >= 0)
				{
					if (borrowList.Remove(obj))
					{
						if (destroy)
						{
							factory.OnDestroy(key, obj);
						}
						else
						{
							factory.OnReturn(key, obj);
							cacheQueue.Enqueue(obj);
						}
						return true;
					}
				}
			}
			return false;
		}
	}

	public class AssetPool<TKey, TAsset> : IAssetPool<TKey, TAsset> where TAsset : Object
	{
		private Dictionary<TKey, IAssetPoolEntry<TAsset>> entries = new Dictionary<TKey, IAssetPoolEntry<TAsset>>();
		private IAssetFactory<TKey, TAsset> factory { get; set; }

		public AssetPool(IAssetFactory<TKey, TAsset> factory)
		{
			this.factory = factory;
		}

		public IAssetPoolEntry<TAsset> this [TKey key]
		{
			get
			{
				if (entries.TryGetValue(key, out var entry))
				{
					return entry;
				}
				return null;
			}
		}

		public void AddCache(TKey key, int count, CacheMethod method)
		{
			if (!entries.TryGetValue(key, out var entry))
			{
				entry = new AssetPoolEntry<TKey, TAsset>(key, factory);
				entries.Add(key, entry);
			}

			count = Math.Max(0, count);
			switch(method)
			{
				case CacheMethod.AddCount:
				count = entry.cacheCount + count;
				break;
				case CacheMethod.AddOne:
				count = entry.cacheCount + 1;
				break;
				case CacheMethod.KeepCount:
				break;
				case CacheMethod.KeepOne:
				count = 1;
				break;
				case CacheMethod.MaxCount:
				count = Math.Max(1, Math.Min(count, entry.cacheCount));
				break;
				case CacheMethod.MaxOne:
				count = Math.Min(1, entry.cacheCount);
				break;
				case CacheMethod.MinCount:
				count = Math.Max(entry.cacheCount, count);
				break;
				case CacheMethod.MinOne:
				count = Math.Max(entry.cacheCount, 1);
				break;
			}
			entry.cacheCount = count;
		}

		public void AddCache(ICollection<TKey> keys, CacheMethod method)
		{
			if (null == keys || keys.Count == 0)
			{
				return;
			}
			var dic = new Dictionary<TKey, int>();
			foreach(var key in keys )
			{
				if (!dic.TryGetValue(key, out int val))
				{
					dic.Add(key, 1);
				}
				else
				{
					dic[key] = val + 1;
				}
			}

			foreach(var kv in dic)
			{
				AddCache(kv.Key, kv.Value, method);
			}
		}

		public TAsset Borrow(TKey key)
		{
			if (!entries.TryGetValue(key, out var entry))
			{
				entry = new AssetPoolEntry<TKey, TAsset>(key, factory);
				entries.Add(key, entry);
			}
			return entry.Borrow();
		}

		public void Clear(TKey key, bool destroyBorrow)
		{
			if (entries.TryGetValue(key, out var entry))
			{
				entry.Clear(destroyBorrow);
			}
		}

		public void ClearAll(bool destroyBorrow)
		{
			foreach (var kv in entries)
			{
				kv.Value.Clear(destroyBorrow);
			}
		}

		public void Remove(TKey key, bool destroyBorrow)
		{
			if (entries.TryGetValue(key, out var entry))
			{
				entry.Clear(destroyBorrow);
				entries.Remove(key);
			}
		}

		public void RemoveAll(bool destroyBorrow)
		{
			foreach(var kv in entries)
			{
				kv.Value.Clear(destroyBorrow);
			}
			entries.Clear();
		}

		public void Return(TKey key, TAsset obj, bool destroy = false)
		{
			if (null != obj && entries.TryGetValue(key, out var entry))
			{
				entry.Return(obj);
			}
		}

		public void Return(TAsset obj, bool destroy = false)
		{
			if (null == obj)
			{
				return;
			}

			foreach (var kv in entries)
			{
				if (kv.Value.Return(obj))
				{
					return;
				}
			}
		}
	}
}
#endif