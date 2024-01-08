using UnityEngine;

public class camera_next : MonoBehaviour
{
    public GameObject cameraObject; // ī�޶� �����ϴ� ���� ������Ʈ�� �����ؾ� �մϴ�.
    private float disableTime = 18.00f;
    private float elapsedTime = 0f;
    private bool cameraDisabled = false;

    private void Update()
    {
        // ����� �ð��� �����մϴ�.
        elapsedTime += Time.deltaTime;

        // ���� �ð�(21.54��)�� ����ϰ� ���� ī�޶� ��Ȱ��ȭ���� �ʾҴٸ� ī�޶� ��Ȱ��ȭ�մϴ�.
        if (elapsedTime >= disableTime && !cameraDisabled)
        {
            cameraObject.SetActive(false);
            cameraDisabled = true; // ī�޶� �� ���� ��Ȱ��ȭ�ǵ��� �÷��׸� �����մϴ�.
        }
    }
}