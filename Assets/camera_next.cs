using UnityEngine;

public class camera_next : MonoBehaviour
{
    public GameObject cameraObject; // 카메라를 포함하는 게임 오브젝트를 설정해야 합니다.
    private float disableTime = 18.00f;
    private float elapsedTime = 0f;
    private bool cameraDisabled = false;

    private void Update()
    {
        // 경과한 시간을 누적합니다.
        elapsedTime += Time.deltaTime;

        // 일정 시간(21.54초)이 경과하고 아직 카메라가 비활성화되지 않았다면 카메라를 비활성화합니다.
        if (elapsedTime >= disableTime && !cameraDisabled)
        {
            cameraObject.SetActive(false);
            cameraDisabled = true; // 카메라가 한 번만 비활성화되도록 플래그를 설정합니다.
        }
    }
}