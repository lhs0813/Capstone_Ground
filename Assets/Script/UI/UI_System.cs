using UnityEngine;

public class UI_System : MonoBehaviour
{
    public RectTransform uiTransform; // UI의 RectTransform 컴포넌트를 연결해주세요.
    public float moveSpeed = 5f; // UI 이동 속도를 설정합니다.

    private float horizontalInput;

    private void Update()
    {
        // 마우스 이동에 따라 horizontalInput 값을 업데이트합니다.
        horizontalInput = Input.GetAxis("Mouse X");

        // UI를 왼쪽 또는 오른쪽으로 이동시킵니다.
        Vector3 uiPosition = uiTransform.position;
        uiPosition.x += horizontalInput * moveSpeed * Time.deltaTime;
        uiTransform.position = uiPosition;
    }
}