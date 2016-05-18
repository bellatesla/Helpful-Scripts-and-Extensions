using UnityEngine;
using System.Collections;


[RequireComponent(typeof(BoxCollider))]
public class GeyserTrigger : MonoBehaviour
{
     
    

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Player Entered Geyser Trigger");
            //Tell controller we entered the transform.
            GeyserControlPooled.Instance.SquirtGeyser(transform);
           // SuitConditionControl.RemoveHealth(10);
           
           
        }
    }
    void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            //tell geyser controller we left
           // Debug.Log("Player Exited Geyser Trigger");
        }
    }
}
