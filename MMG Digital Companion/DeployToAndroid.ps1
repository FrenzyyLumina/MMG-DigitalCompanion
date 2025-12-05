# Unity Android Build and Deploy Helper
# Place this in the project root and run it after Unity builds

$ADB = "C:\Program Files\Unity\Hub\Editor\2022.3.56f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe"
$PACKAGE_NAME = "com.CultofKojima.MMGDigitalCompanion"
$APK_PATH = ".\Library\Bee\Android\Prj\Mono2x\Gradle\launcher\build\outputs\apk\debug\launcher-debug.apk"

Write-Host "=== Unity Android Deploy Helper ===" -ForegroundColor Cyan
Write-Host ""

# Check if APK exists
if (-not (Test-Path $APK_PATH)) {
    Write-Host "Error: APK not found at $APK_PATH" -ForegroundColor Red
    Write-Host "Please build the project in Unity first." -ForegroundColor Yellow
    exit 1
}

$apkInfo = Get-Item $APK_PATH
Write-Host "APK Found:" -ForegroundColor Green
Write-Host "  Path: $($apkInfo.FullName)"
Write-Host "  Size: $([math]::Round($apkInfo.Length/1MB,2)) MB"
Write-Host "  Modified: $($apkInfo.LastWriteTime)"
Write-Host ""

# Check device connection
Write-Host "Checking device connection..." -ForegroundColor Yellow
$devices = & $ADB devices
Write-Host $devices
Write-Host ""

# Install APK
Write-Host "Installing APK..." -ForegroundColor Yellow
& $ADB install -r $APK_PATH
Write-Host ""

# Launch app
Write-Host "Launching app..." -ForegroundColor Yellow
& $ADB shell am start -n "$PACKAGE_NAME/com.unity3d.player.UnityPlayerActivity"
Write-Host ""

Write-Host "Done! Check your device." -ForegroundColor Green
