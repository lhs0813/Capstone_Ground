using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_System : MonoBehaviour
{
    public GameObject targetObject; // Ȱ��ȭ �� ��Ȱ��ȭ�� ��� ������Ʈ
    public GameObject targetObject2; // Ȱ��ȭ �� ��Ȱ��ȭ�� ��� ������Ʈ

    private bool cursorLocked = true; // Ŀ�� ��� ���¸� ��Ÿ���� ����

    void Start()
    {
        LockOrUnlockCursor(); // ���� �� Ŀ���� ��� ���·� ����
    }

    void Update()
    {
        // Ű������ ��(Tab) Ű�� ������ ������Ʈ�� Ȱ��ȭ ���¸� ����
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursorLocked = !cursorLocked;
            LockOrUnlockCursor();
            // ������Ʈ�� Ȱ��ȭ�Ǿ� ������ ��Ȱ��ȭ�ϰ�, ��Ȱ��ȭ�Ǿ� ������ Ȱ��ȭ�մϴ�.
            targetObject.SetActive(!targetObject.activeSelf);
            targetObject2.SetActive(!targetObject2.activeSelf);
        }

        // ESC Ű�� ������ Ŀ�� ����� ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLocked = !cursorLocked;
            LockOrUnlockCursor();
        }
    }

    void LockOrUnlockCursor()
    {
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ���
            Cursor.visible = false; // Ŀ�� ����
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Ŀ�� ��� ����
            Cursor.visible = true; // Ŀ�� ǥ��
        }
    }
}