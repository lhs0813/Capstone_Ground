using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Gun_Script : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;
    

    private bool AIMGUN = false;
    private Animator animator;
    public GameObject m4a1;
    private bool GunOn = false;
    private bool isShooting = false; // 총 발사 상태를 추적하기 위한 변수
    private Transform playerChestTr; // 상체의 Transform
    private Transform playerUpperChestTr; // 상체위의 Transform
    private bool aimMode = false; // 에임 모드 여부

    public GameObject UI_Mode2_P;

    public GameObject Crosshair;

    protected Quaternion chestRotation = Quaternion.identity;
    protected Quaternion local_chestRotation;
    protected Quaternion local_UpperchestRotation;
    protected Quaternion upperChestRotation = Quaternion.identity;


    private bool isMousePressed = false;
    private float mousePressedTime = 0f;
    public float longClickThreshold = 0.5f; // 롱클릭으로 간주하는 시간(초)


    public Transform mainCameraTr; // 메인 카메라의 Transform
    public Vector3 chestOffset = new Vector3(0, 0, 0); // 상체 보정 오프셋
    public Vector3 UpperchestOffset = new Vector3(0, 0, 0); // 상체 보정 오프셋

    private bool AimView = false;

    public int bullets = 30; // 시작할 때 30발의 총알이 있다고 가정
    public int total_bullets = 150;
    private bool isReloading = false;
    private float reloadStartTime = 0f;
    private float reloadDuration = 2f; // 재장전 애니메이션의 지속 시간 (초)

    public AudioSource reloadSound; // 사운드 재생을 위한 AudioSource

    
    public float rotationSpeed = 15.0f; // 회전 속도
    private bool isQPressed = false;
    private bool isEPressed = false;
    private bool isQUp = false;
    private bool isEUp = false;

    public Vector3 UpperchestOffset_Q = new Vector3(0, 0, 0); // 상체 보정 오프셋
    public Vector3 UpperchestOffset_E = new Vector3(0, 0, 0); // 상체 보정 오프셋

    public float vector_x = 0;
    public float vector_y = 0;
    public float vector_z = 0;

    private bool isDelayedRotationRunning = false;
    private bool isDelayedRotationback = false;
    private bool isQAnim = false;
    private bool isEAnim = false;

    private bool ShotFire = false;


    public float a = 0;
    public float b = 0;
    public float c = 0;


    public int bullets_count = 1;
    public TextMeshProUGUI Remainbullets;
    public TextMeshProUGUI Total_bullets;
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("충돌됨2");
        // 충돌한 오브젝트의 태그가 "M4a1"인지 확인
        /*if (collision.gameObject.CompareTag("M4a1"))
        {
            GunOn = true;
            animator.SetBool("GunOn", true);
            Debug.Log("충돌됨");
            // 현재 오브젝트를 활성화
            m4a1.SetActive(true);
            
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        UI_Mode2_P = GameObject.Find("Canvas");
        animator = GetComponent<Animator>();
        playerChestTr = animator.GetBoneTransform(HumanBodyBones.Chest); // 해당 본의 transform 가져오기
        playerUpperChestTr = animator.GetBoneTransform(HumanBodyBones.Chest); // 해당 본의 transform 가져오기


        TextMeshProUGUI Virtual_Text = UI_Mode2_P.transform.Find("Remain_Bullets").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Virtual_Text2 = UI_Mode2_P.transform.Find("Bullets").GetComponent<TextMeshProUGUI>();

        Total_bullets = Virtual_Text;
        Remainbullets = Virtual_Text2;
    }

    

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            

            Total_bullets.text = total_bullets.ToString();
            Remainbullets.text = bullets.ToString();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                isQPressed = true;
                vector_x = 0;
                vector_y = 0;
                vector_z = 0;
                isQUp = false;
                if (isQAnim == false)
                {
                    isQAnim = true;
                }

            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                isQPressed = false;
                //vector_x = 0;
                //vector_y = 0;
                //vector_z = 0;
                isDelayedRotationback = false; // 실행 종료 후 플래그를 다시 false로 설정

                isQUp = true;
            }

            // E 키를 누르면 상체를 오른쪽으로 회전
            if (Input.GetKeyDown(KeyCode.E))
            {
                isEPressed = true;
                vector_x = 0;
                vector_y = 0;
                vector_z = 0;
                isEUp = false;
                if (isEAnim == false)
                {
                    isEAnim = true;
                }
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                isEPressed = false;
                isDelayedRotationback = false; // 실행 종료 후 플래그를 다시 false로 설정
                isEUp = true;
            }



            if (Input.GetKeyDown(KeyCode.R) && bullets <= 29 && !isReloading)
            {
                // 재장전을 시작하고 애니메이션을 실행합니다.
                isReloading = true;
                reloadStartTime = Time.time; // 재장전 시작 시간 기록
                animator.SetTrigger("Reload");


                // 여기서 총알을 재장전하거나 다른 로직을 수행합니다.
                // 예를 들어, 총알 수를 30발로 다시 채웁니다.
                total_bullets = total_bullets - (30 - bullets);
                bullets = 30;

                Debug.Log("재장전!!");
                reloadSound.Play();

                if (reloadSound != null)
                {
                    
                }
            }

            if (isReloading)
            {
                // 현재 시간과 재장전 시작 시간을 비교하여 대기 시간 동안은 입력을 무시합니다.
                if (Time.time - reloadStartTime >= reloadDuration)
                {
                    isReloading = false; // 재장전이 끝났으면 isReloading을 false로 설정합니다.
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                isMousePressed = true;
                mousePressedTime = Time.time;

                Debug.Log("바라보기");
                StartCoroutine(ActivateAimMode());
            }


            if (Input.GetMouseButton(0))
            {
                ShotFire = true;
                // 마우스 왼쪽 버튼을 누르고 있는 동안의 처리
            }

            if (Input.GetMouseButtonUp(0))
            {
                ShotFire = false;
            }

            if (Input.GetMouseButtonUp(1))
            {
                isMousePressed = false;

                // 일정 시간 동안 눌려지지 않았으면 클릭으로 처리
                if (Time.time - mousePressedTime <= longClickThreshold)
                {
                    // 짧은 클릭 처리
                    if (AimView == false)
                    {
                        StartCoroutine(ActivateAimMode());

                        AimView = true;
                    }

                    else if (AimView == true)
                    {
                        StartCoroutine(DeactivateAimMode());

                        AimView = false;
                    }


                }
                else
                {
                    Debug.Log("해제");
                    StartCoroutine(DeactivateAimMode());
                    // 롱 클릭 처리
                }
            }

            if (Input.GetMouseButtonDown(1)) // 우클릭 눌리는 동안
            {

            }
            else if (Input.GetMouseButtonUp(1)) // 우클릭을 때면
            {


            }

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Shoot");
                isShooting = true;

            }

            // 마우스 왼쪽 버튼을 때면 총 발사 상태를 false로 설정
            if (Input.GetMouseButtonUp(0))
            {
                isShooting = false;
                animator.ResetTrigger("Shoot");
            }
        }

       
    }

    private IEnumerator ActivateAimMode()
    {
        
        yield return null; // 에임 모드 활성화 전 대기 (다음 프레임으로)

        animator.SetBool("GunUp", true);
        Crosshair.SetActive(true);
        aimMode = true;
        
    }

    private IEnumerator DeactivateAimMode()
    {
        yield return null; // 에임 모드 비활성화 전 대기 (다음 프레임으로)
        animator.SetBool("GunUp", false);
        Crosshair.SetActive(false);
        aimMode = false;
    }

    IEnumerator DelayedAction()
    {
        Debug.Log("Action Start");

        // 2초 동안 딜레이
        yield return new WaitForSeconds(1.0f);

        Debug.Log("Action after 2 seconds");
    }

    private IEnumerator DelayedRotation()
    {
        if (isDelayedRotationRunning) // 이미 실행 중인 경우, 새로 시작하지 않음
            yield break;

        if (vector_x < 5.0f && vector_y > -20.0f && vector_z > -20.0f)
        {
            isDelayedRotationRunning = true; // 실행 중 플래그를 설정

            for (int i = 0; i < 20; i++)
            {
                vector_x = vector_x + 0.25f;
                vector_y = vector_y - 1.0f;
                vector_z = vector_z - 1.0f;
                //playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));

                yield return new WaitForSeconds(0.01f); // 0.1초 딜레이
                Debug.Log("0.1초 딜레이");
            }

            isDelayedRotationRunning = false; // 실행 종료 후 플래그를 다시 false로 설정
        }
    }

    private IEnumerator DelayedRotation_back()
    {
        if (isDelayedRotationback) // 이미 실행 중인 경우, 새로 시작하지 않음
            yield break;

        if (vector_x >= 5.0f && vector_y <= -20.0f && vector_z <= -20.0f)
        {
            isDelayedRotationback = true; // 실행 중 플래그를 설정

            for (int i = 0; i < 20; i++)
            {
                

                vector_x = vector_x - 0.25f;
                vector_y = vector_y + 1.0f;
                vector_z = vector_z + 1.0f;
                //playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));

                yield return new WaitForSeconds(0.01f); // 0.1초 딜레이
                Debug.Log("0.1초 딜레이_back");
            }

            isDelayedRotationback = false; // 실행 종료 후 플래그를 다시 false로 설정
            isQUp = false;
            isQAnim= false;
        }
    }

    private IEnumerator DelayedRotation_E()
    {
        if (isDelayedRotationRunning) // 이미 실행 중인 경우, 새로 시작하지 않음
            yield break;

        if (vector_x > -5.0f && vector_y < 20.0f && vector_z < 20.0f)
        {
            isDelayedRotationRunning = true; // 실행 중 플래그를 설정

            for (int i = 0; i < 20; i++)
            {
                vector_x = vector_x - 0.25f;
                vector_y = vector_y + 1.0f;
                vector_z = vector_z + 1.0f;
                //playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));

                yield return new WaitForSeconds(0.01f); // 0.1초 딜레이
                Debug.Log("0.1초 딜레이");
            }

            isDelayedRotationRunning = false; // 실행 종료 후 플래그를 다시 false로 설정
        }
    }

    private IEnumerator DelayedRotation_back_E()
    {
        if (isDelayedRotationback) // 이미 실행 중인 경우, 새로 시작하지 않음
            yield break;

        if (vector_x <= -5.0f && vector_y >= 20.0f && vector_z >= 20.0f)
        {
            isDelayedRotationback = true; // 실행 중 플래그를 설정

            for (int i = 0; i < 20; i++)
            {


                vector_x = vector_x + 0.25f;
                vector_y = vector_y - 1.0f;
                vector_z = vector_z - 1.0f;
                //playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));

                yield return new WaitForSeconds(0.01f); // 0.1초 딜레이
                Debug.Log("0.1초 딜레이_back");
            }

            isDelayedRotationback = false; // 실행 종료 후 플래그를 다시 false로 설정
            isEUp = false;
            isEAnim = false;
        }
    }

    private void LateUpdate()
    {
        
        Debug.Log("RPC 호출!!");
        if (PV.IsMine)
        {
            local_chestRotation = playerChestTr.rotation;
            local_UpperchestRotation = playerUpperChestTr.rotation;

            if (aimMode && playerChestTr && playerUpperChestTr)
            {
                // 카메라가 보고 있는 방향
                Vector3 chestDir = mainCameraTr.position + mainCameraTr.forward * 50f;


                playerChestTr.LookAt(chestDir); // 상체를 카메라가 보는 방향으로 보기
                playerChestTr.rotation = playerChestTr.rotation * Quaternion.Euler(chestOffset); // 상체 로테이션 보정

                local_chestRotation = playerChestTr.rotation;
                local_UpperchestRotation = playerUpperChestTr.rotation;

                //Debug.Log(local_chestRotation + "각도가 변하긴 해?");
                //Debug.Log(playerChestTr.rotation + "현재 상체각도");
                //UpperchestOffset.LookAt(chestDir); // 상체를 카메라가 보는 방향으로 보기

                if (isQPressed && isQAnim == true)
                {
                    StartCoroutine(DelayedRotation());
                    playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));
                    local_UpperchestRotation = playerUpperChestTr.rotation;
                }
                else if (isQUp && isQAnim == true)
                {
                    //Debug.Log("0.1초 딜레이_back 실행은 됨?");
                    StartCoroutine(DelayedRotation_back());
                    playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));
                    local_UpperchestRotation = playerUpperChestTr.rotation;
                }
                // 상체의 회전을 조절
                else if (isEPressed && isEAnim == true)
                {
                    StartCoroutine(DelayedRotation_E());
                    playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));
                    local_UpperchestRotation = playerUpperChestTr.rotation;
                }
                else if (isEUp && isEAnim == true)
                {
                    StartCoroutine(DelayedRotation_back_E());
                    playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));
                    local_UpperchestRotation = playerUpperChestTr.rotation;
                }
                else if (!isQPressed && !isEPressed && !isQUp && !isEUp)
                {
                    playerUpperChestTr.rotation = playerUpperChestTr.rotation * Quaternion.Euler(UpperchestOffset); // 상체 로테이션 보정
                    local_UpperchestRotation = playerUpperChestTr.rotation;
                    if (ShotFire = isShooting)
                    {
                        playerUpperChestTr.rotation = playerUpperChestTr.rotation * Quaternion.Euler(Random.Range(-a, a), Random.Range(-b, b), Random.Range(-c, c));
                        local_UpperchestRotation = playerUpperChestTr.rotation;
                    }
                }
            }
        }
        else
        {
            playerChestTr.rotation = chestRotation;
            playerUpperChestTr.rotation = upperChestRotation;
        }




    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            // 내 클라이언트인 경우 상체 회전 값을 보냅니다.
            stream.SendNext(local_chestRotation);
            //Debug.Log(local_chestRotation + "똑같이 보냈니??");
            stream.SendNext(local_UpperchestRotation);
            
            //Debug.Log(playerChestTr.rotation + "보내기");
            //stream.SendNext(playerUpperChestTr.rotation);
        }
        else
        {
            
            // 다른 클라이언트인 경우 상체 회전 값을 받아 설정합니다.
            chestRotation = (Quaternion)stream.ReceiveNext();
            upperChestRotation = (Quaternion)stream.ReceiveNext();
            //Debug.Log(playerChestTr.rotation + "받기");


            //playerUpperChestTr.rotation = (Quaternion)stream.ReceiveNext();


            //Debug.Log((Quaternion)stream.ReceiveNext());


            /*playerChestTr.rotation = chestRotation;
            playerUpperChestTr.rotation = upperChestRotation;*/
        }
    }
    
    [PunRPC]
    public void SyncChestRotation(Quaternion chestRotation, Quaternion upperChestRotation)
    {
        
    }
}