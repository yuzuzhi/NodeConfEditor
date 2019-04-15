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
            Self = GetWindow<NodeEditorWindow>();
            Self.Show(true);
            var data = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/NodeEditor/sss.asset");
            if (data != null)
            {
                Self.nodeWnd = new NodeWindow(Self);
                Self.nodeWnd.target = data;
            }
        }

        void OnGUI()
        {
            BeginWindows();
            if (nodeWnd != null)
                nodeWnd.OnGUI();

            ProcessEvents(Event.current);
            EndWindows();
        }





        private void ProcessEvents(Event e)
        {

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        //ClearConnectionSelection();
                    }

                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        //OnDrag(e.delta);
                    }
                    break;
            }
        }
        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }
        private void OnClickAddNode(Vector2 mousePosition)
        {
            if (nodeWnd == null)
                nodeWnd = new NodeWindow(this);
            Self.nodeWnd.target = CreateInstance<cfeditor.StudentConfObject>();
            AssetDatabase.CreateAsset(Self.nodeWnd.target, "Assets/Editor/NodeEditor/sss.asset");
        }

        private NodeWindow nodeWnd;
        private static NodeEditorWindow Self;
    }
}