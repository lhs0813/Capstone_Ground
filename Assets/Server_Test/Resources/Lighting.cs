using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    public float scaleFactor = 1.0f; // 초기 스케일 값
    public float D_Speed = 0.04f;
    public float Damage = 5.0f;
    private float damageCooldown = 1.0f; // 데미지 적용 간격

    // Dictionary를 사용하여 각 오브젝트별로 lastDamageTime을 저장
    private Dictionary<Collider, float> lastDamageTimes = new Dictionary<Collider, float>();

    private GameObject lightingObject;

    public PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        lightingObject = canvas.transform.Find("Lighting").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // 시간이 지남에 따라 스케일의 X와 Y 축 값을 감소
        scaleFactor -= Time.deltaTime * D_Speed; // 0.1f는 감소 속도를 나타냅니다.

        // 스케일 값을 적용
        Vector3 newScale = new Vector3(Mathf.Max(0.1f, scaleFactor), Mathf.Max(0.1f, scaleFactor), transform.localScale.z);
        transform.localScale = newScale;
    }

    // OnTriggerEnter 함수를 사용하여 트리거 진입시 오브젝트를 Dictionary에 추가
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (!lastDamageTimes.ContainsKey(other))
            {
                lastDamageTimes.Add(other, 0f); // 새 오브젝트를 Dictionary에 추가하고 초기화
            }

            if (other.CompareTag("Player"))
            {
                PV = other.GetComponent<PhotonView>();
                if (PV.IsMine)
                {
                    lightingObject.SetActive(true);
                }
            }
        }
    }

    // OnTriggerExit 함수를 사용하여 트리거를 빠져나간 오브젝트를 Dictionary에서 제거
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            lastDamageTimes.Remove(other);

            if (other.CompareTag("Player"))
            {
                PV = other.GetComponent<PhotonView>();
                if (PV.IsMine)
                {
                    lightingObject.SetActive(false);
                }
            }
        }
    }

    // Update 함수에서 Dictionary에 저장된 lastDamageTime을 확인하고 데미지 처리
    void HandleDamage()
    {
        List<Collider> toRemove = new List<Collider>();

        foreach (var kvp in lastDamageTimes)
        {
            Collider other = kvp.Key;
            float lastDamageTime = kvp.Value;

            if (other == null)
            {
                // 오브젝트가 파괴된 경우, 제거 대상으로 표시
                toRemove.Add(other);
                continue;
            }

            if (Time.time - lastDamageTime >= damageCooldown)
            {
                Debug.Log("접촉 중입니다: " + other.gameObject.name);

                if (other.CompareTag("Player"))
                {
                    Player_HP playerHP = other.GetComponent<Player_HP>();
                    if (playerHP != null)
                    {
                        playerHP.VirtualHP -= Damage;

                        // HP가 0 이하이면 딕셔너리에서 제거
                        if (playerHP.VirtualHP <= 0)
                        {
                            toRemove.Add(other);
                        }
                    }
                }

                if (other.CompareTag("Enemy"))
                {
                    Health health = other.GetComponent<Health>();
                    if (health != null)
                    {
                        health.HP -= Damage;

                        // HP가 0 이하이면 딕셔너리에서 제거
                        if (health.HP <= 0)
                        {
                            toRemove.Add(other);
                        }
                    }

                }
                lastDamageTimes[other] = Time.time; // Dictionary에 새로운 lastDamageTime 설정
            }
        }

        // 제거할 대상을 딕셔너리에서 제거
        foreach (var colliderToRemove in toRemove)
        {
            lastDamageTimes.Remove(colliderToRemove);
        }
    }

    // FixedUpdate 함수에서 데미지 처리를 호출 (타이밍을 맞추려면 FixedUpdate 사용)
    void FixedUpdate()
    {
        HandleDamage();
    }
}