#if SUPPORT_WHOOTHOT
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rol.Game
{
    public static class PathUtils
    {
        private const string KEY_PATH = "{P:";
        private const string KEY_NAME = "{Name}";
        private const string KEY_EXT = "{Ext}";
        private const string KEY_FIRST = "{First}";
        private static readonly string[] KEYS = new string[] { KEY_PATH, KEY_NAME, KEY_EXT };
        private const string CONTROLL_CHARS = "&|!$";

        // {P:n} : 第几级目录， n 为负数表示倒数第几个目录，-1是文件名
        // {Name} ： 名字，不带扩展名
        // {Ext} ： 扩展名，不带点
        // eg : Format("Addr/{P:2}/{P:-2}/{Name}-{Ext}", "Assets/GUI/Icon/Itmes/1234.jpg") => "Addr/Icon/Items/1234-jpg"
        public static string FormatPath(string fmt, string path)
        {
            if (string.IsNullOrEmpty(fmt))
            {
                return fmt;
            }

            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            string name = null;
            string ext = null;
            string firstCharacter = "";
            var paths = path.Split('/');
            if (path[path.Length - 1] != '/')
            {
                var fullName = paths[paths.Length - 1];
                int index = fullName.LastIndexOf('.');
                if (index >= 0)
                {
                    name = fullName.Substring(0, index);
                    ext = fullName.Substring(index + 1);
                }
                else
                {
                    name = fullName;
                    ext = string.Empty;
                }

                if (fullName.Length > 0)
                    firstCharacter = fullName[0].ToString();
            }

            StringBuilder sb = new StringBuilder();
            int cur = 0;
            bool matchKey = false;
            for (int i = 0, max = fmt.Length; i < max; ++i)
            {
                if (matchKey)
                {
                    matchKey = false;
                    string str = fmt.Substring(cur, i - cur + 1);
                    foreach (var key in KEYS)
                    {
                        if (key.StartsWith(str))
                        {
                            if (str.Length == key.Length)
                            {
                                switch (str)
                                {
                                    case KEY_PATH:
                                    {
                                        int index = fmt.IndexOf('}', i);
                                        if (index > 0 && int.TryParse(fmt.Substring(i + 1, index - i - 1),
                                                out int pathIndex))
                                        {
                                            pathIndex = pathIndex >= 0 ? pathIndex : paths.Length + pathIndex;
                                            i = index;
                                            cur = i + 1;
                                            sb.Append(pathIndex >= 0 && pathIndex < paths.Length
                                                ? paths[pathIndex]
                                                : "null");
                                        }
                                    }
                                        break;
                                    case KEY_NAME:
                                    {
                                        cur = i + 1;
                                        sb.Append(name ?? "null");
                                    }
                                        break;
                                    case KEY_EXT:
                                    {
                                        cur = i + 1;
                                        sb.Append(ext ?? "null");
                                    }
                                        break;
                                    case KEY_FIRST:
                                    {
                                        cur = i + 1;
                                        sb.Append(firstCharacter);
                                    }
                                        break;
                                }
                            }
                            else
                            {
                                matchKey = true;
                            }

                            break;
                        }
                    }
                }
                else
                {
                    if (fmt[i] == '{')
                    {
                        if (i != cur)
                        {
                            sb.Append(fmt, cur, i - cur);
                            cur = i;
                        }

                        matchKey = true;
                    }
                }
            }

            int remain = fmt.Length - cur;
            if (remain > 0)
            {
                sb.Append(fmt, cur, remain);
            }

            return sb.ToString();
        }

        /// <summary>
        /// filter解析
        /// </summary>
        public static void ParseFileFilter(string filter, out List<string> containList, out List<string> blockList)
        {
            containList = new List<string>();
            blockList = new List<string>();

            var sb = new StringBuilder();
            var isNegate = false;
            for (int i = 0, max = filter.Length; i <= max; ++i)
            {
                var c = '\0';
                var curIsCtrl = true;
                if (i < max)
                {
                    c = filter[i];
                    curIsCtrl = CONTROLL_CHARS.Contains(c);
                }

                switch (curIsCtrl)
                {
                    case false:
                        sb.Append(c);
                        break;
                    case true:
                    {
                        if (sb.Length > 0)
                        {
                            string suffix = sb.ToString();
                            if (isNegate)
                                blockList.Add(suffix);
                            else
                                containList.Add(suffix);
                            isNegate = false;
                            sb.Clear();
                        }

                        switch (c)
                        {
                            case '|':
                                break;
                            case '&':
                                break;
                            case '!':
                                isNegate = true;
                                break;
                            default:
                                break;
                        }

                        break;
                    }
                }
            }
        }

        public static bool IsPathPassFilter(string path, List<string> containList, List<string> blockList)
        {
            if (string.IsNullOrEmpty(path)) return false;
            if (blockList.Count <= 0) return containList.Count <= 0 || containList.Any(path.EndsWith);
            if (blockList.Any(path.Contains)) return false;
            return containList.Count <= 0 || containList.Any(path.EndsWith);
        }
    }
}
#endif