# AssetSystem API 资源系统

### 使用说明

##### 安装 + 卸载 (也可以使用其他资源系统 利用Proxy代理类实现)

- 【YooAsset】 (1.5.7)
- 【安装】 Unity Menu -> AIO -> CLI -> Install -> YooAsset
- 【卸载】 Unity Menu -> AIO -> CLI -> UnInstall -> YooAsset

##### 使用配置文件

```csharp
// 获取或创建配置文件 
var config = ASConfig.GetOrCreate();           
```

```csharp
// 本地配置文件
var config = ASConfig.GetLocal();
```

```csharp
// 远程配置文件
var config = ASConfig.GetRemote("https://xxx.com/xxx");
```

```csharp
// Editor模式下使用本地配置文件
var config = ASConfig.GetEditor();
```

###### 初始化 本地模式

```csharp
var config = ASConfig.GetLocal(); 
await AssetSystem.Initialize(config);
```

###### 初始化 编译器模式

```csharp
var config = ASConfig.GetEditor();
await AssetSystem.Initialize(config);
```

###### 初始化 远端模式

```csharp
var config = ASConfig.GetRemote("https://xxx.com/xxx");   
await AssetSystem.Initialize(config);
var aevent = new AProgressEvent
{
    OnProgress = Debug.Log,         // 进度回调
    OnError = Debug.LogException,   // 错误回调
    OnComplete = () => { }          // 完成回调
}; 
```

- *以下API均为可选API 但是必须选择其中一个API执行 否则无法使用远端模式*

```csharp
// 预下载 远端全部资源包 TIPS: 使用当前API 则无需调用其他预下载API
await AssetSystem.DownloadPre(aevent); // (可选)
// 预下载 指定标签资源包
await AssetSystem.DownloadPreTag("Config", "Shader", aevent); // (可选)
// 预下载 序列记录文件列表
await AssetSystem.DownloadPreRecord(aevent); // (可选)
// 预下载 更新资源包基础信息 (版本号, 清单文件)
await AssetSystem.DownloadDynamic(aevent); // (可选)
```

- *或者使用 自定义下载器*

```csharp
var handle = AssetSystem.GetDownloader(aevent);
await handle.UpdatePackageVersionTask();    // 更新资源包版本
await handle.UpdatePackageManifestTask();   // 更新资源包清单
await handle.DownloadRecordTask(null);      // 下载全部序列记录文件列表
await handle.DownloadTagTask("1.0.0");      // 下载指定标签资源包
await handle.DownloadTask();                // 下载全部资源包
```

###### 初始化模版 [远端 本地 编译器 通用]

```csharp
await AssetSystem.Initialize(ASConfig.GetOrCreate());
var aevent = new AProgressEvent
{
    OnProgress = Debug.Log,
    OnError = Debug.LogException,
    OnComplete = () => { }
};
await AssetSystem.DownloadDynamic(aevent);
Application.quitting += async () => await AssetSystem.Destroy();
```

###### 同步加载资源

```csharp
var location = "资源定位路径";
AssetSystem.LoadAsset(location);            // 加载资源
AssetSystem.LoadSubAssets(location);        // 加载子资源
AssetSystem.InstGameObject(location);       // 实例化GameObject
AssetSystem.LoadRawFileText(location);      // 加载原生文件文本
AssetSystem.LoadRawFileData(location);      // 加载原生文件数据
AssetSystem.LoadScene(location, (r)=>{});   // 加载场景 实际为异步回调加载
```

###### 异步加载资源

```csharp
var location = "资源定位路径";
await AssetSystem.LoadAssetTask(location);        // 加载资源
await AssetSystem.LoadSubAssetsTask(location);    // 加载子资源
await AssetSystem.InstGameObjectTask(location);   // 实例化GameObject
await AssetSystem.LoadRawFileTextTask(location);  // 加载原生文件文本
await AssetSystem.LoadRawFileDataTask(location);  // 加载原生文件数据
await AssetSystem.LoadSceneTask(location);        // 加载场景
```

###### 协程加载资源

```csharp
var location = "资源定位路径";
yield return AssetSystem.LoadAssetCO(location, result => { });        // 加载资源
yield return AssetSystem.LoadSubAssetsCO(location, result => { });    // 加载子资源
yield return AssetSystem.InstGameObjectCO(location, result => { });   // 实例化GameObject
yield return AssetSystem.LoadRawFileTextCO(location, result => { });  // 加载原生文件文本
yield return AssetSystem.LoadRawFileDataCO(location, result => { });  // 加载原生文件数据
yield return AssetSystem.LoadSceneCO(location, result => { });        // 加载场景
```

###### 网络资源加载 [异步回调] 需要支持UniTask

```csharp
var location = "资源定位路径";
AssetSystem.NetLoadBytes(location, result => { });                              // 加载资源
AssetSystem.NetLoadAssetBundle(location, result => { });                        // 加载AB包
AssetSystem.NetLoadSprite(location, Rect.zero, Vector2.zero, result => { });    // 加载Sprite
AssetSystem.NetLoadTexture(location, result => { });                            // 加载Texture
AssetSystem.NetLoadAudioClip(location, AudioType.UNKNOWN, result => { });       // 加载AudioClip
```

###### 网络资源加载 [异步] 需要支持UniTask

```csharp
var location = "资源定位路径";
await AssetSystem.NetLoadBytesTask(location);                               // 加载资源
await AssetSystem.NetLoadAssetBundleTask(location);                         // 加载AB包
await AssetSystem.NetLoadSpriteTask(location, Rect.zero, Vector2.zero);     // 加载Sprite
await AssetSystem.NetLoadTextureTask(location);                             // 加载Texture
await AssetSystem.NetLoadAudioClipTask(location, AudioType.UNKNOWN);        // 加载AudioClip
```

###### 网络资源加载 [协程]

```csharp
var location = "资源定位路径";
yield return AssetSystem.NetLoadBytesCO(location, result => { });                               // 加载资源
yield return AssetSystem.NetLoadAssetBundleCO(location, result => { });                         // 加载AB包
yield return AssetSystem.NetLoadSpriteCO(location, Rect.zero, Vector2.zero, result => { });     // 加载Sprite
yield return AssetSystem.NetLoadTextureCO(location, result => { });                             // 加载Texture
yield return AssetSystem.NetLoadAudioClipCO(location, AudioType.UNKNOWN, result => { });        // 加载AudioClip
```
