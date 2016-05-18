using System.Collections;
using UnityEngine;

/// <summary>
/// #4 Child -Zoom-updated Aug 8 2015 - added lerp to input and sliders to editor-added scroll is faster on distance
/// 
/// </summary>
public class BZoom : MonoBehaviour
{
    public enum UpdateTypes
    {
         OnUpdate,
        OnLateUpdate,
    };
    public UpdateTypes updateType;
    public AnimationCurve autoCamCurve;
    [Range(1f, 100f)]public float autoCamSpeed = 10f;
    [Range(1f, 100f)] public float mouseScrollSensitivityCalibration = 10f;
    [Range(1f, 100f)] public float mouseScrollSmooth = 10f;
    [Range(1f, 30f)] public float zoomInSpeed = 5f;
    public static float minDistance = .01f;
    public float maxZoomDistance = 25f;
    public float zoomPostion = 0.1f;
    public float wantedDistance = 10f;
    public float currentDistance = 5f;
    public static float TheCurrentDistance;
    public float oldDistance = 10f;
    public bool invertZoomDirection;
    private RaycastHit hit;
    //private RaycastHit oldHitInfo;
    //public float offset = .4f;
    public LayerMask layerMask;
    public float compensatedDistance;
    public float compOldDistance;
    public bool oneShot;
    //public float hitSpeed = 50f;
    //public float delayHitTime;
    public float delayDistanceTime;
    public bool isHitting; //public float hitSpeed = 10f;
    public float desiredDistance = 5f;
    private Transform bpivot;
    public float hitDistance;
    public static float ZoomDistance;
    public float sphereColRadius = .2f;
    public float hitOffset = 1;

    private void Start()
    {
        bpivot = GetComponentInParent<BPivot>().transform;
    }
    
    
    
    private void MoveToZoomingPosition()//During zooming
    {
       
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, -zoomPostion), zoomInSpeed*Time.deltaTime);
        ZoomDistance = transform.localPosition.z;
    }

   
    private void DesiredZoomDistance()
    {
        ZoomDistance = transform.localPosition.z;
       
        if (BInput.Instance.isZooming)
        {   //move to zoom position
            MoveToZoomingPosition();
            return;
        }
        
        var origin = bpivot.position; //A little red Diamond Gizmo
        var direction = -transform.forward;
        desiredDistance -= BInput.Instance.mouseZ * mouseScrollSensitivityCalibration;
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxZoomDistance);
        isHitting = Physics.SphereCast(origin,sphereColRadius, direction, out hit, desiredDistance, layerMask);
        hitDistance = hit.distance-hitOffset;// - sphereColRadius;
        if (!isHitting) hitDistance = desiredDistance;
        currentDistance = Mathf.Min(desiredDistance, hitDistance);
        var dist = transform.localPosition.z - currentDistance;
        dist = NormalizedMinMax(dist, 0, -5, 0, 1);
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, -currentDistance), autoCamSpeed * CurveSpeedByDistance(dist,1,0) * Time.deltaTime);
        TheCurrentDistance = currentDistance;
    }
    
    private void Update()
    {
        if ((int)updateType == 0)
        {
            DesiredZoomDistance();
        }
    }
    void LateUpdate()
    {
        if ((int)updateType == 1) 
        {
            DesiredZoomDistance();
        }
    }


    //private
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
    private float NormalizedMinMax(float currentValue, float max, float min, float a, float b)
    {
        var normalizedFloat = a + (currentValue - min) * (b - a) / (max - min);
        return normalizedFloat;
    }

    //retruns a value from 0 to 1;
    private float CurveSpeedByDistance(float angle, float max, float min)
    {
        var normAngle = (angle - min) / (max - min);
        var time = normAngle;//0-1 curve
        float pitchSpeed = autoCamCurve.Evaluate(time);
        return pitchSpeed;

    }
}