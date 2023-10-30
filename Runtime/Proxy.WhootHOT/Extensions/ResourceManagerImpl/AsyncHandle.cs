
#if SUPPORT_WHOOTHOT
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Rol.Game.Resource
{
    internal struct ConstAsyncHandle<T> : IAsyncHandle<T> where T : class
    {
        public ConstAsyncHandle(T result)
        {
            this.Result = result;
        }

        public T Result { get; private set; }

        public bool IsDone => true;

        public float PercentComplete => 1f;

        public object Current => null;

        public bool MoveNext()
        {
            return false;
        }

        public void Reset() { }
    }

    internal struct AsyncHandle : IAsyncHandle
    {
        public bool IsDone => handle.IsDone;

        public float PercentComplete => handle.PercentComplete;

        public object Current => null;

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset() { }

        public AsyncHandle(AsyncOperationHandle h)
        {
            handle = h;
        }

        AsyncOperationHandle handle;
    }

    internal struct AsyncHandle<T> : IAsyncHandle<T> where T : class
    {
        private IAssetRecord record;
        public AsyncHandle(IAssetRecord record)
        {
            this.record = record;
        }

        public T Result => record.asset as T;

        public bool IsDone => record.isLoaded;

        public float PercentComplete => record.loadingProgress;

        public object Current => null;

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset() { }
    }

    internal class AsyncCacheHandle<T> : IAsyncHandle<IAssetCache<string, T>> where T : UnityEngine.Object
    {
        public IAssetCache<string, T> Result { get; set; }

        public bool IsDone
        {
            get
            {
                if (IsLoadingDone() && null != Result)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsLoadingDone()
        {
            if (null != batchRecord)
            {
                if (batchRecord.isLoaded)
                {
                    batchRecord = null;
                }
                else
                {
                    return false;
                }
            }

            for (int i = 0; i < loadingRecords.Count; ++i)
            {
                var rcd = loadingRecords[i];
                if (rcd.isLoaded)
                {
                    loadingRecords.RemoveAt(i);
                    i--;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public float PercentComplete
        {
            get
            {
                if (IsDone)
                {
                    return 1f;
                }

                float val = 0;
                if (batchRecord == null)
                {
                    val += batchCount;
                }
                else
                {
                    if (batchRecord.isLoaded)
                    {
                        val += batchCount;
                        batchRecord = null;
                    }
                    else
                    {
                        val += batchRecord.loadingProgress * batchCount;
                    }
                }

                for (int i = 0; i < loadingRecords.Count; ++i)
                {
                    var rcd = loadingRecords[i];
                    if (!rcd.isLoaded)
                    {
                        loadingRecords.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        val += rcd.loadingProgress;
                    }
                }
                val += (loadingCount - loadingRecords.Count);

                return val / (batchCount + loadingCount);
            }
        }

        public object Current => null;

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset() { }

        public IAssetRecord batchRecord;
        public List<IAssetRecord> loadingRecords;
        public int batchCount;
        public int loadingCount;
    }
}
#endif