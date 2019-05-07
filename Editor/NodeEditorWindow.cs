using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{
    public struct CurveDraw
    {

        public Vector3 start;
        public Vector3 end;

        public Node startNode;
        public Node endNode;

        public NodeContiner.OnLinkBtn call;
    }

    public class NodeContiner
    {
        public delegate void OnLinkBtn(ref CurveDraw draw);
        public int Count { get { return m_nodeList.Count; } }

        public Node GetByIndex(int i)
        {
            return m_nodeList[i];
        }

        public void add(Node n)
        {
            m_nodeList.Add(n);
        }

        public void remove(Node n)
        {
            m_nodeList.Remove(n);
        }

        public void Repaint()
        {
            if (window != null)
                window.Repaint();
        }
        
        public void DrawCurve(Node s, Node e,Vector3 start, OnLinkBtn call)
        {
            CurveDraw draw = new CurveDraw();
            draw.start = start;
            draw.startNode = s;
            draw.endNode = e;
            draw.call = call;
            m_curveDraw.Add(draw);
        }

        public void Draw()
        {
            //foreach (var curveDraw in m_curveDraw)
            for(int i=0; i< m_curveDraw.Count;++i)
            {
                var curveDraw = m_curveDraw[i];
                Rect rc = new Rect();
                rc.x = curveDraw.start.x;
                rc.y = curveDraw.start.y - Node.kSingleLineHeight * 0.5f;
                rc.width = Node.kSingleLineHeight + 2;
                rc.height = Node.kSingleLineHeight;
                bool btnShow = GUI.Button(rc, curveDraw.endNode != null && curveDraw.endNode.visiable ? ">" : "<",
                    EditorStyles.miniButtonRight);
                if (btnShow)
                {
                    if (curveDraw.endNode != null)
                        curveDraw.endNode.visiable = !curveDraw.endNode.visiable;
                    curveDraw.call(ref curveDraw);
                }
                if (curveDraw.endNode != null && curveDraw.endNode.visiable)
                {
                    var end = curveDraw.endNode.position;
                    Vector3 staPos = curveDraw.start + new Vector3(rc.width, 0, 0);
                    Vector3 endPos = new Vector3(end.x, end.y + 10, 0);
                    DrawNodeCurve(staPos, endPos);
                }
            }
            m_curveDraw.Clear();
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

        List<CurveDraw> m_curveDraw = new List<CurveDraw>();
        protected List<Node> m_nodeList = new List<Node>();

        public EditorWindow window;
    }
    public class NodeEditorWindow : EditorWindow
    {
        [MenuItem("Window/ssss")]
        static void ddd()
        {
            Self.Show(true);

            //Node.Ctrl = new SampShowBehvCtrl();
            //Self.m_assetObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/NodeEditor/sss.asset");
            //if (Self.m_assetObject != null)
            //{
            //    var data = Self.m_assetObject.GetType().GetField("data").GetValue(Self.m_assetObject);
            //    Self.nodeWnd.Add(new TableTypeNode(1, Self, data));
            //}
        }
        int selectedWindow;
        void OnGUI()
        {
            m_continer.window = this;

            DrawToolbarGUI(new Rect(0, 0, Screen.width, 0));


            BeginWindows();
            if (m_continer != null&& m_continer.Count!=0)
            {
                //for (int i = 0; i < m_continer.Count; i++)
                //{
                //    var tableTypeNode = m_continer.GetByIndex(i);
                //    if (tableTypeNode.visiable)
                //        tableTypeNode.OnGUI();
                //}
                Queue<Node> drawQueue = new Queue<Node>();
                for (int i = 0; i < m_continer.Count; ++i)
                    drawQueue.Enqueue(m_continer.GetByIndex(i));

                while (drawQueue.Count!=0)
                {
                    var node = drawQueue.Dequeue();
                    node.OnGUI();
                    for (int i = 0; i < node.childrenCount; i++)
                    {
                        var child = node.GetChild(i);
                        if (!child.visiable)
                            continue;
                        drawQueue.Enqueue(child);
                    }
                }
            }

            m_continer.Draw();

            ProcessEvents(Event.current);
            EndWindows();
        }
        public float toolbarHeight = 17;

        public void DrawToolbarGUI(Rect rect)
        {
            rect.height = toolbarHeight;
            GUILayout.BeginArea(rect, Styles.toolbar);
            GUILayout.BeginHorizontal();
            float curToolbarHeight = 0;

            if (GUILayout.Button("File", Styles.toolbarDropdown, GUILayout.Width(50)))
            {
            }

            GUILayout.EndArea();
            GUILayout.EndHorizontal();
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

        private static string[] classNames
        {
            get
            {
                var allTypes = ConfScritableObject.GetAllTypes();
                List<string> nameList = new List<string>();
                for(int i=0; i<allTypes.Length;++i)
                {
                    nameList.Add(allTypes[i].Name);
                }
                return nameList.ToArray();
            }
        }
        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            for (int i = 0; i < classNames.Length; i++)
            {
                string name = classNames[i];
                genericMenu.AddItem(new GUIContent(name), false,
                    () => OnClickAddListNode(mousePosition, name));
            }



            genericMenu.ShowAsContext();
        }

        private void OnClickAddListNode(Vector2 mousePosition, string name)
        {
            var allTypes = ConfScritableObject.GetAllTypes();
            var listOfBs = (from filterType in allTypes
                            where filterType.Name==name
                            select filterType).ToArray();
            if (listOfBs.Length == 0)
                return;
            
            var assetObject = CreateInstance(listOfBs[0]);
            var assetType = assetObject.GetType();

            string guid = GUID.Generate().ToString();
            assetType.GetField("ident").SetValue(assetObject, guid);
            AddConfObject(assetObject as ConfScritableObject, mousePosition);
            var fullPath = string.Format("{0}{1}.asset", Settings.Instance.confpath, guid);
            AssetDatabase.CreateAsset(assetObject, fullPath);
        }

        public void AddConfObject(ConfScritableObject obj, Vector2 pos)
        {
            var wnd = new ObjReferenceTypeNode(1, m_continer, obj);
            wnd.center = pos;
            m_continer.add(wnd);
        }

        static Dictionary<string,Type> GetClasses(string nameSpace)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            Dictionary<string,Type> classlist = new Dictionary<string,Type>();
            var allTypes = asm.GetTypes();
            foreach (Type type in allTypes)
            {
                if (type.Namespace == nameSpace)
                    classlist.Add(type.Name, type);
            }
            return classlist;
        }



        private NodeContiner m_continer = new NodeContiner();

        public static NodeEditorWindow Self
        {
            get
            {
                if(m_singleton==null)
                    m_singleton = GetWindow<NodeEditorWindow>();
                return m_singleton;
            }
        }
        static NodeEditorWindow m_singleton = null;

    }
}