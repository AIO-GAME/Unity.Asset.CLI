#if SUPPORT_WHOOTHOT
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rol.Game
{
    [Serializable]
    public class PathAddressConfig : ScriptableObject
    {
        [Serializable]
        public class ConfigEntry
        {
            public const string PASS = "*";
            public string path;
            public string fileFilter;
            public string addressFormat;
            public string groupFormat;
            public string labelFormat;
            public CacheLevel cacheLevel;
            public bool StaticContent = true; //该组是否静态
            public bool IsLocalGroup = true; //本地还是远端资源

            public string GetRelativePath(string path)
            {
                int len = this.path.Length;
                return path.Substring(len + (this.path[len - 1] == '/' ? 0 : 1)).Replace('\\', '/');
            }
        }

        public const string ASSET_PATH = "Assets/Editor/PathAddressConfig.asset";

        public static PathAddressConfig Instance
        {
            get
            {
                if (File.Exists(ASSET_PATH))
                {
                    return AssetDatabase.LoadAssetAtPath<PathAddressConfig>(ASSET_PATH);
                }
                else
                {
                    var obj = CreateInstance<PathAddressConfig>();
                    var dir = Directory.GetParent(Application.dataPath);
                    if (dir is null)
                    {
                        Debug.LogError("PathAddressConfig.Instance: Directory.GetParent(Application.dataPath) is null");
                        return null;
                    }

                    var index = ASSET_PATH.LastIndexOf('/');
                    var path = Path.Combine(dir.FullName, ASSET_PATH.Substring(0, index));
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    AssetDatabase.CreateAsset(obj, ASSET_PATH);
                    return obj;
                }
            }
        }

        public List<ConfigEntry> nodes;

        private int Compare(ConfigEntry a, ConfigEntry b)
        {
            int la = a.path.Length;
            int lb = b.path.Length;
            int cmp;
            for (int i = 0, max = Math.Min(la, lb); i < max; ++i)
            {
                cmp = a.path[i] - b.path[i];
                if (0 != cmp)
                {
                    return cmp;
                }
            }

            return la - lb;
        }

        public void SortConfig()
        {
            nodes?.Sort(Compare);
        }

        // 根据路径找配置
        // orRoot 为真时，找到离Path最近的配置； orRoot为假时，找到路径的配置。
        public ConfigEntry GetPathConfigEntry(string path, ref bool parent)
        {
            parent = false;
            if (null == nodes || nodes.Count == 0)
            {
                return null;
            }

            var config = GetPathConfigEntry(path);
            if (config != null)
            {
                return config;
            }

            config = GetPathParentConfigEntry(path);
            if (config != null)
            {
                parent = true;
                return config;
            }

            return null;
        }

        public ConfigEntry GetPathConfigEntry(string path)
        {
            if (null == nodes || nodes.Count == 0)
            {
                return null;
            }

            int i = 0;
            int j = nodes.Count - 1;
            int mid = 0;
            while (i <= j)
            {
                mid = ((i + j) >> 1);
                string config = nodes[mid].path;
                int lc = config.Length;
                int lp = path.Length;
                int min = Math.Min(lc, lp);
                int cmp = 0;
                for (int k = 0; k < min; ++k)
                {
                    cmp = config[k] - path[k];
                    if (cmp < 0)
                    {
                        i = Math.Max(i + 1, mid);
                        break;
                    }

                    if (cmp > 0)
                    {
                        j = Math.Min(j - 1, mid);
                        break;
                    }
                }

                if (cmp == 0)
                {
                    if ((lc == lp) ||
                        (min > 0 && lc == lp - 1 && config[lc - 1] != '/' && path[lp - 1] == '/') ||
                        (min > 0 && lc == lp + 1 && config[lc - 1] == '/' && path[lp - 1] != '/'))
                    {
                        return nodes[mid];
                    }

                    if (lc > lp)
                    {
                        j = Math.Min(j - 1, mid);
                    }

                    if (lc < lp)
                    {
                        i = Math.Max(i + 1, mid);
                    }
                }
            }

            return null;
        }

        public ConfigEntry GetPathParentConfigEntry(string path)
        {
            if (null == nodes || nodes.Count == 0)
            {
                return null;
            }

            ConfigEntry parent = null;
            int lp = path.Length;
            int lc, cmp;
            for (int i = 0, maxi = nodes.Count; i < maxi; ++i)
            {
                var node = nodes[i];
                lc = node.path.Length;
                cmp = 0;
                for (int j = 0, maxj = Math.Min(lp, lc); j < maxj; ++j)
                {
                    cmp = node.path[j] - path[j];
                    if (cmp != 0)
                    {
                        break;
                    }
                }

                if (cmp == 0)
                {
                    if (lc == lp)
                    {
                        return node;
                    }

                    if (lc <= lp)
                    {
                        parent = node;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (cmp > 0)
                {
                    break;
                }
            }

            return parent;
        }

        // 查找是否有给定路径的自路径有配置。
        public bool HasSubConfig(string path)
        {
            if (null == nodes || nodes.Count == 0)
            {
                return false;
            }

            int i = 0;
            int j = nodes.Count - 1;
            while (i <= j)
            {
                int mid = (i + j) >> 1;
                var config = nodes[mid].path;
                int lc = config.Length;
                int lp = path.Length;
                int cmp = 0;
                for (int k = 0, min = Math.Min(lc, lp); k < min; ++k)
                {
                    cmp = config[k] - path[k];
                    if (cmp < 0)
                    {
                        i = Math.Max(i + 1, mid);
                        break;
                    }

                    if (cmp > 0)
                    {
                        j = Math.Min(j - 1, mid);
                        break;
                    }
                }

                if (cmp == 0)
                {
                    if (lc > lp)
                    {
                        return true;
                    }
                    else
                    {
                        i = Math.Max(i + 1, mid);
                    }
                }
            }

            return false;
        }
    }
}
#endif