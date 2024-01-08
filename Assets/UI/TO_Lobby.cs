using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TO_LobbyButton : MonoBehaviour
{
    public Button button; // ��ư�� ������ ����
    public string sceneToLoad; // �̵��Ϸ��� ���� �̸�

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
    }
    void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ�� �Լ��� �Ҵ�
        button.onClick.AddListener(LoadLobbyScene);
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    void LoadLobbyScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}