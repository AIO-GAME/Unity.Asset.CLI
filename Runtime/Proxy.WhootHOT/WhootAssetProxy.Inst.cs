/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_WHOOTHOT
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Rol.Game
{
    public partial class WhootAssetProxy
    {
        public override GameObject InstGameObject(string location, Transform parent = null)
        {
            throw new NotImplementedException();
        }

        public override async Task<GameObject> InstGameObjectTask(string location, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(location, parent);
            await handle.Task;
            return handle.Result;
        }

        public override IEnumerator InstGameObjectCO(string location, Action<GameObject> cb, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(location, parent);
            yield return handle;
            cb?.Invoke(handle.Result);
        }
    }
}
#endif