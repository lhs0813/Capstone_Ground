using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Services.Analytics.Internal;
using UnityEngine.UI;
using TMPro;

public class HP_Cal : MonoBehaviourPunCallbacks
{
    GameObject Player;
    public float HP_Copy;
    public PhotonView PV;
    public float Damage = 80.0f;

    Transform highestParent;

    public ParticleSystem bloodEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        bloodEffectPrefab = Resources.Load<ParticleSystem>("Blood_Effect Variant");
        highestParent = transform;

        // �θ� ������Ʈ�� ã�� �ö󰡸� �ֻ��� �θ� ������Ʈ�� ã���ϴ�.
        while (highestParent.parent != null)
        {
            highestParent = highestParent.parent;
        }

        // �ֻ��� �θ� ������Ʈ�� ������ �� �ֽ��ϴ�.
        Debug.Log("���� ���� �θ� ������Ʈ�� �̸�: " + highestParent.name);

        HP_Copy = highestParent.GetComponent<Player_HP>().HP;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                // �Ѿ� �浹 ������ �����ͼ� �ش� ��ġ�� bloodEffect�� ���
                if (bloodEffectPrefab != null)
                {
                    ContactPoint contact = collision.contacts[0];
                    Vector3 hitPoint = contact.point;
                    Quaternion hitRotation = Quaternion.LookRotation(contact.normal);

                    // RPC�� ����Ͽ� bloodEffect�� ��� �÷��̾�� ����ȭ
                    PV.RPC("ShowBloodEffect", RpcTarget.All, hitPoint, hitRotation);
                }

                // �Ѿ� ������Ʈ�� ��� ����
                //PhotonNetwork.Destroy(collision.gameObject);

                // HP ������Ʈ RPC ȣ��
                PV.RPC("HPUpdate2", RpcTarget.All, Damage);
            }
        }
    }
    private void Die()
    {
        // ĳ���Ͱ� �׾��� �� ó���� �۾��� �̰��� �߰�
        // ���� ���, ���� ������Ʈ ��Ȱ��ȭ �Ǵ� ���� �ִϸ��̼� ��� ���� ������ �� �ֽ��ϴ�.
    }

    private void ApplyDamage()
    {

        // ĳ���Ͱ� �׾��� �� ó���� �۾��� �̰��� �߰�
        // ���� ���, ���� ������Ʈ ��Ȱ��ȭ �Ǵ� ���� �ִϸ��̼� ��� ���� ������ �� �ֽ��ϴ�.
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    

    [PunRPC]
    void HPUpdate2(float damage)
    {
        HP_Copy = highestParent.GetComponent<Player_HP>().VirtualHP;

        HP_Copy -= damage;

        Debug.Log(HP_Copy + "����� ��");

        highestParent.GetComponent<Player_HP>().VirtualHP = HP_Copy;

    }

    [PunRPC]
    void ShowBloodEffect(Vector3 position, Quaternion rotation)
    {
        // bloodEffect�� �ν��Ͻ�ȭ�ϰ� ���
        ParticleSystem blood = Instantiate(bloodEffectPrefab, position, rotation);
        ParticleSystem bloodEffect = blood.GetComponent<ParticleSystem>();
        if (bloodEffect != null)
        {
            bloodEffect.Play();
        }
    }
}
