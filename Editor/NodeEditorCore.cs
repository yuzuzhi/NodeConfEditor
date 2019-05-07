using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace cfeditor
{
    public partial class ConfScritableObject
    {
        const string TableObject = "TableConfScritableObject`1";

        public static Type[] GetAllTypes()
        {
            var t = typeof(ConfScritableObject);
            var ass = t.Assembly.GetTypes();
            var listOfBs = (from assemblyType in ass
                            where t.IsAssignableFrom(assemblyType) && assemblyType.Name != TableObject && assemblyType != t
                            select assemblyType).ToArray();
            return listOfBs;
        }
    }
     public partial class ConfScritableObject: ScriptableObject
    {
        public string ident;
        public string name;
    }
    public class TableConfScritableObject<T> : ConfScritableObject where T : new()
    {
        public TableConfScritableObject()
        {
            data = new T();
        }
        public T data;
        public List<ScriptableObject> localOjbects;

        public List<string> fieldName;
        public List<List<ObjReference>> fieldValue;
    }
    

    [Serializable]
    public struct ObjReference
    {
        public string ident;
        public string SelfType;
        public UnityEngine.Object target;
    }


    [Serializable]
    public class Infomation
    {
        public string desc;
        public float height;
    }


    [Serializable]
    public class Student
    {
        public Infomation info;
        public string name;
        public int age;
        public Vector3 vec3;


        public List<int> listInt = new List<int>();
        public List<string> liststr = new List<string>();
        public List<Vector3> listVec3 = new List<Vector3>();
        public List<Infomation> listobj = new List<Infomation>();
        public List<ObjReference> listrefer = new List<ObjReference>();

        public ObjReference refField;
    }

    [Serializable]
    public class TestClass
    {
        public int aaa;
        public float bbb;
        public string ccc;
    }


}

