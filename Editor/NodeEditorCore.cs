using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace cfeditor
{
    
    public class TableObject<T> : ScriptableObject where T : new()
    {
        public TableObject()
        {
            data = new T();
        }
        public string ident;
        public string name;
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
        public string ident;
        public UnityEngine.Object target;
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

        public List<int> listInt = new List<int>();
        public List<string> liststr = new List<string>();
        public List<Infomation> listobj = new List<Infomation>();

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

