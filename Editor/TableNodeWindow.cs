using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{


    public class TableNodeWindow
    {

        public ScriptableObject target
        {
            get { return m_target; }
        }

        public TableNodeWindow(EditorWindow parent, ScriptableObject target)
        {
            m_parent = parent;
            m_target = target;
        }

        public void OnGUI()
        {
            m_position = HorizResizer(m_position); //right
            m_position = HorizResizer(m_position, false); //left

            m_position = GUI.Window(1, m_position, OnDrawGUI, m_target.name);
        }

        void OnDrawGUI(int id)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            var datafield = m_target.GetType().GetField("data");
            if (datafield == null)
                return;



            var dataType = datafield.FieldType;
            var dataValue = datafield.GetValue(m_target);

            var fieldsList = dataType.GetFields();

            for (int i = 0; i < fieldsList.Length; i++)
            {
                var fieldInfo = fieldsList[i];

                bool hasChanged = false;
                object newValue = null;
                if (DrawBaseObject(fieldInfo.Name, fieldInfo.GetValue(dataValue), fieldInfo.FieldType, ref newValue,
                    ref hasChanged)) ;
                else if(fieldInfo.FieldType.IsList())


                if (hasChanged)
                {
                    fieldInfo.SetValue(dataValue, newValue);
                    m_hasChanged = true;
                }
            }

            if (m_hasChanged)
            {
                EditorUtility.SetDirty(m_target);
            }

        }

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

        bool DrawBaseObject(string strLabelText, object oldValue, Type objType, ref object newValue, ref bool hasChanged)
        {
            newValue = null;
            if (typeof (float) == objType)
            {
                newValue = (object) EditorGUILayout.FloatField(strLabelText, (float) oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
            }
            else if (typeof (bool) == objType)
            {
                newValue = EditorGUILayout.Toggle(strLabelText, (bool) oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
            }
            else if (typeof (int) == objType)
            {
                newValue = EditorGUILayout.IntField(strLabelText, (int) oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
            }
            else if (objType.IsEnum)
            {
                newValue = EditorGUILayout.EnumPopup(strLabelText, (Enum) oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
            }

            else if (typeof (string) == objType)
            {
                if (oldValue == null) oldValue = "";
                newValue = EditorGUILayout.TextField(strLabelText, (string) oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
            }
            else if (objType.Is<Vector3>())
            {
                newValue = EditorGUILayout.Vector3Field(strLabelText, (Vector3) oldValue);
                if (!newValue.Equals(oldValue)) hasChanged = true;
            }

            return newValue != null;
        }


        private bool m_hasChanged = false;
        private string m_name;
        ScriptableObject m_target;
        EditorWindow m_parent;
        Rect m_position = new Rect(10, 30, 200, 200);
        bool m_draggingLeft = false;
        bool m_draggingRight = false;
    }
}