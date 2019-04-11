using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simplistic script to avoid using the ThirdPersonController from Unity's Standard Assets
/// </summary>
public class StickManController : MonoBehaviour
{
    public KeyCode Forward = KeyCode.UpArrow;
    public KeyCode Backward = KeyCode.DownArrow;
    public KeyCode Left = KeyCode.LeftArrow;
    public KeyCode Right = KeyCode.RightArrow;
    public KeyCode Slow = KeyCode.RightShift;

    public float TurnSpeed = 360.0f;
    public float MoveSpeed = 10.0f;

    public Rigidbody Physic;
    public Animator Animation;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Turning        
        if (Input.GetKey(this.Left) == true)
        {
            Vector3 Direction = this.transform.localEulerAngles;
            Direction.y -= this.TurnSpeed * Time.deltaTime;
            this.transform.localEulerAngles = Direction;
        }
        else if (Input.GetKey(this.Right) == true)
        {
            Vector3 Direction = this.transform.localEulerAngles;
            Direction.y += this.TurnSpeed * Time.deltaTime;
            this.transform.localEulerAngles = Direction;
        }

        //Speed
        float Speed = this.MoveSpeed;
        if (Input.GetKey(this.Slow) == true)
        {
            Speed = this.MoveSpeed / 3.0f;
        }

        //Moving (don't touch vertical velocity to let gravity work while moving)
        float VerticalVelocity = this.Physic.velocity.y;
        if (Input.GetKey(this.Forward) == true)
        {
            Vector3 Movement = this.transform.forward * Speed;
            Movement.y = VerticalVelocity;
            this.Physic.velocity = Movement;
            this.Animation.SetFloat("Speed", Speed);
        }
        else if (Input.GetKey(this.Backward) == true)
        {
            Vector3 Movement = -this.transform.forward * Speed;
            Movement.y = VerticalVelocity;
            this.Physic.velocity = Movement;
            this.Animation.SetFloat("Speed", -Speed);
        }
        else
        {
            Vector3 Movement = Vector3.zero;
            Movement.y = VerticalVelocity;
            this.Physic.velocity = Movement;
            this.Animation.SetFloat("Speed", 0);
        }
    }

}
