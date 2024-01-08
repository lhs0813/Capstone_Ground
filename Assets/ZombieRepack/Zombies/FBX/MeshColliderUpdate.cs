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
        colliderMesh = new Mesh(); // 시작 시에 Mesh를 생성합니다.
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

        // 정점 유효성 검사
        Vector3[] vertices = colliderMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (!IsValidVertex(vertices[i]))
            {
                // 유효하지 않은 정점을 수정
                vertices[i] = Vector3.zero; // 또는 다른 유효한 값을 할당
            }
        }
        colliderMesh.vertices = vertices;

        collider.sharedMesh = colliderMesh; // 재할당만 수행합니다.
    }

    bool IsValidVertex(Vector3 vertex)
    {
        // 유효성 검사 로직을 여기에 추가
        // 예: NaN 또는 무한대 값을 검사하고 유효한지 확인
        return !float.IsNaN(vertex.x) && !float.IsNaN(vertex.y) && !float.IsNaN(vertex.z) &&
               !float.IsInfinity(vertex.x) && !float.IsInfinity(vertex.y) && !float.IsInfinity(vertex.z);
    }
}