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
    private bool isShooting = false; // �� �߻� ���¸� �����ϱ� ���� ����
    private Transform playerChestTr; // ��ü�� Transform
    private Transform playerUpperChestTr; // ��ü���� Transform
    private bool aimMode = false; // ���� ��� ����

    public GameObject UI_Mode2_P;

    public GameObject Crosshair;

    protected Quaternion chestRotation = Quaternion.identity;
    protected Quaternion local_chestRotation;
    protected Quaternion local_UpperchestRotation;
    protected Quaternion upperChestRotation = Quaternion.identity;


    private bool isMousePressed = false;
    private float mousePressedTime = 0f;
    public float longClickThreshold = 0.5f; // ��Ŭ������ �����ϴ� �ð�(��)


    public Transform mainCameraTr; // ���� ī�޶��� Transform
    public Vector3 chestOffset = new Vector3(0, 0, 0); // ��ü ���� ������
    public Vector3 UpperchestOffset = new Vector3(0, 0, 0); // ��ü ���� ������

    private bool AimView = false;

    public int bullets = 30; // ������ �� 30���� �Ѿ��� �ִٰ� ����
    public int total_bullets = 150;
    private bool isReloading = false;
    private float reloadStartTime = 0f;
    private float reloadDuration = 2f; // ������ �ִϸ��̼��� ���� �ð� (��)

    public AudioSource reloadSound; // ���� ����� ���� AudioSource

    
    public float rotationSpeed = 15.0f; // ȸ�� �ӵ�
    private bool isQPressed = false;
    private bool isEPressed = false;
    private bool isQUp = false;
    private bool isEUp = false;

    public Vector3 UpperchestOffset_Q = new Vector3(0, 0, 0); // ��ü ���� ������
    public Vector3 UpperchestOffset_E = new Vector3(0, 0, 0); // ��ü ���� ������

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
        //Debug.Log("�浹��2");
        // �浹�� ������Ʈ�� �±װ� "M4a1"���� Ȯ��
        /*if (collision.gameObject.CompareTag("M4a1"))
        {
            GunOn = true;
            animator.SetBool("GunOn", true);
            Debug.Log("�浹��");
            // ���� ������Ʈ�� Ȱ��ȭ
            m4a1.SetActive(true);
            
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        UI_Mode2_P = GameObject.Find("Canvas");
        animator = GetComponent<Animator>();
        playerChestTr = animator.GetBoneTransform(HumanBodyBones.Chest); // �ش� ���� transform ��������
        playerUpperChestTr = animator.GetBoneTransform(HumanBodyBones.Chest); // �ش� ���� transform ��������


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
                isDelayedRotationback = false; // ���� ���� �� �÷��׸� �ٽ� false�� ����

                isQUp = true;
            }

            // E Ű�� ������ ��ü�� ���������� ȸ��
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
                isDelayedRotationback = false; // ���� ���� �� �÷��׸� �ٽ� false�� ����
                isEUp = true;
            }



            if (Input.GetKeyDown(KeyCode.R) && bullets <= 29 && !isReloading)
            {
                // �������� �����ϰ� �ִϸ��̼��� �����մϴ�.
                isReloading = true;
                reloadStartTime = Time.time; // ������ ���� �ð� ���
                animator.SetTrigger("Reload");


                // ���⼭ �Ѿ��� �������ϰų� �ٸ� ������ �����մϴ�.
                // ���� ���, �Ѿ� ���� 30�߷� �ٽ� ä��ϴ�.
                total_bullets = total_bullets - (30 - bullets);
                bullets = 30;

                Debug.Log("������!!");
                reloadSound.Play();

                if (reloadSound != null)
                {
                    
                }
            }

            if (isReloading)
            {
                // ���� �ð��� ������ ���� �ð��� ���Ͽ� ��� �ð� ������ �Է��� �����մϴ�.
                if (Time.time - reloadStartTime >= reloadDuration)
                {
                    isReloading = false; // �������� �������� isReloading�� false�� �����մϴ�.
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                isMousePressed = true;
                mousePressedTime = Time.time;

                Debug.Log("�ٶ󺸱�");
                StartCoroutine(ActivateAimMode());
            }


            if (Input.GetMouseButton(0))
            {
                ShotFire = true;
                // ���콺 ���� ��ư�� ������ �ִ� ������ ó��
            }

            if (Input.GetMouseButtonUp(0))
            {
                ShotFire = false;
            }

            if (Input.GetMouseButtonUp(1))
            {
                isMousePressed = false;

                // ���� �ð� ���� �������� �ʾ����� Ŭ������ ó��
                if (Time.time - mousePressedTime <= longClickThreshold)
                {
                    // ª�� Ŭ�� ó��
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
                    Debug.Log("����");
                    StartCoroutine(DeactivateAimMode());
                    // �� Ŭ�� ó��
                }
            }

            if (Input.GetMouseButtonDown(1)) // ��Ŭ�� ������ ����
            {

            }
            else if (Input.GetMouseButtonUp(1)) // ��Ŭ���� ����
            {


            }

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Shoot");
                isShooting = true;

            }

            // ���콺 ���� ��ư�� ���� �� �߻� ���¸� false�� ����
            if (Input.GetMouseButtonUp(0))
            {
                isShooting = false;
                animator.ResetTrigger("Shoot");
            }
        }

       
    }

    private IEnumerator ActivateAimMode()
    {
        
        yield return null; // ���� ��� Ȱ��ȭ �� ��� (���� ����������)

        animator.SetBool("GunUp", true);
        Crosshair.SetActive(true);
        aimMode = true;
        
    }

    private IEnumerator DeactivateAimMode()
    {
        yield return null; // ���� ��� ��Ȱ��ȭ �� ��� (���� ����������)
        animator.SetBool("GunUp", false);
        Crosshair.SetActive(false);
        aimMode = false;
    }

    IEnumerator DelayedAction()
    {
        Debug.Log("Action Start");

        // 2�� ���� ������
        yield return new WaitForSeconds(1.0f);

        Debug.Log("Action after 2 seconds");
    }

    private IEnumerator DelayedRotation()
    {
        if (isDelayedRotationRunning) // �̹� ���� ���� ���, ���� �������� ����
            yield break;

        if (vector_x < 5.0f && vector_y > -20.0f && vector_z > -20.0f)
        {
            isDelayedRotationRunning = true; // ���� �� �÷��׸� ����

            for (int i = 0; i < 20; i++)
            {
                vector_x = vector_x + 0.25f;
                vector_y = vector_y - 1.0f;
                vector_z = vector_z - 1.0f;
                //playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));

                yield return new WaitForSeconds(0.01f); // 0.1�� ������
                Debug.Log("0.1�� ������");
            }

            isDelayedRotationRunning = false; // ���� ���� �� �÷��׸� �ٽ� false�� ����
        }
    }

    private IEnumerator DelayedRotation_back()
    {
        if (isDelayedRotationback) // �̹� ���� ���� ���, ���� �������� ����
            yield break;

        if (vector_x >= 5.0f && vector_y <= -20.0f && vector_z <= -20.0f)
        {
            isDelayedRotationback = true; // ���� �� �÷��׸� ����

            for (int i = 0; i < 20; i++)
            {
                

                vector_x = vector_x - 0.25f;
                vector_y = vector_y + 1.0f;
                vector_z = vector_z + 1.0f;
                //playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));

                yield return new WaitForSeconds(0.01f); // 0.1�� ������
                Debug.Log("0.1�� ������_back");
            }

            isDelayedRotationback = false; // ���� ���� �� �÷��׸� �ٽ� false�� ����
            isQUp = false;
            isQAnim= false;
        }
    }

    private IEnumerator DelayedRotation_E()
    {
        if (isDelayedRotationRunning) // �̹� ���� ���� ���, ���� �������� ����
            yield break;

        if (vector_x > -5.0f && vector_y < 20.0f && vector_z < 20.0f)
        {
            isDelayedRotationRunning = true; // ���� �� �÷��׸� ����

            for (int i = 0; i < 20; i++)
            {
                vector_x = vector_x - 0.25f;
                vector_y = vector_y + 1.0f;
                vector_z = vector_z + 1.0f;
                //playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));

                yield return new WaitForSeconds(0.01f); // 0.1�� ������
                Debug.Log("0.1�� ������");
            }

            isDelayedRotationRunning = false; // ���� ���� �� �÷��׸� �ٽ� false�� ����
        }
    }

    private IEnumerator DelayedRotation_back_E()
    {
        if (isDelayedRotationback) // �̹� ���� ���� ���, ���� �������� ����
            yield break;

        if (vector_x <= -5.0f && vector_y >= 20.0f && vector_z >= 20.0f)
        {
            isDelayedRotationback = true; // ���� �� �÷��׸� ����

            for (int i = 0; i < 20; i++)
            {


                vector_x = vector_x + 0.25f;
                vector_y = vector_y - 1.0f;
                vector_z = vector_z - 1.0f;
                //playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));

                yield return new WaitForSeconds(0.01f); // 0.1�� ������
                Debug.Log("0.1�� ������_back");
            }

            isDelayedRotationback = false; // ���� ���� �� �÷��׸� �ٽ� false�� ����
            isEUp = false;
            isEAnim = false;
        }
    }

    private void LateUpdate()
    {
        
        Debug.Log("RPC ȣ��!!");
        if (PV.IsMine)
        {
            local_chestRotation = playerChestTr.rotation;
            local_UpperchestRotation = playerUpperChestTr.rotation;

            if (aimMode && playerChestTr && playerUpperChestTr)
            {
                // ī�޶� ���� �ִ� ����
                Vector3 chestDir = mainCameraTr.position + mainCameraTr.forward * 50f;


                playerChestTr.LookAt(chestDir); // ��ü�� ī�޶� ���� �������� ����
                playerChestTr.rotation = playerChestTr.rotation * Quaternion.Euler(chestOffset); // ��ü �����̼� ����

                local_chestRotation = playerChestTr.rotation;
                local_UpperchestRotation = playerUpperChestTr.rotation;

                //Debug.Log(local_chestRotation + "������ ���ϱ� ��?");
                //Debug.Log(playerChestTr.rotation + "���� ��ü����");
                //UpperchestOffset.LookAt(chestDir); // ��ü�� ī�޶� ���� �������� ����

                if (isQPressed && isQAnim == true)
                {
                    StartCoroutine(DelayedRotation());
                    playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));
                    local_UpperchestRotation = playerUpperChestTr.rotation;
                }
                else if (isQUp && isQAnim == true)
                {
                    //Debug.Log("0.1�� ������_back ������ ��?");
                    StartCoroutine(DelayedRotation_back());
                    playerUpperChestTr.rotation = playerUpperChestTr.rotation * (Quaternion.Euler(UpperchestOffset) * Quaternion.Euler(vector_x, vector_y, vector_z));
                    local_UpperchestRotation = playerUpperChestTr.rotation;
                }
                // ��ü�� ȸ���� ����
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
                    playerUpperChestTr.rotation = playerUpperChestTr.rotation * Quaternion.Euler(UpperchestOffset); // ��ü �����̼� ����
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
            // �� Ŭ���̾�Ʈ�� ��� ��ü ȸ�� ���� �����ϴ�.
            stream.SendNext(local_chestRotation);
            //Debug.Log(local_chestRotation + "�Ȱ��� ���´�??");
            stream.SendNext(local_UpperchestRotation);
            
            //Debug.Log(playerChestTr.rotation + "������");
            //stream.SendNext(playerUpperChestTr.rotation);
        }
        else
        {
            
            // �ٸ� Ŭ���̾�Ʈ�� ��� ��ü ȸ�� ���� �޾� �����մϴ�.
            chestRotation = (Quaternion)stream.ReceiveNext();
            upperChestRotation = (Quaternion)stream.ReceiveNext();
            //Debug.Log(playerChestTr.rotation + "�ޱ�");


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