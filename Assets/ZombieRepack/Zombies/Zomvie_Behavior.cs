using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zomvie_Behavior : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        // 초기화 시 무작위 애니메이션을 재생합니다.
        PlayRandomAnimation();
    }

    private void PlayRandomAnimation()
    {
        // 1부터 6까지의 랜덤한 숫자를 생성합니다.
        int randomAnimationIndex = Random.Range(8, 9);

        // 파라미터에 랜덤한 숫자를 설정합니다.
        animator.SetInteger("IDLE_Random", randomAnimationIndex);

        
    }

    // 다른 이벤트 또는 동작에서 무작위 애니메이션을 재생하려면 이 함수를 호출합니다.
    public void PlayRandomAnimationOnClick()
    {
        PlayRandomAnimation();
    }
}