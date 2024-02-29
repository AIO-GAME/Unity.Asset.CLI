using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace AIO
{
    partial class AssetSystem
    {
#if UNITY_EDITOR
        private const string BASE_FORMAT = "<b><color=#9575CD>[ASSET]</color></b>";
        private const string BASE_LOG_FORMAT = BASE_FORMAT + "<b><color=#B3E5FC>[Log]</color></b>";
        private const string BASE_EXCEPTION_FORMAT = BASE_FORMAT + "<b><color=#E91E63>[Exception]</color></b>";
        private const string BASE_WARNING_FORMAT = BASE_FORMAT + "<b><color=#FFC107>[Warning]</color></b>";
        private const string BASE_ERROR_FORMAT = BASE_FORMAT + "<b><color=#F44336>[Error]</color></b>";
#else
        private const string BASE_FORMAT = "[ASSET]";
        private const string BASE_LOG_FORMAT = BASE_FORMAT + "[Log]";           
        private const string BASE_EXCEPTION_FORMAT = BASE_FORMAT + "[Exception]";
        private const string BASE_WARNING_FORMAT = BASE_FORMAT + "[Warning]";
        private const string BASE_ERROR_FORMAT = BASE_LOG_FORMAT + "[Error]";
#endif

        #region LOG

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogWarning(string format, params object[] args)
        {
            if (Parameter.OutputLog)
                Debug.LogWarningFormat($"{BASE_WARNING_FORMAT} {format}", args);
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogWarning(string args)
        {
            if (Parameter.OutputLog)
                Debug.LogWarning($"{BASE_WARNING_FORMAT} {args}");
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(Exception args)
        {
            if (Parameter.OutputLog)
#if UNITY_EDITOR
                Debug.LogError($"{BASE_EXCEPTION_FORMAT} {args}");
#else
                Debug.LogException(new Exception($"{BASE_EXCEPTION_FORMAT} {args}"));
#endif
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(string args)
        {
            if (Parameter.OutputLog)
                Debug.LogError($"{BASE_EXCEPTION_FORMAT} {args}");
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(string format, params object[] args)
        {
            if (Parameter.OutputLog)
                Debug.LogErrorFormat($"{BASE_EXCEPTION_FORMAT} {format}", args);
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void Log(string args)
        {
            if (Parameter.OutputLog)
                Debug.Log($"{BASE_LOG_FORMAT} {args}");
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void Log(string format, params object[] args)
        {
            if (Parameter.OutputLog)
                Debug.LogFormat($"{BASE_LOG_FORMAT} {format}", args);
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogError(string args)
        {
            if (Parameter.OutputLog)
                Debug.LogError($"{BASE_ERROR_FORMAT} {args}");
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogError(string format, params object[] args)
        {
            if (Parameter.OutputLog)
                Debug.LogErrorFormat($"{BASE_ERROR_FORMAT} {format}", args);
        }

        #endregion
    }
}