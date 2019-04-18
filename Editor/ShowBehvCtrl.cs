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

    public class SampShowBehvCtrl : ShowBehvCtrl
    {
        public override bool IsDynField(object target, FieldInfo fieldinfor)
        {
            PropDescAttribute propDesc = null;
            if (!fieldinfor.GetAttribute(ref propDesc))
                return false;

            return propDesc.DynTypes != null;
        }

        public override GUIContent GetFieldLabel(object target, FieldInfo fieldinfor)
        {
            PropDescAttribute propDesc = null;
            if (!fieldinfor.GetAttribute(ref propDesc))
                return base.GetFieldLabel(target, fieldinfor);

            return new GUIContent(propDesc.name, fieldinfor.FieldType.Name);
        }

        public override bool FieldVisible(object target, FieldInfo fieldinfor)
        {
            PropDescAttribute propDesc = null;
            if (!fieldinfor.GetAttribute(ref propDesc))
                return base.FieldVisible(target, fieldinfor);

            return !propDesc.ishide;
        }

        public override string GetNodeTitle(object target)
        {
            TableDescAttribute tableDesc = null;
            if (!target.GetType().GetAttribute(ref tableDesc))
                return base.GetNodeTitle(target);

            return tableDesc.name;
        }
    }


}