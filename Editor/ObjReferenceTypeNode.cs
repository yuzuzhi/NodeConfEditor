using System.Collections;
using System.Collections.Generic;
using cfeditor;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{
    public class ObjReferenceTypeNode : TableTypeNode
    {
        public ObjReferenceTypeNode(int id, NodeContiner parent, ConfScritableObject target) : base(id, parent, null)
        {
            Reset(target);
        }

        public void Reset(ConfScritableObject target)
        {
            SetName = "";
            m_sriptableObj = target;
            if (m_sriptableObj != null)
            {
                var tarType = m_sriptableObj.GetType();
                base.m_target = tarType.GetField("data").GetValue(m_sriptableObj);
                foreach (var VARIABLE in m_childrenByField)
                    parent.remove(VARIABLE.Value.linkNode);
                m_childrenByField.Clear();
                SetName = m_sriptableObj.name;
                if (string.IsNullOrEmpty(base.name))
                    SetName = Ctrl.GetNodeTitle(base.target);
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


        ConfScritableObject m_sriptableObj;
    }
}