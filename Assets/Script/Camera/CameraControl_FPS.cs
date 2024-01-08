using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl_FPS : MonoBehaviour
{
    public Transform target;           // 카메라가 따라다닐 대상 (플레이어 캐릭터)
    public Transform target_GUN;
    public Vector3 offset = new Vector3(-2f, 2f, -5f);   // 카메라와 대상 간의 상대적인 위치
    public float sensitivity = 2f;    // 마우스 감도
    public float rotationSpeed = 2f;  // 카메라 회전 속도

    private float rotationX = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    // 마우스 커서를 화면 가운데로 고정
    }

    private void Update()
    {
        // 마우스 입력을 통해 카메라 회전 처리
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -60f, 60f);   // 상하 각도 제한

        // 대상(플레이어)의 회전과 상관없이 카메라만 좌우 회전
        target.rotation = Quaternion.Euler(0f, target.eulerAngles.y + mouseX, 0f);

        // 카메라 상하 회전 처리
        transform.rotation = Quaternion.Euler(rotationX, target.eulerAngles.y, 0f);

        // 카메라 위치 업데이트
        Vector3 targetPosition = target_GUN.position - transform.forward * offset.z + Vector3.up * offset.y + transform.right * offset.x;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSpeed);


    }
}