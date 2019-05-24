using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformModifier : MonoBehaviour
{
    public Vector3 PositionAdjust;
    public Vector3 RotationAdjust;
    public bool CastAtPlayer;
    private Transform _player_transform;

    void Start()
    {
        _player_transform = FindObjectOfType<PlayerController>().transform;
        if (CastAtPlayer)
        {
            transform.position = _player_transform.position + PositionAdjust;
            transform.rotation = Quaternion.Euler(_player_transform.rotation.eulerAngles);
        }
        else //Should only execute else statement for Water Spray
        {
            transform.position = PositionAdjust;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + RotationAdjust);
        }
    }

    /*
    private void Update()
    {
        if (CastAtPlayer)
        {
            transform.position = _player_transform.position + PositionAdjust;
            transform.rotation = Quaternion.Euler(_player_transform.rotation.eulerAngles + RotationAdjust);
        }
        else //Should only execute else statement for Water Spray
        {
            transform.position = PositionAdjust;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + RotationAdjust);
        }
    }
    */
}
