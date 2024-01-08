using UnityEngine;

public class ONandOF : MonoBehaviour
{
    public GameObject flashlight; // public���� ����� ����

    private bool isObjectActive = true; // �ʱ� ���´� Ȱ��ȭ�Ǿ� ����

    void Update()
    {
        // F Ű�� ������ ��
        if (Input.GetKeyDown(KeyCode.F))
        {
            // ���� ���¿� ���� �ݴ�� ����
            isObjectActive = !isObjectActive;

            // ������Ʈ�� Ȱ��ȭ ���� ����
            flashlight.SetActive(isObjectActive);
        }
    }
}