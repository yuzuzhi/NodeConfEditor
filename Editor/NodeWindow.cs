using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{


    public class NodeWindow
    {

        public ScriptableObject target;

        public EditorWindow m_parent;

        public NodeWindow(EditorWindow parent)
        {
            m_parent = parent;
        }

        public void OnGUI()
        {
            pos = HorizResizer(pos); //right
            pos = HorizResizer(pos, false); //left

            pos = GUI.Window(1, pos, OnDrawGUI, target.name);
        }

        void OnDrawGUI(int id)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            var datafield = target.GetType().GetField("data");
            if (datafield == null)
                return;



            var dataType = datafield.FieldType;
            var dataValue = datafield.GetValue(target);

            var fieldsList = dataType.GetFields();

            for (int i = 0; i < fieldsList.Length; i++)
            {
                var fieldInfo = fieldsList[i];

                bool hasChanged = false;
                var newValue = DrawBaseObject(fieldInfo.Name, fieldInfo.GetValue(dataValue), fieldInfo.FieldType,
                    ref hasChanged);
                if (hasChanged)
                {
                    fieldInfo.SetValue(dataValue, newValue);
                    m_hasChanged = true;
                }
            }

            if (m_hasChanged)
            {
                EditorUtility.SetDirty(target);
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
                draggingLeft = false;
                draggingRight = false;
            }

            // resize window if mouse is being dragged within resizor bounds
            if (current.mousePosition.x > resizer.xMin &&
                current.mousePosition.x < resizer.xMax &&
                current.type == EventType.MouseDrag &&
                current.button == 0 ||
                draggingLeft ||
                draggingRight)
            {
                if (right == !draggingLeft)
                {
                    window.width = current.mousePosition.x + current.delta.x;
                    m_parent.Repaint();
                    draggingRight = true;
                }
                else if (!right == !draggingRight)
                {
                    window.width = window.width - (current.mousePosition.x + current.delta.x);
                    m_parent.Repaint();
                    draggingLeft = true;
                }

            }

            return window;
        }

        object DrawBaseObject(string strLabelText, object oldValue, Type objType, ref bool hasChanged)
        {
            object newValue = 0;
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

            return newValue;
        }


        private bool m_hasChanged = false;
        private string m_name;

        Rect pos = new Rect(10, 30, 200, 200);


        bool draggingLeft = false;
        bool draggingRight = false;
    }
}