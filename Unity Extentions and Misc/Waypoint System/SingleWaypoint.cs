using System;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Has no update method only a start method to set its settings
/// this waypoint also stores any text data by the player during runtime
/// This auto adds self to the proper canvas as well.
/// Works but great need a refactor!
/// </summary>

public class SingleWaypoint : MonoBehaviour
{

    public Image icon; //dont fill
    public Image arrowIcon;
    public Text nameTitle;
    public Text distance;
    public RectTransform arrowPanel;
    public GameObject iconPrefab; //fill with prefab of a rectTransform and image only req.
    public GameObject listItemPrefab;
   
    public Text waypointName;
    public Text waypointDescription;
    public int listNumber;
    public GameObject iconObject, listItem;
    public Button del_button;
    public Toggle toggle;

    public GameObject descriptPanel;
    public bool iconIsVisable = true;
    //Colors
    public Slider alphaSlider;
    public Toggle colorToggle;
    public Button colwhite;
    public Button colblue;
    public Button colpink;
    public Button colred;
    public Color currentColor;
    public Color white = Color.white, blue = Color.cyan, pink = Color.magenta, red = Color.red;

    public float CurrentAlpha = .5f;

    //Happens only one time
    private void Start()
    {
        //Make Icon
        iconObject = Instantiate(iconPrefab);
        listItem = Instantiate(listItemPrefab);
        //Child icons/text and reset transform
        WaypointManager.Instance.CreateNewWaypoint(iconObject,listItem);
        //iconObject.transform.SetParent(GameObject.FindWithTag("HUD Canvas").transform, false);
        //listItem.transform.SetParent(GameObject.FindWithTag("WP").transform, false);
        var yPosition = WaypointManager.Instance.waypoints.Count*-50;
        listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,yPosition);

        nameTitle = listItem.GetComponent<Text>();
        nameTitle.text = "WAYPOINT " + DateTime.Now.ToShortTimeString();
        waypointName = listItem.GetComponent<Text>();
        //waypointDescription = listItem.GetComponentInChildren<Text>();
        //waypointDescription.enabled = false;//Hide text for now
        waypointName.text = "Waypoint " + listNumber;

        //Get Icon image
        icon = iconObject.transform.Find("Icon").GetComponent<Image>();

        //Get Text
        distance = iconObject.transform.Find("Distance Text").GetComponent<Text>();
        arrowPanel = iconObject.transform.Find("Arrow Panel").GetComponent<RectTransform>();
        arrowIcon = arrowPanel.GetComponentInChildren<Image>();
        //Button Ref
        del_button = listItem.GetComponentInChildren<Button>();
        del_button.onClick.AddListener(DestroySelf); //Method #1
        //Color Buttons Basic color palette
        colorToggle = listItem.transform.Find("Color Toggle").GetComponent<Toggle>();
        var colpanel = colorToggle.transform.Find("Color Panel").GetComponent<Image>();
        colwhite = colpanel.transform.Find("White Button").GetComponent<Button>();
        colwhite.onClick.AddListener(() => ChangeIconColor(white)); //Method #2
        colblue = colpanel.transform.Find("Blue Button").GetComponent<Button>();
        colblue.onClick.AddListener(() => ChangeIconColor(blue)); //Method #2
        colpink = colpanel.transform.Find("Pink Button").GetComponent<Button>();
        colpink.onClick.AddListener(() => ChangeIconColor(pink)); //Method #2
        colred = colpanel.transform.Find("Red Button").GetComponent<Button>();
        colred.onClick.AddListener(() => ChangeIconColor(red)); //Method #2

        //Hide Show toggle eye
        toggle = listItem.transform.Find("Hide Toggle").GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(HideIcons);
        //Alpha slider
        alphaSlider = colpanel.GetComponentInChildren<Slider>();
        alphaSlider.onValueChanged.AddListener(delegate { ChangeIconColor(currentColor); });//Change the current colors alpha
        //descriptPanel=listItem.GetComponentInChildren<Image>().gameObject;
        //Add to list
        //FindObjectOfType<WaypointManager>().waypoints.Add(this);
        WaypointManager.Instance.waypoints.Add(this);

        //Hide color panel menu at start
        currentColor=white;//Default start color = white
        currentColor.a = CurrentAlpha;
        colpanel.gameObject.SetActive(false);
    }
    public float MinAlphaPercent=.05f;//5% default min
    private void ChangeIconColor(Color col)
    {
        
        CurrentAlpha = alphaSlider.normalizedValue;
        CurrentAlpha = Mathf.Clamp(CurrentAlpha, MinAlphaPercent, 1f);
        col.a = CurrentAlpha;
        icon.color = col;
        arrowIcon.color = col;
        distance.color = col;
        currentColor = col;
        //WP menu title color
        nameTitle.color = col;
        WaypointAudioManager.Instance.DefaultSound();
    }
    private void HideIcons(bool b)
    {
        //Turn on/off 3 things on hud and stop list from drawing it too.
        icon.enabled = b;
        arrowPanel.gameObject.SetActive(b);
        distance.gameObject.SetActive(b);
        iconIsVisable = b; //for manager script
        if(b)WaypointAudioManager.Instance.PositiveSound();
        else WaypointAudioManager.Instance.NegativeSound();
    }

    private void DestroySelf()//From del_button listener
    {
        WaypointAudioManager.Instance.NegativeSound();
        del_button.onClick.RemoveAllListeners();
        WaypointManager.Instance.waypoints.Remove(this);
        GameObject.DestroyObject(iconObject);
        GameObject.DestroyObject(listItem);
        GameObject.DestroyObject(this, 1f);
    }

    //public void SetAlpha(float alpha)
    //{
    //    Color col = icon.color;
    //    col.a = alpha;
    //    icon.color = col;
    //}

   

}