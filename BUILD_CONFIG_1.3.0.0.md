# ChatClient v1.3.0.0 构建配置完整清单

## 项目属性配置

### 共通属性 (All Projects)
| 属性 | 值 |
|-----|-----|
| Version | 1.3.0.0 |
| AssemblyVersion | 1.3.0.0 |
| FileVersion | 1.3.0.0 |
| Authors | Creaddinscart Team |
| Company | Creaddinscart Team |
| Product | ChatClient |
| Copyright | MIT License - Creaddinscart Team |
| PackageProjectUrl | https://github.com/Creaddinscart/ChatClient |

### Windows 桌面版 (ChatClient.csproj)
| 属性 | 值 |
|-----|-----|
| OutputType | WinExe |
| TargetFramework | net8.0-windows |
| UseWPF | true |
| Nullable | enable |
| ImplicitUsings | enable |
| AssemblyName | ChatClient |
| RootNamespace | ChatClient |
| AssemblyTitle | ChatClient |
| ApplicationManifest | app.manifest |
| PublishSingleFile | true |
| SelfContained | true |
| IncludeNativeLibrariesForSelfExtract | true |
| EnableCompressionInSingleFile | true |
| Deterministic | true |
| Description | ChatClient v1.3.0.0 — English-only chat client with video preview, quotes, copying, emoji and unique server usernames. |

### 跨平台版 (ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj)
| 属性 | 值 |
|-----|-----|
| OutputType | Exe |
| TargetFramework | net8.0 |
| Nullable | enable |
| ImplicitUsings | enable |
| AssemblyName | ChatClient |
| RootNamespace | ChatClient.CrossPlatform |
| AssemblyTitle | ChatClient |
| PublishSingleFile | true |
| SelfContained | true |
| IncludeNativeLibrariesForSelfExtract | true |
| EnableCompressionInSingleFile | true |
| Deterministic | true |
| Description | ChatClient 1.3.0.0 cross-platform client |

## 发布 RID 矩阵

### Windows 平台 (3 个)
| 架构 | RID | 输出文件夹 | 说明 |
|------|-----|----------|-----|
| x86 | win-x86 | windows-x86 | 32位Windows |
| x64 | win-x64 | windows-x64 | 64位Windows |
| ARM64 | win-arm64 | windows-arm64 | Windows on ARM64 |

### Linux (glibc) 平台 (4 个)
| 架构 | RID | 输出文件夹 | 说明 |
|------|-----|----------|-----|
| x86 | linux-x86 | linux-x86 | 32位Linux (glibc) |
| ARM | linux-arm | linux-arm | ARM 32位Linux (glibc) |
| x64 | linux-x64 | linux-x64 | 64位Linux (glibc) |
| ARM64 | linux-arm64 | linux-arm64 | ARM64 Linux (glibc) |

### Linux (musl) 平台 (2 个)
| 架构 | RID | 输出文件夹 | 说明 |
|------|-----|----------|-----|
| x64 | linux-musl-x64 | linux-musl-x64 | 64位Linux (Alpine/musl) |
| ARM64 | linux-musl-arm64 | linux-musl-arm64 | ARM64 Linux (Alpine/musl) |

### macOS 平台 (2 个)
| 架构 | RID | 输出文件夹 | 说明 |
|------|-----|----------|-----|
| x64 | osx-x64 | macos-x64 | macOS Intel (x64) |
| ARM64 | osx-arm64 | macos-arm64 | macOS Apple Silicon (ARM64) |

## 总计：11 个发布目标

## NuGet 依赖 (CrossPlatform 项目)
- Avalonia 11.2.3
- Avalonia.Desktop 11.2.3
- Avalonia.Themes.Fluent 11.2.3

## 构建命令

### 构建 Windows 桌面版
```powershell
dotnet build ChatClient.csproj -c Release
```

### 构建 CrossPlatform 版
```powershell
dotnet build ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release
```

### 发布 Windows 桌面版
```powershell
powershell.exe -ExecutionPolicy Bypass -File .\publish.ps1 -Version 1.3.0.0
```
输出: `publish\1.3.0.0\ChatClient.exe`

### 发布所有跨平台版本
```powershell
powershell.exe -ExecutionPolicy Bypass -File .\publish-crossplatform.ps1 -Version 1.3.0.0
```
输出: `publish-crossplatform\1.3.0.0\{platform-arch}\` (共11个平台)

## 版本更新时间点

如需更新版本，需修改以下文件：
1. `ChatClient.csproj` - 行 10-12 (Version, AssemblyVersion, FileVersion)
2. `ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj` - 行 9-11
3. `publish.ps1` - 不需要修改（自动从 csproj 读取）
4. `publish-crossplatform.ps1` - 行 2 (Version 参数)
5. `README.md` - 第3行及发布命令示例
6. `RELEASE_NOTES_x.x.x.x.md` - 新增发布说明

## 验证清单

- [x] 项目属性完整配置
- [x] 版本号统一到 1.3.0.0
- [x] 跨平台 RID 矩阵完整 (11 种)
- [x] 单文件自包含发布配置
- [x] 原生库自包含开启
- [x] 文件压缩开启
- [x] 确定性构建配置
- [x] 所有项目包含 AssemblyTitle
- [x] 所有项目包含 PackageProjectUrl
- [x] 所有项目包含 Copyright
- [x] CrossPlatform 项目所有共享代码正确链接
