using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Services.Analytics.Internal;
using UnityEngine.UI;
using TMPro;

public class Player_HP : MonoBehaviourPunCallbacks
{
    public float HP = 100.0f;

    public float VirtualHP;

    private AudioSource audioPlayer; // ����� �ҽ� ������Ʈ
    public AudioClip hitClip; // �ǰݽ� ����� �Ҹ�
    public PhotonView PV;

    public GameObject PlayerHP2;
    Slider Hp_Slider;

    Color imageColor;

    public GameObject UI_Mode2_P;

    public Image HP_Color;

    public Collider Head;
    public Collider Body;

    public Collider UR_Arm;
    public Collider R_Hand;
    public Collider UR_Leg;
    public Collider R_Leg;

    public Collider UL_Arm;
    public Collider L_Hand;
    public Collider UL_Leg;
    public Collider L_Leg;


    public GameObject Player_Name;

    public GameObject Loser_Board;
    public GameObject Winner_Board;
    // Start is called before the first frame update
    void Start()
    {
        UI_Mode2_P = GameObject.Find("Canvas");
        audioPlayer = GetComponent<AudioSource>();

        Hp_Slider = UI_Mode2_P.transform.Find("HP_Bar").GetComponent<Slider>();

        PlayerHP2 = Hp_Slider.transform.Find("Fill Area").transform.Find("Fill").gameObject;

        VirtualHP = HP;

        Loser_Board = UI_Mode2_P.transform.Find("Loser_Board").gameObject;
        Winner_Board = UI_Mode2_P.transform.Find("Winner_Board").gameObject;
    }
    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                PV.RPC("HPUpdate", RpcTarget.All);
                if (HP <= 0)
                {
                    // HP�� 0 ���Ϸ� �������� ó���� �۾� �߰� (��: ���� ó��)
                    Die();
                    PV.RPC("Die_Share", RpcTarget.All);
                }
            }
        }
    }*/
    private void Die()
    {
        // ĳ���Ͱ� �׾��� �� ó���� �۾��� �̰��� �߰�
        // ���� ���, ���� ������Ʈ ��Ȱ��ȭ �Ǵ� ���� �ִϸ��̼� ��� ���� ������ �� �ֽ��ϴ�.
    }

    private void ApplyDamage()
    {
        audioPlayer.PlayOneShot(hitClip);

        // ĳ���Ͱ� �׾��� �� ó���� �۾��� �̰��� �߰�
        // ���� ���, ���� ������Ʈ ��Ȱ��ȭ �Ǵ� ���� �ִϸ��̼� ��� ���� ������ �� �ֽ��ϴ�.
    }

    // Update is called once per frame
    void Update()
    {
        
        
        PV.RPC("HPUpdate", RpcTarget.All);

        if (PV.IsMine)
        {
            if (HP <= 0)
            {
                Die();
                Loser_Board.SetActive(true);
                Destroy(Winner_Board);
                PV.RPC("Die_Share", RpcTarget.All);
            }
            else if (HP <= 30)
            {
                PlayerHP2.GetComponent<Image>().color = Color.red;
            }
            else if (HP <= 50)
            {
                PlayerHP2.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                PlayerHP2.GetComponent<Image>().color = Color.white;
            }

            Hp_Slider.value = HP / 100; // HP�� ���� ����
        }
    }

    [PunRPC]
    void Die_Share()
    {
        // LivingEntity�� Die()�� �����Ͽ� �⺻ ��� ó�� ����

        // �ٸ� AI���� �������� �ʵ��� �ڽ��� ��� �ݶ��̴����� ��Ȱ��ȭ
        //GetComponent<Collider>().enabled = false;

        // ��� �ִϸ��̼� ���
        GetComponent<Animator>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        //animator.applyRootMotion = true;
        //animator.SetTrigger("Die");

        Debug.Log("RPC ȣ��");
        //gameObject.name = "Dead_P";
        Destroy(gameObject, 1.0f); // 2�� �Ŀ� ����
    }
    [PunRPC]
    void HPUpdate()
    {
        if(HP != VirtualHP)
        {
            HP = VirtualHP;
        }
        
    }
}
