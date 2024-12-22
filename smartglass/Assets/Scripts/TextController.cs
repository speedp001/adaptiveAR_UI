using UnityEngine;
using System.Linq;
using TMPro; // TextMeshPro 사용
using UnityEngine.UI; // UI 사용
using UnityEngine.Windows.WebCam; // HoloLens 카메라 캡처용

public class TextController : MonoBehaviour
{
    [Header("Text Objects to Modify")]
    public GameObject[] textTargets; // 색상 변경 대상 TextMeshPro 오브젝트 배열 (Subject, Content 등)

    [Header("Button for Mode Control")]
    public Button contrastModeButton; // Contrast Mode 토글 버튼

    // 상태 변수
    private bool isContrastMode = false; // 보색 모드 활성화 상태 (ON/OFF)
    private Color[] originalColors; // 초기 텍스트 색상 저장 배열
    private PhotoCapture photoCapture; // HoloLens 카메라 사진 촬영 객체
    private Texture2D targetTexture; // 캡처된 이미지 데이터를 저장하는 텍스처

    void Start()
    {
        // 텍스트의 초기 색상 저장
        originalColors = new Color[textTargets.Length]; // 초기 색상 배열 초기화
        for (int i = 0; i < textTargets.Length; i++)
        {
            // TextMeshProUGUI 컴포넌트를 가져와 색상을 저장
            var tmp = textTargets[i].GetComponent<TextMeshProUGUI>();
            if (tmp != null) originalColors[i] = tmp.color;
        }

        // 버튼 클릭 이벤트 등록
        contrastModeButton.onClick.AddListener(ToggleContrastMode);

        // 초기 버튼 색상 설정: 빨간색 (OFF 상태)
        UpdateButtonVisual(Color.red);
    }

    // 보색 모드 ON/OFF 함수
    private void ToggleContrastMode()
    {
        isContrastMode = !isContrastMode; // 상태 전환 (ON ↔ OFF)
        Debug.Log($"보색 모드 상태: {(isContrastMode ? "ON" : "OFF")}");

        if (isContrastMode)
        {
            UpdateButtonVisual(Color.green); // 버튼 색상 초록색 (ON 상태)
            StartPhotoCapture(); // 사진 촬영 시작
        }
        else
        {
            UpdateButtonVisual(Color.red); // 버튼 색상 빨간색 (OFF 상태)
            RestoreOriginalColors(); // 원래 텍스트 색상 복원
        }
    }

    // 버튼 색상 업데이트 함수
    private void UpdateButtonVisual(Color buttonColor)
    {
        var buttonImage = contrastModeButton.GetComponent<UnityEngine.UI.Image>(); // 버튼의 Image 컴포넌트 가져오기
        if (buttonImage != null)
        {
            buttonImage.color = buttonColor; // 버튼 색상 변경
        }
        else
        {
            Debug.LogError("Button에 Image 컴포넌트가 없습니다.");
        }
    }

    // 카메라 캡처 시작 함수
    private void StartPhotoCapture()
    {
        // Vuforia 비활성화 (카메라 리소스 충돌 방지)
        var vuforiaBehaviour = FindObjectOfType<Vuforia.VuforiaBehaviour>();
        if (vuforiaBehaviour != null)
        {
            vuforiaBehaviour.enabled = false;
            Debug.Log("Vuforia 비활성화됨.");
        }

        // PhotoCapture 객체 생성
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCapture = captureObject; // PhotoCapture 객체 저장

        // 가장 큰 해상도의 카메라 해상도 선택
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending(res => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // 카메라 파라미터 설정
        CameraParameters cameraParameters = new CameraParameters
        {
            hologramOpacity = 0.0f, // 홀로그램 불투명도
            cameraResolutionWidth = cameraResolution.width,
            cameraResolutionHeight = cameraResolution.height,
            pixelFormat = CapturePixelFormat.BGRA32
        };

        // 사진 모드 시작
        photoCapture.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            // 사진 촬영
            photoCapture.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Photo mode 시작 실패!");
        }
    }

    private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // 캡처된 데이터를 텍스처에 업로드
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            ApplyComplementaryColor(targetTexture); // 보색 적용
        }

        // PhotoCapture 종료
        if (photoCapture != null)
        {
            photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
    }

    // 보색 적용 함수
    private void ApplyComplementaryColor(Texture2D texture)
    {
        // 명도를 이용한 색상 보정
        Color averageColor = GetAverageColor(texture); // 이미지 평균 색상 계산
        float luminance = 0.2126f * averageColor.r + 0.7152f * averageColor.g + 0.0722f * averageColor.b;
        Color complementaryColor = luminance < 0.2f ? Color.white : Color.black; // 명도에 따른 대비 색상 결정

        ////색상의 정반대 값으로 보정
        //Color averageColor = GetAverageColor(texture); // 이미지 평균 색상 계산
        //Color complementaryColor = new Color(1 - averageColor.r, 1 - averageColor.g, 1 - averageColor.b); // 보색 계산

        // 모든 텍스트에 보색 적용
        foreach (GameObject textTarget in textTargets)
        {
            var tmp = textTarget.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.color = complementaryColor;
        }

        Debug.Log($"평균 색상: {averageColor}, 보색 적용: {complementaryColor}");
    }

    // 이미지 평균 색상 계산 함수
    private Color GetAverageColor(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32(); // 이미지의 모든 픽셀 가져오기
        float r = 0, g = 0, b = 0;

        foreach (Color32 pixel in pixels)
        {
            r += pixel.r;
            g += pixel.g;
            b += pixel.b;
        }

        int totalPixels = pixels.Length; // 총 픽셀 수
        return new Color(r / totalPixels / 255f, g / totalPixels / 255f, b / totalPixels / 255f);
    }

    private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCapture.Dispose(); // PhotoCapture 객체 해제
        photoCapture = null;

        // Vuforia 다시 활성화
        var vuforiaBehaviour = FindObjectOfType<Vuforia.VuforiaBehaviour>();
        if (vuforiaBehaviour != null)
        {
            vuforiaBehaviour.enabled = true;
            Debug.Log("Vuforia 다시 활성화됨.");
        }
    }

    // 원래 색상 복원 함수
    private void RestoreOriginalColors()
    {
        for (int i = 0; i < textTargets.Length; i++)
        {
            var tmp = textTargets[i].GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.color = originalColors[i];
        }
        Debug.Log("텍스트 색상이 원래대로 복원되었습니다.");
    }
}
