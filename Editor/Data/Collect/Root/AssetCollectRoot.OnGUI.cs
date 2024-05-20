using System;
using UnityEngine;

namespace AIO.UEditor
{
    partial class AssetCollectRoot
    {
        /// <summary>
        ///     当前选择包下标
        /// </summary>
        [HideInInspector]
        public int CurrentPackageIndex;

        /// <summary>
        ///     当前选择组下标
        /// </summary>
        [HideInInspector]
        public int CurrentGroupIndex;

        /// <summary>
        ///    当前选择收集器下标
        /// </summary>
        [HideInInspector]
        public int CurrentCollectIndex;

        #region OnGUI

        public AssetCollectItem CurrentCollect
        {
            get
            {
                if (CurrentGroup.Collectors is null )
                {
                    CurrentGroup.Collectors = Array.Empty<AssetCollectItem>();
                    CurrentCollectIndex     = -1;
                    return CurrentGroup.Collectors[0];
                }

                if (CurrentGroup.Collectors.Length == 0)
                {
                    CurrentCollectIndex = -1;
                    return null;
                }

                if (CurrentCollectIndex < 0) CurrentCollectIndex = 0;
                else if (CurrentGroup.Collectors.Length <= CurrentCollectIndex)
                    CurrentCollectIndex = CurrentGroup.Collectors.Length - 1;

                return CurrentGroup.Collectors[CurrentCollectIndex];
            }
        }

        public AssetCollectPackage CurrentPackage
        {
            get
            {
                if (Packages is null)
                {
                    Packages            = Array.Empty<AssetCollectPackage>();
                    CurrentPackageIndex = -1;
                    return null;
                }

                if (Packages.Length == 0)
                {
                    CurrentPackageIndex = -1;
                    return null;
                }

                if (CurrentPackageIndex < 0)
                    CurrentPackageIndex = 0;
                else if (Packages.Length <= CurrentPackageIndex)
                    CurrentPackageIndex = Packages.Length - 1;

                return Packages[CurrentPackageIndex];
            }
        }

        public AssetCollectGroup CurrentGroup
        {
            get
            {
                if (CurrentPackage is null) return null;
                if (CurrentPackage.Groups is null)
                {
                    CurrentPackage.Groups = Array.Empty<AssetCollectGroup>();
                    CurrentGroupIndex     = -1;
                    return null;
                }

                if (CurrentPackage.Groups.Length == 0)
                {
                    CurrentGroupIndex = -1;
                    return null;
                }

                if (CurrentGroupIndex < 0)
                {
                    CurrentGroupIndex = 0;
                }
                else if (CurrentPackage.Groups.Length <= CurrentGroupIndex)
                {
                    CurrentGroupIndex = CurrentPackage.Groups.Length - 1;
                }

                return CurrentPackage.Groups[CurrentGroupIndex];
            }
        }

        public bool IsValidPackage()
        {
            if (Packages == null)
                Packages = Array.Empty<AssetCollectPackage>();
            if (CurrentPackageIndex >= Packages.Length)
                CurrentPackageIndex = Packages.Length - 1;
            return Packages.Length > 0;
        }

        public bool IsValidGroup()
        {
            if (!IsValidPackage()) return false;

            if (CurrentPackageIndex < 0) CurrentPackageIndex                = 0;
            if (Packages.Length <= CurrentPackageIndex) CurrentPackageIndex = 0;
            if (Packages[CurrentPackageIndex].Groups != null)
                return Packages[CurrentPackageIndex].Groups.Length > 0;

            Packages[CurrentPackageIndex].Groups = Array.Empty<AssetCollectGroup>();
            return false;
        }

        public bool IsValidCollect()
        {
            if (!IsValidGroup()) return false;

            if (CurrentGroupIndex < 0) CurrentGroupIndex                                            = 0;
            if (Packages[CurrentPackageIndex].Groups.Length <= CurrentGroupIndex) CurrentGroupIndex = 0;

            if (Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors is null)
            {
                Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors = Array.Empty<AssetCollectItem>();
                return false;
            }

            return Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length > 0;
        }

        #endregion
    }
}