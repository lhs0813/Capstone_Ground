using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class P_Control : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    private Animator animator;
    private Transform playerTransform; // �÷��̾� ĳ������ Transform ������Ʈ

    public float moveSpeed = 5.0f;
    private Rigidbody rb;
    private bool isRunning = false;
    private bool canJump = true; // ���� ������ ����

    public float jumpForce = 30.0f; // ���� ��
    public Transform groundCheck; // �÷��̾� �Ʒ��� ���� ����Ű�� Transform
    public LayerMask groundLayer; // ������ ��Ÿ���� ���̾�

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
        playerTransform = transform; // �÷��̾��� Transform�� �����ɴϴ�.
        player = PV.transform.gameObject;

        m4a1PBR = player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r" +
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // ���ϴ� ������Ʈ �̸����� ����
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
                Debug.Log("c������");
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


            // WASD Ű �Է��� �����Ͽ� �÷��̾� �̵� ó��
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

            // �̵� ���� ���
            Vector3 moveDirection = playerTransform.TransformDirection(new Vector3(horizontalInput, 0.0f, verticalInput)).normalized;

            // Rigidbody�� ����Ͽ� �÷��̾� �̵�
            rb.velocity = moveDirection * moveSpeed;

            // "speed" �Ķ���͸� "h" ���� �̿��Ͽ� ����
            animator.SetFloat("Speed", verticalInput);
            animator.SetFloat("Side", horizontalInput);

            // ���� Shift Ű�� ������ �޸��� ���¸� Ȱ��ȭ�ϰ� �ִϸ��̼��� ����մϴ�.
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                moveSpeed = 7.0f;
                isRunning = true;
                SoundWait = 0.3f;
            }

            // ���� Shift Ű�� ���� �޸��� ���¸� ��Ȱ��ȭ�ϰ� �ִϸ��̼��� �����մϴ�.
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isRunning = false;
                moveSpeed = 5.0f;
                SoundWait = 0.5f;
            }

            // ���� ����
            bool isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.1f, groundLayer);

            // �����̽��ٸ� ������ ���� ������ �����̸� ���鿡 ���� ��쿡�� ����
            if (Input.GetKeyDown(KeyCode.Space) && canJump && isGrounded)
            {
                // Jump�� true�� �����Ͽ� SetTrigger�� �ִϸ��̼��� ����մϴ�.
                animator.SetTrigger("Jump");

                // Rigidbody�� ������ ���� �߰��Ͽ� ����
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                // ���� ���̹Ƿ� ���� �Ұ����� ���·� ����
                canJump = false;
            }

            // ���鿡 ����� ���� ���� ������ ���·� ����
            if (isGrounded)
            {
                canJump = true;
                Debug.Log("�̰��� �ֿܼ� ��µǴ� �޽����Դϴ�.");
            }

            // "isRunning" �Ҹ��� �Ķ���͸� Animator�� �����Ͽ� �ִϸ��̼� ����
            animator.SetBool("IsRunning", isRunning);
        }
        
    }
    IEnumerator DelayedAction()
    {
        isFootSoundPlaying = true;
        yield return new WaitForSeconds(SoundWait); // 0.5�� ���� ���

        // FootSound RPC�� ȣ���Ͽ� foot �Ҹ��� ����մϴ�.
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
            // �ٸ� �۾��� �����ϰų� �ƹ��͵� ���� ���� �� �ֽ��ϴ�.
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
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // ���ϴ� ������Ʈ �̸����� ����

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
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // ���ϴ� ������Ʈ �̸����� ����

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
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // ���ϴ� ������Ʈ �̸����� ����

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
            "/lowerarm_r/hand_r/M4A1_PBR").gameObject; // ���ϴ� ������Ʈ �̸����� ����

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