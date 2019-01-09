using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FloatingHealthBar : MonoBehaviour
{
    private static Camera cam;
    private static Canvas canvas;

    public Transform parent;
    private GameObject _sliderObj;
    private Slider _slider;
    private HealthManager _hm;
    private RectTransform _rect;

    public void Activate()
    {
        _slider.maxValue = _hm.maxHealth;
        _slider.minValue = 0f;
        _slider.value = _hm.Health;
        gameObject.SetActive(true);
        if (_slider.value <= 0f) Deactivate();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Initialize(GameObject obj)
    {
        if (cam == null) cam = Camera.main;
        if (canvas == null) canvas = GameObject.Find("EnemyHealthCanvas").GetComponent<Canvas>();
        parent = obj.transform;
        _hm = obj.GetComponent<HealthManager>();
        _slider = GetComponent<Slider>();
        _slider.maxValue = _hm.maxHealth;
        _slider.minValue = 0f;
        _slider.value = _hm.Health;
        _rect = GetComponent<RectTransform>();
        _rect.SetParent(canvas.GetComponent<RectTransform>());
        Deactivate();
    }

    void LateUpdate()
    {
        var rect_transform = _rect;

        rect_transform.anchoredPosition = WorldToCanvas(canvas, parent.position + new Vector3(0f, 2.25f));

    }

    public void UpdateHealth()
    {
        _slider.value = _hm.Health;
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
