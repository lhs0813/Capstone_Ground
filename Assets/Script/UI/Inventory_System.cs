using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_System : MonoBehaviour
{
    public GameObject targetObject; // 활성화 및 비활성화할 대상 오브젝트
    public GameObject targetObject2; // 활성화 및 비활성화할 대상 오브젝트

    private bool cursorLocked = true; // 커서 잠금 상태를 나타내는 변수

    void Start()
    {
        LockOrUnlockCursor(); // 시작 시 커서를 잠금 상태로 설정
    }

    void Update()
    {
        // 키보드의 탭(Tab) 키를 누르면 오브젝트의 활성화 상태를 변경
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursorLocked = !cursorLocked;
            LockOrUnlockCursor();
            // 오브젝트가 활성화되어 있으면 비활성화하고, 비활성화되어 있으면 활성화합니다.
            targetObject.SetActive(!targetObject.activeSelf);
            targetObject2.SetActive(!targetObject2.activeSelf);
        }

        // ESC 키를 누르면 커서 잠금을 변경
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
            Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
            Cursor.visible = false; // 커서 숨김
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제
            Cursor.visible = true; // 커서 표시
        }
    }
}