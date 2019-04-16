﻿using System;
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
        public static bool IsTable(this FieldInfo t)
        {
            return t.FieldType.IsClass;
        }
        public static bool IsObjReference(this FieldInfo t)
        {
            return t.FieldType == typeof (ObjReference);
        }


    }
    public class Node
    {
        public Node(int id, NodeContiner parent)
        {
            m_id = id;
            m_parent = parent;
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

        public void OnGUI()
        {
            m_position = HorizResizer(m_position); //right
            m_position = HorizResizer(m_position, false); //left
            m_position = GUI.Window(m_id, m_position, OnDrawWindow, m_hasChanged ? m_name + "*" : m_name);
        }

        public virtual void OnDrawGUI()
        {
            
        }

        void OnDrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            OnDrawGUI();
        }
        

        public void SetChanged()
        {
            m_hasChanged = true;
        }

        public bool hasChanged { get { return m_hasChanged; } }


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
                    window.width = current.mousePosition.x + current.delta.x;
                    m_parent.Repaint();
                    m_draggingRight = true;
                }
                else if (!right == !m_draggingRight)
                {
                    window.width = window.width - (current.mousePosition.x + current.delta.x);
                    m_parent.Repaint();
                    m_draggingLeft = true;
                }

            }

            return window;
        }
        
        protected void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + 10, 0);
            m_parent.DrawCurve(startPos, endPos);
        }



        public delegate void OnDrawListItem(int i);
        public delegate void OnDrawListItemEnd(int i, int insertIndex, int removeIndex);
        protected static void DrawListObject(IList listObj, ref int insertIndex, ref int removeIndex, OnDrawListItem drawer)
        {
            if (listObj.Count == 0)
            {
                if (GUILayout.Button("+", GUILayout.Width(20)))
                    insertIndex = 0;
            }

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;

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

        protected static bool DrawBaseObject(string strLabelText, object oldValue, Type objType, ref object newValue, ref bool hasChanged)
        {
            newValue = null;
            if (typeof(float) == objType)
            {
                newValue = (object)EditorGUILayout.FloatField(strLabelText, (float)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }
            if (typeof(bool) == objType)
            {
                newValue = EditorGUILayout.Toggle(strLabelText, (bool)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }
            if (typeof(int) == objType)
            {
                newValue = EditorGUILayout.IntField(strLabelText, (int)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }
            if (objType.IsEnum)
            {
                newValue = EditorGUILayout.EnumPopup(strLabelText, (Enum)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }

            if (typeof(string) == objType)
            {
                if (oldValue == null) oldValue = "";
                newValue = EditorGUILayout.TextField(strLabelText, (string)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }
            if (objType.Is<Vector3>())
            {
                newValue = EditorGUILayout.Vector3Field(strLabelText, (Vector3)oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
                return true;
            }

            return false;
        }


        protected NodeContiner parent { get { return m_parent; } }

        protected int increasingIdent = 10001;

        private int m_id;
        NodeContiner m_parent;
        Rect m_position = new Rect(10, 30, 200, 200);
        bool m_draggingLeft = false;
        bool m_draggingRight = false;
        private string m_name;
        bool m_hasChanged = false;
        bool m_visiable = true;
    }

}