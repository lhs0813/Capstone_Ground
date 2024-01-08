using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderUpdate : MonoBehaviour
{
    SkinnedMeshRenderer meshRenderer;
    MeshCollider collider;
    Mesh colliderMesh;

    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        collider = GetComponent<MeshCollider>();
        colliderMesh = new Mesh(); // ���� �ÿ� Mesh�� �����մϴ�.
    }

    private float time = 0;
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 0.5f)
        {
            time = 0;
            UpdateCollider();
        }
    }

    public void UpdateCollider()
    {
        meshRenderer.BakeMesh(colliderMesh);

        // ���� ��ȿ�� �˻�
        Vector3[] vertices = colliderMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (!IsValidVertex(vertices[i]))
            {
                // ��ȿ���� ���� ������ ����
                vertices[i] = Vector3.zero; // �Ǵ� �ٸ� ��ȿ�� ���� �Ҵ�
            }
        }
        colliderMesh.vertices = vertices;

        collider.sharedMesh = colliderMesh; // ���Ҵ縸 �����մϴ�.
    }

    bool IsValidVertex(Vector3 vertex)
    {
        // ��ȿ�� �˻� ������ ���⿡ �߰�
        // ��: NaN �Ǵ� ���Ѵ� ���� �˻��ϰ� ��ȿ���� Ȯ��
        return !float.IsNaN(vertex.x) && !float.IsNaN(vertex.y) && !float.IsNaN(vertex.z) &&
               !float.IsInfinity(vertex.x) && !float.IsInfinity(vertex.y) && !float.IsInfinity(vertex.z);
    }
}