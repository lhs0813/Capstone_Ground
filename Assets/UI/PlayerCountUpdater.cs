using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCountUpdater : MonoBehaviour
{
    public TextMeshProUGUI playerCountText; // TextMeshPro 오브젝트를 연결하세요
    public GameObject targetObject; // 활성화할 대상 오브젝트를 연결하세요
    public float activationDelay = 20.0f; // 초기 대기할 시간

    void Start()
    {
        // 초기 플레이어 수를 표시합니다.
        InvokeRepeating("CheckPlayerCount", activationDelay, 0.01f); // 주기적으로 플레이어 수 확인
    }
    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int currentPlayerCount = players.Length;
        playerCountText.text = currentPlayerCount.ToString();
    }
    void CheckPlayerCount()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int currentPlayerCount = players.Length;

        playerCountText.text = currentPlayerCount.ToString();

        if (currentPlayerCount == 1)
        {
            targetObject.SetActive(true);
        }
    }
}