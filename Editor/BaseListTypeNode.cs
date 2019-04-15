using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{


    public class BaseListTypeNode : Node
    {
        public BaseListTypeNode(int id, NodeEditorWindow parent, IList target): base(id, parent)
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
            for (int i = 0; i < listObj.Count; i++)
            {
                var listItemValue = listObj[i];

                EditorGUI.indentLevel++;
                using (new EditorGUILayout.HorizontalScope())
                {
                    bool bListItemChnged = false;
                    string label = string.Format("[{0}]", i);
                    object newValue = null;
                    DrawBaseObject(label, listItemValue, itemType, ref newValue, ref bListItemChnged);
                    if (bListItemChnged)
                    {
                        listObj[i] = newValue;
                        SetChanged();
                    }
                    if (GUILayout.Button("+", GUILayout.Width(20)))
                        insertIndex = i;
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                        removeIndex = i;
                }
                EditorGUI.indentLevel--;
            }
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
