using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CountDown : MonoBehaviourPunCallbacks
{
    public TMP_Text countdownText;
    private float startTime = 30f;

    private void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        float timeLeft = startTime;

        while (timeLeft > 0)
        {
            countdownText.text = "Time Remaining until game start: " + Mathf.Ceil(timeLeft).ToString();
            yield return new WaitForSeconds(1f);

            if (photonView.IsMine)
            {
                photonView.RPC("SyncCountdown", RpcTarget.All, timeLeft);
            }

            timeLeft -= 1f;
        }

        if (photonView.IsMine)
        {
            // 카운트다운이 종료되면 모든 플레이어를 방에서 나가고 씬을 다같이 이동합니다.
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("CityBuilderUrbanAPODemo");
        }
    }

    [PunRPC]
    private void SyncCountdown(float time)
    {
        countdownText.text = "Time Remaining until game start: " + Mathf.Ceil(time).ToString();
    }
}