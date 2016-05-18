using UnityEngine;


/// <summary>
/// #2 Child -Rotation Around Target Local Up.-updated aug 08/2015- removed time.deltaTime
/// updated Aug 11 2015 -inserted BInput 
/// </summary>
public class BRotation : MonoBehaviour
{
    public enum UpdateTypes
    {
        Disabled,
        OnLateUpdate,
        OnUpdate,
        OnFixedUpdate,

    };


    public AnimationCurve rotateForwardCurve;
    public AnimationCurve rotatePitchCurve;
    public UpdateTypes updateType;
   
    private float rotation; //mouse input ref

    public float alignForwardSpeed = 20;
    public float PitchForwardSpeed = 20;

    private const float MouseSensitivity = 1;

    private BPivot bpivot;
    private Transform tx;//this
    private Transform parent;//this parent

    private void OnEnable()
    {
        tx = transform;
        parent = tx.parent;
        bpivot = GetComponentInChildren<BPivot>();
    }
    
    private void RotateStart()
    {
        bool canPitch = !BMover.isGrounded && (BPlayerGravity.currentGravitySource == null || BPlayerGravity.currentGravitySource.isStructure);

        if (BInput.Instance.freelookMode)
        {
            //align when moving only
            if (BMover.isMoving)
            {
                RotateForward();
                if (canPitch) PitchForward();

            }
        }
        else
        {
            //always align
            RotateForward();
            if (canPitch) PitchForward();
        }

        
    }

    private void PlayerRotate()
    {
        tx.Rotate(0, rotation, 0, Space.Self);//rotate self -mouse x
       
        RotateStart();//rotate/pitch self and player 

        bpivot.UpdatePivot();//pivot self -mouseY
    }

    //only fixed works properly ??
    private void FixedUpdate()
    {
        rotation = BInput.Instance.mouseX * MouseSensitivity;
        if ((int)updateType == 3)PlayerRotate();
    }
    private void Update()
    {
        if ((int) updateType == 2)PlayerRotate();
    }
    private void LateUpdate()
    {
        if ((int) updateType == 1)PlayerRotate();
    }
    
    /// <summary>
    /// Aligns the player up/down. when flying 
    /// </summary>
    private void PitchForward()
    {
        float currentAngle = bpivot.clampedOutput;//-90 to 90
        float absCurrentAngle = Mathf.Abs(currentAngle);//0 to 90 for curve
        float pitchSpeedFromCurve = GetPitchSpeedFromCurvePostion(absCurrentAngle, 90, 0);//0-1 from curve
        currentAngle = NormalizedMinMax(currentAngle, 90, -90, -1, 1);//-1 to 1
        float pitchSpeed = currentAngle * pitchSpeedFromCurve  *  PitchForwardSpeed * Time.deltaTime * 100;

        Quaternion deltaRotation = Quaternion.Euler(pitchSpeed, 0, 0); //++
        BMover.rb.MoveRotation(BMover.rb.rotation*deltaRotation);
        bpivot.clampedOutput -= pitchSpeed;//--
    }

    
    /// <summary>
    /// Aligns the player left/right. To face the camera forward
    /// </summary>
    private void RotateForward()
    {
        float angleAway = AngleToTargetForward();//+/- 180
        float absAngleAway = Mathf.Abs(angleAway);// 0-180 -because i want the same curve when neg or pos.
        float speedFromCurve = GetRotationSpeedFromCurvePostion(absAngleAway,0,180) ;//0-1 normalized
        angleAway = NormalizedMinMax(angleAway,-180,180,1,-1);//-1 to 1 value
        float yRotation = angleAway * speedFromCurve * alignForwardSpeed * Time.deltaTime* 100;

        tx.Rotate(0, -yRotation, 0, Space.Self);//--
        //rotate rb
        Quaternion deltaRotation = Quaternion.Euler(0,yRotation, 0); //++    Correct way for physics rotation
        BMover.rb.MoveRotation(BMover.rb.rotation*deltaRotation); 
        
    }

    /// <summary>
    /// This code returns the angle in degress between two vectors with neg or positve values if left or right of target.forward
    /// </summary>
    /// <returns></returns>
    private float AngleToTargetForward()
    {
        var tarFwd = parent.forward;//alternate
        //get angle to forward
        var a = Vector3.Angle(tx.forward, tarFwd);//degrees
        var dotAngle = Vector3.Dot(tx.right, tarFwd);
        if (dotAngle > 0) a *= -1;//Turn angle into +/-
        return a;
    }
    //Very good Lego
    /// <summary>
    /// Example: var angleAway = NormalizedMinMax(angleAway,-180,180,1,-1); returns a value between 1 and -1 from currentValue between -180 and 180.
    /// </summary>
    /// <param name="currentValue">Current value.</param>
    /// <param name="max">Maximum value.</param>
    /// <param name="min">Minimum value.</param>
    /// <param name="a">Minimum normalized value.</param>
    /// <param name="b">Maximum normalized value.</param>
    /// <returns></returns>
    private float NormalizedMinMax(float currentValue,float max,float min,float a, float b)
    {
        var normalizedFloat = a + (currentValue - min)*(b-a) / (max - min);
        return normalizedFloat;
    }

    //retruns a value from 0 to 1;
    private float GetPitchSpeedFromCurvePostion(float angle, float max, float min)
    {
        var normAngle = (angle - min) / (max - min);
        var time = normAngle;//0-1 curve
        float pitchSpeed = rotatePitchCurve.Evaluate(time);
        return pitchSpeed;

    }
    private float GetRotationSpeedFromCurvePostion(float angle,float max,float min)
    {
        var normAngle = (angle - min) / (max - min);
        var time = normAngle;//0-1 curve
        float rotationSpeed = rotateForwardCurve.Evaluate(time);
        return rotationSpeed;

    }

}