using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    
    public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);

    public override void OnJoinedRoom() 
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

        //PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
    // Start is called before the first frame update
    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        PhotonNetwork.ConnectUsingSettings();


    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
