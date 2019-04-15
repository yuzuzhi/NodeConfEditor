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
            Self.nodeWnd = new List<Node>();
            var data = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/NodeEditor/sss.asset");
            if (data != null)
            {
                Self.nodeWnd.Add(new TableTypeNode(1,Self, data));
            }
        }

        void OnGUI()
        {
            BeginWindows();
            if (nodeWnd != null)
            {
                foreach (var tableTypeNode in Self.nodeWnd)
                {
                    tableTypeNode.OnGUI();
                }
            }

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
            genericMenu.AddItem(new GUIContent("Add list node"), false, () => OnClickAddListNode(mousePosition));
            genericMenu.ShowAsContext();
        }
        private void OnClickAddNode(Vector2 mousePosition)
        {
            var wnd = new TableTypeNode(1, this, CreateInstance<StudentConfObject>());
            wnd.center = mousePosition;
            nodeWnd.Add(wnd);
            AssetDatabase.CreateAsset(wnd.target, "Assets/Editor/NodeEditor/sss.asset");
        }

        private void OnClickAddListNode(Vector2 mousePosition)
        {
            var wnd = new BaseListTypeNode(2, this, new List<int>() {1,2});
            wnd.center = mousePosition;
            nodeWnd.Add(wnd);
        }
        

        private List<Node> nodeWnd;
        private static NodeEditorWindow Self;
    }
}