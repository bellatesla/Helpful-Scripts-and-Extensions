using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// Attach script to UI Image to make the Alpha flicker and glow. Added multiple times for different effects.
/// Added different modes 
/// </summary>
public class SmartBlinkImage : MonoBehaviour
{
    public enum Mode
    {
        Basic,
        Dual
    };
    public Mode mode;

    private Image image;
    public float OnTime = 1;
    public float OffTime = 1f;
    //public bool timeVariation;
    //public Color WarningColor = Color.red;
    public Color NormalColor= Color.green;
    private Color workingColor;
    public float DualOnTime = 5f;
    public float DualOffTime = 10f;
    private void Start()
    {
        image = GetComponent<Image>();
        NormalColor = image.color; //Get the color at start
       
        if (mode == Mode.Basic)StartCoroutine("BasicBlinkOff"); //Start off
        
        if (mode == Mode.Dual) StartCoroutine("DualBlinkOff");
    }
    private IEnumerator DualBlinkOff()
    {
        NormalColor.a = 0; //Start forces off
        image.color = NormalColor;
        yield return new WaitForSeconds(DualOffTime);
        StartCoroutine("BasicBlinkOff");
        StartCoroutine("DualBlinkOn");
    }
    private IEnumerator DualBlinkOn()
    {
        yield return new WaitForSeconds(DualOnTime);

        //End of OnTime stoping other coroutines and start over
        StopCoroutine("BasicBlinkOff");
        StopCoroutine("TurnOn");
        StartCoroutine("DualBlinkOff");
    }

   
    private IEnumerator BasicBlinkOff()
    {
       

        yield return new WaitForSeconds(OffTime);
        image.enabled = true;
        StartCoroutine("TurnOn");
    }
    private IEnumerator TurnOn()
    {
        
        yield return new WaitForSeconds(OnTime);
        image.enabled = false;
        StartCoroutine("BasicBlinkOff");
    }

}