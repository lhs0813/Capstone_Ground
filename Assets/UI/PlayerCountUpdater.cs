using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCountUpdater : MonoBehaviour
{
    public TextMeshProUGUI playerCountText; // TextMeshPro ������Ʈ�� �����ϼ���
    public GameObject targetObject; // Ȱ��ȭ�� ��� ������Ʈ�� �����ϼ���
    public float activationDelay = 20.0f; // �ʱ� ����� �ð�

    void Start()
    {
        // �ʱ� �÷��̾� ���� ǥ���մϴ�.
        InvokeRepeating("CheckPlayerCount", activationDelay, 0.01f); // �ֱ������� �÷��̾� �� Ȯ��
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