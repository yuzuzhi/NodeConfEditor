using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{
    public class TableTypeNode: Node
    {

        public ScriptableObject target
        {
            get { return m_target; }
        }

        public TableTypeNode(int id, EditorWindow parent, ScriptableObject target):base(id, parent)
        {
            m_target = target;
        }
        
        public override void OnDrawGUI()
        {
            var datafield = m_target.GetType().GetField("data");
            if (datafield == null)
                return;



            var dataType = datafield.FieldType;
            var dataValue = datafield.GetValue(m_target);

            var fieldsList = dataType.GetFields();

            for (int i = 0; i < fieldsList.Length; i++)
            {
                var fieldInfo = fieldsList[i];

                bool bchanged = false;
                object newValue = null;
                if (DrawBaseObject(fieldInfo.Name, fieldInfo.GetValue(dataValue), fieldInfo.FieldType, ref newValue,
                    ref bchanged)) ;
                else if(fieldInfo.FieldType.IsList())


                if (hasChanged)
                {
                    fieldInfo.SetValue(dataValue, newValue);
                    SetChanged();
                }
            }

            if (hasChanged)
            {
                EditorUtility.SetDirty(m_target);
            }

        }
        
        ScriptableObject m_target;
    }
}