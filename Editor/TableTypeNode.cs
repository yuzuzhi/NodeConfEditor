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
                if (node.Value.linkNode != null && node.Value.linkNode.hasChanged)
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
                    if (bchanged)
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
                LinkedInfo fieldLink = null;
                if (!m_childrenByField.TryGetValue(fieldInfo, out fieldLink))
                {
                    fieldLink = new LinkedInfo();
                    m_childrenByField.Add(fieldInfo, fieldLink);
                }
                bool btnShow = GUI.Button(r, fieldLink != null && fieldLink.visiable ? "-" : "+", EditorStyles.miniButton);
                if (btnShow)
                    fieldLink.visiable = !fieldLink.visiable;
                if (fieldInfo.FieldType.IsList())
                {
                    if (btnShow && fieldLink.visiable && fieldLink.linkNode == null)
                    {
                        bool isClassObjItem = fieldInfo.FieldType.GetGenericArguments()[0].IsTable();
                        if (isClassObjItem)
                            fieldLink.linkNode = new TableListTypeNode(increasingIdent++, parent, fieldValue as IList);
                        else
                            fieldLink.linkNode = new BaseListTypeNode(increasingIdent++, parent, fieldValue as IList);
                        parent.add(fieldLink.linkNode);
                    }
                }
                else if (fieldInfo.IsObjReference())
                {
                    var objref = (ObjReference)fieldValue;

                    if (btnShow && fieldLink.visiable && fieldLink.linkNode == null)
                    {
                        fieldLink.linkNode = new ObjReferenceTypeNode(increasingIdent++, parent,
                            (ScriptableObject) objref.target);
                        parent.add(fieldLink.linkNode);
                    }

                    r.xMin = EditorGUIUtility.labelWidth;
                    r.width = rect.width - r.xMin - btnWid;
                    var newObj = EditorGUI.ObjectField(r, objref.target, typeof (ScriptableObject),
                        false);
                    if (newObj != objref.target)
                    {
                        objref.target = newObj;
                        fieldInfo.SetValue(m_target, objref);
                        if (fieldLink.linkNode != null)
                            ((ObjReferenceTypeNode) fieldLink.linkNode).Reset((ScriptableObject) objref.target);
                        SetChanged();
                    }
                }
                else if (fieldInfo.IsTable())
                {
                    if (btnShow && fieldLink.visiable && fieldLink.linkNode == null)
                    {
                        fieldLink.linkNode = new TableTypeNode(increasingIdent++, parent, fieldValue);
                        parent.add(fieldLink.linkNode);
                    }
                }

                if (fieldLink.linkNode != null)
                    fieldLink.linkNode.visiable = fieldLink.visiable;
                
                if (fieldLink.linkNode != null && fieldLink.visiable)
                {
                    Rect curvStart = position;
                    curvStart.yMin += (i + 1)*kSingleLineHeight;
                    curvStart.height = kSingleLineHeight;
                    DrawNodeCurve(curvStart, fieldLink.linkNode.position);
                }


            }
        }
        
        protected object m_target;

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

        protected Dictionary<FieldInfo, LinkedInfo> m_childrenByField = new Dictionary<FieldInfo, LinkedInfo>();
    }
}