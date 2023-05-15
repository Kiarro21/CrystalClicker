using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using UnityEngine.UI;

public class ServicesInit : MonoBehaviour, IAppsFlyerConversionData
{
    private const string _devKey = "e7z445jUKxaoAtTeqqyY9d";

    private void Awake()
    {
        AppsFlyer.setIsDebug(true);
        AppsFlyer.initSDK(_devKey, "", this);
        AppsFlyer.startSDK();
    }

    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("onConversionDataFail", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }
}
