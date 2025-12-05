# Android Logcat Monitor for Unity
# Run this while the app is running to see debug logs

$ADB = "C:\Program Files\Unity\Hub\Editor\2022.3.56f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe"

Write-Host "=== Monitoring Unity Logs ===" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host ""

& $ADB logcat -c  # Clear existing logs
& $ADB logcat Unity:V DEBUG:V *:S
