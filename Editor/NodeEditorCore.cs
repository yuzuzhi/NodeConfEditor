using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace cfeditor
{
    
    public class ConfObject<T> : ScriptableObject
    {
        public T data;
    }


    [Serializable]
    public struct ObjReference
    {
        public int ident;
        public string type;
    }



    [Serializable]
    public class Student
    {
        public string name;
        public int age;

        public ObjReference info;

        void ff()
        {
            Type t;
        }
    }


}

