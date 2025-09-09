using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public enum MobType
{
    Follow,
    Circle,
    Distance,
}

public class Mob : MonoBehaviour
{
    [Header("Stats")]
    public HealthObject healthObject;

    [Header("Settings")] 
    public Rigidbody2D body;

    public Collider2D detectCollider;
    public MobType type;
    public float moveSpeed;
    [Header("Distances")]
    public float detectionRange;
    public float keepDistance;
    public float minMoveDistance;
    public float AttackDistance;
    public FollowObject target;
    public List<FollowObject> followables = new List<FollowObject>();

    public Vector3 moveDirection;
    public Vector3 moveTarget;
    public Vector3 moveVelocity;
    #region trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        var followObject = other.gameObject.GetComponent<FollowObject>();
        if (followObject == null) return;
        followables.Add(followObject);
        followables.OrderBy(x => x.priority);
        target = followables[0];
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (followables.Contains(other.gameObject.GetComponent<FollowObject>()))
        {
            followables.Remove(other.gameObject.GetComponent<FollowObject>());
            followables.OrderBy(x => x.priority);
            if (followables.Count >= 1)
            {
                target = followables[0];
            }
            else
            {
                target = null;
            }
        }
    }
    #endregion

    private void moveTowardsTarget()
    {
        moveDirection = Vector3.ClampMagnitude(target.transform.position - transform.position,moveSpeed);
        moveDirection.z = 0;
        body.linearVelocity = moveDirection;
    }
    
    private void moveToDistance()
    {
        var targetDir = -(target.transform.position - transform.position).normalized;
        moveTarget = (target.transform.position + targetDir * keepDistance);
        moveDirection = (moveTarget - transform.position).normalized;
        var distance = Vector3.Distance(transform.position, moveTarget);
        var percentage = math.clamp(distance/minMoveDistance, 0, 1);
        moveVelocity = moveDirection * moveSpeed * percentage;
        body.linearVelocity = moveVelocity;
    }

    private void updateMovement()
    {
        if (target == null)
        {
            body.linearVelocity = Vector3.zero;
            return;
        };
        switch (type)
        {
            case MobType.Follow:
                moveTowardsTarget();
                break;
            case MobType.Circle:
                moveToDistance();
                break;
            case MobType.Distance:
                moveToDistance();
                break;
        }
    }
    private void Update()
    {
        updateMovement();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, moveTarget);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection);
    }
}

public class Followable : MonoBehaviour
{
    public int priority;
}