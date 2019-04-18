using System.Collections;
using System.Collections.Generic;
using cfeditor;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{


    public class ObjReferenceTypeNode : TableTypeNode
    {
        public ObjReferenceTypeNode(int id, NodeContiner parent, ScriptableObject target) : base(id, parent, null)
        {
            Reset(target);
        }

        public void Reset(ScriptableObject target)
        {

            m_sriptableObj = target;
            if (m_sriptableObj != null)
            {
                base.m_target = m_sriptableObj.GetType().GetField("data").GetValue(m_sriptableObj);
                foreach (var VARIABLE in m_childrenByField)
                    parent.remove(VARIABLE.Value.linkNode);
                m_childrenByField.Clear();
            }
        }


        public override void OnDrawGUI()
        {

            var rect = EditorGUILayout.GetControlRect(true);
            float btnwid = 30;
            float edgwid = 5;
            rect.x = position.width - btnwid - edgwid;
            rect.width = btnwid;
            m_heightStart = Styles.titleBarheight + kSingleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (GUI.Button(rect, "S", EditorStyles.miniButtonRight))
            {
                AssetDatabase.SaveAssets();
                this.ClearChanged();

            }

            base.OnDrawGUI();
            if (hasChanged)
                EditorUtility.SetDirty(m_sriptableObj);
        }


        ScriptableObject m_sriptableObj;
    }
}