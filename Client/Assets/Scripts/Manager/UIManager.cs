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
    public PanelRenderer Screen_Menu;
    public PanelRenderer Screen_CreateGame;
   



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
        Screen_Menu.postUxmlReload = BindLogInScreen;
        Screen_CreateGame.postUxmlReload = BindCreateGameScreenScreen;
        

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

    private void ScreenChange(PanelRenderer from, PanelRenderer to)
    {
        SetScreenEnableState(from, false);
        SetScreenEnableState(to, true);
        
    }

    private void GoToLogIn()
    {
        SetScreenEnableState(Screen_Menu, true);
        SetScreenEnableState(Screen_CreateGame, false);
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
        var root = Screen_Menu.visualTree;

        //set button function
        var joinButton = root.Q<Button>("create_session_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                ScreenChange(Screen_Menu, Screen_CreateGame);
                TransitionScreens(Screen_Menu, Screen_CreateGame);
            };
        }
        return null;
    }

    private IEnumerable<Object> BindCreateGameScreenScreen()
    {
        //bind root 
        var root = Screen_CreateGame.visualTree;

        //set button function create game
        var createButton = root.Q<Button>("create_game_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                
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
