# ⚡ ChatClient 1.3.0.0 命令速查表

## 🎯 最常用命令 (赤手可用)

### 发布所有平台 (一键)
```powershell
powershell.exe -ExecutionPolicy Bypass -File .\publish-crossplatform.ps1 -Version 1.3.0.0
```
**用时**: ~6-10 分钟  
**输出**: `publish-crossplatform\1.3.0.0\` (11 个文件夹)

---

## 🏗️ 构建命令

### 构建 Windows WPF 版
```powershell
dotnet build ChatClient.csproj -c Release
```

### 构建 CrossPlatform 版
```powershell
dotnet build ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release
```

### 清空构建输出
```powershell
dotnet clean
```

---

## 📦 单一平台发布

### Windows x64 (最常用)
```powershell
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r win-x64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/win-x64
```

### Linux x64 (Ubuntu/Debian)
```powershell
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r linux-x64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/linux-x64
```

### macOS ARM64 (Apple Silicon)
```powershell
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r osx-arm64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/osx-arm64
```

### 所有 Windows
```powershell
# x86
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r win-x86 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/win-x86

# x64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r win-x64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/win-x64

# ARM64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r win-arm64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/win-arm64
```

### 所有 Linux (glibc)
```powershell
# x64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r linux-x64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/linux-x64

# ARM64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r linux-arm64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/linux-arm64

# ARM
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r linux-arm --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/linux-arm

# x86
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r linux-x86 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/linux-x86
```

### 所有 Linux (musl/Alpine)
```powershell
# x64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r linux-musl-x64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/linux-musl-x64

# ARM64
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r linux-musl-arm64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/linux-musl-arm64
```

### 所有 macOS
```powershell
# x64 (Intel)
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r osx-x64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/osx-x64

# ARM64 (Apple Silicon)
dotnet publish ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release -r osx-arm64 --self-contained true --no-restore -p:PublishSingleFile=true -p:Version=1.3.0.0 -o publish-test/osx-arm64
```

---

## 🔍 验证命令

### 检查编译输出
```powershell
# Windows
file .\ChatClient.exe

# Linux/macOS
file ./ChatClient
```

### 查看执行文件信息
```powershell
# Windows
Get-Item ChatClient.exe | Select-Object -Property Name, Length

# Linux
ls -lh ChatClient

# macOS
ls -lh ChatClient
```

### 查看依赖 (Linux glibc)
```bash
ldd ./ChatClient
```

### 查看依赖 (macOS)
```bash
otool -L ./ChatClient
```

### 测试运行 (显示版本信息)
```powershell
# Windows
.\ChatClient.exe

# Linux
./ChatClient

# macOS
./ChatClient
```

---

## 📁 文件/目录命令

### 列出发布输出
```powershell
# 列出所有平台文件夹
ls publish-crossplatform/1.3.0.0/

# 列出特定平台内容
ls publish-crossplatform/1.3.0.0/linux-x64/

# 显示所有文件大小
Get-ChildItem publish-crossplatform/1.3.0.0/ -Recurse | Select-Object -Property Name, @{Name="Size(MB)"; Expression={[math]::Round($_.Length/1MB,2)}}
```

### 清空旧版本
```powershell
# 删除旧发布目录
Remove-Item publish-crossplatform/1.2.7 -Recurse -Force

# 只删除特定平台
Remove-Item publish-crossplatform/1.3.0.0/linux-x64 -Recurse -Force
```

### 计算文件哈希 (用于验证)
```powershell
# Windows
Get-FileHash .\publish-crossplatform\1.3.0.0\windows-x64\ChatClient.exe -Algorithm SHA256

# Linux
sha256sum publish-crossplatform/1.3.0.0/linux-x64/ChatClient

# macOS
shasum -a 256 publish-crossplatform/1.3.0.0/macos-x64/ChatClient
```

---

## 📝 编辑/更新命令

### 更新版本号 (升级到 1.3.1.0)
```powershell
# 编辑 ChatClient.csproj
# 编辑 ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj
# 将所有版本号从 1.3.0.0 改为 1.3.1.0

# 编辑 publish-crossplatform.ps1
# 将第 2 行的版本改为 1.3.1.0
```

### 恢复到特定版本 (Git)
```powershell
# 查看版本历史
git log --oneline

# 恢复到特定提交
git checkout <commit-hash>

# 恢复到特定标签
git checkout v1.3.0.0
```

---

## 🐛 故障排除命令

### 检查 .NET 版本
```powershell
dotnet --version
dotnet --info
```

### 清理 NuGet 缓存
```powershell
dotnet nuget locals all --clear
```

### 强制重新还原依赖
```powershell
dotnet restore ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj --force
```

### 查看最后的构建日志
```powershell
# Windows (PowerShell)
Get-Content $env:TEMP\msbuild.log -Tail 50

# Linux/macOS
tail -50 /tmp/msbuild.log
```

### 关闭占用文件的进程 (Windows)
```powershell
# 关闭所有 ChatClient 进程
Get-Process ChatClient -ErrorAction SilentlyContinue | Stop-Process -Force

