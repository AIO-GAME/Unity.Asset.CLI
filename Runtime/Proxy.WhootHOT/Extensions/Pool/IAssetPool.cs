
#if SUPPORT_WHOOTHOT
using System.Collections.Generic;
using UnityEngine;

namespace Rol.Game
{
	public enum CacheMethod
	{
		// CacheCount <= Capacity && CacheCount == Count 
		KeepCount = 1,

		// CacheCount <= Capacity && CacheCount += Count 
		AddCount = 2,

		// CacheCount <= Capacity && CacheCount >= Count
		MinCount = 3,

		// CacheCount <= Capacity && 0 < CacheCount && CacheCount <= Count 
		MaxCount = 4,

		// CacheCount <= Capacity && CacheCount == 1 
		KeepOne = 5,

		// CacheCount <= Capacity && CacheCount += 1
		AddOne = 6,

		// CacheCount <= Capacity && CacheCount >= 1 
		MinOne = 7,

		// CacheCount <= Capacity && CacheCount >= 1 
		MaxOne = 8,
	}

	public interface IAssetFactory<TKey, TObject> where TObject : Object
	{
		TObject OnCreate(TKey key);
		void OnDestroy(TKey key, TObject obj);
		void OnBorrow(TKey key, TObject obj);
		void OnReturn(TKey key, TObject obj);
	}

	public interface IAssetPoolEntry<TAsset> where TAsset : Object
	{
		int capacity { get; set; }
		int cacheCount { get; set; }
		int borrowCount { get; }
		TAsset Borrow();
		bool Return(TAsset obj, bool destroy = false);
		void Clear(bool destroyBorrow = false);
	}

	public interface IAssetPool<TKey, TAsset> where TAsset : Object
	{
		IAssetPoolEntry<TAsset> this [TKey key] { get; }
		TAsset Borrow(TKey key);
		void Return(TKey key, TAsset obj, bool destroy = false);

		/// <summary>
		/// 优先使用Return(key, obj), 此接口将遍历全部IPoolEntry中全部借出的对象；直到找到为止！找不到将抛出异常
		/// </summary>
		/// <param name="obj"></param>
		void Return(TAsset obj, bool destroy = false);

		/// <summary>
		/// 移除对象池，移除之前
		/// </summary>
		void Remove(TKey key, bool destroyBorrow = false);

		/// <summary>
		/// 移除所有的对象池
		/// </summary>
		void RemoveAll(bool destroyBorrow = false);

		/// <summary>
		/// 清空对象池
		/// </summary>
		/// <param name="key"></param>
		/// <param name="destroyBorrow"></param>
		void Clear(TKey key, bool destroyBorrow = false);

		/// <summary>
		/// 清空所有对象池
		/// </summary>
		/// <param name="destroyBorrow"></param>
		void ClearAll(bool destroyBorrow = false);
		/// <summary>
		/// 提前创建对象
		/// </summary>
		/// <param name="key"></param>
		/// <param name="count">缓存数量</param>
		/// <param name="method">缓存方法</param>
		void AddCache(TKey key, int count = 1, CacheMethod method = CacheMethod.KeepCount);

		/// <summary>
		/// 提前创建对象，如果对象池预先不存在，将创建对象池
		/// </summary>
		/// <param name="keys">当中Key重复的次数将作为缓存参数</param>
		/// <param name="method">缓存方法</param>
		void AddCache(ICollection<TKey> keys, CacheMethod method = CacheMethod.KeepCount);
	}
}
#endif