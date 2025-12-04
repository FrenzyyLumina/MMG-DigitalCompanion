using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        SetUpCamera();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraRender();
    }

    private void SetUpCamera(){
        
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0)
        {
            _isCamAvailable = false;
            Debug.LogError("No camera devices found!");
            return;
        }
        
        // Get camera resolution (use default if scanZone not assigned)
        int width = 1280;
        int height = 720;
        if (_scanZone != null)
        {
            width = (int)_scanZone.rect.width;
            height = (int)_scanZone.rect.height;
        }
        
        for(int i = 0; i<devices.Length; i++)
        {
            if(devices[i].isFrontFacing == false)
            {
                _camTexture = new WebCamTexture(devices[i].name, width, height);
                break;
            }
        }
        
        // If no back camera found, use any available camera
        if (_camTexture == null && devices.Length > 0)
        {
            _camTexture = new WebCamTexture(devices[0].name, width, height);
        }
        
        if (_camTexture == null)
        {
            _isCamAvailable = false;
            Debug.LogError("Failed to initialize camera texture!");
            return;
        }
        
        _camTexture.Play();
        
        if (_rawImageBG != null)
        {
            _rawImageBG.texture = _camTexture;
        }
        
        _isCamAvailable = true;
    }

    private void UpdateCameraRender() 
    { 
        if (_isCamAvailable == false || _camTexture == null)
        {
            return;
        }
        
        if (_aspectRatioFitter != null)
        {
            float ratio = (float)_camTexture.width / (float)_camTexture.height;
            _aspectRatioFitter.aspectRatio = ratio;
        }

        if (_rawImageBG != null)
        {
            int orientation = _camTexture.videoRotationAngle;
            _rawImageBG.rectTransform.localEulerAngles = new Vector3(0,0, orientation);
        }
    }

    public void OnClickScan()
    private void Scan()
    {
        if (_camTexture == null)
        {
            if (_textResult != null) _textResult.text = "Camera not available!";
            Debug.LogError("Cannot scan: Camera texture is null");
            return;
        }
        
        try
        {
            // Configure BarcodeReader specifically for QR codes
            IBarcodeReader barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                }
            };
            
            Result result = barcodeReader.Decode(_camTexture.GetPixels32(), _camTexture.width, _camTexture.height);
            if (result != null)
            {
                if (_textResult != null) _textResult.text = result.Text;
                ProcessQRCode(result.Text);
            }
            else
            {
                if (_textResult != null) _textResult.text = "FAILED TO READ QR CODE!";
            }
        }
        catch (System.Exception e)
        {
            if (_textResult != null) _textResult.text = "SCAN FAILED!";
            Debug.LogError("QR Scan Error: " + e.Message);
        }
    }       }
        }
        catch (System.Exception e)
        {
            _textResult.text = "SCAN FAILED!";
            Debug.LogError("QR Scan Error: " + e.Message);
        }
    }

    private void ProcessQRCode(string qrContent)
    {
        // Parse the QR code content to determine the role
        GameEnums.Role scannedRole = GameEnums.Role.Gent; // Default

        // Convert QR content to role (case-insensitive)
        string content = qrContent.Trim().ToLower();
        
        if (content.Contains("gent"))
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
        else
        {
            Debug.LogWarning($"Unknown role in QR code: {qrContent}. Defaulting to Gent.");
        }

        // Stop the camera
        if (_camTexture != null && _camTexture.isPlaying)
        {
    private IEnumerator ReturnToGameStart(GameEnums.Role role)
    {
        // Wait a moment to show the result
        yield return new WaitForSeconds(1.5f);
        
        // Use GameManager to handle the transition
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnFromQRScanner(role);
        }
        else
        {
            Debug.LogError("GameManager instance is null!");
        }
    }rivate IEnumerator ReturnToGameStart(GameEnums.Role role)
    {
        // Wait a moment to show the result
        yield return new WaitForSeconds(1.5f);
        
        // Use GameManager to handle the transition
        GameManager.Instance.ReturnFromQRScanner(role);
    }
}
