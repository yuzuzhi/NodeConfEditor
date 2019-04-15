using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{
    public class NodeEditorWindow : EditorWindow
    {
        [MenuItem("Window/ssss")]
        static void ddd()
        {
            var w = GetWindow<NodeEditorWindow>();
            w.Show(true);
            w.nodeWnd = new NodeWindow(w);
            w.nodeWnd.target = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/NodeEditor/sss.asset");
        }

        void OnGUI()
        {
            BeginWindows();

            nodeWnd.OnGUI();
            EndWindows();
        }


        private NodeWindow nodeWnd;
    }
}