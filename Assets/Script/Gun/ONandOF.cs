using UnityEngine;

public class ONandOF : MonoBehaviour
{
    public GameObject flashlight; // public으로 선언된 변수

    private bool isObjectActive = true; // 초기 상태는 활성화되어 있음

    void Update()
    {
        // F 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 현재 상태에 따라 반대로 변경
            isObjectActive = !isObjectActive;

            // 오브젝트의 활성화 상태 변경
            flashlight.SetActive(isObjectActive);
        }
    }
}