using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace singleton
{
    // 싱글톤
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    var obj = GameObject.Find(typeof(T).Name) ?? new GameObject(typeof(T).Name);
                    _instance = obj.GetComponent<T>() ?? obj.AddComponent<T>();
                }
                return _instance;
            }
        }
    }
}
