using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Item_Confirm : MonoBehaviourPunCallbacks
{
    public GameObject itemPrefab; // �������� Inspector���� �Ҵ�
    private GameObject spawnedItem; // ������ ������ �������� ������ ����
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
                GameObject inventory = GameObject.Find("Inventory"); // Inventory ������Ʈ�� ã�� ����
                if (inventory != null)
                {
                    // ������ ������ ���� �� ���� ����
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
                // ������ ������ ����
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
            if (Input.GetMouseButtonDown(1) && canSpawnNew) // ������ ���콺 Ŭ�� Ȯ��
            {
                // ������ ������ ����
                Destroy(spawnedItem);
                canSpawnNew = false;

                // ������ �������� ���� �����ϰ� ��ġ�� ������ ����
                Gun_Num1 = Instantiate(itemPrefab, BigItem_Pos, Quaternion.identity);
                Gun_Num1.transform.localScale = BigItem_Scale;
                Gun_Num1.transform.parent = GameObject.Find("Inventory").transform;

                canSpawnNew = false;

                // IS.Mine �÷��̾�� M4a1_PBR ������Ʈ ã�Ƽ� Ȱ��ȭ
                Item.RPC("Item_On", RpcTarget.All);
            }

        }
    }

    [PunRPC]
    void Item_On()
    {
        player = PV.transform.gameObject;
        //player.transform.Find("Player(Clone)");
        // �÷��̾� ������Ʈ�� ã�� ����
        if (player != null)
        {
            m4a1PBR = player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r" +
                "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // ���ϴ� ������Ʈ �̸����� ����
            if (m4a1PBR != null)
            {
                m4a1PBR.gameObject.SetActive(true);
                player.GetComponent<Animator>().SetBool("GunOn", true);
            }
        }
        Destroy(gameObject);
    }
}