using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;
using UnityEngine.UI;

public class PlayMovie : MonoBehaviour {
    

    void Start()
    {
        ((MovieTexture)GetComponent<RawImage>().mainTexture).Play();//..Plays movie but no sound
        GetComponent<AudioSource>().Play();//adds the sound
    }

}
