using UnityEngine;
using System.Collections;



public class LightFlicker : MonoBehaviour
{

    public float randomNumber;
    public bool freezePosition=true;
    private Light light;
    [Range(.001f,10)]	public float speed1 = 1;
    [Range(.001f, 10)]	public float speed2 = 1;
    [Range(1, 5)]		public float intensity = 2;
    [Range(.001f, 100)]	public float timeMultiplier = 1;

    private void OnEnable()
    {
        randomNumber = Random.value * 100;
        light = GetComponent<Light>();
    }


    private void Update()
    {
        light.intensity = intensity * Mathf.PerlinNoise(randomNumber + Time.time * timeMultiplier * speed1, randomNumber + Time.time * timeMultiplier * speed2);
        if (freezePosition) return;
        float x = Mathf.PerlinNoise(randomNumber + 0 + Time.time * 2, randomNumber + 1 + Time.time * 2) - 0.5f;
        float y = Mathf.PerlinNoise(randomNumber + 2 + Time.time * 2, randomNumber + 3 + Time.time * 2) - 0.5f;
        float z = Mathf.PerlinNoise(randomNumber + 4 + Time.time * 2, randomNumber + 5 + Time.time * 2) - 0.5f;
        transform.localPosition = Vector3.up + new Vector3(x, y, z) * 1;
    }
    
    
}
