﻿using Articy.The_Captain_s_Chair;
using Articy.The_Captain_s_Chair.Features;
using Articy.The_Captain_s_Chair.GlobalVariables;
using Articy.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RifRafInGamePopUp : MonoBehaviour
{
    public GameObject MainPopupPanel;
    public GameObject AcceptButton;
    public GameObject ResetPuzzleButton;
    public GameObject AcceptJobText, SuspendJobText;
    public MissionHint MissionHint;
    public GameObject QuitConfirmPopup;
    public RifRafExchangeJobBoard ExchangeBoard;
    public VolumeControl MusicVolume;
    public VolumeControl SoundFXVolume;
    public MCP MCP;
    public Text Cash;
    [Header("MainPopUpButtons")]
    public Button[] MainPopUpButtons;

    private void Awake()
    {
        StaticStuff.PrintRifRafUI("RifRafInGamePopUp.Awake()");
        //  Debug.Log("RifRafInGamePopUp.Awake()");

        MainPopupPanel.SetActive(false);       
        this.MissionHint.Init();
        MissionHint.gameObject.SetActive(false);
        //ExchangeBoard.gameObject.SetActive(false); // monewui
        QuitConfirmPopup.gameObject.SetActive(false);

        this.gameObject.SetActive(false); // monewui           
    }
    public void SetMCP(MCP mcp)
    {
        //StaticStuff.PrintRifRafUI("RifRafInGamePopUp.Init()");
        this.MCP = mcp;
    }
    public void OnClickBurger()
    {
        StaticStuff.PrintRifRafUI("OnClickBurger()");
        //Debug.Log("OnClickBurger()");
        if (PopupActiveCheck() == false) return;

        // burger won't be clickable with new UI so we don't need to check this        
        if (MainPopupPanel.activeSelf == true)
        {
            this.MCP.StartFreeRoam();            
            StaticStuff.SaveCurrentSettings("OnClickBurger()");
        }
        else
        {
            this.MCP.StartPopupPanel();
            Cash.text = ArticyGlobalVariables.Default.Captains_Chair.Crew_Money.ToString();
        }      
    }

    void ToggleMainPopUpButtons(bool isActive)
    {
        //Debug.Log("*********ToggleMainPopUpButtons(): " + isActive);
        foreach (Button b in MainPopUpButtons) b.interactable = isActive;
    }

    public void ToggleMainPopupPanel(bool isActive)
    {
        //Debug.LogWarning("monewui CHECK THIS ToggleMainPopupPanel(): " + isActive);
        MainPopupPanel.SetActive(isActive);       
        ToggleMainPopUpButtons(true); 
    }

    [Header("Menu Content")]
    public ArticyRef MissionFlowRef;
    public ArticyRef CodexRef;
    public ScrollRect ContentScrollView;
    public GameObject ExchangeContent;
    public GameObject TasksContent;
    public GameObject CodexContent;
    public GameObject ShipLogContent;
    public MenuButton ButtonPrefab;
    public Text FullJobNameText;
    public Text JobLocationText;
    public Text POCText;
    public Text JobDescriptionText;    
    MenuButton CurJobButton;
    enum eInGameMenus { EXCHANGE, TASKS, CODEX, SHIPS_LOG};
    eInGameMenus CurMenu;
    
    public void TurnOnPopupMenu( bool initContents )
    {        
       // Debug.Log("RifRafInGamePopUp.TurnOnPopupMenu()");
       
        ToggleMainPopupPanel(true);
        if (initContents == false) return;
        int debugVar = 0;
       /* ArticyGlobalVariables.Default.Mission.Exchange_001 = 1;
        ArticyGlobalVariables.Default.Mission.Exchange_002 = 1;
        ArticyGlobalVariables.Default.Mission.Exchange_003 = 1;
        ArticyGlobalVariables.Default.Mission.Exchange_Group_A = true;
        ArticyGlobalVariables.Default.Mission.Exchange_Group_B = true;
        ArticyGlobalVariables.Default.Mission.Task_001 = 1;
        ArticyGlobalVariables.Default.Mission.Task_002 = 1;
        ArticyGlobalVariables.Default.Mission.Task_Group_A = true;*/

        MusicVolume.Slider.value = this.MCP.GetMusicVolume();
        MusicVolume.Toggle.isOn = (MusicVolume.Slider.value > 0f);
        SoundFXVolume.Slider.value = this.MCP.GetSoundFXVolume();
        SoundFXVolume.Toggle.isOn = (SoundFXVolume.Slider.value > 0f);
                
        List<FlowFragment> containersToCheck = new List<FlowFragment>();        
        List<Job_Card> jobs = new List<Job_Card>();
        List<Articy.The_Captain_s_Chair.Codex> codexes = new List<Articy.The_Captain_s_Chair.Codex>();
        containersToCheck.Add(MissionFlowRef.GetObject() as FlowFragment);
        containersToCheck.Add(CodexRef.GetObject() as FlowFragment);             
        while(containersToCheck.Count > 0)
        {            
            FlowFragment container = containersToCheck[0];            
            containersToCheck.RemoveAt(0);
            if (container == null) Debug.LogError("WTF 1");
            if (container.Children == null) Debug.LogError("WTF 2");
            foreach(ArticyObject child in container.Children)
            {
                if (child == null) Debug.LogError("WTF 3");
                Job_Card job = child as Job_Card;
                Articy.The_Captain_s_Chair.Codex codex = child as Articy.The_Captain_s_Chair.Codex;               
                if (job != null) 
                {     
                   // Debug.Log("We've found a JOB called " + job.DisplayName);
                    jobs.Add(job);
                }               
                else if(codex != null)
                {
                    //Debug.Log("We've got a CODEX called: " + codex.DisplayName);
                    codexes.Add(codex);
                }
                else
                {
                    FlowFragment childFrag = child as FlowFragment;
                  if(childFrag.InputPins[0].Text.CallScript() == true)
                   //if(true)
                    {
                       // Debug.Log("add container: " + childFrag.DisplayName + " to the containers to check");
                        containersToCheck.Add(childFrag);
                    }
                    else
                    {
                        //Debug.Log("Do not add container: " + childFrag.DisplayName + " to containers to check;");
                    }
                }
            }
            if(debugVar++ > 100)
            {
                Debug.LogError("something in the loop is messed up");
                break;
            }
        }

        //Debug.Log("num jobs: " + jobs.Count);
        foreach (Job_Card job in jobs)
        {
            MenuButton button = Instantiate<MenuButton>(ButtonPrefab);            

            button.JobNameText.text = job.Template.Exchange_Mission.Job_Name;
            button.JobNumText.text = job.Template.Exchange_Mission.Job_ID;

            button.JobLocation = job.Template.Exchange_Mission.Job_Location;
            button.PointOfContact = job.Template.Exchange_Mission.Point_Of_Contact;
            button.JobDescription = job.Template.Exchange_Mission.Job_Description;            
            if (job.Template.Exchange_Mission.Job_Type == Job_Type.Exchange) button.transform.SetParent(ExchangeContent.transform);
            else button.transform.SetParent(TasksContent.transform);

            button.ExchangeMission = job.Template.Exchange_Mission;
            button.LoadingScreen = job.Template.LoadingScreen;
            button.PuzzlesToPlay = job.Template.Mini_Game_Puzzles_To_Play;
            button.DialogueList = job.Template.Dialogue_List;
            button.SuccessResult = job.Template.Success_Mini_Game_Result;
            button.QuitResult = job.Template.Quit_Mini_Game_Result;
            button.SuccessSaveFragment = job.Template.Success_Save_Fragment;
            button.PaymentFragment = job.Template.payment;

            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => OnClickMenuButton(button));
        }

        foreach(Articy.The_Captain_s_Chair.Codex codex in codexes)
        {
            MenuButton button = Instantiate<MenuButton>(ButtonPrefab);
            button.JobNameText.text = codex.Template.Codex.Entry_Name;
            button.JobNumText.text = "";

            button.JobLocation = "";
            button.PointOfContact = "";
            button.JobDescription = codex.Template.Codex.Entry_Info;
            button.transform.SetParent(CodexContent.transform);

            

            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => OnClickMenuButton(button));
        }

        ToggleContent(false);
        ExchangeContent.SetActive(true);
        ContentScrollView.content = ExchangeContent.GetComponent<RectTransform>();
        InitMenuButtonInfo(null, eInGameMenus.EXCHANGE);

        if (FindObjectOfType<TheCaptainsChair>() != null)
        {   // main game
            AcceptJobText.SetActive(true);
            SuspendJobText.SetActive(false);
        }
        else
        {   // mini game
            AcceptJobText.SetActive(false);
            SuspendJobText.SetActive(true);
        }
    }

    private void Update()
    {
        if (FindObjectOfType<TheCaptainsChair>() == null)
        {
            AcceptButton.SetActive(true);
        }
        else
        {
            if (CurJobButton == null || CurMenu == eInGameMenus.CODEX || CurMenu == eInGameMenus.SHIPS_LOG)
            {
                AcceptButton.SetActive(false);
            }
            else
            {
                AcceptButton.SetActive(true);
            }
        }            
    }

    public MenuButton GetCurJobButton()
    {
        return CurJobButton;
    }

    /*private void OnGUI()
    {
        string s = "CurMenu: " + CurMenu + "\nCurButton: " + (CurJobButton == null ? "null" : CurJobButton.JobNameText.text);
        if(GUI.Button(new Rect(0,0,300,100), s ))
        {
            ClearContent();          
        }
    }*/

    void InitMenuButtonInfo(MenuButton button, eInGameMenus menu)
    {
        CurMenu = menu;
        CurJobButton = button;        

        FullJobNameText.text = (button == null ? "" : button.JobNameText.text);
        JobLocationText.text = (button == null ? "" : button.JobLocation);
        POCText.text = (button == null ? "" : button.PointOfContact);
        JobDescriptionText.text = (button == null ? "" : button.JobDescription);
    }

    void ClearContent()
    {
        ToggleContent(true);
        foreach (Transform child in ExchangeContent.transform) Destroy(child.gameObject);
        foreach (Transform child in TasksContent.transform) Destroy(child.gameObject);
        foreach (Transform child in CodexContent.transform) Destroy(child.gameObject);
        foreach (Transform child in ShipLogContent.transform) Destroy(child.gameObject);
    }
    void ToggleContent(bool isActive)
    {
        ExchangeContent.SetActive(isActive);
        TasksContent.SetActive(isActive);
        CodexContent.SetActive(isActive);
        ShipLogContent.SetActive(isActive);
    }
    
    public void OnClickMenuTab(Button button)
    {
        GameObject currentContent = ExchangeContent;
        ToggleContent(false);
        if (button.name.Contains("Exchange") && CurMenu != eInGameMenus.EXCHANGE)
        {
            ExchangeContent.SetActive(true);
            currentContent = ExchangeContent;
            InitMenuButtonInfo(null, eInGameMenus.EXCHANGE);
        }
        else if(button.name.Contains("Task") && CurMenu != eInGameMenus.TASKS)
        {
            TasksContent.SetActive(true);
            currentContent = TasksContent;
            InitMenuButtonInfo(null, eInGameMenus.TASKS);
        }
        else if (button.name.Contains("Codex") && CurMenu != eInGameMenus.CODEX)
        {
            CodexContent.SetActive(true);
            currentContent = CodexContent;
            InitMenuButtonInfo(null, eInGameMenus.CODEX);
        }
        else if (button.name.Contains("Log") && CurMenu != eInGameMenus.SHIPS_LOG)
        {
            ShipLogContent.SetActive(true);
            currentContent = ShipLogContent;
            InitMenuButtonInfo(null, eInGameMenus.SHIPS_LOG);
        }

        ContentScrollView.content = currentContent.GetComponent<RectTransform>();
    }
    void OnClickMenuButton(MenuButton button)
    {
        Debug.Log("OnClickMenuButton(): " + button.name);
        InitMenuButtonInfo(button, CurMenu);
    }

    public void ShutOffExchangeBoard()
    {
        Debug.Log("ShutOffExchangeBoard()");
        ExchangeBoard.ShutOffQuitAcceptPopups();        
        ToggleExchangeBoard(false);
        ClearContent();
    }

    public bool MenusActiveCheck()
    {
       // Debug.LogWarning("monewui MenusActiveCheck() CHECK THIS .PopupActiveCheck(): " + PopupActiveCheck() + ", MainPopupPanel.activeSelf: " + MainPopupPanel.activeSelf);        
        return PopupActiveCheck() && MainPopupPanel.activeSelf == false;
    }
    public bool PopupActiveCheck()
    {
        return MissionHint.gameObject.activeSelf == false /*&& this.gameObject.activeSelf == false*/ && QuitConfirmPopup.gameObject.activeSelf == false; 
    }

