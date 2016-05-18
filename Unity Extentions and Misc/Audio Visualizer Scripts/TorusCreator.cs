using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class TorusCreator : MonoBehaviour
{
    public static float Radius;
    public float radiusInner = 10f;
    public float radiusOuter = 5f;
    //Dynamics for UI
    public float raduisA { get { return radiusInner; } set { radiusInner = value; } }
    public float raduisB { get { return radiusOuter; } set { radiusOuter = value; } }


    public int ObjectAmount = 4;
    public int ObjectChildren = 4;
    //Dynamics for UI
    public float objectsA { get { return ObjectAmount; } set { ObjectAmount = (int)value; } }
    public float objectsB { get { return ObjectChildren; } set { ObjectChildren = (int)value; } }

    public int TotalObjectCount;//Current object amount for inspector

    [Range(-360f,360f)]
    public float zDegrees = 90f;
    //Dynamics for UI
    public float zAxisDegrees { get { return zDegrees; } set { zDegrees = value; } }

    public Vector3 rotateChildSpeed;//Inspector Control
    public static Vector3 RotateChildrenSpeed;
    //Dynamics for UI
    public float speedX { get { return rotateChildSpeed.x; } set { rotateChildSpeed.x = value; } }
    public float speedY { get { return rotateChildSpeed.y; } set { rotateChildSpeed.y = value; } }
    public float speedZ { get { return rotateChildSpeed.z; } set { rotateChildSpeed.z = value; } }

    public Color startColor = Color.yellow;
    public Color endColor = Color.blue;
    public Material material;
    int time = 0;
    private bool countingDown;
    //private bool hideCenterObjects;
    //public bool scaleY;
    public static bool isBuildingTorus;
    public GameObject prefabCenter;//This is the center and can/should be invisible
    public GameObject prefabOuter;//This is the visual object
    public List<GameObject> cubes;
    //Stored refs
    private float radA, radB, objA, objB;

   

  

    private void Update()
    {
        //Rebuild on change if in torus mode
        //if ((int)AudioSpectrumAnalizer.Shape != 4) { return; }
        if(isBuildingTorus)return;
        if (cubes.Count == 0) return;
        if (radiusInner != radA) { radA = radiusInner; StartTorus(); }
        if (radiusOuter != radB) { radB = radiusOuter; StartTorus(); }
        if (ObjectAmount != objA) { objA = ObjectAmount; StartTorus(); }
        if (ObjectChildren != objB) { objB = ObjectChildren; StartTorus(); }

        //Update the static var
        if (RotateChildrenSpeed != rotateChildSpeed) RotateChildrenSpeed = rotateChildSpeed;

        //Color lerp a material
        if (time < 1000 && !countingDown)
        {
            
            material.SetColor("_TintColor", Color.Lerp(startColor, endColor, time * .001f));
            time++;
        }
        else
        {
            countingDown = true;
            material.SetColor("_TintColor", Color.Lerp(startColor, endColor, time * .001f));            
            time--;
            if (time < 1) { countingDown = false; }
        }
      
    }

   
    public void StartTorus()
    {
       
        if (isBuildingTorus ) return;
        isBuildingTorus = true;
        //Radius = radiusInner;
        StartCoroutine("DestroyTorus");
       
    }

    private IEnumerator MakeTorus()
    {
        //Make Torus
        for (var i = 0; i < ObjectAmount; i++)
        {
            //Circle 1
            var angle = i*Mathf.PI*2f/ObjectAmount;
            var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle))*radiusInner;
            //Instantiate
            var block = (GameObject) Instantiate(prefabCenter, pos, transform.localRotation); //when i=0 pos 1,0,0
            block.transform.SetParent(transform, false); //Child all objects
            block.transform.LookAt(transform); //Look Away
            var x = block.transform.localEulerAngles.x;
            var y = block.transform.localEulerAngles.y;
            //Children prefab Circle 2
            for (var ii = 0; ii < ObjectChildren; ii++)
            {
                var angle2 = ii*Mathf.PI*2f/ ObjectChildren;
                var pos2 = new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2))*radiusOuter; //smaller radius
               
                //Instantiate
                //pos2 = block.transform.localPosition;
                var block2 = (GameObject) Instantiate(prefabOuter, pos2, block.transform.localRotation); //when i=0 pos 1,0,0
                block2.transform.SetParent(block.transform, false); //Child all objects or maybe even each block
                block2.transform.LookAt(block.transform);
            }
            //Final rotate block(stands blocks2's up)
            block.transform.localEulerAngles = new Vector3(x, y, zDegrees);
            
            //Make center invisible     //Use an empty game Object      
            //block.GetComponent<MeshRenderer>().enabled = false;
            cubes.Add(block);//Only need to add parent to list.
            yield return new WaitForSeconds(.1f);
        }
        isBuildingTorus = false;
    }
    IEnumerator DestroyTorus()
    {
        if(cubes.Count!=0)
        foreach (GameObject go in cubes) { Destroy(go); }
        cubes.Clear();
        yield return new WaitForSeconds(1f);
        //Make a torus if in torus mode 
        //if ((int)AudioSpectrumAnalizer.Shape == 4)
         StartCoroutine("MakeTorus");
    }

    public void HideTorus()
    {
        isBuildingTorus = true;
        if (cubes.Count != 0)foreach (GameObject go in cubes) { Destroy(go); }
        cubes.Clear();//wont run update when cubes is 0
        isBuildingTorus = false;
    }

    public void ResetTorus()
    {
        radA = 2;
        radB = 2;

        objectsA = 8;
        objectsB = 8;

        zAxisDegrees = 90;

        speedX = 0;
        speedY = 0;
        speedZ = 0;

        StartTorus();
    }
}