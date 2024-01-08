using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentManager))]
public class EnvironmentManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EnvironmentManager environmentManager = (EnvironmentManager) target;
        
        if (GUILayout.Button("Randomize Planets"))
        {
            environmentManager.SetupRandomEnvironment();
        }
    }
}
