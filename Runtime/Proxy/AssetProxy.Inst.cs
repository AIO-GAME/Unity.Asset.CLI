/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace AIO.UEngine
{
    public partial class AssetProxy
    {
        public abstract GameObject InstGameObject(string location, Transform parent = null);

        public abstract Task<GameObject> InstGameObjectTask(string location, Transform parent = null);

        public abstract IEnumerator InstGameObjectCO(string location, Action<GameObject> cb, Transform parent = null);

    }
}
