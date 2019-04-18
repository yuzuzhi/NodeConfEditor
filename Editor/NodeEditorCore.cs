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

        public List<string> fieldName;
        public List<List<ObjReference>> fieldValue;
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

