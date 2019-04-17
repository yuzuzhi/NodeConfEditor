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
        }

        public override void OnDrawGUI()
        {
            var listObj = m_target;
            if (listObj == null)
                return;

            var itemType = listObj.GetType().GetGenericArguments()[0];
            int insertIndex = -1, removeIndex = -1;

            DrawListObject(listObj, ref insertIndex, ref removeIndex, delegate (int i)
            {
                var listItemValue = listObj[i];
                
                if (i >= m_nodeOfItems.Count)
                    m_nodeOfItems.Add(null);
                var fieldLink = m_nodeOfItems[i];
                if (fieldLink == null)
                    fieldLink = m_nodeOfItems[i] = new LinkedInfo();


                bool bListItemChnged = false;
                string label = string.Format("[{0}]", i);

                Rect rect = EditorGUILayout.GetControlRect(true);
                Rect r = rect;
                GUI.Label(r, label);
                int btnWid = 30;
                int edgeWid = 0;
                r.xMin = r.xMax - btnWid - edgeWid;
                r.width = btnWid;



                Rect curvStart = CalcuControlRect(i, null);
                DrawNodeCurve(this, fieldLink.linkNode, curvStart, delegate (ref CurveDraw draw)
                {
                    if (fieldLink.linkNode == null)
                    {
                        fieldLink.linkNode = new TableTypeNode(increasingIdent, parent, listItemValue);
                        draw.endNode = fieldLink.linkNode;
                        parent.add(fieldLink.linkNode);
                    }
                });


                if (fieldLink.linkNode!=null&& fieldLink.linkNode.hasChanged)
                {
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
                listObj.RemoveAt(removeIndex);
                SetChanged();
            }
        }



        IList m_target;
        List<LinkedInfo> m_nodeOfItems = new List<LinkedInfo>();
    }
}