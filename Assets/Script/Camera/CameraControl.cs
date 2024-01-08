using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraControl : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public Transform target;           // 카메라가 따라다닐 대상 (플레이어 캐릭터)
    public Vector3 defaultOffset = new Vector3(0.3f, 1.7f, 0.6f);   // 초기 카메라 위치
    private Vector3 currentOffset;     // 현재 카메라 위치
    public float sensitivity = 2f;    // 마우스 감도
    public float rotationSpeed = 2f;  // 카메라 회전 속도

    public GameObject fpscam;

    private bool Sit = true;


    private float rotationX = 0f;
    private bool isRightClicking = false;


    private bool isMousePressed = false;
    private float mousePressedTime = 0f;
    public float longClickThreshold = 0.5f; // 롱클릭으로 간주하는 시간(초)

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    // 마우스 커서를 화면 가운데로 고정
        currentOffset = defaultOffset;  // 초기 카메라 위치 설정


    }

    private void Update()
    {
        
        if (PV.IsMine)
        {
            // 마우스 입력을 통해 카메라 회전 처리
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -45f, 60f);   // 상하 각도 제한

            // 대상(플레이어)의 회전과 상관없이 카메라만 좌우 회전
            target.rotation = Quaternion.Euler(0f, target.eulerAngles.y + mouseX, 0f);

            // 카메라 상하 회전 처리
            transform.rotation = Quaternion.Euler(rotationX, target.eulerAngles.y, 0f);

            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("c가눌림");
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

                //fpscam.SetActive(true); // 짧게 눌렀을 때 만 카메라 전환
            }

            // 마우스 오른쪽 클릭 여부 확인
            if (Input.GetMouseButton(1))
            {
                isRightClicking = true; // 길게 누르면 견착

            }
            else if (Input.GetMouseButtonUp(1))
            {

                //



                // 일정 시간 동안 눌려지지 않았으면 클릭으로 처리
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

                    // 짧은 클릭 처리
                }
                else
                {
                    isRightClicking = false;
                    // 롱 클릭 처리
                }
            }

            // 마우스 오른쪽 클릭을 유지하는 동안 offset 값을 변경
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
                // 마우스 오른쪽 클릭이 해제된 경우 다시 초기 카메라 위치로 돌아감
                currentOffset = defaultOffset;
            }
        }

        if (!PV.IsMine)
        {
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }
        // 카메라 위치 업데이트
        Vector3 targetPosition = target.position - transform.forward * currentOffset.z + Vector3.up * currentOffset.y + transform.right * currentOffset.x;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSpeed);
    }

         
}