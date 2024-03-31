using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

[assembly: Preserve]
#if UNITY_2018_3_OR_NEWER
[assembly: AlwaysLinkAssembly]
#endif

[assembly: ComVisible(false)]
[assembly: InternalsVisibleTo("AIO.Asset.Editor")]
[assembly: InternalsVisibleTo("AIO.FGUI.Runtime")]
[assembly: InternalsVisibleTo("AIO.FGUI.Editor")]
[assembly: InternalsVisibleTo("AIO.CLI.YooAsset.Runtime")]
[assembly: InternalsVisibleTo("AIO.CLI.YooAsset.Editor")]