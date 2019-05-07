using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace cfeditor
{

    public class TableListTypeNode : Node
    {
        public TableListTypeNode(int id, NodeContiner parent, IList target): base(id, parent)
        {
            m_target = target;

            var itemType = m_target.GetType().GetGenericArguments()[0];
            var p = position;
            p.width = itemType.Is<ObjReference>() ? Styles.objRfListNodeWidth : Styles.comnListNodeWidth;
            SetPosition = p;
        }

        public override void OnDrawGUI()
        {
            var listObj = m_target;
            if (listObj == null)
                return;

            var itemType = listObj.GetType().GetGenericArguments()[0];
            int insertIndex = -1, removeIndex = -1;

            NodeUtils.DrawListObject(listObj, ref insertIndex, ref removeIndex, delegate (int i)
            {
                var listItemValue = listObj[i];
                Rect curvStart = CalcuControlRect(i, null);

                if (i >= m_nodeOfItems.Count)
                    m_nodeOfItems.Add(null);
                var fieldLink = m_nodeOfItems[i];
                if (fieldLink == null)
                    fieldLink = m_nodeOfItems[i] = new LinkedInfo();
                
                DrawNodeCurve(this, fieldLink.linkNode, curvStart, delegate (ref CurveDraw draw)
                {
                    if (fieldLink.linkNode == null)
                    {
                        if (itemType.Is<ObjReference>())
                        {
                            var objRefer = (ObjReference) listItemValue;
                            fieldLink.linkNode = new ObjReferenceTypeNode(increasingIdent++, parent,
                                (ScriptableObject) objRefer.target);
                        }
                        else
                            fieldLink.linkNode = new TableTypeNode(increasingIdent++, parent, listItemValue);
                        fieldLink.linkNode.SetPosition = ReCalcuChildPos(fieldLink.linkNode.position, curvStart);
                        this.AddChild(fieldLink.linkNode);
                        draw.endNode = fieldLink.linkNode;
                    }
                });

                if (itemType.Is<ObjReference>())
                {
                    var objRefer = (ObjReference)listItemValue;
                    var strLabel = string.Format("[{0}]", i);
                    var newObj = EditorGUILayout.ObjectField(strLabel, objRefer.target, typeof (ScriptableObject), false);
                    inccomnheightpos();
                    if (newObj != objRefer.target)
                    {
                        objRefer.target = newObj;
                        m_target[i] = objRefer;
                        if (fieldLink.linkNode != null)
                            ((ObjReferenceTypeNode)fieldLink.linkNode).Reset((ScriptableObject)objRefer.target);
                        SetChanged();
                    }
                }
                else
                {
                    GUILayout.Label(string.Format("[{0}]", i), GUILayout.Width(EditorGUIUtility.labelWidth));
                    inccomnheightpos();
                    if (fieldLink.linkNode != null && fieldLink.linkNode.hasChanged)
                        SetChanged();
                }
            });



            if (insertIndex != -1)
            {
                var defType = itemType;
                object addingObj = null;
                if (defType.Is<string>())
                    addingObj = "";
                else
                    addingObj = Activator.CreateInstance(defType);
                listObj.Insert(insertIndex, addingObj);
                SetChanged();
            }
            if (removeIndex != -1)
            {
                var item = m_nodeOfItems[removeIndex];
                if (item.linkNode != null)
                    parent.remove(item.linkNode);
                m_nodeOfItems.RemoveAt(removeIndex);
                listObj.RemoveAt(removeIndex);
                SetChanged();
            }
        }
        


        IList m_target;
        List<LinkedInfo> m_nodeOfItems = new List<LinkedInfo>();
    }
}