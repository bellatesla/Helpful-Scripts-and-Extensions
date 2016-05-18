using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class MathB 
{
    public enum Shapes
    {
        HalfCircle = 0,
        Circle = 1,
        
    }

    public static Vector3[] CircleOfPositions(float radius, int amount)
    {
        Vector3[] objectPositions = new Vector3[amount];
        for (int i = 0; i < amount; i++)
        {
            //Circle Shape
            float angle = i * Mathf.PI * 2f / amount;
            Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            objectPositions[i] = position;
        }
        return objectPositions;
    }

    public static List<GameObject> CircleOfGameObjects(GameObject pf,float radius, int amount,bool fullCircle)
    {
        List<GameObject> objects = new List<GameObject>(amount);
        for (int i = 0; i < amount; i++)
        {
            //Circle Shape
            float n = fullCircle ? 2 : 1;//2=full circle 1=half circle
            float angle = i * Mathf.PI * n / amount;
            Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            var obj = Object.Instantiate(pf, position, Quaternion.identity) as GameObject;
            objects.Add(obj);

        }
        return objects;
    }

    //public static List<GameObject> CircleOfGameObjects(GameObject pf, float radius, int amount, Shapes shape)
    //{
    //    List<GameObject> objects = new List<GameObject>(amount);
    //    Vector3 position=Vector3.zero;
    //    for (int i = 0; i < amount; i++)
    //    {
    //        float angle = 0;
    //        switch (shape)
    //        {
    //            case (Shapes.Circle):
    //                angle = i * Mathf.PI * 2 / amount;
    //                position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    //                break ;
    //            case (Shapes.HalfCircle):
    //                angle = i * Mathf.PI * 1 / amount;
    //                position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    //                break;
                
    //        }
            
            
    //        var obj = Object.Instantiate(pf, position, Quaternion.identity) as GameObject;
    //        objects.Add(obj);

    //    }
    //    return objects;
    //}


}
