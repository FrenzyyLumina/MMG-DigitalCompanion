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
            return;
        }
        for(int i = 0; i<devices.Length; i++)
        {
            if(devices[i].isFrontFacing == false)
            {
                _camTexture = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
            }
        }
        _camTexture.Play();
        _rawImageBG.texture = _camTexture;
        _isCamAvailable = true;
    }

    private void UpdateCameraRender() 
    { 
        if (_isCamAvailable == false)
        {
            return;
        }
        float ratio = (float)_camTexture.width / (float)_camTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = _camTexture.videoRotationAngle;
        _rawImageBG.rectTransform.localEulerAngles = new Vector3(0,0, orientation);
    }

    public void OnClickScan()
    {
        if(_isCamAvailable)
        {
            Scan();
        }
    }

    private void Scan()
    {
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
                _textResult.text = result.Text;
                ProcessQRCode(result.Text);
            }
            else
            {
                _textResult.text = "FAILED TO READ QR CODE!";
            }
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
        else if (content.Contains("double") || content.Contains("agent"))
        {
            scannedRole = GameEnums.Role.Double_Agent;
        }
        else
        {
            Debug.LogWarning($"Unknown role in QR code: {qrContent}. Defaulting to Gent.");
        }

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
}
