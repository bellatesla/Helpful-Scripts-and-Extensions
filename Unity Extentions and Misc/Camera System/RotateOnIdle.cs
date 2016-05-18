using UnityEngine;

/// <summary>
/// Rotate the camera after no input from player
/// </summary>
public class RotateOnIdle : MonoBehaviour
{

	public float rotationSpeed=1;
	public float idleTime=10;
	public float idleCounter;
	
	
	void Update ()
	{
		if (!BMover.isMoving && BInput.Instance.mouseX < .1f && BInput.Instance.mouseX > -.1f)
		{
			idleCounter += Time.deltaTime;
			if (idleCounter > idleTime)
			{
				//Rotate
				transform.Rotate(0 , rotationSpeed * Time.deltaTime , 0, Space.Self);

			}
		}
		else idleCounter = 0;
	}
}
