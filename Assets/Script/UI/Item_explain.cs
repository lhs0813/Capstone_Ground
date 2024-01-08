using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_explain : MonoBehaviour
{
    public RawImage rawImage; // ��⸦ ������ RawImage

    private Color normalColor; // �⺻ ����
    private Color highlightColor; // ���콺�� �÷��� ���� ����
    private bool isMouseOver; // ���콺�� UI ��� ���� �ִ��� ����

    void Start()
    {
        // �⺻ ���� ����
        normalColor = rawImage.color;
        // ���콺�� �÷��� ���� ���� (���)
        highlightColor = new Color(1f, 1f, 1f, 1f);
    }

    void Update()
    {
        // ���콺�� UI �̹��� ���� �÷��� ��
        if (IsMouseOverUIElement(rawImage))
        {
            if (!isMouseOver)
            {
                // ���� ȿ���� ���� ���� ����
                rawImage.color = highlightColor;
                isMouseOver = true;
            }
        }
        else
        {
            if (isMouseOver)
            {
                // ���콺�� UI �̹��� ������ �����ϸ� �⺻ �������� ����
                rawImage.color = normalColor;
                isMouseOver = false;
            }
        }
    }

    bool IsMouseOverUIElement(RawImage uiElement)
    {
        if (uiElement == null)
            return false;

        // ���콺 ��ġ�� ��ũ�� ��ǥ�� ��ȯ
        Vector2 mousePosition = Input.mousePosition;

        // RawImage�� ȭ�� ��ġ �� ũ�� ��������
        Vector3[] corners = new Vector3[4];
        uiElement.rectTransform.GetWorldCorners(corners);

        float minX = corners[0].x;
        float maxX = corners[2].x;
        float minY = corners[0].y;
        float maxY = corners[1].y;

        // ���콺 ��ġ�� RawImage ���� ���� �ִ��� Ȯ��
        return (mousePosition.x >= minX && mousePosition.x <= maxX && mousePosition.y >= minY && mousePosition.y <= maxY);
    }
}