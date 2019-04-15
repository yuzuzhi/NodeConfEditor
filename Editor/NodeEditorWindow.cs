using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{
    struct CurveDraw
    {
        public Vector3 start;
        public Vector3 end;
    }
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
                foreach (var tableTypeNode in nodeWnd)
                {
                    if (tableTypeNode.visiable)
                        tableTypeNode.OnGUI();
                }
            }

            foreach (var curveDraw in m_curveDraw)
                DrawNodeCurve(curveDraw.start, curveDraw.end);
            m_curveDraw.Clear();


            ProcessEvents(Event.current);
            EndWindows();
        }


        void DrawNodeCurve(Vector3 startPos, Vector3 endPos)
        {
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);
            for (int i = 0; i < 3; i++) // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            //Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
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

        public void DrawCurve(Vector3 start, Vector3 end)
        {
            CurveDraw draw = new CurveDraw();
            draw.start = start;
            draw.end = end;
            m_curveDraw.Add(draw);
        }
        

        public List<Node> nodeWnd;
        private static NodeEditorWindow Self;

        List<CurveDraw> m_curveDraw = new List<CurveDraw>();

    }
}