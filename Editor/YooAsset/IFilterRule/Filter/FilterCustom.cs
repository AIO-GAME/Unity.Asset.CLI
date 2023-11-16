/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-11-16
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("使用 UserData 实现自定义过滤规则")]
    public class FilterRuleCustomAsset : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            return ParseFileFilter(data.AssetPath, data.UserData);
        }

        private const string CONTROL_CHARS = "&|!$";

        /// <summary>
        /// filter解析
        /// </summary>
        private static bool ParseFileFilter(string path, string filter)
        {
            var containList = new List<string>();
            var blockList = new List<string>();

            var sb = new StringBuilder();
            var isNegate = false;
            for (int i = 0, max = filter.Length; i <= max; ++i)
            {
                var c = '\0';
                var curIsCtrl = true;
                if (i < max)
                {
                    c = filter[i];
                    curIsCtrl = CONTROL_CHARS.Contains(c);
                }

                if (curIsCtrl)
                {
                    if (sb.Length > 0)
                    {
                        var suffix = sb.ToString();
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
                    }
                }
                else sb.Append(c);
            }

            return IsPathPassFilter(path, containList, blockList);
        }

        private static bool IsPathPassFilter(string path, ICollection<string> containList,
            ICollection<string> blockList)
        {
            if (string.IsNullOrEmpty(path)) return false;

            if (blockList.Count > 0)
            {
                if (blockList.Any(path.Contains)) return false;
            }

            if (containList.Count > 0)
            {
                if (containList.Any(path.EndsWith)) return true;
            }
            else return true;

            return false;
        }
    }
}
#endif