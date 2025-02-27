﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MiniGame : MonoBehaviour
{
    protected MiniGameMCP MiniGameMCP;
    protected bool IsSolo;
    protected bool DialogueActive;    
    public string SceneName;

    //[Header("Sound")]
   // public List<SoundFX.FXInfo> SoundFXUsedInScene;

    [Header("Debug")]
    public Text DebugText;
    public virtual void Awake()
    {
      //  Debug.Log("MiniGame.Awake()");
        DialogueActive = false;        
        MiniGameMCP mcp = FindObjectOfType<MiniGameMCP>();
        if(mcp == null)
        {
            //Debug.Log("we're running solo");
            IsSolo = true;
            SceneName = SceneManager.GetActiveScene().name;
        }
        else
        {
            //Debug.Log("we're part of a MCP group");
            IsSolo = false;
        }                        
    }
    public virtual void Init(MiniGameMCP mcp, string sceneName, List<SoundFX.FXInfo> soundFXUsedInScene, Button resetButton)
    {
        //Debug.Log("MiniGame.Init()");
        this.MiniGameMCP = mcp;
        SceneName = sceneName;
        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(ResetGame);
       // this.SoundFXUsedInScene = soundFXUsedInScene;
    }

    public virtual void ResetGame()
    {
        //Debug.Log("MiniGame.ResetGame()");
        FindObjectOfType<RifRafInGamePopUp>().ResetGameClicked();
    }

    public virtual void BeginPuzzleStartTime()
    {
        if (DialogueActive == false) ResetPuzzleTimer();
    }

    public virtual void TMP_WinGame()
    {

    }
    
    public virtual void ResetPostDialogueState()
    {
       // Debug.Log("MiniGame.ResetPostDialogueState()");
    }
    public void DialogueEnded()
    {
      //  Debug.Log("MiniGAme.DialogueEnded()");
        SetDialogueActive(false);
        ResetPuzzleTimer();
        ResetPostDialogueState();
        if(FindObjectOfType<MCP>() != null)
        {
            FindObjectOfType<MCP>().StartMiniGame();
        }
    }
    
    public void SetDialogueActive( bool val )
    {
      //  Debug.Log("MiniGame.SetDialogueActive() val: " + val);
        DialogueActive = val;
    }

    protected float PuzzleStartTime = 0f;
    void ResetPuzzleTimer()
    {
        PuzzleStartTime = Time.time;
    }
     
    public void EndPuzzleTime(bool didFinish)
    {
        float gameTime = Time.time - PuzzleStartTime;
        TimeSpan time = TimeSpan.FromSeconds(gameTime);
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("Total Time To Solve", time);
        string tag = (didFinish == true ? "_solved" : "_quit");
        StaticStuff.TrackEvent(SceneName + tag, parameters);
    }

    [System.Serializable]
    public class MiniGameTransformSave
    {
        public Vector3 StartPos;
        public Vector3 StartLocalPos;
        public Vector3 StartLocalScale;
        public Quaternion StartRot;
        public Quaternion StartLocalRot;

        public MiniGameTransformSave( Transform t )
        {
            StartPos = t.position;
            StartLocalPos = t.localPosition;
            StartLocalScale = t.localScale;
            StartRot = t.rotation;
            StartLocalRot = t.localRotation;
        }
        public void ResetTransform(Transform t)
        {
            t.position = StartPos;
           // t.localPosition = StartLocalPos;
            t.localScale = StartLocalScale;
            t.rotation = StartRot;
           // t.localRotation = StartLocalRot;
        }
    }
}