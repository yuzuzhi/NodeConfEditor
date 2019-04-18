using System.Reflection;

namespace cfeditor
{
    public class ShowBehvCtrl
    {
        public virtual bool IsDynField(object target, FieldInfo fieldinfor)
        {
            //fieldinfor.get
            return true;
        }
    }

}