using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ���ӽ����̽��� ����ϱ� ���� �߰�

public class UI_Script : MonoBehaviour
{
    
    public GameObject Player;
    public int bullets_count = 1;
    public TextMeshProUGUI Remainbullets;
    public TextMeshProUGUI Total_bullets;


    // Start is called before the first frame update
    void Start()
    {
        

        // �ʱ� �ؽ�Ʈ ���� (��: "���� �ؽ�Ʈ")
        
        

    }

    // Update is called once per frame
    void Update()
    {
        /*Total_bullets.text = Player.GetComponent<Gun_Script>().total_bullets.ToString();
        Remainbullets.text = Player.GetComponent<Gun_Script>().bullets.ToString();*/

    }
}
