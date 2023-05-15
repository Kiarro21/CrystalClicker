using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Text _text;

    void Start()
    {
        _text.text = AppsFlyerSDK.AppsFlyer.getAppsFlyerId();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
