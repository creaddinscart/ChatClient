# ChatClient 1.3.0.0 平台支持矩阵

## 📋 完整的跨平台支持

### ✅ Windows (3 种架构)
```
┌─ Windows 64-bit (x64)        [win-x64]
├─ Windows 32-bit (x86)        [win-x86]
└─ Windows ARM64               [win-arm64]
```
- **最新要求**: Windows 7 SP1 或更高版本
- **运行时**: .NET 8 Runtime
- **格式**: 自包含单文件 (.exe)

### ✅ Linux (6 种配置: glibc + musl)

#### glibc 发行版 (Debian/Ubuntu, RHEL/CentOS, etc.)
```
┌─ Linux x64 (glibc)           [linux-x64]
├─ Linux ARM64 (glibc)         [linux-arm64]
├─ Linux ARM 32-bit (glibc)    [linux-arm]
└─ Linux x86 (glibc)           [linux-x86]
```

#### musl 发行版 (Alpine Linux, etc.)
```
┌─ Linux x64 (musl/Alpine)     [linux-musl-x64]
└─ Linux ARM64 (musl/Alpine)   [linux-musl-arm64]
```
- **最低版本**: glibc 2.17+ 或 musl 1.1.11+
- **运行时**: .NET 8 Runtime (glibc 或 musl)
- **格式**: 自包含单文件 (ELF)

### ✅ macOS (2 种架构)
```
┌─ macOS Intel (x64)           [osx-x64]
└─ macOS Apple Silicon (ARM64) [osx-arm64]
```
- **最低版本**: macOS 10.15 Catalina 或更高
- **运行时**: .NET 8 Runtime
- **格式**: 自包含单文件 (Mach-O)

## 发布输出结构

```
publish-crossplatform/1.3.0.0/
├── windows-x86/
│   ├── ChatClient.exe
│   ├── ChatClient.dll
│   └── ... (运行时文件)
├── windows-x64/
│   ├── ChatClient.exe
│   ├── ChatClient.dll
│   └── ... (运行时文件)
├── windows-arm64/
│   └── ... (运行时文件)
├── linux-x64/
│   ├── ChatClient
│   ├── ChatClient.dll
│   └── ... (运行时文件)
├── linux-arm64/
│   └── ... (运行时文件)
├── linux-arm/
│   └── ... (运行时文件)
├── linux-x86/
│   └── ... (运行时文件)
├── linux-musl-x64/
│   └── ... (运行时文件)
├── linux-musl-arm64/
│   └── ... (运行时文件)
├── macos-x64/
│   ├── ChatClient
│   ├── ChatClient.dll
│   └── ... (运行时文件)
└── macos-arm64/
	├── ChatClient
	├── ChatClient.dll
	└── ... (运行时文件)
```

## 每个平台的可执行文件

| 平台 | 可执行文件名 | 大小 (典型) |
|------|-----------|----------|
| Windows | ChatClient.exe | ~80-120 MB |
| Linux | ChatClient | ~80-120 MB |
| macOS | ChatClient | ~80-120 MB |

*注: 大小因包含的依赖和配置而异，所有都是自包含的*

## 系统要求总结

### Windows
- Windows 7 SP1, Windows 8, Windows 10, Windows 11
- 支持 x86, x64, ARM64 架构

### Linux
- **推荐**: Ubuntu 16.04+, Debian 9+, CentOS 7+, RHEL 7+
- **Alpine**: 支持 Alpine Linux 3.6+
- 支持 x86, ARM, x64, ARM64 架构
- 支持 glibc 2.17+ 和 musl 1.1.11+

### macOS
- macOS 10.15 Catalina 或更新版本
- 支持 Intel (x64) 和 Apple Silicon (ARM64)
- 通用二进制文件可使用 lipo 工具组合

## 发布命令

### 一键发布所有平台 (11 个)
```powershell
powershell.exe -ExecutionPolicy Bypass -File .\publish-crossplatform.ps1 -Version 1.3.0.0
```

### 发布特定平台 (手动命令)
```powershell
# 例: 发布 Linux x64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj `
  -c Release -r linux-x64 --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:EnableCompressionInSingleFile=true `
  -p:Version=1.3.0.0 `
  -o publish-crossplatform/1.3.0.0/linux-x64
```

## 运行方式

### Windows
```cmd
.\ChatClient.exe
```

### Linux
```bash
chmod +x ./ChatClient
./ChatClient
```

### macOS
```bash
chmod +x ./ChatClient
./ChatClient
```

## 验证发布产物

### 快速验证
```bash
# Linux/macOS
file ./ChatClient

# Windows
file .\ChatClient.exe
```

### 查看依赖
```bash
# Linux (glibc 版本)
ldd ./ChatClient

# macOS
otool -L ./ChatClient
```

## 性能优化

所有发布构建均启用：
- ✅ 单文件发布: 合并所有程序集
- ✅ 自包含: 包含 .NET 运行时
- ✅ 原生库自包含: 包含原生依赖
- ✅ 文件压缩: 减少最终包大小

## 已知限制

### 尚不支持的平台
- 32-bit x86 macOS (deprecated)
- Older Windows versions < Windows 7 SP1
- Older glibc < 2.17

### 平台特定注意事项
- **Windows on ARM64**: 需要 Windows 11 S 或更高
- **Linux ARM**: 需要对应架构的硬件或寄存器
- **macOS**: 可能需要绕过 Gatekeeper (代码签名问题)

## 许可证
MIT License - Creaddinscart Team
