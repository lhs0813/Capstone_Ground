using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Services.Analytics.Internal;
using UnityEngine.AI;

public class Health : MonoBehaviourPunCallbacks
{
    public NavMeshAgent agent; // 경로계산 AI 에이전트
    public float HP = 100;
    private AudioSource audioPlayer; // 오디오 소스 컴포넌트
    public AudioClip hitClip; // 피격시 재생할 소리
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

           // HP가 0 이하로 떨어지면 처리할 작업 추가 (예: 죽음 처리)
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
                HP -= 35; // HP를 10 감소
                ApplyDamage();
                if (HP <= 0)
                {
                    // HP가 0 이하로 떨어지면 처리할 작업 추가 (예: 죽음 처리)
                    Die();
                    PV.RPC("Die_Share", RpcTarget.All);
                }

                // 총알 충돌 지점을 가져와서 해당 위치에 이펙트를 생성
                if (hitEffect != null)
                {
                    ContactPoint contact = collision.contacts[0]; // 첫 번째 충돌 지점 가져오기
                    Vector3 hitPoint = contact.point;
                    Quaternion hitRotation = Quaternion.LookRotation(contact.normal); // 충돌 법선 방향으로 이펙트 회전

                    Instantiate(hitEffect, hitPoint, hitRotation);
                }
            }
        }
    }

    private void Die()
    {
        // 캐릭터가 죽었을 때 처리할 작업을 이곳에 추가
        // 예를 들어, 게임 오브젝트 비활성화 또는 죽음 애니메이션 재생 등을 수행할 수 있습니다.
    }

    private void ApplyDamage()
    {
        audioPlayer.PlayOneShot(hitClip);

        // 캐릭터가 죽었을 때 처리할 작업을 이곳에 추가
        // 예를 들어, 게임 오브젝트 비활성화 또는 죽음 애니메이션 재생 등을 수행할 수 있습니다.
    }

    [PunRPC]
    void Die_Share()
    {
        agent.enabled = false;
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행

        // 다른 AI들을 방해하지 않도록 자신의 모든 콜라이더들을 비활성화
        //GetComponent<Collider>().enabled = false;

        // 사망 애니메이션 재생
        GetComponent<Animator>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        //animator.applyRootMotion = true;
        //animator.SetTrigger("Die");

        Debug.Log("RPC 호출");

        Destroy(gameObject, 2.0f); // 2초 후에 제거
    }

}