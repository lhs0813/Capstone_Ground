using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // �Ѿ��� �������� �߻��մϴ�.
        rb.velocity = transform.forward * bulletSpeed;

        // ���� �ð� �Ŀ� �Ѿ��� �����մϴ�.
        Destroy(gameObject, 2f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            // �浹�� ������Ʈ�� "Player" �±׸� ������ �ִٸ� �����մϴ�.
            // �Ѿ˵� ����
            Destroy(gameObject);
        }
    }
}