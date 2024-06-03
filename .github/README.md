```
 ██╗   ██╗███╗   ██╗██╗████████╗██╗   ██╗    █████╗ ███████╗███████╗███████╗████████╗
 ██║   ██║████╗  ██║██║╚══██╔══╝╚██╗ ██╔╝   ██╔══██╗██╔════╝██╔════╝██╔════╝╚══██╔══╝
 ██║   ██║██╔██╗ ██║██║   ██║    ╚████╔╝    ███████║███████╗███████╗█████╗     ██║   
 ██║   ██║██║╚██╗██║██║   ██║     ╚██╔╝     ██╔══██║╚════██║╚════██║██╔══╝     ██║   
 ╚██████╔╝██║ ╚████║██║   ██║      ██║      ██║  ██║███████║███████║███████╗   ██║    
  ╚═════╝ ╚═╝  ╚═══╝╚═╝   ╚═╝      ╚═╝      ╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝   ╚═╝  
```
<p align="center"> <a href="README_EN.md"> English </a> | 简体中文 </p>
<p align="center">
<a href="https://github.com/AIO-GAME/Unity.Asset.CLI/security/policy"> <img alt="" src="https://img.shields.io/github/package-json/unity/AIO-GAME/Unity.Asset.CLI"> </a>
<a href="https://github.com/AIO-GAME/Unity.Asset.CLI/blob/main/LICENSE.md"> <img alt="" src="https://img.shields.io/github/license/AIO-GAME/Unity.Asset.CLI"> </a>
<a href="https://img.shields.io/github/languages/code-size/AIO-GAME/Unity.Asset.CLI"> <img alt="" src="https://img.shields.io/github/languages/code-size/AIO-GAME/Unity.Asset.CLI"> </a>
<a href="https://github.com/AIO-GAME/Unity.Asset.CLI/issues"> <img alt="" src="https://img.shields.io/github/issues/AIO-GAME/Unity.Asset.CLI"> </a>
</p>
<p align="center">
<a href="https://github.com/AIO-GAME/Unity.Asset.CLI/tags"> <img alt="" src="https://img.shields.io/github/package-json/version/AIO-GAME/Unity.Asset.CLI"> </a>
<a href="https://openupm.com/packages/com.aio.cli.asset/"> <img alt="" src="https://img.shields.io/npm/v/com.aio.cli.asset?label=openupm&amp;registry_uri=https://package.openupm.com" /> </a>
<a href="https://github.com/AIO-GAME/Unity.Asset.CLI"> <img alt="" src="https://img.shields.io/github/stars/AIO-GAME/Unity.Asset.CLI"> </a>
</p>

### ⚙ 安装

<details>
<summary>
<span style="color: deepskyblue; "> <b> Packages Manifest </b> </span>
</summary>

````json
{
  "dependencies": {
    "com.aio.cli.asset": "latest"
  },
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.aio.cli.asset",
        "com.aio.package"
      ]
    }
  ]
}
````

</details>

<details>
<summary>
<span style="color: deepskyblue; "> <b> Unity PackageManager </b> </span>
</summary>

> openupm *中国版*
>> `Name: package.openupm.cn`
> > `URL: https://package.openupm.cn`
> > `Scope(s): com.aio.cli.asset`
>
> openupm *国际版*
>> `Name: package.openupm.com`
> > `URL: https://package.openupm.com`
> > `Scope(s): com.aio.cli.asset`

</details>

<details>
<summary>
<span style="color: deepskyblue; "> <b> Command Line </b> </span>
</summary>

> *openupm-cli*
>> `openupm add com.aio.cli.asset`

</details>

### ⭐ 关于

- **这是 Unity 的资源加载接口(CLI)包。它提供了一组命令和工具来增强效率和开发体验。**

> [!IMPORTANT]
> - ✅ **支持 Unity 2019.4 及以上版本**
> - ✅ **支持 资源 同步加载/异步加载/协程加载 接口**
> - ✅ **支持 资源 本地模式/远程模式/编译器模式 接口**
> - ✅ **支持 空包 首包 整包 自定义分包**
> - ✅ **支持 Android/iOS/Windows/Mac/WebGL**
> - ✅ **支持 CI/CD 流水线资源打包**
> - ✅ **支持 .NET 4.x**
> - ✅ **支持 il2cpp**
> - ✅ **支持 Unity 增量构建**
> - ✅ **支持 [UniTask](https://github.com/Cysharp/UniTask)**
> - ✅ **支持 [YooAsset](https://github.com/tuyoogame/YooAsset)**
> - ❌ **支持 [XAsset](https://github.com/xasset/xasset)**
> - ❌ **支持 [Addressable](https://github.com/Unity-Technologies/Addressables-Sample)**
> - ❌ **支持 自定义代理第三方插件**

### 📖 文档

- [_**文档目录**_](https://github.com/AIO-GAME/Unity.Asset.CLI/wiki)
- [_**API**_](./API_USAGE/AssetSystem.md)
- [_**配置讲解**_](./API_USAGE/Config.md)
- [_**工具使用**_](./API_USAGE/ToolWindow.md)

### 🔗 第三方参考资料和工具

**Please refer to the wiki for a list of references and tools used in this package.**

|                         **Doc**                          |                     **Description**                     |
|:--------------------------------------------------------:|:-------------------------------------------------------:|
|     **[Common](https://github.com/AIO-GAME/Common)**     |         _**C# Unity 通用基础函数库,用于帮助研发团队快速构建框架。**_          |
| **[UniTask](https://github.com/Cysharp/UniTask#readme)** |      _**为 Unity 提供了一个有效的分配自由的 async / await 集成。**_      |
|         **[YooAsset](https://www.yooasset.com)**         | _**YooAsset 是一套用于 Unity3D 的资源管理系统，用于帮助研发团队快速部署和交付游戏。**_ |

### ✨ 贡献者

<!-- readme: collaborators,contributors -start -->
<table>
	<tbody>
		<tr>
            <td align="center">
                <a href="https://github.com/xinansky">
                    <img src="https://avatars.githubusercontent.com/u/45371089?v=4" width="64;" alt="xinansky"/>
                    <br />
                    <sub><b>xinansky</b></sub>
                </a>
            </td>
            <td align="center">
                <a href="https://github.com/Starkappa">
                    <img src="https://avatars.githubusercontent.com/u/155533864?v=4" width="64;" alt="Starkappa"/>
                    <br />
                    <sub><b>Starkappa</b></sub>
                </a>
            </td>
		</tr>
	<tbody>
</table>
<!-- readme: collaborators,contributors -end -->

### 📢 致谢

- **谢谢您选择我们的扩展包。**
- **如果此软件包对您有所帮助。**
- **请考虑通过添加⭐来表示支持。**