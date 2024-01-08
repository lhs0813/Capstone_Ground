using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraControl : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public Transform target;           // ī�޶� ����ٴ� ��� (�÷��̾� ĳ����)
    public Vector3 defaultOffset = new Vector3(0.3f, 1.7f, 0.6f);   // �ʱ� ī�޶� ��ġ
    private Vector3 currentOffset;     // ���� ī�޶� ��ġ
    public float sensitivity = 2f;    // ���콺 ����
    public float rotationSpeed = 2f;  // ī�޶� ȸ�� �ӵ�

    public GameObject fpscam;

    private bool Sit = true;


    private float rotationX = 0f;
    private bool isRightClicking = false;


    private bool isMousePressed = false;
    private float mousePressedTime = 0f;
    public float longClickThreshold = 0.5f; // ��Ŭ������ �����ϴ� �ð�(��)

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    // ���콺 Ŀ���� ȭ�� ����� ����
        currentOffset = defaultOffset;  // �ʱ� ī�޶� ��ġ ����


    }

    private void Update()
    {
        
        if (PV.IsMine)
        {
            // ���콺 �Է��� ���� ī�޶� ȸ�� ó��
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -45f, 60f);   // ���� ���� ����

            // ���(�÷��̾�)�� ȸ���� ������� ī�޶� �¿� ȸ��
            target.rotation = Quaternion.Euler(0f, target.eulerAngles.y + mouseX, 0f);

            // ī�޶� ���� ȸ�� ó��
            transform.rotation = Quaternion.Euler(rotationX, target.eulerAngles.y, 0f);

            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("c������");
                Sit = !Sit;
            }

            if (Sit)
            {
                currentOffset = new Vector3(0.3f, 1.1f, 1.1f);
            }

            
            

            if (Input.GetMouseButtonDown(1))
            {
                isRightClicking = true;

                mousePressedTime = Time.time;

                //fpscam.SetActive(true); // ª�� ������ �� �� ī�޶� ��ȯ
            }

            // ���콺 ������ Ŭ�� ���� Ȯ��
            if (Input.GetMouseButton(1))
            {
                isRightClicking = true; // ��� ������ ����

            }
            else if (Input.GetMouseButtonUp(1))
            {

                //



                // ���� �ð� ���� �������� �ʾ����� Ŭ������ ó��
                if (Time.time - mousePressedTime <= longClickThreshold)
                {

                    if (isMousePressed == true)
                    {
                        isRightClicking = false;
                        fpscam.SetActive(false);
                        isMousePressed = false;
                    }
                    else if (isMousePressed == false)
                    {
                        isRightClicking = true;
                        fpscam.SetActive(true);
                        isMousePressed = true;
                    }

                    // ª�� Ŭ�� ó��
                }
                else
                {
                    isRightClicking = false;
                    // �� Ŭ�� ó��
                }
            }

            // ���콺 ������ Ŭ���� �����ϴ� ���� offset ���� ����
            if (isRightClicking)
            {
                if (Sit == true)
                {
                    currentOffset = new Vector3(0.4f, 1.1f, 0.7f);
                }

                else
                {
                    currentOffset = new Vector3(0.4f, 1.7f, 0.6f);
                }
            }
            else if(isRightClicking == false && Sit == false)
            {
                // ���콺 ������ Ŭ���� ������ ��� �ٽ� �ʱ� ī�޶� ��ġ�� ���ư�
                currentOffset = defaultOffset;
            }
        }

        if (!PV.IsMine)
        {
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }
        // ī�޶� ��ġ ������Ʈ
        Vector3 targetPosition = target.position - transform.forward * currentOffset.z + Vector3.up * currentOffset.y + transform.right * currentOffset.x;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSpeed);
    }

         
}