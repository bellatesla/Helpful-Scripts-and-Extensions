using System;
using UnityEngine;

/// <summary>
/// #1 Parent -Position Pick a target. 
/// Req #2-#4 child objects to work and child #1 Parent-pos>#2-rot>#3-pivot>#4-zoom>then a camera.
/// It will stick to the target like glue on all rotations with offsets. 
/// Mimics a child object position and rotation with out childing.
/// 
/// </summary>
public class BPosition : MonoBehaviour
{
    void OnEnable()
    {
        useTarget = false;
        tx = transform;
    }
    public enum UpdateTypes
    {
        Disabled,
        OnLateUpdate,
        OnUpdate,
        OnFixedUpdate,
        onUpdateAndOnFixed
    };

    public UpdateTypes updateType;
    public Rigidbody target; //ragdoll
    private Transform tx;//this

    private void LateUpdate()
    {
        if (updateType == UpdateTypes.OnLateUpdate) MoveToPosition();
    }
    private void Update()
    {
        if (updateType == UpdateTypes.OnUpdate) MoveToPosition();
        if (updateType == UpdateTypes.onUpdateAndOnFixed) MoveToPosition();
    }
    private void FixedUpdate()
    {
        if (updateType == UpdateTypes.OnFixedUpdate) MoveToPosition();
        if (updateType == UpdateTypes.onUpdateAndOnFixed) MoveToPosition();
    }
    public static bool useTarget;
    public float followSpeed = 5;

    private void MoveToPosition()
    {
        if (useTarget)
        {
            var pivot = GetComponentInChildren<BPivot>();
            pivot.cameraOffets=Vector3.zero;
            updateType=UpdateTypes.OnFixedUpdate;
            BMover.isMoving = false;
            tx.position = Vector3.Slerp(tx.position, target.position, Time.deltaTime*Time.deltaTime*followSpeed);
           
        }
        else
        {
            tx.position = BMover.rb.transform.position;
            tx.rotation = BMover.rb.transform.rotation;
        }
    }

}