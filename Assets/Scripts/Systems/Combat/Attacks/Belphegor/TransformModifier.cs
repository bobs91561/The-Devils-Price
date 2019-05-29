using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformModifier : MonoBehaviour
{
    public Vector3 PositionAdjust;
    public Vector3 RotationAdjust;
    public bool CastAtPlayer;

    public bool SummonRock;
    public bool Slimeball;
    private Transform _player_transform;
    private bool _once;
    private Vector3 _enemy_to_player;

    void Start()
    {
        _player_transform = FindObjectOfType<PlayerController>().transform;
        _once = true;
        _enemy_to_player = _player_transform.position - transform.position;
        if (Slimeball)
        {
            GetComponent<EffectSettings>().MoveVector = new Vector3(_enemy_to_player.normalized.x, 0, _enemy_to_player.normalized.z);
        }
    }

    private void Update()
    {
        
        while(_once) //rotation adjustment wasn't working in Start
        {
            if (CastAtPlayer)
            {
                transform.position = _player_transform.position + PositionAdjust;
                transform.rotation = Quaternion.Euler(_player_transform.rotation.eulerAngles + RotationAdjust);
            }
            else if (SummonRock)
            {
                transform.position = _player_transform.position;
                Vector3 adjustmentVector = -_enemy_to_player.normalized * PositionAdjust.x; ;
                transform.position += new Vector3(adjustmentVector.x, PositionAdjust.y, adjustmentVector.z);
                
                Vector3 playerToRock = (transform.position - _player_transform.position).normalized;
                transform.rotation = Quaternion.Euler(  -40,
                                                        Mathf.Rad2Deg * Mathf.Atan(playerToRock.x/playerToRock.z),
                                                        70);
                if (Mathf.Sign(playerToRock.z) < 0)
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0));
            }
            _once = false;
        }
    }
}
