using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{

    public static class Utils
    {
        public static bool IsList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
        
        public static bool IsTable(this Type t)
        {
            return t.IsClass && !t.Is<string>();

        }

        public static bool Is<T>(this Type type)
        {
            var targetType = type;
            var unityObjType = typeof(T);
            while (targetType != null)
            {
                if (targetType == unityObjType)
                    return true;
                targetType = targetType.BaseType;
            }

            return false;
        }

        public static bool IsTable(this FieldInfo t)
        {
            return t.FieldType.IsClass;
        }
        public static bool IsObjReference(this FieldInfo t)
        {
            return t.FieldType == typeof (ObjReference);
        }

        public static bool GetAttribute<T>(this FieldInfo field, ref T res)
        {
            var attr = field.GetCustomAttributes(typeof(T), false);
            if (attr == null || attr.Length == 0)
                return false;

            res = (T)attr[0];

            return true;
        }

        public static bool GetAttribute<T>(this Type type, ref T res)
        {
            var attr = type.GetCustomAttributes(typeof(T), false);
            if (attr == null || attr.Length == 0)
                return false;

            res = (T)attr[0];
            return true;
        }
    }

    public static class Styles
    {
        public static GUIStyle styleWindowResize = GUI.skin.GetStyle( "WindowResizer" );

        public static GUIStyle toolbarButton = GUI.skin.FindStyle("toolbarButton");
        public static GUIStyle toolbarLabel = GUI.skin.FindStyle("toolbarButton");
        public static GUIStyle toolbarDropdown = GUI.skin.FindStyle("toolbarDropdown");
        public static GUIStyle toolbar = GUI.skin.FindStyle("toolbar");

        public static float titleBarheight = 20;
        public static float comnListNodeWidth = 100;
        public static float objRfListNodeWidth = 200;

    }

    public static class GUIContents
    {
        public static GUIContent gcDrag = new GUIContent("", "drag to resize");
    }


    public static class NodeUtils
    {
        public delegate void OnDrawListItem(int i);

        public static void DrawListObject(IList listObj, ref int insertIndex, ref int removeIndex, OnDrawListItem drawer)
        {
            if (listObj.Count == 0)
            {
                if (GUILayout.Button("+", GUILayout.Width(20)))
                    insertIndex = 0;
            }

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30;

            for (int i = 0; i < listObj.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (drawer != null)
                        drawer(i);
                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                        insertIndex = i;
                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                        removeIndex = i;
                }
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;

        }

        public static bool DrawBaseObject(GUIContent strLabelText, object oldValue, Type objType, ref object newValue, ref bool hasChanged, ref float height)
        {
            newValue = null;
            if (typeof(float) == objType)
            {
                height += Node.kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                newValue = (object)EditorGUILayout.FloatField(strLabelText, (float)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }
            if (typeof(bool) == objType)
            {
                height += Node.kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                newValue = EditorGUILayout.Toggle(strLabelText, (bool)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }
            if (typeof(int) == objType)
            {
                height += Node.kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                newValue = EditorGUILayout.IntField(strLabelText, (int)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }
            if (objType.IsEnum)
            {
                height += Node.kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                newValue = EditorGUILayout.EnumPopup(strLabelText, (Enum)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }

            if (typeof(string) == objType)
            {
                height += Node.kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (oldValue == null) oldValue = "";
                newValue = EditorGUILayout.TextField(strLabelText, (string)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }
            if (objType.Is<Vector3>())
            {
                GUIContent label = new GUIContent(strLabelText);
                height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label) + EditorGUIUtility.standardVerticalSpacing;
                newValue = EditorGUILayout.Vector3Field(label, (Vector3)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }

            return false;
        }


        public static Rect ResizeWindow(Rect windowRect, ref bool isResizing, ref Rect resizeStart,
            Vector2 minWindowSize)
        {
            Vector2 mouse =
                GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
            Rect r = GUILayoutUtility.GetRect(GUIContents.gcDrag, Styles.styleWindowResize);
            if (Event.current.type == EventType.MouseDown)
            {
                bool c = r.Contains(mouse);
                isResizing = true;
                resizeStart = new Rect(mouse.x, mouse.y, windowRect.width, windowRect.height);
                //Event.current.Use();  // the GUI.Button below will eat the event, and this way it will show its active state
            }
            else if (Event.current.type == EventType.MouseUp && isResizing)
            {
                isResizing = false;
            }
            else if (!Input.GetMouseButton(0))
            {
                // if the mouse is over some other window we won't get an event, this just kind of circumvents that by checking the button state directly
                isResizing = false;
            }
            else if (isResizing)
            {
                windowRect.width = Mathf.Max(minWindowSize.x, resizeStart.width + (mouse.x - resizeStart.x));
                windowRect.height = Mathf.Max(minWindowSize.y, resizeStart.height + (mouse.y - resizeStart.y));
                windowRect.xMax = Mathf.Min(Screen.width, windowRect.xMax); // modifying xMax affects width, not x
                windowRect.yMax = Mathf.Min(Screen.height, windowRect.yMax); // modifying yMax affects height, not y
            }
            GUI.Button(r, mouse.ToString()/*GUIContents.gcDrag, Styles.styleWindowResize*/);
            return windowRect;
        }

    }


    public class Node
    {
        protected class LinkedInfo
        {
            public LinkedInfo()
            {
                linkNode = null;
                visiable = false;

            }
            public Node linkNode;
            public bool visiable;
        }
        public const float kSingleLineHeight = 16;

        public Node(int id, NodeContiner parent, bool canResize = false)
        {
            if (Node.Ctrl == null)
                Node.Ctrl = new ShowBehvCtrl();

            m_id = id;
            m_parent = parent;
            m_canResize = canResize;
        }

        public Vector2 center
        {
            set { m_position.center = value; }
        }

        public bool visiable
        {
            set { m_visiable = value; }
            get { return m_visiable; }
        }

        public Rect position { get { return m_position; } }
        public Rect SetPosition { set { m_position = value; } }
        public string SetName { set { m_name = value; } }
        public string name { get { return m_name; } }

        public int childrenCount { get { return m_children.Count; } }
        public Node GetChild(int index)
        {
            return m_children[index];
        }

        public void AddChild(Node child)
        {
            m_children.Add(child);
        }

        public void RemoveChild(Node child)
        {
            m_children.Remove(child);
        }

        public Node children { get { return m_childrennode;} }
        public Node nextnode { get { return m_nextnode;} }
        public Node prefnode { get { return m_prefnode;} }

        protected Node SetChildren { set { m_childrennode = value; } }

        protected Node SetNextNode
        {
            set
            {
                var old = m_nextnode;
                value.m_nextnode = old;
                value.m_prefnode = this;

                m_nextnode = value;
            }
        }
        protected Node SetPrefNode
        {
            set
            {
                var old = m_prefnode;
                m_prefnode = value;
                m_prefnode.m_prefnode = old;
            }
        }



        public void OnGUI()
        {
            if (m_canResize)
                m_position = HorizResizer(m_position); //right
            //m_position = HorizResizer(m_position, false); //left
            string name = m_name != null ? m_name : "";
            m_position = GUI.Window(m_id, m_position, OnDrawWindow, m_hasChanged ? name + "*" : name);
        }

        public virtual void OnDrawGUI()
        {
            
        }

        void OnDrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            m_heightStart = Styles.titleBarheight;
            OnDrawGUI();
            
            var wndrect = position;
            if (wndrect.height < m_heightStart + 20)
            {
                wndrect.height = m_heightStart + 20;
                SetPosition = wndrect;
            }
        }
        

        public void SetChanged()
        {
            m_hasChanged = true;
        }

        public void ClearChanged()
        {
            m_hasChanged = false;

            foreach (var mChild in m_children)
            {
                mChild.ClearChanged();
            }
        }

        public bool hasChanged { get { return m_hasChanged; } }

        private float ttt;
        private Rect HorizResizer(Rect window, bool right = true, float detectionRange = 8f)
        {
            detectionRange *= 0.5f;
            Rect resizer = window;

            if (right)
            {
                resizer.xMin = resizer.xMax - detectionRange;
                resizer.xMax += detectionRange;
            }
            else
            {
                resizer.xMax = resizer.xMin + detectionRange;
                resizer.xMin -= detectionRange;
            }

            Event current = Event.current;
            EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeHorizontal);

            if (current.type == EventType.MouseDown)
            {
                ttt = current.mousePosition.x-m_position.xMax;
            }
            // if mouse is no longer dragging, stop tracking direction of drag
            if (current.type == EventType.MouseUp)
            {
                m_draggingLeft = false;
                m_draggingRight = false;
            }

            // resize window if mouse is being dragged within resizor bounds
            if (current.mousePosition.x > resizer.xMin &&
                current.mousePosition.x < resizer.xMax &&
                current.type == EventType.MouseDrag &&
                current.button == 0 ||
                m_draggingLeft ||
                m_draggingRight)
            {
                if (right == !m_draggingLeft)
                {
                    window.width = current.mousePosition.x-ttt;// + current.delta.x;
                    m_parent.Repaint();
                    m_draggingRight = true;
                }
                else if (!right == !m_draggingRight)
                {
                    window.width = current.mousePosition.x - ttt;//window.width - (current.mousePosition.x + current.delta.x);
                    m_parent.Repaint();
                    m_draggingLeft = true;
                }

            }

            return window;
        }
        
        protected void DrawNodeCurve(Node s, Node e, Rect start, NodeContiner.OnLinkBtn call)
        {
            Vector3 startPos = new Vector3(start.x, start.y + start.height/2, 0);
            m_parent.DrawCurve(s, e, startPos, call);
        }
        protected Rect ReCalcuChildPos(Rect childPos, Rect pos)
        {
            var w = childPos.width;
            var h = childPos.height;
            childPos.x = this.position.xMax + 50;
            childPos.y = pos.y;
            childPos.width = w;
            childPos.height = h;
            return childPos;
        }

        protected Rect CalcuControlRect(int index, Type t)
        {
            //EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label)
            Rect curvStart = position;
            curvStart.x = position.x + position.width;
            curvStart.y += m_heightStart;//+= m_controlStartY + (index + 1)*(kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            curvStart.height = kSingleLineHeight;
            curvStart.width = kSingleLineHeight;
            return curvStart;
        }

        protected void inccomnheightpos()
        {
            m_heightStart += kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        
        protected NodeContiner parent { get { return m_parent; } }

        protected static int increasingIdent = 10001;

        private int m_id;
        protected float m_controlStartY = 0;
        NodeContiner m_parent;
        private Rect m_position = new Rect(10, 30, 300, Styles.titleBarheight + 10);
        bool m_draggingLeft = false;
        bool m_draggingRight = false;
        private string m_name;
        bool m_hasChanged = false;
        bool m_visiable = true;

        bool isResizing = false;
        Rect resizeStart = new Rect();

        List<Node> m_children = new List<Node>();
        private Node m_childrennode;
        private Node m_prefnode;
        private Node m_nextnode;

        bool m_canResize;
        protected float m_heightStart;

        public static ShowBehvCtrl Ctrl;
    }

}