using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl_FPS : MonoBehaviour
{
    public Transform target;           // ī�޶� ����ٴ� ��� (�÷��̾� ĳ����)
    public Transform target_GUN;
    public Vector3 offset = new Vector3(-2f, 2f, -5f);   // ī�޶�� ��� ���� ������� ��ġ
    public float sensitivity = 2f;    // ���콺 ����
    public float rotationSpeed = 2f;  // ī�޶� ȸ�� �ӵ�

    private float rotationX = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    // ���콺 Ŀ���� ȭ�� ����� ����
    }

    private void Update()
    {
        // ���콺 �Է��� ���� ī�޶� ȸ�� ó��
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -60f, 60f);   // ���� ���� ����

        // ���(�÷��̾�)�� ȸ���� ������� ī�޶� �¿� ȸ��
        target.rotation = Quaternion.Euler(0f, target.eulerAngles.y + mouseX, 0f);

        // ī�޶� ���� ȸ�� ó��
        transform.rotation = Quaternion.Euler(rotationX, target.eulerAngles.y, 0f);

        // ī�޶� ��ġ ������Ʈ
        Vector3 targetPosition = target_GUN.position - transform.forward * offset.z + Vector3.up * offset.y + transform.right * offset.x;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSpeed);


    }
}