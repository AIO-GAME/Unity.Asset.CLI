/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Threading.Tasks;

namespace AIO.UEngine
{
    public partial class AssetProxy
    {
        public abstract Task UnloadSceneTask(string location);

        public abstract IEnumerator UnloadSceneCO(string location, Action cb = null);
    }
}