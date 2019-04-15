using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{
    public class TableTypeNode: Node
    {

        public object target
        {
            get { return m_target; }
        }

        public TableTypeNode(int id, NodeEditorWindow parent, object target):base(id, parent)
        {
            m_target = target;
        }
        internal const float kSingleLineHeight = 16;


        public override void OnDrawGUI()
        {
            if (m_target == null)
                return;

            foreach (var node in m_childrenByField)
            {
                if (node.Value.hasChanged)
                {
                    SetChanged();
                    break;
                }
            }
            

            var fieldsList = m_target.GetType().GetFields();

            for (int i = 0; i < fieldsList.Length; i++)
            {
                var fieldInfo = fieldsList[i];
                var fieldValue = fieldInfo.GetValue(m_target);

                bool bchanged = false;
                object newValue = null;
                if (DrawBaseObject(fieldInfo.Name, fieldValue, fieldInfo.FieldType, ref newValue,
                    ref bchanged))
                {
                    if (hasChanged)
                    {
                        fieldInfo.SetValue(m_target, newValue);
                        SetChanged();
                    }
                }
                else if (fieldInfo.FieldType.IsList())
                {
                    //m_childrenByField
                    Rect r = EditorGUILayout.GetControlRect(true);
                    GUI.Label(r, fieldInfo.Name);
                    int btnWid = 30;
                    int edgeWid = 20;
                    r.xMin = position.xMax - btnWid - edgeWid;
                    r.width = btnWid;
                    Node node;
                    if (!m_childrenByField.TryGetValue(fieldInfo, out node))
                    {
                        node = new BaseListTypeNode(increasingIdent++, parent, fieldValue as IList);
                        m_childrenByField.Add(fieldInfo, node);
                        parent.nodeWnd.Add(node);
                    }

                    bool btnShow = GUI.Button(r, node.visiable ? "-" : "+");
                    if (btnShow)
                        node.visiable = !node.visiable;

                    if (node.visiable)
                    {
                        Rect curvStart = position;
                        curvStart.yMin += (i + 1)*kSingleLineHeight;
                        curvStart.height = kSingleLineHeight;
                        DrawNodeCurve(curvStart, node.position);
                    }
                }


            }
        }
        
        object m_target;

        Dictionary<FieldInfo, Node> m_childrenByField = new Dictionary<FieldInfo, Node>();
    }
}