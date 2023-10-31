using System.IO;

#if UNITY_EDITOR

internal static class Setting
{
    /// <summary>
    /// 名称
    /// </summary>
    public static string Name = "AIO.Asset";

    /// <summary>
    /// 版本
    /// </summary>
    public static string Version = "";

    [UnityEditor.InitializeOnLoadMethod]
    public static void Initialize()
    {
        var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(Setting).Assembly);
        var packageJson = AHelper.Json.ToHashTable(File.ReadAllText(package.resolvedPath + "/package.json"));
        Version = packageJson["version"].ToString();
    }
}
#endif