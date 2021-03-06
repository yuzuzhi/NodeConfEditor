﻿using System;
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
                var label = new GUIContent(string.Format("[{0}]", i));
                object newValue = null;
                NodeUtils.DrawBaseObject(label, listItemValue, itemType, ref newValue, ref bListItemChnged,
                    ref m_heightStart);
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
            }
            if (removeIndex != -1)
            {
                listObj.RemoveAt(removeIndex);
                SetChanged();
            }
        }

        IList m_target;
    }
}
