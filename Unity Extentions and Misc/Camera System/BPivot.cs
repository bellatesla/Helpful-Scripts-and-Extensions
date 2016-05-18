using UnityEngine;
/// <summary>
///     #3 Child -Pivot Up and Down--updated on aug 8th
///     updated Aug 11 2015 -inserted BInput
///     updated-Aug 18 - Insert offset for local xz plane
///     April 2016-totally rewrote now its called by parent script
/// </summary>
public class BPivot : MonoBehaviour
{
    public static float CurrentPivotAngle { get; private set; }//for player Hud smart angle
    public static Transform PivotTx;//for other to get easily
    private float inputY;
    [Header("Current angle up or down. At a start value between min and max")]
    public float clampedOutput=45;//start at 45 degrees up
    private const float MouseInputSensitivityCalibration = 1f; //leave at 1 unless required

    [Header("Max and Min Degrees Adjust")]
    [Range(0f, 120f)] public float maxAngleUp = 90f;
    [Range(-120f, 0f)]public float minAngleDown = -90f;
    [Header("Camera Pivot Offset Adjust")]public Vector3 cameraOffets=new Vector3(0,1.6f,0);

    void Start()
    {
        PivotTx = transform;
    }
    public void UpdatePivot()
    {
        PivotTx.localPosition = cameraOffets;
        CurrentPivotAngle = clampedOutput;
        inputY = BInput.Instance.mouseY*MouseInputSensitivityCalibration;
        clampedOutput = Mathf.Clamp(clampedOutput += inputY, minAngleDown, maxAngleUp);
        PivotTx.localEulerAngles = new Vector3(clampedOutput,0,0);
    }
}