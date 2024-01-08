using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MineMap_Icon : MonoBehaviourPunCallbacks
{

    public PhotonView PV;
    public GameObject MiniMap_Icon;
    // Start is called before the first frame update
    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(MiniMap_Icon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
