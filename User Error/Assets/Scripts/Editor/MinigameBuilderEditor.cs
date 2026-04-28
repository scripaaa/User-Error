using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MinigameBuilder))]
public class MinigameBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MinigameBuilder builder = (MinigameBuilder)target;
        
        GUILayout.Space(10);
        if (GUILayout.Button("BUILD HACKING MINIGAME", GUILayout.Height(40)))
        {
            builder.BuildMinigame();
        }
    }
}
