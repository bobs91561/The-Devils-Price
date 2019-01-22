using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FixedHealthBar : HealthBar
{
    private string _name;
    private TextMeshProUGUI _text;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _name = obj.name;
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.text = _name;
        _rect.anchoredPosition = new Vector2(0, 0);
        Deactivate();
    }
}
