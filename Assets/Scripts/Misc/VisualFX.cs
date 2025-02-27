﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualFX : MonoBehaviour
{    
    [System.Serializable]
    public class VisualFXInfo
    {
        public string Name;
        public GameObject FX;
    }
    public List<VisualFXInfo> VisualFXList;
    List<string> VisualFXNames = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (VisualFXInfo vfxInfo in VisualFXList)
        {
            vfxInfo.Name = vfxInfo.FX.name;
            VisualFXNames.Add(vfxInfo.Name);
        }
    }

    public GameObject PlayFX(string fxID, Vector3 pos)
    {
       // Debug.Log("******************PlayFX() " + fxID);
        int fxIndex = VisualFXNames.IndexOf(fxID);
        if (fxIndex == -1) { Debug.LogError("Trying to play a visual effect that doesn't exist: " + fxID); return null; }
        GameObject vfx = Instantiate(VisualFXList[fxIndex].FX, pos, Quaternion.identity) as GameObject;
        
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        ETFXProjectileScript et = vfx.GetComponent<ETFXProjectileScript>();                

        //Debug.Log("duration: " + ps.main.duration + ", loop: " + ps.main.loop + ", startDelay: " + ps.main.startDelay);
        if (ps == null)
        {
       //     Debug.Log("null PS");
        }
        else
        {
            if (ps.main.loop == true) vfx.tag = "Looped VFX";
        }
        if (et == null)
        {
    //        Debug.Log("null ET");
        }

      //  int x = 5;
     //   x++;

        return vfx;
    }
}

public static class VisualFXPlayer
{
    static VisualFX VisualFX;
    public static void Init(VisualFX visualFX)
    {
        VisualFX = visualFX;
    }

    public static GameObject Play(string fxID, Vector3 pos)
    {
        if (VisualFX == null) { Debug.LogError("Trying to play visual fx with no visual fx player"); return null; }
        return VisualFX.PlayFX(fxID, pos);
    }
}
