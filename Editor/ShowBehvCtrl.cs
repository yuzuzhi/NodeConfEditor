using System;
using System.Reflection;
using UnityEngine;

namespace cfeditor
{
    public class ShowBehvCtrl
    {
        public virtual bool IsDynField(object target, FieldInfo fieldinfor)
        {
            return false;
        }

        public virtual Type[] DynTypeList(object target, FieldInfo fieldinfor)
        {
            return null;
        }

        public virtual GUIContent GetFieldLabel(object target, FieldInfo fieldinfor)
        {
            return new GUIContent(fieldinfor.Name, fieldinfor.FieldType.Name);
        }

        public virtual bool FieldVisible(object target, FieldInfo fieldinfor)
        {
            return true;
        }

        public virtual string GetNodeTitle(object target)
        {
            return target == null ? "null" : "";
        }


        public virtual string[] toNameList(Type[] typeList)
        {
            string[] typeNameList = new string[typeList.Length];
            for (int i = 0; i < typeNameList.Length; i++)
                typeNameList[i] = typeList[i].Name;

            return typeNameList;
        }

        public int typeIndex(Type tarType, Type[] typeList)
        {
            int res = -1;
            for (int i = 0; i < typeList.Length; i++)
                if (tarType == typeList[i])
                {
                    res = i;
                    break;
                }

            return res;
        }
    }
    
}