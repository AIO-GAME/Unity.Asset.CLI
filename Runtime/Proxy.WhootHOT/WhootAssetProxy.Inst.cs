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
using Object = UnityEngine.Object;

namespace Rol.Game
{
    public partial class WhootAssetProxy
    {
        public override GameObject InstGameObject(string location, Transform parent = null)
        {
            throw new NotImplementedException();
        }

        public override Task<GameObject> InstGameObjectTask(string location, Transform parent = null)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator InstGameObjectCO(string location, Action<GameObject> cb, Transform parent = null)
        {
            var go = ResourceManager.LoadAsset<GameObject>(location);
            yield return go;
            cb?.Invoke(Object.Instantiate(go.Result, parent));
        }
    }
}
#endif