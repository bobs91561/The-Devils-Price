using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBar : MonoBehaviour
{
    public Transform parent;

    protected static Canvas canvas;

    protected RectTransform _rect;
    protected GameObject _sliderObj;
    protected Slider _slider;
    protected HealthManager _hm;


    private void OnDestroy()
    {
        EventManager.DeathAction -= Deactivate;
    }

    public void OnEnable()
    {
        //if (!parent) Destroy(gameObject);
    }

    public void Activate()
    {
        _slider.maxValue = _hm.maxHealth;
        _slider.minValue = 0f;
        _slider.value = _hm.Health;
        gameObject.SetActive(true);
        if (_slider.value <= 0f)
            Deactivate();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        if (_slider.value <= 0f) Destroy(gameObject);
    }

    public void UpdateHealth()
    {
        _slider.value = _hm.Health;
    }

    public virtual void Initialize(GameObject obj)
    {
        _hm = obj.GetComponent<HealthManager>();
        _slider = GetComponent<Slider>();
        _slider.maxValue = _hm.maxHealth;
        _slider.minValue = 0f;
        _slider.value = _hm.Health;


        if (canvas == null) canvas = GameObject.Find("EnemyHealthCanvas").GetComponent<Canvas>();
        parent = obj.transform;
        _rect = GetComponent<RectTransform>();
        _rect.SetParent(canvas.GetComponent<RectTransform>());
        _rect.localScale = new Vector3(1f, 1f, 1f);

        EventManager.DeathAction += Deactivate;
    }
}
