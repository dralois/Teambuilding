using System;
using System.Collections;
using System.Collections.Generic;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    // UI Panel Renderers
    // Note: normally you can just use one Panel Rendere and just hide or swap in/out
    // (using ve.style.display) elements at runtime. It's a lot more efficient
    // to use a single PanelRenderer.


    public PanelRenderer Screen_JoinSession;

    public PanelRenderer Screen_Menu;
    
    public PanelRenderer Screen_CreateGame;
    public PanelRenderer Screen_JoinGame;

    public PanelRenderer Screen_Settings;

    public PanelRenderer Screen_Statistic_manager;
    public PanelRenderer Screen_Statistic_employee;




    // Pre-loaded UI assets (ie. UXML/USS).
    //List for choosing a game
    public VisualTreeAsset m_GameListItem;
    public StyleSheet m_GameListItemStyles;

    //List for the statistics (SessionStat and employeestat)
    public VisualTreeAsset m_statisticListItem;
    public StyleSheet m_statisticListItemStyles;



    // The Panel Renderer can optionally track assets to enable live
    // updates to any changes made in the UI Builder for specific UI
    // assets (ie. UXML/USS).
    private List<Object> m_TrackedAssetsForLiveUpdates;

    // We need to update the values of some UI elements so here are
    // their remembered references after being queried from the cloned
    // UXML.
    //Values of the statistic (employee)            //we need a list for that
    /*private Label stat_e_name;
    private Label stat_e_note;

    private Label stat_e_value1;
    private Label stat_e_value2;
    private Label stat_e_value3;

    //Values of the statistic (session)             //we need a list for that
    private Label stat_sess_name;
    private Label stat_sess_date;
    private Label stat_sess_participants;

    private Label stat_sess_value1;
    private Label stat_sess_value2;
    private Label stat_sess_value3;*/


    //variable
    private Boolean manager = false;



    // OnEnable
    // Register our postUxmlReload callbacks to be notified if and when
    // the UXML or USS assets being user are changed (by the UI Builder).
    // In these callbacks, we just rebind UI VisualElements to data or
    // to click events.
    private void OnEnable()
    {
        Screen_JoinSession.postUxmlReload = BindJoinSessionScreen;
        Screen_Menu.postUxmlReload = BindMenuScreen;
        Screen_CreateGame.postUxmlReload = BindCreateGameScreenScreen;
        Screen_JoinGame.postUxmlReload = BindJoinGameScreen;
        Screen_Settings.postUxmlReload = BindSettingsScreen;
        Screen_Statistic_employee.postUxmlReload = BindEmployeeStatisticScreen;
        Screen_Statistic_manager.postUxmlReload = BindManagerStatisticScreen;


        m_TrackedAssetsForLiveUpdates = new List<Object>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
#if !UNITY_EDITOR
        if (Screen.fullScreen)
            Screen.fullScreen = false;
#endif
        //open LogIn as start menu
        GoToStartScreen();
    }


    // Update is called once per frame
    void Update()
    {
        /* If any of our UI Labels have not been bound, do nothing.
            if (m_SpeedLabel == null || m_Tanks.Length == 0 || m_Player1Movement == null || m_Player1Health == null)
            return;*/

        /* Update UI label text.
        m_SpeedLabel.text = m_Player1Movement.m_Speed.ToString();*/
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // Screen Transition Logic

    //change Screen 
    private void ScreenChange(PanelRenderer from, PanelRenderer to)
    {
        from.visualTree.style.display = DisplayStyle.None;
        from.gameObject.GetComponent<UIElementsEventSystem>().enabled = false;
        to.enabled = true;
    }

    private void GoToStartScreen()
    {
        SetScreenEnableState(Screen_JoinSession, false); //als erstes key eingeben um session beizutreteten
        SetScreenEnableState(Screen_Menu, true);
        SetScreenEnableState(Screen_CreateGame, false);
        SetScreenEnableState(Screen_Settings, false);
        SetScreenEnableState(Screen_JoinGame, false);
        SetScreenEnableState(Screen_Statistic_employee, false);
        SetScreenEnableState(Screen_Statistic_manager, false);


    }

    void SetScreenEnableState(PanelRenderer screen, bool state)
    {
        if (state)
        {
            screen.visualTree.style.display = DisplayStyle.Flex;
            screen.enabled = true;
            screen.gameObject.GetComponent<UIElementsEventSystem>().enabled = true;
        }
        else
        {
            screen.visualTree.style.display = DisplayStyle.None;
            screen.enabled = false;
            screen.gameObject.GetComponent<UIElementsEventSystem>().enabled = false;
        }
    }



    ///////////////////////////////////////////////////////////////////////////////
    ///Bind 
    ///

    private IEnumerable<Object> BindMenuScreen()
    {
        //bind root 
        var root = Screen_Menu.visualTree;

        //set button function
        var joinButton = root.Q<Button>("join_game_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                ScreenChange(Screen_Menu, Screen_JoinGame);
            };
        }
        
        joinButton = root.Q<Button>("create_game_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                ScreenChange(Screen_Menu, Screen_CreateGame);
            };
        }

        joinButton = root.Q<Button>("statistics_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                if (manager)
                {
                    ScreenChange(Screen_Menu, Screen_Statistic_manager);
                } else
                {
                    ScreenChange(Screen_Menu, Screen_Statistic_employee);
                }
                
            };
        }

        joinButton = root.Q<Button>("settings_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                ScreenChange(Screen_Menu, Screen_Settings);
            };
        }
        return null;
    }

    private IEnumerable<Object> BindCreateGameScreenScreen()
    {
        //bind root 
        var root = Screen_CreateGame.visualTree;

        // TODO list view mit auswählen

        //set button function create game
        var createButton = root.Q<Button>("create_game_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                //TODO je nachdem welches spiel ausgewählt wurde
            };
        }
        return null;
    }

    private IEnumerable<Object> BindJoinGameScreen()
    {
        //bind root 
        var root = Screen_CreateGame.visualTree;

        //TODO
        //in list view anzeigen welches spiel vom manager ausgewählt wurde

        //set button function create game
        var createButton = root.Q<Button>("join_game_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                //TODO
                // zum Spiel screen
            };
        }
        return null;
    }

    private IEnumerable<Object> BindJoinSessionScreen()
    {
        //bind root 
        var root = Screen_CreateGame.visualTree;

        //TODO
        // input des session key und des namen

        //set button function create game

        var createButton = root.Q<Button>("join_session_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                //TODO 
                //beim clicken soll in die richtige session gegangen werden.
                ScreenChange(Screen_JoinSession, Screen_Menu);
            };
        }
        return null;
    }

    private IEnumerable<Object> BindSettingsScreen()
    {
        //bind root 
        var root = Screen_CreateGame.visualTree;

        //TODO
        //Name input 

        //set button function create game
        var createButton = root.Q<Button>("logout_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                Debug.Log("drinnen ist er logout");
                //TODO
                //alle daten müssen gelöscht werden von der vorherigen Session
                ScreenChange(Screen_Settings, Screen_JoinSession);
            };
        }
        return null;
    }

    private IEnumerable<Object> BindEmployeeStatisticScreen()
    {
        //bind root 
        var root = Screen_CreateGame.visualTree;

        //TODO
        //List view implementieren


        //info: button hat keine funktion
        return null;
    }

    private IEnumerable<Object> BindManagerStatisticScreen()
    {
        //bind root 
        var root = Screen_CreateGame.visualTree;

        //TODO
        //list view implementieren mit 

        //set button function create game
        var createButton = root.Q<Button>("change_statistic_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                //TODO
                //List view inhalt wird geändert zwischen employee und session informationen
                //label des buttons wird geändert 
            };
        }
        return null;
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///Game Logic
    
}
