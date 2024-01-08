using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_explain : MonoBehaviour
{
    public RawImage rawImage; // 밝기를 조절할 RawImage

    private Color normalColor; // 기본 색상
    private Color highlightColor; // 마우스를 올렸을 때의 색상
    private bool isMouseOver; // 마우스가 UI 요소 위에 있는지 여부

    void Start()
    {
        // 기본 색상 저장
        normalColor = rawImage.color;
        // 마우스를 올렸을 때의 색상 (흰색)
        highlightColor = new Color(1f, 1f, 1f, 1f);
    }

    void Update()
    {
        // 마우스를 UI 이미지 위에 올렸을 때
        if (IsMouseOverUIElement(rawImage))
        {
            if (!isMouseOver)
            {
                // 밝은 효과를 위해 색상 변경
                rawImage.color = highlightColor;
                isMouseOver = true;
            }
        }
        else
        {
            if (isMouseOver)
            {
                // 마우스를 UI 이미지 위에서 제거하면 기본 색상으로 변경
                rawImage.color = normalColor;
                isMouseOver = false;
            }
        }
    }

    bool IsMouseOverUIElement(RawImage uiElement)
    {
        if (uiElement == null)
            return false;

        // 마우스 위치를 스크린 좌표로 변환
        Vector2 mousePosition = Input.mousePosition;

        // RawImage의 화면 위치 및 크기 가져오기
        Vector3[] corners = new Vector3[4];
        uiElement.rectTransform.GetWorldCorners(corners);

        float minX = corners[0].x;
        float maxX = corners[2].x;
        float minY = corners[0].y;
        float maxY = corners[1].y;

        // 마우스 위치가 RawImage 영역 내에 있는지 확인
        return (mousePosition.x >= minX && mousePosition.x <= maxX && mousePosition.y >= minY && mousePosition.y <= maxY);
    }
}