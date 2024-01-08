using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    public float scaleFactor = 1.0f; // �ʱ� ������ ��
    public float D_Speed = 0.04f;
    public float Damage = 5.0f;
    private float damageCooldown = 1.0f; // ������ ���� ����

    // Dictionary�� ����Ͽ� �� ������Ʈ���� lastDamageTime�� ����
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
        // �ð��� ������ ���� �������� X�� Y �� ���� ����
        scaleFactor -= Time.deltaTime * D_Speed; // 0.1f�� ���� �ӵ��� ��Ÿ���ϴ�.

        // ������ ���� ����
        Vector3 newScale = new Vector3(Mathf.Max(0.1f, scaleFactor), Mathf.Max(0.1f, scaleFactor), transform.localScale.z);
        transform.localScale = newScale;
    }

    // OnTriggerEnter �Լ��� ����Ͽ� Ʈ���� ���Խ� ������Ʈ�� Dictionary�� �߰�
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (!lastDamageTimes.ContainsKey(other))
            {
                lastDamageTimes.Add(other, 0f); // �� ������Ʈ�� Dictionary�� �߰��ϰ� �ʱ�ȭ
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

    // OnTriggerExit �Լ��� ����Ͽ� Ʈ���Ÿ� �������� ������Ʈ�� Dictionary���� ����
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

    // Update �Լ����� Dictionary�� ����� lastDamageTime�� Ȯ���ϰ� ������ ó��
    void HandleDamage()
    {
        List<Collider> toRemove = new List<Collider>();

        foreach (var kvp in lastDamageTimes)
        {
            Collider other = kvp.Key;
            float lastDamageTime = kvp.Value;

            if (other == null)
            {
                // ������Ʈ�� �ı��� ���, ���� ������� ǥ��
                toRemove.Add(other);
                continue;
            }

            if (Time.time - lastDamageTime >= damageCooldown)
            {
                Debug.Log("���� ���Դϴ�: " + other.gameObject.name);

                if (other.CompareTag("Player"))
                {
                    Player_HP playerHP = other.GetComponent<Player_HP>();
                    if (playerHP != null)
                    {
                        playerHP.VirtualHP -= Damage;

                        // HP�� 0 �����̸� ��ųʸ����� ����
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

                        // HP�� 0 �����̸� ��ųʸ����� ����
                        if (health.HP <= 0)
                        {
                            toRemove.Add(other);
                        }
                    }

                }
                lastDamageTimes[other] = Time.time; // Dictionary�� ���ο� lastDamageTime ����
            }
        }

        // ������ ����� ��ųʸ����� ����
        foreach (var colliderToRemove in toRemove)
        {
            lastDamageTimes.Remove(colliderToRemove);
        }
    }

    // FixedUpdate �Լ����� ������ ó���� ȣ�� (Ÿ�̹��� ���߷��� FixedUpdate ���)
    void FixedUpdate()
    {
        HandleDamage();
    }
}