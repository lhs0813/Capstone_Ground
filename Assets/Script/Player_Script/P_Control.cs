using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class P_Control : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    private Animator animator;
    private Transform playerTransform; // 플레이어 캐릭터의 Transform 컴포넌트

    public float moveSpeed = 5.0f;
    private Rigidbody rb;
    private bool isRunning = false;
    private bool canJump = true; // 점프 가능한 상태

    public float jumpForce = 30.0f; // 점프 힘
    public Transform groundCheck; // 플레이어 아래의 점을 가리키는 Transform
    public LayerMask groundLayer; // 지면을 나타내는 레이어

    private bool Sit = false;
    private bool GunUP = false;

    public GameObject player;
    public GameObject m4a1PBR;

    public AudioSource foot;

    public bool iswalking = false;

    private bool isFootSoundPlaying = false;

    private float SoundWait = 0.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerTransform = transform; // 플레이어의 Transform을 가져옵니다.
        player = PV.transform.gameObject;

        m4a1PBR = player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r" +
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // 원하는 오브젝트 이름으로 변경
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            //PV.RPC("GunVisual", RpcTarget.All);
        }

        if (PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                PV.RPC("GunUPDOWN", RpcTarget.All);

                //GunUP = !GunUP;
            }
            

            if (Input.GetKeyDown(KeyCode.C)) 
            {
                Debug.Log("c가눌림");
                Sit = !Sit;
                if (Sit)
                {
                    
                    animator.SetLayerWeight(2, 0);
                    animator.SetLayerWeight(1, 1);
                }
                else if(Sit == false)
                {
                    
                    animator.SetLayerWeight(2, 1);
                    animator.SetLayerWeight(1, 0);
                }

            }


            // WASD 키 입력을 감지하여 플레이어 이동 처리
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            if(horizontalInput != 0)
            {
                //foot.PlayOneShot(foot.clip);
                //foot.Play();
                iswalking = true;

            }
            else if (verticalInput != 0)
            {
                //foot.PlayOneShot(foot.clip);
                //foot.Play();
                iswalking = true;
            }
            else
            {
                iswalking = false;
            }

            // 이동 방향 계산
            Vector3 moveDirection = playerTransform.TransformDirection(new Vector3(horizontalInput, 0.0f, verticalInput)).normalized;

            // Rigidbody를 사용하여 플레이어 이동
            rb.velocity = moveDirection * moveSpeed;

            // "speed" 파라미터를 "h" 값을 이용하여 제어
            animator.SetFloat("Speed", verticalInput);
            animator.SetFloat("Side", horizontalInput);

            // 왼쪽 Shift 키를 누르면 달리기 상태를 활성화하고 애니메이션을 재생합니다.
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                moveSpeed = 7.0f;
                isRunning = true;
                SoundWait = 0.3f;
            }

            // 왼쪽 Shift 키를 떼면 달리기 상태를 비활성화하고 애니메이션을 정지합니다.
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isRunning = false;
                moveSpeed = 5.0f;
                SoundWait = 0.5f;
            }

            // 지면 감지
            bool isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.1f, groundLayer);

            // 스페이스바를 누르고 점프 가능한 상태이며 지면에 닿은 경우에만 점프
            if (Input.GetKeyDown(KeyCode.Space) && canJump && isGrounded)
            {
                // Jump를 true로 설정하여 SetTrigger로 애니메이션을 재생합니다.
                animator.SetTrigger("Jump");

                // Rigidbody에 위로의 힘을 추가하여 점프
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                // 점프 중이므로 점프 불가능한 상태로 설정
                canJump = false;
            }

            // 지면에 닿았을 때만 점프 가능한 상태로 변경
            if (isGrounded)
            {
                canJump = true;
                Debug.Log("이것은 콘솔에 출력되는 메시지입니다.");
            }

            // "isRunning" 불리언 파라미터를 Animator에 전달하여 애니메이션 제어
            animator.SetBool("IsRunning", isRunning);
        }
        
    }
    IEnumerator DelayedAction()
    {
        isFootSoundPlaying = true;
        yield return new WaitForSeconds(SoundWait); // 0.5초 동안 대기

        // FootSound RPC를 호출하여 foot 소리를 재생합니다.
        PV.RPC("FootSound", RpcTarget.All);

        isFootSoundPlaying = false;
    }

    private void LateUpdate()
    {
        if (iswalking && !isFootSoundPlaying)
        {
            StartCoroutine(DelayedAction());
        }
        else
        {
            // 다른 작업을 수행하거나 아무것도 하지 않을 수 있습니다.
        }
    }


    [PunRPC]
    void GunUPDOWN()
    {
        if (m4a1PBR.gameObject.activeSelf == false)
        {
            Animator MyAnim = player.GetComponent<Animator>();
            MyAnim.SetTrigger("GunUP");

            if (m4a1PBR != null)
            {
                player = PV.transform.gameObject;
                player.GetComponent<Animator>().SetBool("GunOn", true);
                m4a1PBR = player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r" +
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // 원하는 오브젝트 이름으로 변경

                m4a1PBR.gameObject.SetActive(true);

            }
        }
        else if (m4a1PBR.gameObject.activeSelf == true)
        {
            Animator MyAnim = player.GetComponent<Animator>();
            MyAnim.SetTrigger("GunDown");

            if (m4a1PBR != null)
            {
                player = PV.transform.gameObject;
                player.GetComponent<Animator>().SetBool("GunOn", false);
                m4a1PBR = player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r" +
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // 원하는 오브젝트 이름으로 변경

                m4a1PBR.gameObject.SetActive(false);

            }
        }
    }

    [PunRPC]
    void GunVisual()
    {
        if (GunUP)
        {
            animator.SetTrigger("GunUP");
            if (m4a1PBR != null)
            {
                player = PV.transform.gameObject;
                player.GetComponent<Animator>().SetBool("GunOn", true);
                m4a1PBR = player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r" +
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // 원하는 오브젝트 이름으로 변경

                m4a1PBR.gameObject.SetActive(true);
                
            }
        }
        else if (GunUP == false)
        {
            animator.SetTrigger("GunDown");
            if (m4a1PBR != null)
            {
                player = PV.transform.gameObject;
                player.GetComponent<Animator>().SetBool("GunOn", false);
                m4a1PBR = player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r" +
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // 원하는 오브젝트 이름으로 변경

                m4a1PBR.gameObject.SetActive(false);
                
            }
        }
    }


    [PunRPC]
    void FootSound()
    {
        foot.PlayOneShot(foot.clip);
    }
}