# 查看哪些进程在使用特定文件
Get-Process | Where-Object {$_.ProcessName -like "*ChatClient*"}
```

---

## 📊 信息查询命令

### 获取项目信息
```powershell
# 从 csproj 读取版本
[xml]$xml = Get-Content ChatClient.csproj -Raw
$xml.Project.PropertyGroup.Version

# 读取多个属性
[xml]$xml = Get-Content ChatClient.csproj -Raw
$props = $xml.Project.PropertyGroup
@{
	Version=$props.Version
	AssemblyVersion=$props.AssemblyVersion
	Authors=$props.Authors
	Company=$props.Company
}
```

### 查看发布配置
```powershell
# 查看 csproj 中的所有属性
[xml]$xml = Get-Content ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -Raw
$xml.Project.PropertyGroup | Get-Member -MemberType Property
```

### 统计文档
```powershell
# 统计所有 1.3.0.0 文档
Get-ChildItem *1.3.0.0*.md | Measure-Object -Property Length -Sum

# 统计单个文档行数
(Get-Content QUICK_RELEASE_GUIDE_1.3.0.0.md).Count
```

---

## 🚀 快速脚本 (复制即用)

### 全流程发布脚本 (Batch)
```powershell
# 保存为 auto-release.ps1
param([string]$Version = "1.3.0.0")

Write-Host "准备发布 ChatClient $Version..." -ForegroundColor Cyan
Write-Host ""

# 1. 构建
Write-Host "步骤 1: 构建项目..." -ForegroundColor Yellow
dotnet build ChatClient.CrossPlatform/ChatClient.CrossPlatform.csproj -c Release
if ($LASTEXITCODE -ne 0) { throw "构建失败" }

# 2. 发布
Write-Host "步骤 2: 发布到所有平台..." -ForegroundColor Yellow
powershell.exe -ExecutionPolicy Bypass -File .\publish-crossplatform.ps1 -Version $Version
if ($LASTEXITCODE -ne 0) { throw "发布失败" }

# 3. 验证
Write-Host "步骤 3: 验证输出..." -ForegroundColor Yellow
$count = (Get-ChildItem "publish-crossplatform\$Version\" -Directory).Count
Write-Host "✅ 发布完成: 生成 $count 个平台的可执行文件" -ForegroundColor Green

# 4. 显示输出目录
Write-Host ""
Write-Host "输出目录: $(Resolve-Path "publish-crossplatform\$Version\")" -ForegroundColor Cyan
```

### 验证所有输出 (Check Script)
```powershell
# 保存为 verify-releases.ps1
param([string]$Version = "1.3.0.0")

$dir = "publish-crossplatform\$Version"
$folders = Get-ChildItem $dir -Directory

Write-Host "验证 ChatClient $Version 所有发布版本..." -ForegroundColor Cyan
Write-Host ""

foreach ($folder in $folders) {
	$exeName = if ($folder.Name -like "windows*") { "ChatClient.exe" } else { "ChatClient" }
	$exePath = Join-Path $folder.FullName $exeName

	if (Test-Path $exePath) {
		$size = (Get-Item $exePath).Length / 1MB
		Write-Host "✅ $($folder.Name): $([math]::Round($size, 2)) MB" -ForegroundColor Green
	} else {
		Write-Host "❌ $($folder.Name): 文件未找到" -ForegroundColor Red
	}
}
```

---

## 📋 RID 完整列表 (快速参考)

```
Windows:
  win-x86          → windows-x86
  win-x64          → windows-x64
  win-arm64        → windows-arm64

Linux (glibc):
  linux-x86        → linux-x86
  linux-arm        → linux-arm
  linux-x64        → linux-x64
  linux-arm64      → linux-arm64

Linux (musl):
  linux-musl-x64   → linux-musl-x64
  linux-musl-arm64 → linux-musl-arm64

macOS:
  osx-x64          → macos-x64
  osx-arm64        → macos-arm64
```

---

## ⏱️ 预计耗时

| 操作 | 耗时 |
|------|------|
| 构建 (增量) | 2-3 秒 |
| 构建 (全量) | 5-10 秒 |
| 单平台发布 | 30-60 秒 |
| 11 平台全量发布 | 6-10 分钟 |
| 验证输出 | 1-2 秒 |

---

## 📞 快速参考

| 需求 | 命令 |
|------|------|
| 发布所有平台 | `powershell .\publish-crossplatform.ps1` |
| 发布 Linux x64 | `dotnet publish ... -r linux-x64 ...` |
| 发布 macOS | `dotnet publish ... -r osx-x64/osx-arm64 ...` |
| 检查版本 | `[xml](gc ChatClient.csproj) | % {$_.Project.PropertyGroup.Version}` |
| 清空输出 | `rm publish-crossplatform -Recurse` |
| 查看帮助 | 参考 QUICK_RELEASE_GUIDE_1.3.0.0.md |

---

**速查表版本**: 1.3.0.0  
**最后更新**: 2024  
**许可证**: MIT License - Creaddinscart Team
