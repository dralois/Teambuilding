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


    public PanelRenderer Screen_Start;
    public PanelRenderer Screen_JoinSession;
    public PanelRenderer Screen_Menu;
    public PanelRenderer Screen_CreateGame;
    public PanelRenderer Screen_JoinGame;
    public PanelRenderer Screen_Settings;
    public PanelRenderer Screen_Statistic_manager;
    public PanelRenderer Screen_Statistic_employee;

    public PanelRenderer Screen_GamePuzzle;

    //list view JOINGAME_screen
    public VisualTreeAsset games_item;
    public StyleSheet games_item_styles;

    //list view statistic_employee_screen
    public VisualTreeAsset statistic_employee_item;
    public StyleSheet statistic_employee_item_style;

    //list view statistic_manager_screen
    public VisualTreeAsset statistic_manager_item;
    public StyleSheet statistic_manager_item_style;

    //list view inGame overview
    public VisualTreeAsset overview_item;
    public StyleSheet overview_item_style;

    //Listen für die ListViews
    private List<Game> games = new List<Game>();
    private List<StatisticEmployee> statisticsEmployee = new List<StatisticEmployee>();
    private List<StatisticEmployee> statisticsManager = new List<StatisticEmployee>();
    private List<OverViewListData> overviewGamePuzzle = new List<OverViewListData>();                         //ist dort schon ein PuzzleTeil gesetzt?

    //Logic
    private Boolean manager = false;


    // OnEnable
    // Register our postUxmlReload callbacks to be notified if and when
    // the UXML or USS assets being user are changed (by the UI Builder).
    // In these callbacks, we just rebind UI VisualElements to data or
    // to click events.
    private void OnEnable()
    {
        //Bind Screens
        Screen_Start.postUxmlReload = BindStartScreen;
        Screen_JoinSession.postUxmlReload = BindJoinSessionScreen;
        Screen_Menu.postUxmlReload = BindMenuScreen;
        Screen_CreateGame.postUxmlReload = BindCreateGameScreenScreen;
        Screen_JoinGame.postUxmlReload = BindJoinGameScreen;
        Screen_Settings.postUxmlReload = BindSettingsScreen;
        Screen_Statistic_employee.postUxmlReload = BindEmployeeStatisticScreen;
        Screen_Statistic_manager.postUxmlReload = BindManagerStatisticScreen;
        Screen_GamePuzzle.postUxmlReload = BindGamePuzzleScreen;
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
       
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // Screen Transition Logic

    //change Screen 
    IEnumerator ScreenChange(PanelRenderer from, PanelRenderer to)
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

    private void GoToStartScreen()
    {

        SetScreenEnableState(Screen_GamePuzzle, true);

        SetScreenEnableState(Screen_Start, false);
        SetScreenEnableState(Screen_Menu, false);
        SetScreenEnableState(Screen_JoinSession, false);
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
    private IEnumerable<Object> BindStartScreen()
    {
        //bind root 
        var root = Screen_Start.visualTree;

        //set button function
        var joinButton = root.Q<Button>("start_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                StartCoroutine(ScreenChange(Screen_Start, Screen_JoinSession));
            };
        }
        return null;
    }

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
                StartCoroutine(ScreenChange(Screen_Menu, Screen_JoinGame));
            };
        }
        
        joinButton = root.Q<Button>("create_game_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                StartCoroutine(ScreenChange(Screen_Menu, Screen_CreateGame));
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
                    StartCoroutine(ScreenChange(Screen_Menu, Screen_Statistic_manager));
                } else
                {
                    StartCoroutine(ScreenChange(Screen_Menu, Screen_Statistic_employee));
                }
                
            };
        }

        joinButton = root.Q<Button>("settings_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                StartCoroutine(ScreenChange(Screen_Menu, Screen_Settings));
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
        var root = Screen_JoinGame.visualTree;

        //set button function create game
        var createButton = root.Q<Button>("join_game_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                StartCoroutine(ScreenChange(Screen_JoinGame, Screen_GamePuzzle));

            };
        }

        //test
        Game g = new Game();
        g.createGame("erstens", 1);
        games.Add(g);
        

        //in list view anzeigen welches spiel vom manager ausgewählt wurde
        var listView = root.Q<ListView>("games-list");
        if (listView != null)
        {
            listView.selectionType = SelectionType.None;

            if (listView.makeItem == null)
                listView.makeItem = MakeItemJoinGame;
            if (listView.bindItem == null)
                listView.bindItem = BindItemJoinGame;

            listView.itemsSource = games;
            listView.Refresh();
        }

        return null;
    }

    private IEnumerable<Object> BindJoinSessionScreen()
    {
        //bind root 
        var root = Screen_JoinSession.visualTree;

        //TODO
        // input des session key und des namen

        //set button function
        var joinButton = root.Q<Button>("join_session_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                StartCoroutine(ScreenChange(Screen_JoinSession, Screen_Menu));
            };
        }
        return null;
    }

    private IEnumerable<Object> BindSettingsScreen()
    {
        //bind root 
        var root = Screen_Settings.visualTree;

        //TODO
        //Name input 

        //set button function create game
        var createButton = root.Q<Button>("logout_button");
        if (createButton != null)
        {
            //button function
            createButton.clickable.clicked += () =>
            {
                //TODO
                //alle daten müssen gelöscht werden von der vorherigen Session
                StartCoroutine(ScreenChange(Screen_Settings, Screen_JoinSession));
            };
        }
        return null;
    }

    private IEnumerable<Object> BindEmployeeStatisticScreen()
    {
        //bind root 
        var root = Screen_Statistic_employee.visualTree;

        //TODO
        //List view implementieren

        //test
        StatisticEmployee s = new StatisticEmployee();
        s.createStatistic("erstens", 1);
        statisticsEmployee.Add(s);

        //in list view anzeigen welches spiel vom manager ausgewählt wurde
        var listView = root.Q<ListView>("statistic-list");
        if (listView != null)
        {
            listView.selectionType = SelectionType.None;

            if (listView.makeItem == null)
                listView.makeItem = MakeItemStatisticEmployee;
            if (listView.bindItem == null)
                listView.bindItem = BindItemStatisticEmployee;

            listView.itemsSource = statisticsEmployee;
            listView.Refresh();
        }



        //info: button hat keine funktion
        return null;
    }

    private IEnumerable<Object> BindManagerStatisticScreen()
    {
        //bind root 
        var root = Screen_Statistic_manager.visualTree;

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

        //TODO
        //list view implementieren mit 
        var listView = root.Q<ListView>("statistic-list");
        if (listView != null)
        {
            listView.selectionType = SelectionType.None;

            if (listView.makeItem == null)
                listView.makeItem = MakeItemStatisticEmployee;
            if (listView.bindItem == null)
                listView.bindItem = BindItemStatisticEmployee;

            listView.itemsSource = statisticsEmployee;
            listView.Refresh();
        }
        return null;
    }

    private IEnumerable<Object> BindGamePuzzleScreen()
    {
        //bind root 
        var root = Screen_GamePuzzle.visualTree;

        //set left button function
        var joinButton = root.Q<Button>("left_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                Debug.Log("Links gedrückt");
            };
        }

        //set right button function
        joinButton = root.Q<Button>("right_button");
        if (joinButton != null)
        {
            //button function
            joinButton.clickable.clicked += () =>
            {
                Debug.Log("Rechts gedrückt");
            };
        }

        //TODO
        //list view implementieren mit 
        var listView = root.Q<ListView>("overview-list");
        //test
        OverViewListData g = new OverViewListData();
        g.createOverViewListData(true);
        overviewGamePuzzle.Add(g);
         

        if (listView != null)
        {
            Debug.Log("Drinnen in overview");
            listView.selectionType = SelectionType.None;

            if (listView.makeItem == null)
                listView.makeItem = MakeItemGamePuzzleOverview;
            if (listView.bindItem == null)
                listView.bindItem = BindItemGamePuzzleOverview;

            listView.itemsSource = games;
            listView.Refresh();
        }
        return null;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Game Logic




    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // In-Game Virtualized ListView Implementation

    //LIST VIEW JOIN GAME /////////////////////////////////////////////////////////////////////////////
    private VisualElement MakeItemJoinGame()
    {
        var element = games_item.CloneTree();

        //element.schedule.Execute(() => UpdateHealthBar(element)).Every(200);

        return element;
    }

    private void BindItemJoinGame(VisualElement element, int index)
    {
        //element.Q<Label>("game-name").text = "Game Jan";

        var playerColor = Color.blue;
        playerColor.a = 0.9f;
        element.Q("icon").style.unityBackgroundImageTintColor = playerColor;

        //element.userData = games[index];

        UpdateHealthBar(element);
    }

    private void UpdateHealthBar(VisualElement element)
    {
        //var tank = element.userData as TankManager;
        //if (tank == null)
        //   return;

        //var healthBar = element.Q("health-bar");
        //var healthBarFill = element.Q("health-bar-fill");

        //var totalWidth = healthBar.resolvedStyle.width;

        //var healthComponent = tank.m_Instance.GetComponent<TankHealth>();
        //var currentHealth = healthComponent.m_CurrentHealth;
        //var startingHealth = healthComponent.m_StartingHealth;
        //var percentHealth = currentHealth / startingHealth;

        //healthBarFill.style.width = totalWidth * percentHealth;
    }


    //LIST VIEW STATISTIC EMPLOYEE ///////////////////////////////////////////////////////////////////////////
    private VisualElement MakeItemStatisticEmployee()
    {
        var element = statistic_employee_item.CloneTree();
        return element;
    }

    private void BindItemStatisticEmployee(VisualElement element, int index)
    {
        element.Q<Label>("session-name").text = "Game Jan";

        var playerColor = Color.blue;
        playerColor.a = 0.9f;
        element.Q("icon").style.unityBackgroundImageTintColor = playerColor;
    }

    //LIST VIEW OVERVIEW INGAME ///////////////////////////////////////////////////////////////////////////////
    private VisualElement MakeItemGamePuzzleOverview()
    {
        var element = overview_item.CloneTree();
        //element.schedule.Execute(() => UpdateOverview(element)).Every(200);
        return element;
    }

    private void BindItemGamePuzzleOverview(VisualElement element, int index)
    {
        element.Q<Label>("game-name").text = "Game Jan";

        var playerColor = Color.blue;
        playerColor.a = 0.9f;
        element.Q("icon").style.unityBackgroundImageTintColor = playerColor;
    }

    private void UpdateOverview(VisualElement element)
    {
        //var tank = element.userData as TankManager;
        //if (tank == null)
        //   return;

        //var healthBar = element.Q("health-bar");
        //var healthBarFill = element.Q("health-bar-fill");

        //var totalWidth = healthBar.resolvedStyle.width;

        //var healthComponent = tank.m_Instance.GetComponent<TankHealth>();
        //var currentHealth = healthComponent.m_CurrentHealth;
        //var startingHealth = healthComponent.m_StartingHealth;
        //var percentHealth = currentHealth / startingHealth;

        //healthBarFill.style.width = totalWidth * percentHealth;
    }
}
