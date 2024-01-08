using UnityEngine;
using TMPro;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class GunFire : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public PhotonView EffectPV;
    public GameObject Player;
    public ParticleSystem gunfireEffect;
    public TextMeshProUGUI modeText;
    private bool isBurstMode = false;
    private bool isFiring = false;
    private float targetAlpha = 0f;
    private float targetAlpha2 = 1f;

    private float alphaSpeed = 1f;

    private float alphaChangeDuration = 0.5f;

    public GameObject UI_Mode2_P;
    public GameObject UI_Mode3_P;


    public GameObject UI_Mode2;
    public GameObject UI_Mode3;

    public AudioSource gunfireSound; // 총 발사 사운드를 위한 AudioSource

    public float fireRate = 0.1f; // 발사 간격

    public int bullets_count = 1;

    public Transform firePoint; // 발사 위치
    public GameObject bulletPrefab; // 총알 프리팹
    public float bulletForce = 200f; // 총알 발사 속도

    private Transform playerUpperChestTr; // 상체위의 Transform
    public Animator animator;

    private void OnEnable()
    {
        // 게임 오브젝트가 활성화될 때 실행되는 코드
        EffectPV.RPC("GunEffect_Stop", RpcTarget.AllBuffered);//건파이어 스탑
        //gunfireEffect.Stop();
        //PV.RPC("GunEffect_Play", RpcTarget.All);//건파이어 플레이

    }

    private void Start()
    {
        // 파티클 시스템 비활성화
        
        UpdateModeText();
        bullets_count = Player.GetComponent<Gun_Script>().bullets;

        
        if (animator)
            playerUpperChestTr = animator.GetBoneTransform(HumanBodyBones.Chest);

        UI_Mode2_P = GameObject.Find("Canvas");

        if (UI_Mode2_P != null)
        {
            Transform transform2 = UI_Mode2_P.transform.Find("Bullet_UI_2");
            Transform transform3 = UI_Mode2_P.transform.Find("Bullet_UI_3");
            TextMeshProUGUI Virtual_Text = UI_Mode2_P.transform.Find("GunMode_T").GetComponent<TextMeshProUGUI>();

            modeText = Virtual_Text;

            if (transform2 != null)
            {
                UI_Mode2 = transform2.gameObject; // Transform을 GameObject로 변환
            }

            if (transform3 != null)
            {
                UI_Mode3 = transform3.gameObject; // Transform을 GameObject로 변환
            }
        }
        //UI_Mode2 = UI_Mode2_P.Find("Bullet_UI_2");
        //UI_Mode3 = UI_Mode2_P.Find("Bullet_UI_3");

    }

    private void Update()
    {
        //gunfireEffect.Stop();
        if (PV.IsMine)
        {
            bullets_count = Player.GetComponent<Gun_Script>().bullets;

            // B 키를 누를 때 단발과 연사 모드를 전환
            if (Input.GetKeyDown(KeyCode.B))
            {
                isBurstMode = !isBurstMode;
                UpdateModeText();

                if (isBurstMode)
                {
                    // 연사 모드에서 벗어날 때 파티클 중지
                    EffectPV.RPC("GunEffect_Stop", RpcTarget.All);//건파이어 스탑
                }
            }

            if (isBurstMode)
            {
                UI_Mode2.SetActive(true);
                UI_Mode3.SetActive(true);
                // 연사 모드
                if (Input.GetMouseButtonDown(0) && !isFiring)
                {
                    isFiring = true;
                    StartCoroutine(ContinuousGunfire());
                    EffectPV.RPC("GunEffect_Play", RpcTarget.All);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isFiring = false;
                    EffectPV.RPC("GunEffect_Stop", RpcTarget.All);//건파이어 스탑
                }
            }
            else
            {
                UI_Mode2.SetActive(false);
                UI_Mode3.SetActive(false);
                // 단발 모드
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(DelayedFunction());
                }
                if (Input.GetMouseButtonUp(0))
                {
                    EffectPV.RPC("GunEffect_Stop", RpcTarget.All);
                }
            }
        }
    }






    IEnumerator ContinuousGunfire()
    {
        isFiring = true;

        while (Input.GetButton("Fire1"))
        {
            //playerUpperChestTr.rotation = playerUpperChestTr.rotation * Quaternion.Euler(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            //gunfireSound.PlayOneShot(gunfireSound.clip);
            EffectPV.RPC("GunSound_Play_one",RpcTarget.All);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 총알에 힘을 가합니다.
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(- firePoint.forward * bulletForce, ForceMode.Impulse);
            }
            Player.GetComponent<Gun_Script>().bullets = bullets_count -1;
            yield return new WaitForSeconds(fireRate);

            //Vector3 recoil = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0f);
            //playerUpperChestTr.transform.Rotate(recoil);

            
        }

        isFiring = false;
    }

    private void UpdateModeText()
    {
        StartCoroutine(UpdateTextAlpha());
    }

    IEnumerator DelayedFunction()
    {
        // 2초 대기

        EffectPV.RPC("GunEffect_Play", RpcTarget.All);
        EffectPV.RPC("GunSound_Play", RpcTarget.All);
        //gunfireSound.Play();

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 총알에 힘을 가합니다.
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(-firePoint.forward * bulletForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(0.1f);

        EffectPV.RPC("GunEffect_Stop", RpcTarget.All);//건파이어 스탑


        // 이곳에서 원하는 작업을 수행합니다.
        Debug.Log("0.1초 후에 실행됩니다.");
    }

    private IEnumerator UpdateTextAlpha()
    {
        if (isBurstMode)
        {
            modeText.color = new Color(modeText.color.r, modeText.color.g, modeText.color.b, 0f);

            modeText.text = "Mode: FullAuto";
            targetAlpha = 1f; // 전환 후 서서히 밝아질 투명도 값
            targetAlpha2 = 0f;

            float elapsedTime = 0f;
            while (elapsedTime < alphaChangeDuration) // 지정된 시간 동안 알파값 조절
            {
                float newAlpha = Mathf.Lerp(0f, targetAlpha, elapsedTime / alphaChangeDuration);
                modeText.color = new Color(modeText.color.r, modeText.color.g, modeText.color.b, newAlpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1.0f); // 1초 대기

            elapsedTime = 0f;
            while (elapsedTime < alphaChangeDuration) // 지정된 시간 동안 알파값 조절
            {
                float newAlpha = Mathf.Lerp(targetAlpha, targetAlpha2, elapsedTime / alphaChangeDuration);
                modeText.color = new Color(modeText.color.r, modeText.color.g, modeText.color.b, newAlpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            modeText.color = new Color(modeText.color.r, modeText.color.g, modeText.color.b, 0f);

            modeText.text = "Mode: SemiAuto";
            targetAlpha = 1f; // 전환 후 서서히 밝아질 투명도 값
            targetAlpha2 = 0f;

            float elapsedTime = 0f;
            while (elapsedTime < alphaChangeDuration) // 지정된 시간 동안 알파값 조절
            {
                float newAlpha = Mathf.Lerp(0f, targetAlpha, elapsedTime / alphaChangeDuration);
                modeText.color = new Color(modeText.color.r, modeText.color.g, modeText.color.b, newAlpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1.0f); // 1초 대기

            elapsedTime = 0f;
            while (elapsedTime < alphaChangeDuration) // 지정된 시간 동안 알파값 조절
            {
                float newAlpha = Mathf.Lerp(targetAlpha, targetAlpha2, elapsedTime / alphaChangeDuration);
                modeText.color = new Color(modeText.color.r, modeText.color.g, modeText.color.b, newAlpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    [PunRPC]
    void GunEffect_Stop()
    {
        gunfireEffect.Stop();
    }
    [PunRPC]
    void GunEffect_Play()
    {
        gunfireEffect.Play();
    }
    [PunRPC]
    void GunSound_Play_one()
    {
        gunfireSound.PlayOneShot(gunfireSound.clip);
    }
    [PunRPC]
    void GunSound_Play()
    {
        gunfireSound.Play();
    }



}