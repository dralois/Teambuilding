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
    public PanelRenderer me_LogInScreen;
    public PanelRenderer m_CreateGameScreen;
    public PanelRenderer me_MainMenu_GameScreen;
    public PanelRenderer e_MainMenu_settingsScreen;         //employee settings
    public PanelRenderer m_MainMenu_settingsScreen;         //manager settings
    public PanelRenderer e_MainMenu_statScreen;             //employee statistic
    public PanelRenderer m_MainMenu_statScreen;             //manager statistic



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
    private Boolean manager;



    // OnEnable
    // Register our postUxmlReload callbacks to be notified if and when
    // the UXML or USS assets being user are changed (by the UI Builder).
    // In these callbacks, we just rebind UI VisualElements to data or
    // to click events.
    private void OnEnable()
    {
        me_LogInScreen.postUxmlReload = BindLogInScreen;
        m_CreateGameScreen.postUxmlReload = BindCreateGameScreenScreen;
        me_MainMenu_GameScreen.postUxmlReload = BindGameScreen;
        e_MainMenu_settingsScreen.postUxmlReload = Bind_e_SettingsScreen;
        m_MainMenu_settingsScreen.postUxmlReload = Bind_m_SettingsScreen;
        e_MainMenu_statScreen.postUxmlReload = Bind_e_StatisticScreen;
        m_MainMenu_statScreen.postUxmlReload = Bind_m_StatisticScreen;

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
        GoToLogIn();
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

    private void GoToLogIn()
    {
        SetScreenEnableState(me_LogInScreen, true);
        SetScreenEnableState(m_CreateGameScreen, false);
        SetScreenEnableState(me_MainMenu_GameScreen, false);
        SetScreenEnableState(e_MainMenu_settingsScreen, false);
        SetScreenEnableState(m_MainMenu_settingsScreen, false);
        SetScreenEnableState(e_MainMenu_statScreen, false);
        SetScreenEnableState(m_MainMenu_statScreen, false);
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

    //transition from a screen to another screen
    IEnumerator TransitionScreens(PanelRenderer from, PanelRenderer to)
    {
        from.visualTree.style.display = DisplayStyle.None;
        from.gameObject.GetComponent<UIElementsEventSystem>().enabled = false;

        to.enabled = true;

        yield return null;
        yield return null;
        yield return null;

        to.visualTree.style.display = DisplayStyle.Flex;
        to.visualTree.style.visibility = Visibility.Hidden;
        to.gameObject.GetComponent<UIElementsEventSystem>().enabled = true;

        yield return null;
        yield return null;
        yield return null;

        to.visualTree.style.visibility = Visibility.Visible;

        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        from.enabled = false;
    }


    ///////////////////////////////////////////////////////////////////////////////
    ///Bind 
    ///

    private IEnumerable<Object> BindLogInScreen()
    {
        //bind root 
        var root = me_LogInScreen.visualTree;

        //get input name und session key
        var inputName = root.Q<TextField>("name_input");
        //TODO
        
        //set button function
        var joinButton = root.Q<Button>("join_session_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                //check if input is correct and open next screen depending on that.
                if (CheckLogInData())
                {
                    if (manager)
                    {
                        //transition to CreateGameScreen
                        TransitionScreens(me_LogInScreen, m_CreateGameScreen);
                    }
                    else
                    {
                        //transition to Game Menu
                        TransitionScreens(me_LogInScreen, me_MainMenu_GameScreen);
                    }
                }
            };
        }
        return null;
    }

    private IEnumerable<Object> BindCreateGameScreenScreen()
    {
        //bind root 
        var root = m_CreateGameScreen.visualTree;

        //set button function create game
        var createButton = root.Q<Button>("create_game_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                //check if input is given and open next screen depending on that.
                if (CheckIfGameSelected())
                {
                    if (manager)
                    {
                        //transition to CreateGameScreen
                        TransitionScreens(me_LogInScreen, m_CreateGameScreen);
                    }
                    else
                    {
                        //transition to Game Menu
                        TransitionScreens(me_LogInScreen, me_MainMenu_GameScreen);
                    }
                }
            };
        }
        return null;
    }

    private IEnumerable<Object> BindGameScreen()
    {
        //bind root 
        var root = me_MainMenu_GameScreen.visualTree;

        //set button function create game
        var joinGameButton = root.Q<Button>("join_game_button");
        if (joinGameButton != null)
        {
            //button function
            joinGameButton.clickable.clicked += () =>
            {
                //TODO
                //TransitionScreens(me_MainMenu_GameScreen, //InGameDesign//);
            };
        }

        //set choosen GameItem into Container
        //TODO

        return null;
    }

    private IEnumerable<Object> Bind_e_SettingsScreen()
    {
        //bind root 
        var root = e_MainMenu_settingsScreen.visualTree;

        //set button function create game
        var changeNameButton = root.Q<Button>("change_name_button");
        if (changeNameButton != null)
        {
            //button function
            changeNameButton.clickable.clicked += () =>
            {
                //TODO
                //change name
            };
        }
        return null;
    }

    private IEnumerable<Object> Bind_m_SettingsScreen()
    {
        //bind root 
        var root = e_MainMenu_settingsScreen.visualTree;

        //set button function create game
        var changeNameButton = root.Q<Button>("change_name_button");
        if (changeNameButton != null)
        {
            //button function
            changeNameButton.clickable.clicked += () =>
            {
                //TODO
                //change name
            };
        }

        //Button for Game change
        var gameChangeButton = root.Q<Button>("game_change_button");
        if (gameChangeButton != null)
        {
            //button function
            gameChangeButton.clickable.clicked += () =>
            {
                //go to create Game back
                TransitionScreens(m_MainMenu_settingsScreen, m_CreateGameScreen);
            };
        }
        return null;
    }

    private IEnumerable<Object> Bind_e_StatisticScreen()
    {
        //noch keine statistic für employee
        return null;
    }

    private IEnumerable<Object> Bind_m_StatisticScreen()
    {
        //bind root 
        var root = m_MainMenu_settingsScreen.visualTree;

        //list view change (employee / session view)
        var listviewChange = root.Q<Button>("listview_change_button");
        if (listviewChange != null)
        {
            //button function
            listviewChange.clickable.clicked += () =>
            {
                //change the listview contant
                //TODO
            };
        }
        return null;
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///Game Logic

    private bool CheckLogInData()
    {
        //TODO
        return false;
    }

    private bool CheckIfGameSelected()
    {
        //TODO
        return false;
    }
}
