using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Item_Confirm : MonoBehaviourPunCallbacks
{
    public GameObject itemPrefab; // 프리팹을 Inspector에서 할당
    private GameObject spawnedItem; // 생성된 아이템 프리팹의 참조를 저장
    private GameObject Gun_Num1;

    public Vector3 BigItem_Pos;
    public Vector3 BigItem_Scale;

    public PhotonView PV;
    public PhotonView Item;

    private bool canSpawnNew = true;

    public GameObject player;
    public GameObject m4a1PBR;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PV = other.GetComponent<PhotonView>();
            if (PV != null && PV.IsMine)
            {
                GameObject inventory = GameObject.Find("Inventory"); // Inventory 오브젝트를 찾는 예시
                if (inventory != null)
                {
                    // 아이템 프리팹 생성 및 참조 저장
                    spawnedItem = Instantiate(itemPrefab, inventory.transform);
                    canSpawnNew = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && spawnedItem != null)
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                // 아이템 프리팹 제거
                Destroy(spawnedItem);
                spawnedItem = null;
                canSpawnNew = true;
            }
        }
    }

    private void Update()
    {
        if (spawnedItem != null)
        {
            if (Input.GetMouseButtonDown(1) && canSpawnNew) // 오른쪽 마우스 클릭 확인
            {
                // 아이템 프리팹 삭제
                Destroy(spawnedItem);
                canSpawnNew = false;

                // 아이템 프리팹을 새로 생성하고 위치와 스케일 변경
                Gun_Num1 = Instantiate(itemPrefab, BigItem_Pos, Quaternion.identity);
                Gun_Num1.transform.localScale = BigItem_Scale;
                Gun_Num1.transform.parent = GameObject.Find("Inventory").transform;

                canSpawnNew = false;

                // IS.Mine 플레이어에서 M4a1_PBR 오브젝트 찾아서 활성화
                Item.RPC("Item_On", RpcTarget.All);
            }

        }
    }

    [PunRPC]
    void Item_On()
    {
        player = PV.transform.gameObject;
        //player.transform.Find("Player(Clone)");
        // 플레이어 오브젝트를 찾는 예시
        if (player != null)
        {
            m4a1PBR = player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r" +
                "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // 원하는 오브젝트 이름으로 변경
            if (m4a1PBR != null)
            {
                m4a1PBR.gameObject.SetActive(true);
                player.GetComponent<Animator>().SetBool("GunOn", true);
            }
        }
        Destroy(gameObject);
    }
}