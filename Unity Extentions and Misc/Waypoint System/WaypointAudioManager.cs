
using UnityEngine;
using System.Collections;



[System.Serializable]
public class WaypointAudioClips
{
	public AudioClip openWaypointSound;
	public AudioClip closeWaypointSound;
	public AudioClip placeWaypointSound;
	public AudioClip scanningSound1;
	public AudioClip scanningSound2;
	public AudioClip scanningSound3;
	public AudioClip defaultButtonSound;
	public AudioClip positiveSound;
	public AudioClip negativeSound;
}

public class WaypointAudioManager : MonoBehaviour
{
	public WaypointAudioClips audioInfo;
	
	public static WaypointAudioManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			DestroyImmediate(gameObject);
			return;
		}
		Instance = this;
		//DontDestroyOnLoad(gameObject);
	}
	//Scanner

	public void PlayScanningSounds1()
	{
		if (!isScanning)
		{
			StartCoroutine("Scanning");
			audioSource.clip = RanScanSound();
			audioSource.Play();
		}
	}
	private bool isScanning;
    public float scanningTime = .5f;
    private IEnumerator Scanning()
	{
		isScanning = true;
		yield return new WaitForSeconds(scanningTime);
	   
		isScanning = false;
	}
	private AudioClip RanScanSound()
	{
		int i=(int)Random.Range(0, 2);
		if (i == 0) return audioSource.clip = audioInfo.scanningSound1;
		if (i == 1) return audioSource.clip = audioInfo.scanningSound2;
		if (i == 2) return audioSource.clip = audioInfo.scanningSound3;
		else return null;
	}



	private AudioSource audioSource;
	void Start ()
	{
		audioSource = GetComponent<AudioSource>();
	}
	public void PlaceWaypointSound()
	{
		//Plays one shot instead since very low calls
		//audioSource.PlayOneShot(audioInfo.placeWaypointSound);
        audioSource.clip = audioInfo.placeWaypointSound;
        audioSource.Play();
    }

	
	public void OpenSound()
	{
		audioSource.clip =audioInfo.openWaypointSound;
		audioSource.Play();
	}
	public void CloseSound()
	{
		audioSource.clip = audioInfo.closeWaypointSound;
		audioSource.Play();
	}
	public void DefaultSound()
	{
		//Added null check since getting error at start only 
		if (audioInfo.scanningSound1 == null) return;
		int num = (int) Random.Range(0, 2);//??weird had to call unity engine
		if (num == 0)  audioSource.clip = audioInfo.scanningSound1;
		if (num == 1) audioSource.clip = audioInfo.scanningSound2;
		if (num == 2) audioSource.clip = audioInfo.scanningSound3;
		audioSource.Play();
	}
	public void PositiveSound()
	{
		audioSource.clip = audioInfo.positiveSound;
		audioSource.Play();
	}
	public void NegativeSound()
	{
		audioSource.clip = audioInfo.negativeSound;
		audioSource.Play();
	}

}
