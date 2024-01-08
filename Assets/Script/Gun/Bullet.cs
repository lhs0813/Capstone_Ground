using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 총알을 전방으로 발사합니다.
        rb.velocity = transform.forward * bulletSpeed;

        // 일정 시간 후에 총알을 제거합니다.
        Destroy(gameObject, 2f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            // 충돌한 오브젝트가 "Player" 태그를 가지고 있다면 삭제합니다.
            // 총알도 삭제
            Destroy(gameObject);
        }
    }
}