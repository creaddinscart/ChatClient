# 🚀 ChatClient 1.3.0.0 快速发布指南

## 📋 一句话总结
**ChatClient 已全面升级到 1.3.0.0，支持 Windows、Linux (6种配置)、macOS，共 11 个发行平台。**

---

## ⚡ 快速开始

### 一键发布所有 11 个平台
```powershell
powershell.exe -ExecutionPolicy Bypass -File .\publish-crossplatform.ps1 -Version 1.3.0.0
```

### 一键构建并发布 Windows 版
```powershell
dotnet build ChatClient.csproj -c Release
powershell.exe -ExecutionPolicy Bypass -File .\publish.ps1 -Version 1.3.0.0
```

---

## 🖥️ 平台速览

| 平台 | 支持架构 | 发行数量 | 输出格式 |
|------|--------|--------|--------|
| **Windows** | x86, x64, ARM64 | 3 | .exe |
| **Linux (glibc)** | x86, ARM, x64, ARM64 | 4 | ELF |
| **Linux (musl)** | x64, ARM64 | 2 | ELF |
| **macOS** | x64, ARM64 | 2 | Mach-O |
| **合计** | | **11** | |

---

## 📂 输出目录结构

```
publish-crossplatform/1.3.0.0/
├── windows-x86/       ← Windows 32位
├── windows-x64/       ← Windows 64位
├── windows-arm64/     ← Windows ARM64
├── linux-x86/         ← Linux 32位
├── linux-arm/         ← Linux ARM
├── linux-x64/         ← Linux 64位
├── linux-arm64/       ← Linux ARM64
├── linux-musl-x64/    ← Alpine Linux x64
├── linux-musl-arm64/  ← Alpine Linux ARM64
├── macos-x64/         ← macOS Intel
└── macos-arm64/       ← macOS Apple Silicon
```

---

## ✅ 版本属性清单

### 核心属性 (所有项目)
```
Version:          1.3.0.0
AssemblyVersion:  1.3.0.0
FileVersion:      1.3.0.0
Authors:          Creaddinscart Team
Company:          Creaddinscart Team
Product:          ChatClient
Copyright:        MIT License - Creaddinscart Team
```

### Windows 版特有属性
```
TargetFramework:  net8.0-windows
UseWPF:           true
OutputType:       WinExe
```

### CrossPlatform 版特有属性
```
TargetFramework:  net8.0
OutputType:       Exe
PublishSingleFile:           true
SelfContained:               true
IncludeNativeLibrariesForSelfExtract: true
EnableCompressionInSingleFile:        true
Deterministic:    true
```

---

## 📄 文档导航

| 文档 | 用途 |
|------|------|
| **README.md** | 项目概述和使用说明 |
| **RELEASE_NOTES_1.3.0.0.md** | 1.3.0.0 版本发布说明 |
| **BUILD_CONFIG_1.3.0.0.md** | 完整的构建配置参考 |
| **PLATFORM_SUPPORT_1.3.0.0.md** | 平台支持详细矩阵 |
| **PUBLISH_VERIFICATION_1.3.0.0.md** | 发布前验证清单 |
| **QUICK_RELEASE_GUIDE_1.3.0.0.md** | 本快速指南 |

---

## 🔧 常用命令

### 仅构建（不发布）
```powershell
# Windows WPF 版
dotnet build ChatClient.csproj -c Release

# CrossPlatform 版
dotnet build ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release
```

### 发布特定平台
```powershell
# 例: Linux x64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj `
  -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o publish-out

# 例: macOS ARM64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj `
  -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -o publish-out
```

### 验证输出
```bash
# Linux/macOS 验证
file ChatClient

# Windows 验证
file ChatClient.exe
```

---

## 🎯 发布前检查清单

- [x] 版本号已更新到 1.3.0.0
- [x] 所有项目属性已配置
- [x] CrossPlatform 项目可正常编译
- [x] 发布脚本已测试语法
- [x] RID 矩阵包含全部 11 个平台
- [x] ReadMe 和文档已更新
- [x] 文件压缩已启用
- [x] 自包含发布已启用

---

## 🚢 发布工作流

### 步骤 1: 验证代码
```powershell
git status
# 确保没有未提交的更改或清理暂存区
```

### 步骤 2: 构建测试
```powershell
dotnet build ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release
# ✅ 应该成功
```

### 步骤 3: 发布全部平台
```powershell
powershell.exe -ExecutionPolicy Bypass -File .\publish-crossplatform.ps1 -Version 1.3.0.0
# 等待所有 11 个平台完成发布
```

### 步骤 4: 验证产物
```powershell
ls publish-crossplatform/1.3.0.0/
# 应该看到 11 个文件夹
```

### 步骤 5: 创建 GitHub Release
- 上传 `publish-crossplatform/1.3.0.0/` 中所有文件夹中的可执行文件
- 标签: `v1.3.0.0`
- 标题: `ChatClient v1.3.0.0 - Full Cross-Platform Release`
- 描述: 参考 RELEASE_NOTES_1.3.0.0.md

---

## 🐛 故障排除

### 错误: "未找到 dotnet 命令"
```powershell
# 安装 .NET 8 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### 错误: "RID xxx 不支持"
```
某些 RID (如 linux-x86) 在某些环境不可用
跳过失败的 RID，发布脚本会继续处理其他平台
```

### 错误: "文件被占用"
```powershell
# 关闭所有 ChatClient 进程
Get-Process ChatClient | Stop-Process -Force
```

### 大小过大 (超过 150 MB)
```
这是正常的 - 包含了完整 .NET 8 运行时
启用了压缩 (EnableCompressionInSingleFile=true)
```

---

## 📊 性能指标

| 指标 | 值 |
|------|-----|
| 构建时间 | ~2-3 秒 (增量) |
| 全量构建 | ~5-10 秒 |
| 单平台发布 | ~30-60 秒 |
| 全 11 平台发布 | ~6-10 分钟 |
| 单个可执行文件大小 | ~80-120 MB |

---

## 💡 最佳实践

1. **本地测试后再发布** - 先在至少一个平台上测试
2. **保存构建日志** - 用于调试和审计
3. **标记版本** - 使用 git tag 标记每个发布
4. **备份前一版本** - 以防需要回滚
5. **验证哈希** - 发布前计算文件 SHA256

---

## 📞 更新版本

如需更新到新版本 (如 1.3.1.0):

### 1. 更新项目文件
编辑 `ChatClient.csproj` 和 `ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj`:
```xml
<Version>1.3.1.0</Version>
<AssemblyVersion>1.3.1.0</AssemblyVersion>
<FileVersion>1.3.1.0</FileVersion>
```

### 2. 更新脚本
编辑 `publish-crossplatform.ps1` 第 2 行:
```powershell
[string]$Version = '1.3.1.0'
```

### 3. 更新文档
- 更新 README.md
- 新建 RELEASE_NOTES_1.3.1.0.md

### 4. 发布
```powershell
powershell.exe -ExecutionPolicy Bypass -File .\publish-crossplatform.ps1 -Version 1.3.1.0
```

---

## 📜 许可证
MIT License - Creaddinscart Team

---

**版本**: 1.3.0.0  
**最后更新**: 2024  
**状态**: ✅ 生产就绪
