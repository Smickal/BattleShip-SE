using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScripts : MonoBehaviour
{
    // Start is called before the first frame update
    public static SingletonScripts instance;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

}
