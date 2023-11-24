/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan 
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Threading.Tasks;
using AIO.UEngine.YooAsset;

namespace AIO.UEngine
{
    public partial class YAssetProxy
    {
        public override void UnloadUnusedAssets(bool isForce = false)
        {
            YAssetSystem.UnloadUnusedALLAssets(isForce);
        }

        public override Task UnloadSceneTask(string location)
        {
            return YAssetSystem.UnLoadSceneTask(location);
        }

        public override IEnumerator UnloadSceneCO(string location, Action cb = null)
        {
            return YAssetSystem.UnLoadSceneCO(location, cb);
        }
    }
}
#endif