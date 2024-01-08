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

        // 부모 오브젝트를 찾아 올라가며 최상위 부모 오브젝트를 찾습니다.
        while (highestParent.parent != null)
        {
            highestParent = highestParent.parent;
        }

        // 최상위 부모 오브젝트에 접근할 수 있습니다.
        Debug.Log("가장 상위 부모 오브젝트의 이름: " + highestParent.name);

        HP_Copy = highestParent.GetComponent<Player_HP>().HP;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                // 총알 충돌 지점을 가져와서 해당 위치에 bloodEffect를 재생
                if (bloodEffectPrefab != null)
                {
                    ContactPoint contact = collision.contacts[0];
                    Vector3 hitPoint = contact.point;
                    Quaternion hitRotation = Quaternion.LookRotation(contact.normal);

                    // RPC를 사용하여 bloodEffect를 모든 플레이어에게 동기화
                    PV.RPC("ShowBloodEffect", RpcTarget.All, hitPoint, hitRotation);
                }

                // 총알 오브젝트를 즉시 삭제
                //PhotonNetwork.Destroy(collision.gameObject);

                // HP 업데이트 RPC 호출
                PV.RPC("HPUpdate2", RpcTarget.All, Damage);
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

        // 캐릭터가 죽었을 때 처리할 작업을 이곳에 추가
        // 예를 들어, 게임 오브젝트 비활성화 또는 죽음 애니메이션 재생 등을 수행할 수 있습니다.
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

        Debug.Log(HP_Copy + "대미지 줌");

        highestParent.GetComponent<Player_HP>().VirtualHP = HP_Copy;

    }

    [PunRPC]
    void ShowBloodEffect(Vector3 position, Quaternion rotation)
    {
        // bloodEffect를 인스턴스화하고 재생
        ParticleSystem blood = Instantiate(bloodEffectPrefab, position, rotation);
        ParticleSystem bloodEffect = blood.GetComponent<ParticleSystem>();
        if (bloodEffect != null)
        {
            bloodEffect.Play();
        }
    }
}
