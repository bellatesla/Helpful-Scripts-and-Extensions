using UnityEngine;
using System.Collections;

public class GeyserControlPooled : MonoBehaviour
{

    //Singleton
    public static GeyserControlPooled Instance { get; private set; }

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        else Instance = this;
    }
    
    //get water droplets
    public float fadeTime = 1;
    //no 1
    public ParticleSystem steam;
    public ParticleSystem.EmissionModule em;
    //no 2
    public ParticleSystem droplets;
    public ParticleSystem.EmissionModule em2;
    private AudioSource aud;

    void OnEnable()
    {
        aud = steam.GetComponent<AudioSource>();
        em = steam.emission;
        steam.Stop();

        em2 = droplets.emission;
        droplets.Stop();
       steam.gameObject.SetActive(false);
    }

    //Spawn the water droplets at start then disaable. 
    //when a geyserTrigger is activated move the water droplets(particles) into place and enable them for 5 sec.
    public void SquirtGeyser(Transform tx)
    {

        //Move the prefab here ^ then activate
        steam.transform.position = tx.position;
        steam.transform.rotation = tx.rotation;
        steam.gameObject.SetActive(true);
        //particles play
        steam.time = 0;
        em.enabled = true;
        steam.Play();

        droplets.time = 0;
        em2.enabled = true;
        droplets.Play();

        //audio play
        aud.loop = true;
        aud.volume = 1;
        aud.Play();
        StartCoroutine("ReduceAudioToMute", 5);

    }

    //Private methods
    private IEnumerator ReduceAudioToMute(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (aud.volume > 0)
        {
            aud.volume -= Time.deltaTime * fadeTime;
            yield return new WaitForEndOfFrame();
            if (aud.volume <= 0)
            {
                aud.volume = 0;

                steam.gameObject.SetActive(false);
                break;
            }
        }

    }
}
