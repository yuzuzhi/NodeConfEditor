using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace cfeditor
{
    
    public class TableObject<T> : ScriptableObject
    {
        public T data;
        public List<ScriptableObject> localOjbects;
    }

    public class ListObject<T> : ScriptableObject
    {
        public List<T> data;
    }

    public class IntListObject: ListObject<int> { }
    public class FloatListObject : ListObject<float> { }
    public class StringListObject : ListObject<string> { }

    [Serializable]
    public struct ObjReference
    {
        public int ident;
        public string type;
        public bool local;
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
        public string name;
        public int age;

        public Infomation info;

        public List<int> listInt;
        public List<string> liststr;
        public List<Infomation> listobj;

        public ObjReference refField;
    }


}

