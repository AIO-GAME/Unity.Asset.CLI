using UnityEditor.IMGUI.Controls;

namespace AIO.UEditor
{
    public class TreeViewBuildSetting : TreeViewRowSingle
    {
        private ASBuildConfig config;

        public TreeViewBuildSetting(TreeViewState state, MultiColumnHeader header) : base(state, header)
        {
            showAlternatingRowBackgrounds = false;
            showBorder                    = true;
            MainColumnIndex               = 1;
        }

        protected override void OnInitialize() { config = ASBuildConfig.GetOrCreate(); }

        public static TreeViewBuildSetting Create(float width = 100, float min = 80, float max = 200)
        {
            return new TreeViewBuildSetting(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                GetMultiColumnHeaderColumn("标题", 100, 75, 100, false),
                GetMultiColumnHeaderColumn("选项", width, min, max, false),
            })));
        }

        private readonly string[] Options =
        {
            "功能按钮",
            "输出路径",
            "生成Latest版本",
            "验证构造结构",
            "构建版本",
            "构建平台",
            "构建包名",
            "构建管线",
            "压缩模式",
            "构建模式",
            "首包标签",
            "缓存清理数量"
        };

        protected override void OnBuildRows(TreeViewItem root)
        {
            for (var index = 0; index < Options.Length; index++)
            {
                root.AddChild(new TreeViewItemBuildSetting(index, Options[index], config));
            }
        }
    }
}