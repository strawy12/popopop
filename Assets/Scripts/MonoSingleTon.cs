using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T inst = null;

    public static T Inst
    {
        get
        {
            if(inst == null)
            {
                inst = FindObjectOfType<T>();
                if (inst == null)
                {
                    inst = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }   
            }
            DontDestroyOnLoad(inst);
            return inst;
        }
    }    
}
