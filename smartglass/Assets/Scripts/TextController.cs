using UnityEngine;
using System.Linq;
using TMPro; // TextMeshPro ���
using UnityEngine.UI; // UI ���
using UnityEngine.Windows.WebCam; // HoloLens ī�޶� ĸó��

public class TextController : MonoBehaviour
{
    [Header("Text Objects to Modify")]
    public GameObject[] textTargets; // ���� ���� ��� TextMeshPro ������Ʈ �迭 (Subject, Content ��)

    [Header("Button for Mode Control")]
    public Button contrastModeButton; // Contrast Mode ��� ��ư

    // ���� ����
    private bool isContrastMode = false; // ���� ��� Ȱ��ȭ ���� (ON/OFF)
    private Color[] originalColors; // �ʱ� �ؽ�Ʈ ���� ���� �迭
    private PhotoCapture photoCapture; // HoloLens ī�޶� ���� �Կ� ��ü
    private Texture2D targetTexture; // ĸó�� �̹��� �����͸� �����ϴ� �ؽ�ó

    void Start()
    {
        // �ؽ�Ʈ�� �ʱ� ���� ����
        originalColors = new Color[textTargets.Length]; // �ʱ� ���� �迭 �ʱ�ȭ
        for (int i = 0; i < textTargets.Length; i++)
        {
            // TextMeshProUGUI ������Ʈ�� ������ ������ ����
            var tmp = textTargets[i].GetComponent<TextMeshProUGUI>();
            if (tmp != null) originalColors[i] = tmp.color;
        }

        // ��ư Ŭ�� �̺�Ʈ ���
        contrastModeButton.onClick.AddListener(ToggleContrastMode);

        // �ʱ� ��ư ���� ����: ������ (OFF ����)
        UpdateButtonVisual(Color.red);
    }

    // ���� ��� ON/OFF �Լ�
    private void ToggleContrastMode()
    {
        isContrastMode = !isContrastMode; // ���� ��ȯ (ON �� OFF)
        Debug.Log($"���� ��� ����: {(isContrastMode ? "ON" : "OFF")}");

        if (isContrastMode)
        {
            UpdateButtonVisual(Color.green); // ��ư ���� �ʷϻ� (ON ����)
            StartPhotoCapture(); // ���� �Կ� ����
        }
        else
        {
            UpdateButtonVisual(Color.red); // ��ư ���� ������ (OFF ����)
            RestoreOriginalColors(); // ���� �ؽ�Ʈ ���� ����
        }
    }

    // ��ư ���� ������Ʈ �Լ�
    private void UpdateButtonVisual(Color buttonColor)
    {
        var buttonImage = contrastModeButton.GetComponent<UnityEngine.UI.Image>(); // ��ư�� Image ������Ʈ ��������
        if (buttonImage != null)
        {
            buttonImage.color = buttonColor; // ��ư ���� ����
        }
        else
        {
            Debug.LogError("Button�� Image ������Ʈ�� �����ϴ�.");
        }
    }

    // ī�޶� ĸó ���� �Լ�
    private void StartPhotoCapture()
    {
        // Vuforia ��Ȱ��ȭ (ī�޶� ���ҽ� �浹 ����)
        var vuforiaBehaviour = FindObjectOfType<Vuforia.VuforiaBehaviour>();
        if (vuforiaBehaviour != null)
        {
            vuforiaBehaviour.enabled = false;
            Debug.Log("Vuforia ��Ȱ��ȭ��.");
        }

        // PhotoCapture ��ü ����
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCapture = captureObject; // PhotoCapture ��ü ����

        // ���� ū �ػ��� ī�޶� �ػ� ����
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending(res => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // ī�޶� �Ķ���� ����
        CameraParameters cameraParameters = new CameraParameters
        {
            hologramOpacity = 0.0f, // Ȧ�α׷� ������
            cameraResolutionWidth = cameraResolution.width,
            cameraResolutionHeight = cameraResolution.height,
            pixelFormat = CapturePixelFormat.BGRA32
        };

        // ���� ��� ����
        photoCapture.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            // ���� �Կ�
            photoCapture.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Photo mode ���� ����!");
        }
    }

    private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // ĸó�� �����͸� �ؽ�ó�� ���ε�
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            ApplyComplementaryColor(targetTexture); // ���� ����
        }

        // PhotoCapture ����
        if (photoCapture != null)
        {
            photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
    }

    // ���� ���� �Լ�
    private void ApplyComplementaryColor(Texture2D texture)
    {
        // ���� �̿��� ���� ����
        Color averageColor = GetAverageColor(texture); // �̹��� ��� ���� ���
        float luminance = 0.2126f * averageColor.r + 0.7152f * averageColor.g + 0.0722f * averageColor.b;
        Color complementaryColor = luminance < 0.2f ? Color.white : Color.black; // ���� ���� ��� ���� ����

        ////������ ���ݴ� ������ ����
        //Color averageColor = GetAverageColor(texture); // �̹��� ��� ���� ���
        //Color complementaryColor = new Color(1 - averageColor.r, 1 - averageColor.g, 1 - averageColor.b); // ���� ���

        // ��� �ؽ�Ʈ�� ���� ����
        foreach (GameObject textTarget in textTargets)
        {
            var tmp = textTarget.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.color = complementaryColor;
        }

        Debug.Log($"��� ����: {averageColor}, ���� ����: {complementaryColor}");
    }

    // �̹��� ��� ���� ��� �Լ�
    private Color GetAverageColor(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32(); // �̹����� ��� �ȼ� ��������
        float r = 0, g = 0, b = 0;

        foreach (Color32 pixel in pixels)
        {
            r += pixel.r;
            g += pixel.g;
            b += pixel.b;
        }

        int totalPixels = pixels.Length; // �� �ȼ� ��
        return new Color(r / totalPixels / 255f, g / totalPixels / 255f, b / totalPixels / 255f);
    }

    private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCapture.Dispose(); // PhotoCapture ��ü ����
        photoCapture = null;

        // Vuforia �ٽ� Ȱ��ȭ
        var vuforiaBehaviour = FindObjectOfType<Vuforia.VuforiaBehaviour>();
        if (vuforiaBehaviour != null)
        {
            vuforiaBehaviour.enabled = true;
            Debug.Log("Vuforia �ٽ� Ȱ��ȭ��.");
        }
    }

    // ���� ���� ���� �Լ�
    private void RestoreOriginalColors()
    {
        for (int i = 0; i < textTargets.Length; i++)
        {
            var tmp = textTargets[i].GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.color = originalColors[i];
        }
        Debug.Log("�ؽ�Ʈ ������ ������� �����Ǿ����ϴ�.");
    }
}
