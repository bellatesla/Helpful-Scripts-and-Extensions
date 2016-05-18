using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// Move's a UI slider randomly for effect or test
/// </summary>
public class SmartSliderAutoMove : MonoBehaviour
{
	public float moveSpeed=10f;
	public float updateSpeed=1f;//in seconds
	public bool smoothMovement;
	

	private Slider slider;
	private float randomNum;

	void Start ()
	{
		slider = GetComponent<Slider>();
		StartCoroutine("PickRandomNumber");
	}
	
	
	void Update ()
	{
		
		
		//float absValue = Mathf.Abs(slider.value - randomNum);
		if(smoothMovement)slider.value = Mathf.Lerp(slider.value,randomNum , moveSpeed*.01f);
		else slider.value = randomNum;


	}
	private IEnumerator PickRandomNumber()
	{
		yield return new WaitForSeconds(updateSpeed);
		float maxV = slider.maxValue;
		float minV = slider.minValue;
		randomNum = Random.Range(minV, maxV);
		StartCoroutine("PickRandomNumber");
	}
}
