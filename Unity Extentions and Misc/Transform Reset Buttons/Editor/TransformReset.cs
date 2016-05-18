using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformReset : Editor {

    bool foldout = false;

    public static Vector3 resetPosition = Vector3.zero;
    public static Vector3 resetRotation = Vector3.zero;
    public static Vector3 resetScale = Vector3.one;

    public override void OnInspectorGUI()
    {
        /*get the transform of the target gameobject*/
        Transform data = target as Transform;

        /*make the transform inspector looks like the default inspector*/
        //EditorGUIUtility.LookLikeControls();//obsolete
        //DrawDefaultInspector();//Show even more like local coords
        EditorGUI.indentLevel = 0;
        
        Vector3 position = EditorGUILayout.Vector3Field("Position", data.localPosition);
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", data.localEulerAngles);
        Vector3 scale = EditorGUILayout.Vector3Field("Scale", data.localScale);


#region Buttons
        /*add the reset position, rotation and scale buttons*/
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;
        GUILayout.Label("Reset");
        GUI.color = Color.cyan;
        if (GUILayout.Button("Position", EditorStyles.miniButtonLeft))
        {
            position = resetPosition;
        }
        if(GUILayout.Button("Rotation", EditorStyles.miniButtonMid)){
            eulerAngles = resetRotation;
        }
        if(GUILayout.Button("Scale", EditorStyles.miniButtonRight)){
            scale = resetScale;
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
#endregion


        /*puts option to set the vectors for the reset position, rotation and scale*/
        foldout = EditorGUILayout.Foldout(foldout, "Set Custom Origin");
        if (foldout)
        {
            if (GUILayout.Button("Reset Custom Origin", EditorStyles.miniButton))
            {
                resetPosition = Vector3.zero;
                resetRotation = Vector3.zero;
                resetScale = Vector3.one;
            }
            resetPosition = EditorGUILayout.Vector3Field("Custom Position", resetPosition);
            resetRotation = EditorGUILayout.Vector3Field("Custom Rotation", resetRotation);
            resetScale    = EditorGUILayout.Vector3Field("Custom Scale", resetScale);
        }

        /*apply the modifications to the targets transform*/
        if (GUI.changed)
        {
            Undo.RecordObject(data, "Transform Change");
            data.localPosition = FormatVector(position);
            data.localEulerAngles = FormatVector(eulerAngles);
            data.localScale = FormatVector(scale);
        }

    }

    /*prevent the input of non numbers values*/
    private Vector3 FormatVector(Vector3 vector)
    {
        if(float.IsNaN(vector.x))
        {
            vector.x = 0;
        }
        if(float.IsNaN(vector.y))
        {
            vector.y = 0;
        }
        if(float.IsNaN(vector.z))
        {
            vector.z = 0;
        }

        return vector;
    }
    
}


