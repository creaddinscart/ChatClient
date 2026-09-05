param(
	[string] $Version = ''
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'ChatClient.csproj'

# Read current version from csproj if no version argument provided
if ([string]::IsNullOrWhiteSpace($Version)) {
	try {
		[xml]$xml = Get-Content $project -Raw
		$vnode = $xml.Project.PropertyGroup.Version
		if ($vnode) { $Version = $vnode.'#text' }
	} catch { $Version = '' }
}

if ([string]::IsNullOrWhiteSpace($Version)) { throw 'Version is required. Use -Version 1.1.1.' }

# Ensure Assembly/File version are 4-part numbers
function To-FourPartVersion($v) {
	$parts = $v -split '\.' | Where-Object { $_ -ne '' }
	while ($parts.Length -lt 4) { $parts += '0' }
	return ($parts[0..3] -join '.')
}

$assemblyVersion = To-FourPartVersion $Version
$fileVersion = $assemblyVersion

$out = Join-Path $PSScriptRoot ("publish\$Version")

# If ChatClient is running anywhere, prompt to close
function Ensure-NotRunning {
	$procs = Get-Process -Name 'ChatClient' -ErrorAction SilentlyContinue
	if ($procs) {
		Write-Host "Detected running ChatClient process(es)."
		$answer = Read-Host "Close running instances now? (Y to close, N to abort publish)"
		if ($answer -match '^[Yy]') {
			foreach ($p in $procs) {
				try { $p.CloseMainWindow() | Out-Null; Start-Sleep -Milliseconds 800; if (!$p.HasExited) { $p.Kill(); } }
				catch { Write-Host "Could not stop process $($p.Id): $($_.Exception.Message)" }
			}
			Start-Sleep -Seconds 1
		} else { throw "Publish aborted by user because ChatClient.exe is running." }
	}
}

Ensure-NotRunning

if (Test-Path $out) { Remove-Item $out -Recurse -Force }

dotnet publish $project -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:Version=$Version -p:AssemblyVersion=$assemblyVersion -p:FileVersion=$fileVersion -o $out

Write-Host "Published: $out\ChatClient.exe"
