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
            return "";
        }
    }
    
}