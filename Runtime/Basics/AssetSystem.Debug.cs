using System;
using System.Diagnostics;
using UnityEngine;
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

        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogWarning(string format, params object[] args)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
                Debug.LogWarningFormat($"{BASE_WARNING_FORMAT} {format}", args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogWarning(string message)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
                Debug.LogWarning($"{BASE_WARNING_FORMAT} {message}");
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="exception">Runtime Exception.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(Exception exception)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
#if UNITY_EDITOR
                Debug.LogError($"{BASE_EXCEPTION_FORMAT} {exception}");
#else
                Debug.LogException(new Exception($"{BASE_EXCEPTION_FORMAT} {exception}"));
#endif
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="exception">Runtime Exception.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(string exception)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
                Debug.LogError($"{BASE_EXCEPTION_FORMAT} {exception}");
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogException(string format, params object[] args)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
                Debug.LogErrorFormat($"{BASE_EXCEPTION_FORMAT} {format}", args);
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void Log(string message)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
                Debug.Log($"{BASE_LOG_FORMAT} {message}");
        }

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void Log(string format, params object[] args)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
                Debug.LogFormat($"{BASE_LOG_FORMAT} {format}", args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogError(string message)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
                Debug.LogError($"{BASE_ERROR_FORMAT} {message}");
        }

        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump, Conditional("DEBUG")]
        public static void LogError(string format, params object[] args)
        {
#if UNITY_EDITOR
            if (Parameter is null || Parameter.OutputLog)
#else
            if (Parameter.OutputLog)
#endif
                Debug.LogErrorFormat($"{BASE_ERROR_FORMAT} {format}", args);
        }

        #endregion
    }
}