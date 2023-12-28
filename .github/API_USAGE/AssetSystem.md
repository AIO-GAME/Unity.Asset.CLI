# AssetSystem API 资源系统

### 使用说明

##### 安装 + 卸载 (也可以使用其他资源系统 利用 Proxy 代理类实现)
    - 【YooAsset】 (1.5.7)
    - 【安装】 Unity Menu -> AIO -> CLI -> Install -> YooAsset
    - 【卸载】 Unity Menu -> AIO -> CLI -> UnInstall -> YooAsset

#### 创建配置文件 搭配 API [`AssetSystem.Initialize(ASConfig.GetOrCreate());`](../../Runtime/Basics/AssetSystem.cs#L25)

| Create Config API                                               | Description                                | Mode         |
| :-------------------------------------------------------------- | :----------------------------------------- | :----------- |
| [`ASConfig.GetOrCreate`](../../Runtime/Config/ASConfig.cs#L214) | 获取已存在配置文件夹 或创建编辑器配置文件  | 配置自选模式 |
| [`ASConfig.GetLocal`](../../Runtime/Config/ASConfig.cs#L286)    | 获取已存在配置文件夹 或 创建本地配置文件   | 本地模式     |
| [`ASConfig.GetRemote`](../../Runtime/Config/ASConfig.cs#L256)   | 获取已存在配置文件夹 或 创建远端配置文件   | 远端模式     |
| [`ASConfig.GetEditor`](../../Runtime/Config/ASConfig.cs#L309)   | 获取已存在配置文件夹 或 创建编辑器配置文件 | 编辑器模式   |

#### *初始化 远端模式*

```csharp
public async Task RemoteInitialize()
{
    await AssetSystem.Initialize(ASConfig.GetRemote("https://xxx.com/xxx"));
    var AssetEvent = new DownlandAssetEvent
    {
        OnProgress = Debug.Log,         // 进度回调
        OnError = Debug.LogException,   // 错误回调
        OnComplete = () => { }          // 完成回调
    };
    // Tips : 以下 API 均为可选 API 但是必须选择其中一个 API 执行 否则无法使用远端模式
    // 下载指定标签资源包 并且下载序列记录文件
    await AssetSystem.DownloadTagWithRecord("Config", AssetEvent);
    // or 下载全部资源包
    await AssetSystem.DownloadAll(AssetEvent);
    // or 下载全部资源包基础信息 (版本号, 清单文件) 不下载实际资源包
    await AssetSystem.DownloadHeader(AssetEvent);
    // or 下载全部序列记录文件列表
    await AssetSystem.DownloadRecord(AssetEvent);
    // or 使用自定义下载器
    using (var handle = AssetSystem.GetDownloader(AssetEvent))
    {
        await handle.UpdateHeader();
        handle.Begin();
        // 下载根据需求调用API 也可以不执行下面函数 只更新资源包版本和清单文件
        handle.CollectNeedTag("Config");    // 下载指定标签资源包
        handle.CollectNeedRecord();         // 下载序列记录文件
        handle.CollectNeedAll();            // 下载全部资源包
        await handle.WaitCo();

        // 操作 API 如果需要暂停等操作可执行以下API
        handle.Cancel(); // 取消下载
        handle.Pause();  // 暂停下载
        handle.Resume(); // 恢复下载
    }
}
```

| *Load Sync API*                                                                              | *Description*               |
| :------------------------------------------------------------------------------------------- | :-------------------------- |
| [`AssetSystem.LoadAsset("location")`](../../Runtime/Basics/AssetSystem.Load.cs#L156)         | 加载资源                    |
| [`AssetSystem.LoadSubAssets("location")`](../../Runtime/Basics/AssetSystem.Load.cs#27)       | 加载子资源                  |
| [`AssetSystem.InstGameObject("location")`](../../Runtime/Basics/AssetSystem.Inst.cs#24)      | 实例化 GameObject            |
| [`AssetSystem.LoadRawFileText("location")`](../../Runtime/Basics/AssetSystem.Load.cs#458)    | 加载原生文件文本            |
| [`AssetSystem.LoadRawFileData("location")`](../../Runtime/Basics/AssetSystem.Load.cs#490)    | 加载原生文件数据            |
| [`AssetSystem.LoadScene("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Load.cs#400) | 加载场景 实际为异步回调加载 |

| *Load Async API*                                                                                    | *Description*               |
| :-------------------------------------------------------------------------------------------------- | :-------------------------- |
| [`await AssetSystem.LoadAssetTask("location")`](../../Runtime/Basics/AssetSystem.Load.cs#361)       | 加载资源                    |
| [`await AssetSystem.LoadSubAssetsTask("location")`](../../Runtime/Basics/AssetSystem.Load.cs#141)   | 加载子资源                  |
| [`await AssetSystem.InstGameObjectTask("location")`](../../Runtime/Basics/AssetSystem.Inst.cs#48)   | 实例化 GameObject            |
| [`await AssetSystem.LoadRawFileTextTask("location")`](../../Runtime/Basics/AssetSystem.Load.cs#478) | 加载原生文件文本            |
| [`await AssetSystem.LoadRawFileDataTask("location")`](../../Runtime/Basics/AssetSystem.Load.cs#510) | 加载原生文件数据            |
| [`await AssetSystem.LoadSceneTask("location")`](../../Runtime/Basics/AssetSystem.Load.cs#439)       | 加载场景 实际为异步回调加载 |

| *Load Coroutine API*                                                                                              | *Description*               |
| :---------------------------------------------------------------------------------------------------------------- | :-------------------------- |
| [`yield return AssetSystem.LoadAssetCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Load.cs#306)       | 加载资源                    |
| [`yield return AssetSystem.LoadSubAssetsCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Load.cs#65)    | 加载子资源                  |
| [`yield return AssetSystem.InstGameObjectCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Inst.cs#97)   | 实例化 GameObject            |
| [`yield return AssetSystem.LoadRawFileTextCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Load.cs#532) | 加载原生文件文本            |
| [`yield return AssetSystem.LoadRawFileDataCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Load.cs#521) | 加载原生文件数据            |
| [`yield return AssetSystem.LoadSceneCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Load.cs#420)       | 加载场景 实际为异步回调加载 |


| *Net Load Async Callback API (Need Support UniTask)*                                                  | *Description* |
| :---------------------------------------------------------------------------------------------------- | :------------ |
| [`AssetSystem.NetLoadBytes("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#L121)       | 加载资源      |
| [`AssetSystem.NetLoadAssetBundle("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#L156) | 加载 AB 包      |
| [`AssetSystem.NetLoadSprite("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#L87)       | 加载 Sprite    |
| [`AssetSystem.NetLoadTexture("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#L68)      | 加载 Texture   |
| [`AssetSystem.NetLoadAudioClip("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#L139)   | 加载 AudioClip |


| *Net Load Async API (Need Support UniTask)*                                                            | *Description* |
| :----------------------------------------------------------------------------------------------------- | :------------ |
| [`await AssetSystem.NetLoadBytesTask("location")`](../../Runtime/Basics/AssetSystem.Net.cs#L320)       | 加载资源      |
| [`await AssetSystem.NetLoadAssetBundleTask("location")`](../../Runtime/Basics/AssetSystem.Net.cs#L350) | 加载 AB 包      |
| [`await AssetSystem.NetLoadSpriteTask("location")`](../../Runtime/Basics/AssetSystem.Net.cs#L291)      | 加载 Sprite    |
| [`await AssetSystem.NetLoadTextureTask("location")`](../../Runtime/Basics/AssetSystem.Net.cs#L274)     | 加载 Texture   |
| [`await AssetSystem.NetLoadAudioClipTask("location")`](../../Runtime/Basics/AssetSystem.Net.cs#L336)   | 加载 AudioClip |


| *Net Load Coroutine API*                                                                                            | *Description* |
| :------------------------------------------------------------------------------------------------------------------ | :------------ |
| [`yield return AssetSystem.NetLoadBytesCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#L256)      | 加载资源      |
| [`yield return AssetSystem.NetLoadAssetBundleCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#213) | 加载 AB 包      |
| [`yield return AssetSystem.NetLoadSpriteCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#195)      | 加载 Sprite    |
| [`yield return AssetSystem.NetLoadTextureCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#179)     | 加载 Texture   |
| [`yield return AssetSystem.NetLoadAudioClipCO("location", (r)=>{})`](../../Runtime/Basics/AssetSystem.Net.cs#228)   | 加载 AudioClip |

