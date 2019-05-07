using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace cfeditor
{
    [CustomEditor(typeof(ConfScritableObject),true)]
    public class ConfScritableObjectEditor : Editor
    {
        bool btnOpen = false;
        
        public override void OnInspectorGUI()
        {

            var genericArgs = target.GetType().BaseType.GetGenericArguments();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ident"));
            if (genericArgs.Length != 0)
                EditorGUILayout.TextField("Type", genericArgs[0].Name);

            EditorGUI.EndDisabledGroup();

            btnOpen = GUILayout.Button("打开");

            ProceeEvent();


        }

        void ProceeEvent()
        {
            if(btnOpen)
            {
                NodeEditorWindow.Self.AddConfObject(target as ConfScritableObject, new Vector2(100, 100));
                NodeEditorWindow.Self.Repaint();
            }
        }

    }
}
