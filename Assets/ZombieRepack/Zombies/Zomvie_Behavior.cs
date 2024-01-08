using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zomvie_Behavior : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        // �ʱ�ȭ �� ������ �ִϸ��̼��� ����մϴ�.
        PlayRandomAnimation();
    }

    private void PlayRandomAnimation()
    {
        // 1���� 6������ ������ ���ڸ� �����մϴ�.
        int randomAnimationIndex = Random.Range(8, 9);

        // �Ķ���Ϳ� ������ ���ڸ� �����մϴ�.
        animator.SetInteger("IDLE_Random", randomAnimationIndex);

        
    }

    // �ٸ� �̺�Ʈ �Ǵ� ���ۿ��� ������ �ִϸ��̼��� ����Ϸ��� �� �Լ��� ȣ���մϴ�.
    public void PlayRandomAnimationOnClick()
    {
        PlayRandomAnimation();
    }
}