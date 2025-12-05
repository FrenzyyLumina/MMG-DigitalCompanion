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
    [SerializeField]
    private Button _endScanButton;

    private bool _isCamAvailable;
    private WebCamTexture _camTexture;
    private bool _isScanning = false;
    private float _scanInterval = 2.0f;
    private float _lastScanTime = 0f;
    private ScreenOrientation _previousOrientation;

    void Start()
    {
        _previousOrientation = Screen.orientation;
        Screen.orientation = ScreenOrientation.Portrait;
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
    }

    void Update()
    {
        UpdateCameraRender();
        
        if (_isCamAvailable && !_isScanning && Time.time - _lastScanTime >= _scanInterval)
        {
            _lastScanTime = Time.time;
            Scan();
        }
    }

    private void SetUpCamera()
    {
        // Wire up End Scan button listener for item scanning mode
        if (_endScanButton != null)
        {
            _endScanButton.onClick.RemoveAllListeners();
            _endScanButton.onClick.AddListener(OnEndScanPressed);
        }
        
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
        
        for(int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"Camera {i}: {devices[i].name}, Front facing: {devices[i].isFrontFacing}");
            
            if(!devices[i].isFrontFacing)
            {
                _camTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                break;
            }
        }
        
        if(_camTexture == null && devices.Length > 0)
        {
            _camTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
            Debug.Log("No back camera found, using first available camera");
        }
        
        if(_camTexture == null)
        {
            _isCamAvailable = false;
            _textResult.text = "Failed to initialize camera!";
            Debug.LogError("Failed to create WebCamTexture!");
            return;
        }
        
        _camTexture.Play();
        _rawImageBG.texture = _camTexture;
        _isCamAvailable = true;
        
        if (GameManager.Instance != null && GameManager.Instance.IsItemScanningMode)
        {
            _textResult.text = "Scan item cards...\nPress 'End Scan' when done";
            if (_endScanButton != null)
            {
                _endScanButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Scan";
            }
        }
        else
        {
            _textResult.text = "Point camera at QR code...";
        }
        
        _lastScanTime = Time.time;
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
        if (GameManager.Instance != null && GameManager.Instance.IsItemScanningMode)
        {
            OnEndScanPressed();
            return;
        }
        
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
    
    public void OnEndScanPressed()
    {
        Debug.Log("End Scan pressed - finishing item scanning");
        
        if (_camTexture != null && _camTexture.isPlaying)
        {
            _camTexture.Stop();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinishItemScanning();
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

            if(result != null)
            {
                Debug.Log($"QR Code detected: {result.Text}");
                ProcessQRCode(result.Text);
            }
            else
            {
                _isScanning = false;
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
        string content = qrContent.Trim().ToLower();
        Debug.Log($"Processing QR content: {content}");
        
        if (GameManager.Instance != null && GameManager.Instance.IsItemScanningMode)
        {
            GameEnums.Item scannedItem = ParseItemFromQR(content);
            if (scannedItem != GameEnums.Item.None)
            {
                GameManager.Instance.AddScannedItem(scannedItem);
                _textResult.text = $"Scanned: {scannedItem}\nTotal: {GameManager.Instance.ScannedItems.Count}";
                _isScanning = false;
                return;
            }
            else
            {
                _textResult.text = $"Unknown item: {qrContent}";
                _isScanning = false;
                return;
            }
        }
        
        GameEnums.Role scannedRole = ParseRoleFromQR(content);
        
        if (scannedRole == GameEnums.Role.Unknown)
        {
            _textResult.text = $"Unknown role: {qrContent}";
            _isScanning = false;
            Debug.LogWarning($"Unknown role in QR code: {qrContent}");
            return;
        }

        Debug.Log($"Role detected: {scannedRole}");
        
        if (_camTexture != null && _camTexture.isPlaying)
        {
            _camTexture.Stop();
        }

        StartCoroutine(ReturnToGameStart(scannedRole));
    }
    
    private GameEnums.Item ParseItemFromQR(string content)
    {
        // Special McGuffin item - rickroll QR code
        if (content.Contains("dQw4w9WgXcQ")) return GameEnums.Item.McGuffin;
        
        if (content.Contains("dice")) return GameEnums.Item.Dice;
        if (content.Contains("mcguffin")) return GameEnums.Item.McGuffin;
        if (content.Contains("body_armor")) return GameEnums.Item.Body_Armor;
        if (content.Contains("personal_radar")) return GameEnums.Item.Personal_Radar;
        if (content.Contains("rations")) return GameEnums.Item.Rations;
        if (content.Contains("adrenaline")) return GameEnums.Item.Adrenaline;
        if (content.Contains("active_projectile_dome")) return GameEnums.Item.Active_Projectile_Dome;
        if (content.Contains("revival_pill")) return GameEnums.Item.Revival_Pill;
        if (content.Contains("hush_puppy")) return GameEnums.Item.Hush_Puppy;
        if (content.Contains("poison_blowdart")) return GameEnums.Item.Poison_Blowdart;
        if (content.Contains("m9_bayonet")) return GameEnums.Item.M9_Bayonet;
        if (content.Contains("tripmine")) return GameEnums.Item.Tripmine;
        if (content.Contains("universal_multi_tool")) return GameEnums.Item.Universal_Multi_Tool;
        if (content.Contains("proximity_detector")) return GameEnums.Item.Proximity_Detector;
        if (content.Contains("truth_serum")) return GameEnums.Item.Truth_Serum;
        if (content.Contains("sneaking_suit")) return GameEnums.Item.Sneaking_Suit;
        if (content.Contains("cardboard_box")) return GameEnums.Item.Cardboard_Box;
        if (content.Contains("coin")) return GameEnums.Item.Coin;
        if (content.Contains("the_doohickey")) return GameEnums.Item.The_Doohickey;
        
        return GameEnums.Item.None;
    }
    
    private GameEnums.Role ParseRoleFromQR(string content)
    {
        if (content.Contains("the_gent") || content.Contains("gent"))
            return GameEnums.Role.Gent;
        if (content.Contains("the_soldier") || content.Contains("soldier"))
            return GameEnums.Role.Soldier;
        if (content.Contains("the_thief") || content.Contains("thief"))
            return GameEnums.Role.Thief;
        if (content.Contains("the_assassin") || content.Contains("assassin"))
            return GameEnums.Role.Assassin;
        if (content.Contains("the_hacker") || content.Contains("hacker"))
            return GameEnums.Role.Hacker;
        if (content.Contains("double_agent"))
            return GameEnums.Role.Double_Agent;
        if (content.Contains("the_vengeful") || content.Contains("vengeful"))
            return GameEnums.Role.Vengeful;
        
        return GameEnums.Role.Unknown;
    }

    private IEnumerator ReturnToGameStart(GameEnums.Role role)
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.ReturnFromQRScanner(role);
    }
    
    void OnDestroy()
    {
        Screen.orientation = _previousOrientation;
        
        if (_camTexture != null && _camTexture.isPlaying)
        {
            _camTexture.Stop();
        }
    }
}
