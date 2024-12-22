//using UnityEngine;
//using System.Collections.Generic; // ����Ʈ ����� ���� ���ӽ����̽�

//public class ButtonController : MonoBehaviour
//{
//    // ���� ������Ʈ�� �����ϴ� ����Ʈ
//    public List<GameObject> targetObjects;

//    // UI ���ü� ���� (true = ���̱�, false = �����)
//    private bool isVisible = true;

//    // ��ư Ŭ�� �� ȣ��Ǵ� �޼���
//    public void ToggleVisibility()
//    {
//        isVisible = !isVisible;

//        // ��� targetObjects�� Ȱ��ȭ ���� ����
//        foreach (GameObject obj in targetObjects)
//        {
//            if (obj != null) // null üũ
//            {
//                obj.SetActive(isVisible);
//            }
//        }

//        Debug.Log($"UI ����: {(isVisible ? "Ȱ��ȭ" : "��Ȱ��ȭ")}");
//    }
//}

using UnityEngine;
using System.Collections.Generic; // ����Ʈ ���
using UnityEngine.UI; // UI ���

public class UIController : MonoBehaviour
{
    [Header("UI Objects to Toggle")]
    public List<GameObject> targetObjects; // UI ������Ʈ ���

    [Header("Button for Mode Control")]
    public Button toggleUIButton; // ��ư ������Ʈ

    // UI ���� ����
    private bool isVisible = true;

    void Start()
    {
        // ���� �� ��ư ���� ���� (�ʷϻ�: UI Ȱ��ȭ ����)
        UpdateButtonColor(Color.green);
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void ToggleVisibility()
    {
        isVisible = !isVisible; // UI ���� ����

        // ��� targetObjects�� Ȱ��ȭ ���� ����
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null) // null üũ
            {
                obj.SetActive(isVisible);
            }
        }

        // UI ���¿� ���� ��ư ���� ����
        UpdateButtonColor(isVisible ? Color.green : Color.red);

        Debug.Log($"UI ����: {(isVisible ? "Ȱ��ȭ" : "��Ȱ��ȭ")}");
    }

    // ��ư ���� ���� �Լ�
    private void UpdateButtonColor(Color newColor)
    {
        var buttonImage = toggleUIButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = newColor; // ��ư ���� ������Ʈ
        }
        else
        {
            Debug.LogError("Button�� Image ������Ʈ�� �����ϴ�.");
        }
    }
}
