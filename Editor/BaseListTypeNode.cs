using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{


    public class BaseListTypeNode : Node
    {
        public BaseListTypeNode(int id, NodeContiner parent, IList target) : base(id, parent, true)
        {
            m_target = target;
            ResizeHeight();
        }

        public override void OnDrawGUI()
        {
            var listObj = m_target;
            if (listObj == null)
                return;

            var itemType = listObj.GetType().GetGenericArguments()[0];
            int insertIndex = -1, removeIndex = -1;

            NodeUtils.DrawListObject(listObj, ref insertIndex, ref removeIndex, delegate(int i)
            {
                var listItemValue = listObj[i];

                bool bListItemChnged = false;
                string label = string.Format("[{0}]", i);
                object newValue = null;
                NodeUtils.DrawBaseObject(label, listItemValue, itemType, ref newValue, ref bListItemChnged);
                if (bListItemChnged)
                {
                    listObj[i] = newValue;
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
                ResizeHeight();
            }
            if (removeIndex != -1)
            {
                listObj.RemoveAt(removeIndex);
                SetChanged();
                ResizeHeight();
            }
        }

        void ResizeHeight()
        {
            if (m_target == null)
                return;
            var p = position;
            int listCount = m_target != null ? m_target.Count : 0;
            int count = Mathf.Max(listCount, 1);
            p.height = (count+2)*(kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            SetPosition = p;
        }



IList m_target;
    }
}
