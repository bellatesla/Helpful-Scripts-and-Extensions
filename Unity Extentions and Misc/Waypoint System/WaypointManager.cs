using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Placed on HUD Canvas and Manages childed waypoints. 
/// </summary>
public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    private Transform cameraTransform;
    public List<SingleWaypoint> waypoints; //Way point complete with postion and icon

    private RectTransform canvas; //Reference to Canvas
    

    //public float angleCorrection = -90f;
    [Range(.5f, .9f)] public float offscreenTextPostion = .9f;
    [Range(.5f, .9f)] public float onscreenTextPostion = .9f;
    [Range(0f, 1f)] public float onScreenBorderOffset = 0f; //1 = always on 0 = edge of screen
    public Vector3 offsetY;
    //manual assign
    public GameObject waypointPanel;
    public GameObject wpList;//the list wp panel
    public float minIconSize = .3f;
    //private const string ms = " m/s";
    private string d = "9999 m";
    private void Start()
    {
        WaypointMenuSetActive(false);
        cameraTransform = Camera.main.transform;
        canvas = GetComponent<RectTransform>();
       
    }


    void LateUpdate()
    {
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        
        if (BInput.Instance.journal)
        {
            WaypointMenuSetActive(!waypointPanel.activeSelf);//toggle active state
        }

        updateTextTimer += Time.deltaTime;
        if (updateTextTimer > updateTextTime)
        {
            updateTextTimer = 0;
            updateText = true;
        }
        else
        {
            updateText = false;
        }

        int wpc = waypoints.Count;
        for (int i = 0; i < wpc; i++)
        {
            IconUpdate(i);
        }
    }


    private float updateTextTimer;
    private float updateTextTime=.5f;
    private bool updateText;
    private bool isFacingTarget;
    private void IconUpdate(int i)
    {
        if (!waypoints[i].iconIsVisable) return; //Check if target is in front or behind us
       
       
         #region Icon

        var target = waypoints[i].transform.position;
        var distanceToTarget = Vector3.Distance(target, cameraTransform.position);

        if ((Vector3.Dot(cameraTransform.forward, target - cameraTransform.position) < 0f))
        {
            //waypoints[i].icon.enabled=false;
            SetAlpha(waypoints[i].icon,0);
            isFacingTarget = false;
        }
        else
        {
            //waypoints[i].icon.enabled = true;
            SetAlpha(waypoints[i].icon,1);//nicely done
            isFacingTarget = true;
        }
        //Refernces
       
        var arrowImage = waypoints[i].arrowIcon;//Good update

        //Calculate the 2D position where the icon should be drawn
        //var point = Camera.main.WorldToViewportPoint(target); //Return a value based on 0-1and -1 is outside canvas 0,0 is center 1,1 is top right corner
        var point = Camera.main.WorldToViewportPoint(target);
        //Reverse the vector if im looking away
        if (point.z <= 0) point *= -1f;

       
        //Scale Icon Size by distance
        float iconScale = 250f/distanceToTarget;
        iconScale = Mathf.Clamp(iconScale, minIconSize, 1f);
        Vector3 scale = new Vector3(iconScale, iconScale, 1);
        waypoints[i].icon.transform.localScale = scale;
        arrowImage.transform.localScale = scale;
        //Icon Position
        var iconScreenPosition = new Vector2(((point.x*canvas.sizeDelta.x) - (canvas.sizeDelta.x*0.5f)),((point.y*canvas.sizeDelta.y) - (canvas.sizeDelta.y*0.5f)));
        waypoints[i].icon.rectTransform.anchoredPosition = iconScreenPosition;

        //Debug.DrawLine(canvas.anchoredPosition3D, waypoints[i].icon.rectTransform.position, Color.white);//tada!!
        //Debug.DrawRay(canvas.anchoredPosition3D,canvas.up, Color.red);

        var distanceText = waypoints[i].distance;
        if(updateText)//Distance Text on and off screen
        {
            int dist = Mathf.RoundToInt(distanceToTarget);
            d = dist.ToString();
            distanceText.text = d;
        }

        #endregion Icon
        #region Arrow 
        //When Icon is out of view
        if (IconIsOnScreen(point.x, point.y) && isFacingTarget)//on Screen
        {
            SetAlpha(waypoints[i].icon, 1);
            //Text Postion when isOnScreen
            distanceText.rectTransform.anchoredPosition = iconScreenPosition; //Position
            distanceText.transform.localPosition += offsetY; //Move text down below Icon 
            //arrowImage.enabled = false;//
            SetAlpha(arrowImage, 0);
        }
        else//is off screen
        {
            SetAlpha(waypoints[i].icon, 0);
            //Rotate Arrow panel to Icon 
            //arrowImage.enabled = true;
            SetAlpha(arrowImage, 1);
            var dir = waypoints[i].icon.rectTransform.position - canvas.anchoredPosition3D; //Look at target
            var up = canvas.up;

            angleApart = Vector3.Angle(up, dir);
            if (Vector3.Dot(canvas.right, dir) < 0) angleApart = 360-angleApart;
            
            waypoints[i].arrowPanel.localEulerAngles = new Vector3(0, 0, -angleApart);
            //Put text at Arrow postion
            Vector3 textPosition = arrowImage.transform.position;
            distanceText.transform.position = textPosition;
            distanceText.transform.localPosition *= offscreenTextPostion; //A dynamic offset 
           
        }
        #endregion Arrow
    }
    public Color SetAlpha(Image image, float alpha)
    {
        Color col = image.color;
        col.a = alpha;
        image.color = col;
        return col;
    }

    public float angleApart;
   
    private bool IconIsOnScreen(float x, float y)
    {
        var offset = onScreenBorderOffset;

        if (x < 0 + offset || x > 1 - offset || y < 0 + offset || y > 1 - offset)
        {
            return false;
        }

        return true;
    }

    public void DeleteWaypoint(int i)
    {
        waypoints.RemoveAt(i);
        WaypointAudioManager.Instance.NegativeSound();//Audio open panel
    }

    public void CreateNewWaypoint(GameObject iconPf,GameObject menuPf)//called on start of new single waypoint
    {
        iconPf.transform.SetParent(Instance.transform,false);
        menuPf.transform.SetParent(Instance.wpList.transform,false);
       
    }

    public void ActivateCanvas(bool activeState)
    {
        GetComponent<GraphicRaycaster>().enabled = activeState;
        SupremeCommanderThor.Instance.UiEventSystemSetActive(activeState);
    }

    public void WaypointMenuSetActive(bool activeState)
    {
        waypointPanel.gameObject.SetActive(activeState);
      
       ActivateCanvas(activeState);
        BInput.Instance.KIllAllInput = activeState;
        WaypointAudioManager.Instance.OpenSound();//Audio open panel
    }

    public GameObject waypointPrefab;
    
   //called by scan sensor
    public void CreateNewWaypoint(Vector3 position)
    {
        Instantiate(waypointPrefab,position, Quaternion.identity); //Makes a waypoint todo pooler
        WaypointAudioManager.Instance.PlaceWaypointSound();
    }
}