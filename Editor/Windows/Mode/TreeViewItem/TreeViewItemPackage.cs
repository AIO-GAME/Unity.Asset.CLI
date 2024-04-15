using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// TreeViewItemPackage
    /// </summary>
    internal sealed class TreeViewItemPackage : TreeViewItem
    {
        private static Texture _MainIcon;
        private static Texture _Icon;

        private static Texture Icon     => _Icon ?? (_Icon = Resources.Load<Texture>("Editor/Icon/Color/-school-bag"));
        private static Texture MainIcon => _MainIcon ?? (_MainIcon = Resources.Load<Texture>("Editor/Icon/Color/-briefcase"));

        public AssetCollectPackage Package { get; }

        public TreeViewItemPackage(int id, AssetCollectPackage package) : base(id, id, package.Name)
        {
            Package = package;
        }

        public override Texture2D icon        => Package.Default ? MainIcon as Texture2D : Icon as Texture2D;
        public override string    displayName => Package.Name;
        public override string    ToString()  => Package.ToString();
    }
}