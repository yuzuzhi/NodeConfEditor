using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cfeditor
{
    [CreateAssetMenu]
    public class Settings : ScriptableObject
    {
        public string confpath;



        private static Settings m_Instance;
        public static Settings Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    var finded = AssetDatabase.FindAssets("t:Settings");
                    if (finded.Length > 0)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(finded[0]);
                        m_Instance = AssetDatabase.LoadAssetAtPath<Settings>(path);
                    }
                }
                return m_Instance;
            }
        }

        public static string rootPath
        {
            get
            {
                if (m_Instance != null)
                {
                    return AssetDatabase.GetAssetPath(m_Instance);
                }
                return "Assets/Editor/";
            }
        }

        public static string resPath
        {
            get { return rootPath + "res/"; }
        }

        
    }
}
