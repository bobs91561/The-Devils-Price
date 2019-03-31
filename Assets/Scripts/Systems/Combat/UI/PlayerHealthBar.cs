using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : HealthBar
{
    private Canvas PlayerCanvas;

    public override void Initialize(GameObject obj)
    {
        _hm = obj.GetComponent<HealthManager>();
        _slider = GetComponent<Slider>();
        _slider.maxValue = _hm.maxHealth;
        _slider.minValue = 0f;
        _slider.value = _hm.Health;

        PlayerCanvas = GameManager.GetDialogueManager().GetComponentInChildren<Canvas>();
        parent = obj.transform;
        _rect = GetComponent<RectTransform>();
        _rect.SetParent(PlayerCanvas.GetComponent<RectTransform>());
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, 0);
        _rect.localScale = new Vector3(1f,1f,1f);
    }
}
