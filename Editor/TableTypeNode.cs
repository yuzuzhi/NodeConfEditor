﻿using System;
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

        public TableTypeNode(int id, NodeContiner parent, object target):base(id, parent, true)
        {
            m_target = target;
            if (m_target != null)
                SetName = Ctrl.GetNodeTitle(m_target);
        }


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

                if (!Ctrl.FieldVisible(m_target, fieldInfo))
                    continue;
                var label = Ctrl.GetFieldLabel(m_target, fieldInfo);

                Rect curvStart = CalcuControlRect(i, fieldInfo.FieldType);


                bool bchanged = false;
                object newValue = null;
                if (NodeUtils.DrawBaseObject(label, fieldValue, fieldInfo.FieldType, ref newValue,
                    ref bchanged, ref m_heightStart))
                {
                    if (bchanged)
                    {
                        fieldInfo.SetValue(m_target, newValue);
                        SetChanged();
                    }
                    continue;
                }
                
                LinkedInfo fieldLink = null;
                if (!m_childrenByField.TryGetValue(fieldInfo, out fieldLink))
                {
                    fieldLink = new LinkedInfo();
                    m_childrenByField.Add(fieldInfo, fieldLink);
                }

                NodeContiner.OnLinkBtn call = null;
                if (fieldInfo.FieldType.IsList())
                {
                    var listValue = fieldValue as IList;
                    call = delegate(ref CurveDraw draw)
                    {
                        if (fieldLink.linkNode == null)
                        {
                            var typeOfListItem = fieldInfo.FieldType.GetGenericArguments()[0];
                            if (typeOfListItem.IsTable())
                                fieldLink.linkNode = new TableListTypeNode(increasingIdent++, parent,
                                    fieldValue as IList);
                            else if(typeOfListItem.Is<ObjReference>())
                                fieldLink.linkNode = new TableListTypeNode(increasingIdent++, parent,
                                    fieldValue as IList);
                            else
                                fieldLink.linkNode = new BaseListTypeNode(increasingIdent++, parent, fieldValue as IList);
                            fieldLink.linkNode.SetName = "列表";
                            fieldLink.linkNode.SetPosition = ReCalcuChildPos(fieldLink.linkNode.position, curvStart);
                            this.AddChild(fieldLink.linkNode);
                            draw.endNode = fieldLink.linkNode;
                        }
                    };

                    EditorGUILayout.TextField(label, "List " + listValue.Count);
                    inccomnheightpos();
                }
                else if (fieldInfo.IsObjReference())
                {
                    var objref = (ObjReference)fieldValue;

                    call = delegate(ref CurveDraw draw)
                    {
                        if (fieldLink.linkNode == null)
                        {
                            fieldLink.linkNode = new ObjReferenceTypeNode(increasingIdent++, parent,
                                (ConfScritableObject) objref.target);
                            fieldLink.linkNode.SetPosition = ReCalcuChildPos(fieldLink.linkNode.position, curvStart);
                            this.AddChild(fieldLink.linkNode);
                            draw.endNode = fieldLink.linkNode;
                        }
                    };
                    //r.xMin = EditorGUIUtility.labelWidth;
                    //r.width = rect.width - r.xMin - btnWid;
                    var newObj = EditorGUILayout.ObjectField(label,objref.target, typeof (ConfScritableObject),
                        false);
                    inccomnheightpos();
                    if (newObj != objref.target)
                    {
                        objref.target = newObj;
                        fieldInfo.SetValue(m_target, objref);
                        if (fieldLink.linkNode != null)
                            ((ObjReferenceTypeNode) fieldLink.linkNode).Reset((ConfScritableObject) objref.target);
                        SetChanged();
                    }
                }
                else if (fieldInfo.IsTable())
                {
                    call = delegate(ref CurveDraw draw)
                    {
                        if (fieldLink.linkNode == null)
                        {
                            fieldLink.linkNode = new TableTypeNode(increasingIdent++, parent, fieldValue);
                            fieldLink.linkNode.SetPosition = ReCalcuChildPos(fieldLink.linkNode.position, curvStart);
                            this.AddChild(fieldLink.linkNode);
                            draw.endNode = fieldLink.linkNode;
                        }
                    };
                    if (Ctrl.IsDynField(m_target, fieldInfo))
                    {
                        var typeList = Ctrl.DynTypeList(m_target, fieldInfo);
                        var nameList = Ctrl.toNameList(typeList);                        
                    }
                    else
                        EditorGUILayout.TextField(label, fieldInfo.FieldType.Name);
                    inccomnheightpos();
                }



                DrawNodeCurve(this, fieldLink.linkNode, curvStart, call);
            }   //End-for field list


        }

        
        protected object m_target;


        protected Dictionary<FieldInfo, LinkedInfo> m_childrenByField = new Dictionary<FieldInfo, LinkedInfo>();
    }
}