using UnityEngine;

public class UI_System : MonoBehaviour
{
    public RectTransform uiTransform; // UI�� RectTransform ������Ʈ�� �������ּ���.
    public float moveSpeed = 5f; // UI �̵� �ӵ��� �����մϴ�.

    private float horizontalInput;

    private void Update()
    {
        // ���콺 �̵��� ���� horizontalInput ���� ������Ʈ�մϴ�.
        horizontalInput = Input.GetAxis("Mouse X");

        // UI�� ���� �Ǵ� ���������� �̵���ŵ�ϴ�.
        Vector3 uiPosition = uiTransform.position;
        uiPosition.x += horizontalInput * moveSpeed * Time.deltaTime;
        uiTransform.position = uiPosition;
    }
}