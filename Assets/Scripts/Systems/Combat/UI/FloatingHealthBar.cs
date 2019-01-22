using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FloatingHealthBar : HealthBar
{
    private static Camera cam;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        if (cam == null) cam = Camera.main;
        _rect.localScale = new Vector3(0.25f, 0.5f, 1f);
        Deactivate();
    }

    void LateUpdate()
    {
        var rect_transform = _rect;

        rect_transform.anchoredPosition = WorldToCanvas(canvas, parent.position + new Vector3(0f, 2.25f));
        if (!parent.GetComponent<SkillSet>().combat) gameObject.SetActive(false);

    }

    public static Vector2 WorldToCanvas(Canvas canvas,
                                        Vector3 world_position,
                                        Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        var viewport_position = camera.WorldToViewportPoint(world_position);
        var canvas_rect = canvas.GetComponent<RectTransform>();

        return new Vector2((viewport_position.x * canvas_rect.sizeDelta.x) - (canvas_rect.sizeDelta.x * 0.5f),
                           (viewport_position.y * canvas_rect.sizeDelta.y) - (canvas_rect.sizeDelta.y * 0.5f));
    }


}
