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

        public TableTypeNode(int id, NodeContiner parent, object target):base(id, parent)
        {
            m_target = target;
        }
        internal const float kSingleLineHeight = 16;


        public override void OnDrawGUI()
        {
            if (m_target == null)
            {
                return;
            }

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
                    continue;
                }

                Rect rect = EditorGUILayout.GetControlRect(true);
                Rect r = rect;
                GUI.Label(r, fieldInfo.Name);
                int btnWid = 30;
                int edgeWid = 0;
                r.xMin = r.xMax - btnWid - edgeWid;
                r.width = btnWid;
                Node node = null;
                m_childrenByField.TryGetValue(fieldInfo, out node);
                bool btnShow = GUI.Button(r, node != null && node.visiable ? "-" : "+", EditorStyles.miniButton);

                if (fieldInfo.FieldType.IsList())
                {
                    if (node == null)
                    {
                        bool isClassObjItem = fieldInfo.FieldType.GetGenericArguments()[0].IsTable();
                        if (isClassObjItem)
                            node = new TableListTypeNode(increasingIdent++, parent, fieldValue as IList);
                        else
                            node = new BaseListTypeNode(increasingIdent++, parent, fieldValue as IList);
                        m_childrenByField.Add(fieldInfo, node);
                        parent.add(node);
                    }
                }
                else if (fieldInfo.IsObjReference())
                {
                    if (node == null)
                    {
                        node = new ObjReferenceTypeNode(increasingIdent++, parent, null);
                        node.visiable = false;
                        m_childrenByField.Add(fieldInfo, node);
                        parent.add(node);
                    }

                    var objref = (ObjReference) fieldValue;
                    r.xMin = EditorGUIUtility.labelWidth;
                    r.width = rect.width - r.xMin - btnWid;
                    var newObj = EditorGUI.ObjectField(r, objref.target, typeof (ScriptableObject),
                        false);
                    if (newObj != objref.target)
                    {
                        objref.target = newObj;
                        fieldInfo.SetValue(m_target, objref);
                        ((ObjReferenceTypeNode) node).Reset((ScriptableObject) objref.target);
                        SetChanged();
                    }
                }


                if (btnShow && node != null)
                    node.visiable = !node.visiable;
                if (node != null && node.visiable)
                {
                    Rect curvStart = position;
                    curvStart.yMin += (i + 1)*kSingleLineHeight;
                    curvStart.height = kSingleLineHeight;
                    DrawNodeCurve(curvStart, node.position);
                }


            }
        }
        
        protected object m_target;

        protected Dictionary<FieldInfo, Node> m_childrenByField = new Dictionary<FieldInfo, Node>();
    }
}