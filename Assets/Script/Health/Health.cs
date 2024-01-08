using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Services.Analytics.Internal;
using UnityEngine.AI;

public class Health : MonoBehaviourPunCallbacks
{
    public NavMeshAgent agent; // ��ΰ�� AI ������Ʈ
    public float HP = 100;
    private AudioSource audioPlayer; // ����� �ҽ� ������Ʈ
    public AudioClip hitClip; // �ǰݽ� ����� �Ҹ�
    public PhotonView PV;

    public ParticleSystem hitEffect;

    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (HP <= 0)
        {

           // HP�� 0 ���Ϸ� �������� ó���� �۾� �߰� (��: ���� ó��)
           Die();
           PV.RPC("Die_Share", RpcTarget.All);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                HP -= 35; // HP�� 10 ����
                ApplyDamage();
                if (HP <= 0)
                {
                    // HP�� 0 ���Ϸ� �������� ó���� �۾� �߰� (��: ���� ó��)
                    Die();
                    PV.RPC("Die_Share", RpcTarget.All);
                }

                // �Ѿ� �浹 ������ �����ͼ� �ش� ��ġ�� ����Ʈ�� ����
                if (hitEffect != null)
                {
                    ContactPoint contact = collision.contacts[0]; // ù ��° �浹 ���� ��������
                    Vector3 hitPoint = contact.point;
                    Quaternion hitRotation = Quaternion.LookRotation(contact.normal); // �浹 ���� �������� ����Ʈ ȸ��

                    Instantiate(hitEffect, hitPoint, hitRotation);
                }
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
        audioPlayer.PlayOneShot(hitClip);

        // ĳ���Ͱ� �׾��� �� ó���� �۾��� �̰��� �߰�
        // ���� ���, ���� ������Ʈ ��Ȱ��ȭ �Ǵ� ���� �ִϸ��̼� ��� ���� ������ �� �ֽ��ϴ�.
    }

    [PunRPC]
    void Die_Share()
    {
        agent.enabled = false;
        // LivingEntity�� Die()�� �����Ͽ� �⺻ ��� ó�� ����

        // �ٸ� AI���� �������� �ʵ��� �ڽ��� ��� �ݶ��̴����� ��Ȱ��ȭ
        //GetComponent<Collider>().enabled = false;

        // ��� �ִϸ��̼� ���
        GetComponent<Animator>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        //animator.applyRootMotion = true;
        //animator.SetTrigger("Die");

        Debug.Log("RPC ȣ��");

        Destroy(gameObject, 2.0f); // 2�� �Ŀ� ����
    }

}