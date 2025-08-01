using UnityEngine;
using System;
public class MobMovement : MonoBehaviour
{
    [SerializeField]
    private float MovementSpeed = 5f;

    [SerializeField]
    bool CanMove = true;
    bool MovementComplete = false;

    [SerializeField]
    Transform TargetPosition;

    /// <summary>
    /// A public event that you can subscribe to if you want the mob to do anything once they reach thier destination
    /// </summary>
    public event Action OnMovementFinish;

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            if (TargetPosition == null)
            {
                Debug.LogWarning("Warning! No Target Position has been set for this Enemy use SetTargetTransform() to set a Target");
                return;
            }
            MoveMobTick(TargetPosition);
        }   
    }

    /// <summary>
    /// Set the new target position and also update the transform Y postion if desired
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <param name="setNewYPostion"></param>
    public void SetTargetTransform(Transform targetTransform, bool setNewYPostion)
    {
        this.TargetPosition = targetTransform;
        if (setNewYPostion)
        {
            transform.position = new Vector3(transform.position.x, targetTransform.position.y, transform.position.z);
        }
    }

    /// <summary>
    /// Change the movmementspeed that the mob will move when moving
    /// </summary>
    /// <param name="value"></param>
    public void SetMobMovementSpeed(float value)
    {
        MovementSpeed = value;
    }

    /// <summary>
    /// Simple setter to determine if this mob can move right now or not
    /// </summary>
    /// <param name="value"></param>
    public void SetCanMove(bool value)
    {
        CanMove = value;
    }

    /// <summary>
    /// Moves the mob closer to the targetPosition 
    /// </summary>
    /// <param name="targetPosition"></param>
    public void MoveMobTick(Transform targetPosition)
    {
        if (MovementComplete) return;

        Vector3 direction = (targetPosition.position - transform.position).normalized;

        Vector3 newPosition = transform.position + (direction * MovementSpeed) * Time.deltaTime;

        transform.position = newPosition;

        if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            MovementComplete = true;
            OnMovementFinish?.Invoke();
            //Event Call that you should subscribe to if you want anything to happen when a MobCompletes their movement.
        }

    }


}
