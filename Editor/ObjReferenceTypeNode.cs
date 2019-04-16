using System.Collections;
using System.Collections.Generic;
using cfeditor;
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
            base.OnDrawGUI();
        }


        ScriptableObject m_sriptableObj;
    }
}