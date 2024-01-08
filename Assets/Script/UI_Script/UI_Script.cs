using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스를 사용하기 위해 추가

public class UI_Script : MonoBehaviour
{
    
    public GameObject Player;
    public int bullets_count = 1;
    public TextMeshProUGUI Remainbullets;
    public TextMeshProUGUI Total_bullets;


    // Start is called before the first frame update
    void Start()
    {
        

        // 초기 텍스트 설정 (예: "시작 텍스트")
        
        

    }

    // Update is called once per frame
    void Update()
    {
        /*Total_bullets.text = Player.GetComponent<Gun_Script>().total_bullets.ToString();
        Remainbullets.text = Player.GetComponent<Gun_Script>().bullets.ToString();*/

    }
}
