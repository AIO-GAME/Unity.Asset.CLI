/*|============|*|
|*|Author:     |*| xi nan
|*|Date:       |*| 2024-01-07
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace AIO
{
    public partial class AssetSystem
    {
        #region LOG

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogWarning(string format, params object[] args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogWarningFormat($"<b><color=#9575CD>[ASSET]</color><color=#FFC107>[Warning]</color></b> {format}",
                    args);
#else
                Debug.LogWarning(new Exception($"[ASSET][Warning] {string.Format(format, args)}"));
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogWarning(string args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogWarningFormat("<b><color=#9575CD>[ASSET]</color><color=#FFC107>[Warning]</color></b> {0}", args);
#else
                Debug.LogWarning(new Exception($"[ASSET][Warning] {args}"));
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(Exception args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogErrorFormat("<b><color=#9575CD>[ASSET]</color><color=#E91E63>[Exception]</color></b> {0}", args);
#else
                Debug.LogException(new Exception($"[ASSET][Exception] {args}"));
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(string args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogErrorFormat("<b><color=#9575CD>[ASSET]</color><color=#E91E63>[Exception]</color></b> {0}", args);
#else
                Debug.LogException(new Exception($"[ASSET][Exception] {args}"));
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(string format, params object[] args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogErrorFormat($"<b><color=#9575CD>[ASSET]</color><color=#E91E63>[Exception]</color></b> {format}",
                    args);
#else
                Debug.LogException(new Exception($"[ASSET][Exception] {string.Format(format, args)}"));
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void Log(string args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogFormat("<b><color=#9575CD>[ASSET]</color><color=#B3E5FC>[Log]</color></b> {0}", args);
#else
                Debug.LogFormat("[ASSET][Log] {0}", args);
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void Log(string format, params object[] args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogFormat($"<b><color=#9575CD>[ASSET]</color><color=#B3E5FC>[Log]</color></b> {format}", args);
#else
                Debug.LogFormat($"[ASSET][Log] {format}", args);
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogError(string args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogErrorFormat("<b><color=#9575CD>[ASSET]</color><color=#F44336>[Error]</color></b> {0}", args);
#else
                Debug.LogErrorFormat("[ASSET][Error] {0}", args);
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogError(string format, params object[] args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogErrorFormat($"<b><color=#9575CD>[ASSET]</color><color=#F44336>[Error]</color></b> {format}", args);
#else
                Debug.LogErrorFormat($"[ASSET][Error] {format}", args);
#endif
        }

        #endregion
    }
}