#region MAIN_POPUP
    
    
    public void ToggleExchangeBoard(bool isActive)
    {
        Debug.LogError("monewui FIX THIS ToggleExchangeBoard(): " + isActive);
        if (isActive == true) ExchangeBoard.FillBoard();        
        ToggleMainPopUpButtons(!isActive);
        if (isActive == false) ClearContent();
    }

    public void OnClickExchangeBoard()
    {
        StaticStuff.PrintRifRafUI("OnClickExchangeBoard()");      
        if (PopupActiveCheck() == false) return;

        if (FindObjectOfType<TheCaptainsChair>() != null)
        {
            ToggleExchangeBoard(true);
        }
        else
        {
            Mini_Game_Jump jumpSave = ArticyDatabase.GetObject<Mini_Game_Jump>("Mini_Game_Data_Container");
            ArticyGlobalVariables.Default.Mini_Games.Coming_From_Main_Game = false;
            ArticyGlobalVariables.Default.Mini_Games.Returning_From_Mini_Game = true;
            ArticyGlobalVariables.Default.Mini_Games.Mini_Game_Success = false;
            string sceneName = jumpSave.Template.Quit_Mini_Game_Result.SceneName;
            FindObjectOfType<MCP>().LoadNextScene(sceneName, null, jumpSave);
        }
    }

    public void BailMiniGame()
    {
        Mini_Game_Jump jumpSave = ArticyDatabase.GetObject<Mini_Game_Jump>("Mini_Game_Data_Container");
        ArticyGlobalVariables.Default.Mini_Games.Coming_From_Main_Game = false;
        ArticyGlobalVariables.Default.Mini_Games.Returning_From_Mini_Game = true;
        ArticyGlobalVariables.Default.Mini_Games.Mini_Game_Success = false;
        string sceneName = jumpSave.Template.Quit_Mini_Game_Result.SceneName;
        FindObjectOfType<MCP>().LoadNextScene(sceneName, null, jumpSave);
    }

    public void ResetGameClicked()
    {
        StaticStuff.PrintRifRafUI("ResetGameClick");
        ToggleMissionHint(false);
        this.MCP.StartFreeRoam();
    }

    public void ToggleMissionHint(bool isActive)
    {
        //StaticStuff.PrintRifRafUI("ToggleMissionHint(): " + isActive);        
        //Debug.Log("ToggleMissionHint(): " + isActive);        
        //MissionHint.ToggleResetMiniGameButton(false);
        if (isActive == true)
        {
            // Debug.LogWarning("Get the hint ready");
            MissionHint.SetupHint();
           /* if(FindObjectOfType<MiniGame>() != null) // monewui took out Feb 3
            {
                MissionHint.ToggleResetMiniGameButton(true);
            }*/
        }
        MissionHint.gameObject.SetActive(isActive);
        ToggleMainPopUpButtons(!isActive);
    }

    public void OnClickMissionHint()
    {
        StaticStuff.PrintRifRafUI("OnClickMissionHint()");
      //  Debug.Log("OnClickMissionHint()");
        if (PopupActiveCheck() == false) return;

        ToggleMissionHint(true);
    }

    public void ShowResultsText(string result)
    {
         // Debug.Log("ShowResultsText()");
        MissionHint.gameObject.SetActive(true);
        MissionHint.HintText.text = result;
        //MissionHint.ToggleResetMiniGameButton(false);
        ResetPuzzleButton.SetActive(false);
    }
    public void HideResultsText()
    {
        //Debug.Log("HideResultsText()");
        MissionHint.gameObject.SetActive(false);
    }

    public void OnClickResumeGame()
    {
        //StaticStuff.PrintRifRafUI("OnClickResumeGame() PUT THIS BACK");
       // Debug.Log("OnClickResumeGame()");
        if (PopupActiveCheck() == false) return;

        StaticStuff.SaveCurrentSettings("OnClickResumeGame()");
        ClearContent();
        this.MCP.StartFreeRoam();

    }

    void ToggleQuitConfirmPopUp(bool isActive)
    {
        QuitConfirmPopup.gameObject.SetActive(isActive);
        ToggleMainPopUpButtons(!isActive);
    }

    public void OnClickQuitToMainMenu()
    {
        StaticStuff.PrintRifRafUI("OnClickQuitToMainMenu()");
       // Debug.Log("OnClickQuitToMainMenu()");
        if (PopupActiveCheck() == false) return;
        
        //QuitConfirmPopup.gameObject.SetActive(true);
        ToggleQuitConfirmPopUp(true);
    }
    public void OnClickQuitToMainCancel()
    {
       // Debug.Log("OnClickQuitToMainCancel()");
        //QuitConfirmPopup.gameObject.SetActive(false);
        ToggleQuitConfirmPopUp(false);
    }
    public void OnClickQuitToMainConfirm()
    {
       // Debug.Log("OnClickQuitToMainConfirm()");
        //QuitConfirmPopup.gameObject.SetActive(false);
        ToggleQuitConfirmPopUp(false);
        this.MCP.LoadNextScene("Front End Launcher");
    }
    
    public void OnSliderAudioVolume(Slider slider)
    {
        StaticStuff.PrintRifRafUI("OnSliderAudioVolume()");
      //  Debug.Log("OnSliderAudioVolume(): " + slider.gameObject.name);
        if(slider == MusicVolume.Slider)
        {
           // Debug.Log("RifRafMenuUI().OnSliderAudioVolume() Music: " + slider.value);
            this.MCP.SetMusicVolume((int)slider.value);
            MusicVolume.Toggle.isOn = (MusicVolume.Slider.value > 0f);
        }
        else
        {
          //  Debug.Log("RifRafMenuUI().OnSliderAudioVolume() SFX: " + slider.value);
            this.MCP.SetSoundFXVolume((int)slider.value);
            SoundFXVolume.Toggle.isOn = (SoundFXVolume.Slider.value > 0f);
        }        
    }

    public void OnToggleAudioVolume(Toggle toggle)
    {
        //Debug.Log("OnToggleAudioVolume(): " + toggle.gameObject.name);
        if (toggle == MusicVolume.Toggle)
        {
            if (toggle.isOn == true) this.MCP.SetMusicVolume(100);
            else this.MCP.SetMusicVolume(0);
            MusicVolume.Slider.value = this.MCP.GetMusicVolume();
        }
        else
        {
            if (toggle.isOn == true) this.MCP.SetSoundFXVolume(100);
            else this.MCP.SetSoundFXVolume(0);
            SoundFXVolume.Slider.value = this.MCP.GetSoundFXVolume();
        }        
    }
#endregion

#region MISSION_HINT
    public void OnClickMissionHintBack()
    {
        StaticStuff.PrintRifRafUI("OnClickMissionHintBack()");
      //  Debug.Log("OnClickMissionHintBack()");

        ToggleMissionHint(false);
    }
#endregion

    [System.Serializable]
    public class VolumeControl
    {
        public Slider Slider;
        public Toggle Toggle;
    }
}
