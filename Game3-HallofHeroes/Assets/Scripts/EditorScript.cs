using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(AbstractGenerator), true)]

public class EditorScript : Editor
{
   AbstractGenerator generator;

   private void Awake() {
         generator = (AbstractGenerator)target;
   }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate")) {
            generator.RunProceduralGen();
        }
        if (GUILayout.Button("Clear")) {
            generator.ClearFloorTiles();
        }
    }
}
#endif