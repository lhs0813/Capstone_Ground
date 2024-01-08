using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TO_LobbyButton : MonoBehaviour
{
    public Button button; // 버튼을 참조할 변수
    public string sceneToLoad; // 이동하려는 씬의 이름

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
    }
    void Start()
    {
        // 버튼 클릭 이벤트에 함수를 할당
        button.onClick.AddListener(LoadLobbyScene);
    }

    // 버튼 클릭 시 호출되는 함수
    void LoadLobbyScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}