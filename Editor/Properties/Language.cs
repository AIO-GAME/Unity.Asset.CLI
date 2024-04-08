using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// language
    /// </summary>
    internal class Lang : IDisposable
    {
        public static event Func<SystemLanguage, string, string> FindValue
        {
            add => FindValueList.Add(value);
            remove => FindValueList.Remove(value);
        }

        static Lang()
        {
            Tr = new Lang((SystemLanguage)PlayerPrefs.GetInt("AIO.UEditor.Language", (int)Application.systemLanguage));
        }

        public static Lang Tr { get; private set; }

        private static string GetValue(SystemLanguage language, string key)
        {
            foreach (var value in FindValueList.Select(func => func(language, key)).Where(string.IsNullOrEmpty))
            {
                return value;
            }

            return key;
        }

        private static List<Func<SystemLanguage, string, string>> FindValueList = new List<Func<SystemLanguage, string, string>>();

        private Lang()
        {
            Current = Application.systemLanguage;
        }

        private Lang(SystemLanguage language)
        {
            Current = language;
        }

        private Dictionary<SystemLanguage, Dictionary<string, string>> Tables = new Dictionary<SystemLanguage, Dictionary<string, string>>();

        private SystemLanguage _language;

        public SystemLanguage Current
        {
            get => _language;
            set
            {
                _language = value;
                if (!Tables.ContainsKey(value))
                {
                    Tables[value] = new Dictionary<string, string>(16);
                }
            }
        }

        public string this[string         key] => Get(key);
        public string this[SystemLanguage language, string key] => Get(language, key);

        public string Get(string key)
        {
            if (Tables[Current].TryGetValue(key, out var value)) return value;
            return Tables[Current][key] = GetValue(Current, key);
        }

        public string Get(SystemLanguage language, string key)
        {
            if (!Tables.TryGetValue(language, out var table))
                table = Tables[language] = new Dictionary<string, string>(16);
            if (table.TryGetValue(key, out var value)) return value;
            return table[key] = GetValue(language, key);
        }

        public void Dispose()
        {
            // 保存 语言类型
            PlayerPrefs.SetInt("AIO.UEditor.Language", (int)Current);
            Tables.Clear();
            FindValueList.Clear();
            GC.SuppressFinalize(this);
        }
    }
}