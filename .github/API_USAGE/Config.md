# Asset System

> ## `类`
>> ### <a name="AIO.UEditor.ASBuildConfig"> AIO.UEditor.ASBuildConfig </a>  [<i> 资源打包配置 </i>]
>>> | Type | Field | Default | Description |
>>> |:---- |:----- |:------- |:---------- |
>>> |[`EBuildPipeline`](#AIO.UEditor.EBuildPipeline)|`BuildPipeline` | BuiltinBuildPipeline | 打包管线 |
>>> |[`EBuildMode`](#AIO.UEditor.EBuildMode)|`BuildMode` | ForceRebuild | 构建模式 |
>>> |`string`|`BuildVersion` | string.Empty | 构建版本号  |
>>> |`string`|`PackageName` | string.Empty | 资源包名称 |
>>> |`string`|`EncyptionClassName` | string.Empty | 加密模式 |
>>> |`string`|`CompressedModeName` | string.Empty | 压缩模式 |
>>> |`string`|`FirstPackTag` | string.Empty | 首包标签集合 |
>>> |`string`|`BuildOutputPath` | string.Empty | 构建结果输出路径 |
>>> |`bool`|`ValidateBuild` | false | 验证构建结果 |
>> ---
>> ### <a name="AIO.UEditor.AssetCollectRoot"> AIO.UEditor.AssetCollectRoot </a>  [<i> 资源收集配置 </i>]
>>> | Type | Field | Default | Description |
>>> |:---- |:----- |:------- |:---------- |
>>> |`bool`|`EnableAddressable` | false | 开启地址化 |
>>> |`bool`|`UniqueBundleName` | false | Bundle 名称唯一 |
>>> |`bool`|`IncludeAssetGUID` | false | 包含资源 GUID |
>>> |`AssetCollectPackage[]`|`Packages` | Array.Empty | 资源收集配置 |
>> ---
>> ###  <a name="AIO.UEngine.ASConfig"> AIO.UEngine.ASConfig </a> [<i> 资源系统配置 </i>]
>>> | Type | Field | Default | Description |
>>> |:---- |:----- |:------- |:---------- |
>>> |[`EASMode`](#AIO.UEngine.EASMode)|`ASMode` | EASMode.Editor | 资源加载模式 |
>>> |`string`|`URL` | string.Empty | 热更新资源包服务器地址 |
>>> |`bool`|`AutoSaveVersion` | true | 自动激活清单 |
>>> |`bool`|`AppendTimeTicks` | true | URL 请求附加时间搓 |
>>> |`bool`|`LoadPathToLower` | true | 加载路径转小写 |
>>> |`bool`|`EnableSequenceRecord` | false | 自动序列记录 |
>>> |`bool`|`OutputLog` | false | 日志输出 |
>>> |`int`|`DownloadFailedTryAgain` | 1 | 下载失败尝试次数 |
>>> |`int`|`LoadingMaxTimeSlice` | 144 | 资源加载的最大数量 |
>>> |`int`|`Timeout` | 60 | 超时时间 |
>>> |`string`|`RuntimeRootDirectory` | BuiltinFiles | 运行时内置文件根目录 |
>> ---
>> ###  <a name="AIO.UEngine.AssetsPackageConfig"> AIO.UEngine.AssetsPackageConfig </a> [<i> 资源包配置 </i>]
>>> | Type | Field | Default | Description |
>>> |:---- |:----- |:------- |:---------- |
>>> |`string`|`Name` | string.Empty | 名称 |
>>> |`string`|`Version` | string.Empty | 版本 |
>>> |`bool`|`IsDefault` | false | 是否为默认包 |
>>> |`bool`|`IsSidePlayWithDownload` | false | 资源包动态下载加载 |
> ---
> ## `枚举`
>> ### AIO.UEngine.EASMode <a name="AIO.UEngine.EASMode"> </a> [<i> 资源加载模式 </i>]
>>> | Name| Value | Description |
>>> |:---|:--:|:--- |
>>> |`Editor`|`0`| 编辑器模式 |
>>> |`Remote`|`1`| 远端模式 |
>>> |`Local`|`2`| 本地模式 |
>> ---
>> ### AIO.UEditor.EBuildPipeline <a name="AIO.UEditor.EBuildPipeline"> </a> [<i> 构建管线 </i>]
>>> | Name| Value | Description |
>>> |:---|:--:|:--- |
>>> |`BuiltinBuildPipeline`|`0`| 内置打包管线 |
>>> |`ScriptableBuildPipeline`|`1`| 自定义打包管线(需安装) |
>> ---
>> ### AIO.UEditor.EBuildPipeline <a name="AIO.UEditor.EBuildMode"> </a> [<i> 资源包流水线的构建模式 </i>]
>>> | Name| Value | Description |
>>> |:---|:--:|:--- |
>>> |`ForceRebuild`|`0`| 强制重建模式 |
>>> |`IncrementalBuild`|`1`| 增量构建模式 |
>>> |`DryRunBuild`|`2`| 演练构建模式 |
>>> |`SimulateBuild`|`3`| 模拟构建模式 |



