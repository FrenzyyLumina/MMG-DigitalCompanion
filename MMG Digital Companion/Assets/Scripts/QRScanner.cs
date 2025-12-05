using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Android;

public class QRScanner : MonoBehaviour
{

    [SerializeField]
    private RawImage _rawImageBG;
    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;
    [SerializeField]
    private TextMeshProUGUI _textResult;
    [SerializeField]
    private RectTransform _scanZone;

    private bool _isCamAvailable;
    private WebCamTexture _camTexture;
    private bool _isScanning = false;
    private float _scanInterval = 0.5f; // Scan every 0.5 seconds
    private float _lastScanTime = 0f;

    void Start()
    {
        RequestCameraPermission();
    }
    
    void RequestCameraPermission()
    {
        #if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            StartCoroutine(WaitForPermission());
        }
        else
        {
            SetUpCamera();
        }
        #else
        SetUpCamera();
        #endif
    }
    
    IEnumerator WaitForPermission()
    {
        float timeout = 10f;
        float elapsed = 0f;
        
        while (!Permission.HasUserAuthorizedPermission(Permission.Camera) && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            SetUpCamera();
        }
        else
        {
            _textResult.text = "Camera permission denied!";
            _isCamAvailable = false;
            Debug.LogError("Camera permission was not granted!");
        }
    void Update()
    {
        UpdateCameraRender();
        
        // Auto-scan continuously when camera is available
        if (_isCamAvailable && !_isScanning && Time.time - _lastScanTime >= _scanInterval)
        {
            _lastScanTime = Time.time;
            Scan();
        }
    }
        UpdateCameraRender();
    }

    private void SetUpCamera()
    {
        // Check if UI elements are assigned
        if (_textResult == null)
        {
            Debug.LogError("_textResult is not assigned in Inspector!");
            _isCamAvailable = false;
            return;
        }
        
        if (_rawImageBG == null)
        {
            Debug.LogError("_rawImageBG is not assigned in Inspector!");
            _textResult.text = "UI not configured!";
            _isCamAvailable = false;
            return;
        }
        
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0)
        {
            _isCamAvailable = false;
            _textResult.text = "No camera detected!";
            Debug.LogError("No camera devices found!");
            return;
        }
        
        Debug.Log($"Found {devices.Length} camera device(s)");
        
        // Try to find back camera first, then fall back to any camera
        for(int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"Camera {i}: {devices[i].name}, Front facing: {devices[i].isFrontFacing}");
            
            if(!devices[i].isFrontFacing)
            {
                _camTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                break;
            }
        }
        
        // If no back camera found, use first available camera
        if(_camTexture == null && devices.Length > 0)
        {
            _camTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
            Debug.Log("No back camera found, using first available camera");
        }
        
        if(_camTexture == null)
        _camTexture.Play();
        _rawImageBG.texture = _camTexture;
        _isCamAvailable = true;
        _textResult.text = "Point camera at QR code...";
        _lastScanTime = Time.time;
        
        Debug.Log($"Camera started: {_camTexture.deviceName}, {_camTexture.width}x{_camTexture.height}");
        
        _camTexture.Play();
        _rawImageBG.texture = _camTexture;
        _isCamAvailable = true;
        _textResult.text = "Camera ready - Press SCAN to scan QR code";
        
        Debug.Log($"Camera started: {_camTexture.deviceName}, {_camTexture.width}x{_camTexture.height}");
    }

    private void UpdateCameraRender() 
    { 
        if (_isCamAvailable == false || _camTexture == null)
        {
            return;
        }
        
        if (!_camTexture.isPlaying)
        {
            return;
        }
        
        float ratio = (float)_camTexture.width / (float)_camTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = _camTexture.videoRotationAngle;
        _rawImageBG.rectTransform.localEulerAngles = new Vector3(0, 0, -orientation);
    }

    public void OnClickScan()
    {
        if(_isCamAvailable && !_isScanning)
        {
            _isScanning = true;
            _textResult.text = "Scanning...";
            Scan();
        }
        else if(_isScanning)
        {
            _textResult.text = "Already scanning, please wait...";
        }
        else
        {
            _textResult.text = "Camera not available!";
        }
    }

    private void Scan()
    {
        try
        {
            if (_camTexture == null || !_camTexture.isPlaying)
            {
                _textResult.text = "Camera not ready!";
                _isScanning = false;
                return;
            }
            
            // Configure BarcodeReader specifically for QR codes
            IBarcodeReader barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    TryInverted = true,
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                }
            };
            
            Result result = barcodeReader.Decode(_camTexture.GetPixels32(), _camTexture.width, _camTexture.height);
            if (result != null)
            {
                _textResult.text = $"Scanned: {result.Text}";
                Debug.Log($"QR Code detected: {result.Text}");
                ProcessQRCode(result.Text);
            else
            {
                // No QR code found, reset scanning flag to try again
                _isScanning = false;
            }   _isScanning = false;
            }
        }
        catch (System.Exception e)
        {
            _textResult.text = "SCAN FAILED! Try again.";
            _isScanning = false;
            Debug.LogError("QR Scan Error: " + e.Message + "\n" + e.StackTrace);
        }
    }

    private void ProcessQRCode(string qrContent)
    {
        // Parse the QR code content to determine the role
        GameEnums.Role scannedRole = GameEnums.Role.Unknown;

        // Convert QR content to role (case-insensitive)
        string content = qrContent.Trim().ToLower();
        
        Debug.Log($"Processing QR content: {content}");
        
        if (content.Contains("gent") && !content.Contains("agent"))
        {
            scannedRole = GameEnums.Role.Gent;
        }
        else if (content.Contains("soldier"))
        {
            scannedRole = GameEnums.Role.Soldier;
        }
        else if (content.Contains("thief"))
        {
            scannedRole = GameEnums.Role.Thief;
        }
        else if (content.Contains("double") || (content.Contains("agent") && !content.Contains("gent")))
        {
            scannedRole = GameEnums.Role.Double_Agent;
        }
        
        if (scannedRole == GameEnums.Role.Unknown)
        {
            _textResult.text = $"Unknown role: {qrContent}";
            _isScanning = false;
            Debug.LogWarning($"Unknown role in QR code: {qrContent}");
            return;
        }

        Debug.Log($"Role detected: {scannedRole}");
        
        // Stop the camera
        if (_camTexture != null && _camTexture.isPlaying)
        {
            _camTexture.Stop();
        }

        // Return to GameStart scene with the scanned role
        StartCoroutine(ReturnToGameStart(scannedRole));
    }

    private IEnumerator ReturnToGameStart(GameEnums.Role role)
    {
        // Wait a moment to show the result
        yield return new WaitForSeconds(1.5f);
        
        // Use GameManager to handle the transition
        GameManager.Instance.ReturnFromQRScanner(role);
    }
    
    void OnDestroy()
    {
        // Clean up camera when leaving scene
        if (_camTexture != null && _camTexture.isPlaying)
        {
            _camTexture.Stop();
        }
    }
}
