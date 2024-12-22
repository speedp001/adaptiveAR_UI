//using UnityEngine;
//using System.Collections.Generic; // 리스트 사용을 위한 네임스페이스

//public class ButtonController : MonoBehaviour
//{
//    // 여러 오브젝트를 관리하는 리스트
//    public List<GameObject> targetObjects;

//    // UI 가시성 상태 (true = 보이기, false = 숨기기)
//    private bool isVisible = true;

//    // 버튼 클릭 시 호출되는 메서드
//    public void ToggleVisibility()
//    {
//        isVisible = !isVisible;

//        // 모든 targetObjects의 활성화 상태 변경
//        foreach (GameObject obj in targetObjects)
//        {
//            if (obj != null) // null 체크
//            {
//                obj.SetActive(isVisible);
//            }
//        }

//        Debug.Log($"UI 상태: {(isVisible ? "활성화" : "비활성화")}");
//    }
//}

using UnityEngine;
using System.Collections.Generic; // 리스트 사용
using UnityEngine.UI; // UI 사용

public class UIController : MonoBehaviour
{
    [Header("UI Objects to Toggle")]
    public List<GameObject> targetObjects; // UI 오브젝트 목록

    [Header("Button for Mode Control")]
    public Button toggleUIButton; // 버튼 컴포넌트

    // UI 상태 변수
    private bool isVisible = true;

    void Start()
    {
        // 시작 시 버튼 색상 설정 (초록색: UI 활성화 상태)
        UpdateButtonColor(Color.green);
    }

    // 버튼 클릭 시 호출되는 메서드
    public void ToggleVisibility()
    {
        isVisible = !isVisible; // UI 상태 반전

        // 모든 targetObjects의 활성화 상태 변경
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null) // null 체크
            {
                obj.SetActive(isVisible);
            }
        }

        // UI 상태에 따라 버튼 색상 변경
        UpdateButtonColor(isVisible ? Color.green : Color.red);

        Debug.Log($"UI 상태: {(isVisible ? "활성화" : "비활성화")}");
    }

    // 버튼 색상 변경 함수
    private void UpdateButtonColor(Color newColor)
    {
        var buttonImage = toggleUIButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = newColor; // 버튼 색상 업데이트
        }
        else
        {
            Debug.LogError("Button에 Image 컴포넌트가 없습니다.");
        }
    }
}
