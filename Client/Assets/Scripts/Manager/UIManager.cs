using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
	public UIDocument Screen_Start;
	public UIDocument Screen_Menu;
	public UIDocument Screen_Settings;
	public UIDocument Screen_JoinRoom;
	public UIDocument Screen_RoomLobby;
	public UIDocument Screen_CreateRoom;
	public UIDocument Screen_StartGame;
	public UIDocument Screen_GamePuzzle;
	public UIDocument Screen_Statistic_manager;
	public UIDocument Screen_Statistic_employee;

	// List view items
	public VisualTreeAsset player_ready_item;
	public VisualTreeAsset puzzle_piece_item;
	public VisualTreeAsset statistic_employee_item;
	public VisualTreeAsset statistic_manager_item;

	// Puzzles
	public Puzzle[] puzzles;

	// Listen für die ListViews
	private List<StatisticEmployee> statisticsEmployee = new List<StatisticEmployee>();
	private List<StatisticEmployee> statisticsManager = new List<StatisticEmployee>();

	// Logic
	private const int updateInterval = 2000;
	private string playerId;
	private string roomId;
	private bool isManager;

	private void OnEnable()
	{
		//Bind Screens
		BindStartScreen();
		BindMenuScreen();
		BindSettingsScreen();
		BindJoinRoomScreen();
		BindRoomLobbyScreen();
		BindCreateRoomScreen();
		BindStartGameScreen();
		BindGamePuzzleScreen();
		BindEmployeeStatisticScreen();
		BindManagerStatisticScreen();
	}

	void Start()
	{

#if !UNITY_EDITOR
				if (Screen.fullScreen)
						Screen.fullScreen = false;
#endif
		//open LogIn as start menu
		GoToStartScreen();
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////
	// Screen Transition Logic
	///////////////////////////////////////////////////////////////////////////////////////////////////

	IEnumerator ScreenChange(UIDocument from, UIDocument to)
	{
		from.rootVisualElement.style.display = DisplayStyle.None;

		to.enabled = true;

		yield return null;
		yield return null;
		yield return null;

		to.rootVisualElement.style.display = DisplayStyle.Flex;
		to.rootVisualElement.style.visibility = Visibility.Hidden;

		yield return null;
		yield return null;
		yield return null;

		to.rootVisualElement.style.visibility = Visibility.Visible;

		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;

		from.enabled = false;
	}

	private void GoToStartScreen()
	{
		SetScreenEnableState(Screen_Start, true);
		SetScreenEnableState(Screen_Menu, false);
		SetScreenEnableState(Screen_JoinRoom, false);
		SetScreenEnableState(Screen_CreateRoom, false);
		SetScreenEnableState(Screen_Settings, false);
		SetScreenEnableState(Screen_RoomLobby, false);
		SetScreenEnableState(Screen_StartGame, false);
		SetScreenEnableState(Screen_GamePuzzle, false);
		SetScreenEnableState(Screen_Statistic_employee, false);
		SetScreenEnableState(Screen_Statistic_manager, false);
	}

	void SetScreenEnableState(UIDocument screen, bool state)
	{
		if (state)
		{
			screen.rootVisualElement.style.display = DisplayStyle.Flex;
			screen.enabled = true;
		}
		else
		{
			screen.rootVisualElement.style.display = DisplayStyle.None;
			screen.enabled = false;
		}
	}

	///////////////////////////////////////////////////////////////////////////////
	///Bind 
	///////////////////////////////////////////////////////////////////////////////

	private void BindStartScreen()
	{
		//bind root 
		var root = Screen_Start.rootVisualElement;

		//set button function
		root.Q<Button>("start_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Start, Screen_Menu));
		};
	}

	private void BindMenuScreen()
	{
		//bind root 
		var root = Screen_Menu.rootVisualElement;

		root.Q<Button>("join_game_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Menu, Screen_JoinRoom));
		};

		root.Q<Button>("create_game_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Menu, Screen_CreateRoom));
		};

		root.Q<Button>("statistics_button").clickable.clicked += () =>
		{
			if (isManager)
			{
				StartCoroutine(ScreenChange(Screen_Menu, Screen_Statistic_manager));
			}
			else
			{
				StartCoroutine(ScreenChange(Screen_Menu, Screen_Statistic_employee));
			}
		};

		root.Q<Button>("settings_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Menu, Screen_Settings));
		};
	}

	private void BindSettingsScreen()
	{
		//bind root 
		var root = Screen_Settings.rootVisualElement;

		//TODO
		//Name input 

		//set button function create game
		root.Q<Button>("logout_button").clickable.clicked += () =>
		{
			//TODO
			//alle daten müssen gelöscht werden von der vorherigen Session
			StartCoroutine(ScreenChange(Screen_Settings, Screen_JoinRoom));
		};

		//set button function create game
		root.Q<Button>("back_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Settings, Screen_Menu));
		};
	}

	private void BindCreateRoomScreen()
	{
		var root = Screen_CreateRoom.rootVisualElement;

		// Puzzle Liste
		var lv = root.Q<ListView>("puzzle_selection");
		lv.selectionType = SelectionType.Single;
		lv.itemsSource = puzzles;
		lv.bindItem = (e, i) => puzzles[i].MakePuzzleItem(e);
		lv.makeItem = () =>
		{
			var curr = new VisualElement();
			curr.style.flexDirection = FlexDirection.Row;
			curr.style.alignItems = Align.Center;
			curr.style.flexWrap = Wrap.Wrap;
			curr.style.overflow = Overflow.Hidden;
			curr.style.paddingLeft = new StyleLength(new Length(5, LengthUnit.Percent));
			curr.style.paddingRight = new StyleLength(new Length(5, LengthUnit.Percent));
			return curr;
		};

		// Room Erstellung
		var createButton = root.Q<Button>("create_room_button");
		createButton.clickable.clicked += () =>
		{
			// Room Code
			roomId = root.Q<TextField>("input_roomid").value;
			// Sende Befehl
			StartCoroutine(HttpClient.CallMethod("CREATEROOM",
				new Headers().AddHeader("picture", (lv.selectedIndex + 1).ToString()).AddHeader("room_id", roomId),
				(result) =>
				{
					if (result.GetHeader<bool>("success"))
					{
						isManager = true;
						playerId = result.GetHeader<string>("pers_id");
						StartCoroutine(ScreenChange(Screen_CreateRoom, Screen_StartGame));
					}
				}
			));
		};

		// Zum Menue
		var backButton = root.Q<Button>("back_button");
		backButton.clickable.clicked += () => StartCoroutine(ScreenChange(Screen_CreateRoom, Screen_Menu));
	}

	private void BindStartGameScreen()
	{
		var root = Screen_StartGame.rootVisualElement;

		bool startGame = false;
		bool startPossible = false;
		List<Player> playerlist = new List<Player>();

		// Erstelle Playerliste
		var lv = root.Q<ListView>("player-list");
		lv.selectionType = SelectionType.None;
		lv.itemsSource = playerlist;
		lv.makeItem = () =>
		{
			var newEle = player_ready_item.CloneTree();
			return newEle;
		};
		lv.bindItem = (e, i) =>
		{
			e.Q<Toggle>().label = playerlist[i].name;
			e.Q<Toggle>().value = playerlist[i].ready;
		};

		// Updatet Playerliste
		lv.schedule.Execute(() =>
		{
			StartCoroutine(HttpClient.CallMethod("UPDATE",
				new Headers(),
				(result) =>
				{
					var fetched = JSONArray.FromJson<Player>(result.GetHeader<string>("participants"));
					foreach (var player in fetched)
					{
						if (!playerlist.Exists(p =>
						{
							if (p.identifier == player.identifier)
							{
								p.name = player.name;
								p.ready = player.ready;
								return true;
							}
							return false;
						})
						)
						{
							playerlist.Add(player);
							lv.Refresh();
						}
					}
					startPossible = playerlist.All(p => p.ready);
				}
			));
		}).Every(updateInterval).Until(() => startGame);

		// Game start
		root.Q<Button>("start-game-button").clickable.clicked += () =>
		{
			if (startPossible)
			{
				startGame = true;
				StartCoroutine(ScreenChange(Screen_StartGame, Screen_GamePuzzle));
			}
		};
	}

	private void BindJoinRoomScreen()
	{
		//bind root 
		var root = Screen_JoinRoom.rootVisualElement;

		// Join room
		root.Q<Button>("join-room-button").clickable.clicked += () =>
		{
			//test
			roomId = root.Q<TextField>("input_roomkey").value;
			var name = root.Q<TextField>("input_name").value;

			StartCoroutine(HttpClient.CallMethod("JOINROOM",
				new Headers().AddHeader("room_id", roomId).AddHeader("name", name),
				(result) =>
				{
					playerId = result.GetHeader<string>("pers_id");
				}
			));

			StartCoroutine(ScreenChange(Screen_JoinRoom, Screen_RoomLobby));
		};

		// Zum Menue
		root.Q<Button>("back_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_JoinRoom, Screen_Menu));

		};
	}

	private void BindRoomLobbyScreen()
	{
		var root = Screen_RoomLobby.rootVisualElement;

		bool startGame = false;
		List<Player> playerlist = new List<Player>();

		// Erstelle Playerliste
		var lv = root.Q<ListView>("player-list");
		lv.selectionType = SelectionType.None;
		lv.itemsSource = playerlist;
		lv.makeItem = () =>
		{
			var newEle = player_ready_item.CloneTree();
			return newEle;
		};
		lv.bindItem = (e, i) =>
		{
			e.Q<Toggle>().label = playerlist[i].name;
			e.Q<Toggle>().value = playerlist[i].ready;
		};

		// Updatet Playerliste
		lv.schedule.Execute(() =>
		{
			StartCoroutine(HttpClient.CallMethod("UPDATE",
				new Headers(),
				(result) =>
				{
					startGame = result.GetHeader<int>("gamestate") == 2;
					if (startGame)
					{
						StartCoroutine(ScreenChange(Screen_RoomLobby, Screen_GamePuzzle));
					}
					else
					{
						var fetched = JSONArray.FromJson<Player>(result.GetHeader<string>("participants"));
						foreach (var player in fetched)
						{
							if (!playerlist.Exists(p =>
							{
								if (p.identifier == player.identifier)
								{
									p.name = player.name;
									p.ready = player.ready;
									return true;
								}
								return false;
							})
							)
							{
								playerlist.Add(player);
							}
						}
						lv.Refresh();
					}
				}
			));
		}).Every(updateInterval).Until(() => startGame);

		var ready = root.Q<VisualElement>("ready");
		var notready = root.Q<VisualElement>("not-ready");

		// Ready setzen
		ready.Q<Button>("ready-button").clickable.clicked += () =>
		{
			notready.style.display = DisplayStyle.Flex;
			ready.style.display = DisplayStyle.None;
			StartCoroutine(HttpClient.CallMethod("READY",
				new Headers().AddHeader("room_id", roomId).AddHeader("pers_id", playerId).AddHeader("ready", true.ToString()),
				(result) =>
				{
				}
			));
		};

		// Unready setzen
		notready.Q<Button>("not-ready-button").clickable.clicked += () =>
		{
			notready.style.display = DisplayStyle.None;
			ready.style.display = DisplayStyle.Flex;
			StartCoroutine(HttpClient.CallMethod("READY",
				new Headers().AddHeader("room_id", roomId).AddHeader("pers_id", playerId).AddHeader("ready", false.ToString()),
				(result) =>
				{
				}
			));
		};
	}

	private void BindGamePuzzleScreen()
	{
		// TODO
	}

	private void BindEmployeeStatisticScreen()
	{
		//bind root 
		var root = Screen_Statistic_employee.rootVisualElement;

		//TODO
		//List view implementieren

		//test
		StatisticEmployee s = new StatisticEmployee();
		s.createStatistic("erstens", 1);
		statisticsEmployee.Add(s);

		//in list view anzeigen welches spiel vom manager ausgewählt wurde
		var listView = root.Q<ListView>("statistic-list");
		listView.makeItem = () => statistic_employee_item.CloneTree();
		listView.bindItem = (e, i) =>
		{
			e.Q<Label>("session-name").text = statisticsEmployee[i].name;
			var playerColor = Color.blue;
			playerColor.a = 0.9f;
			e.Q("icon").style.unityBackgroundImageTintColor = playerColor;
		};
		listView.itemsSource = statisticsEmployee;
		listView.Refresh();

		//set button function create game
		root.Q<Button>("back_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Statistic_employee, Screen_Menu));
		};
	}

	private void BindManagerStatisticScreen()
	{
		//bind root 
		var root = Screen_Statistic_manager.rootVisualElement;

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
		listView.makeItem = () => statistic_manager_item.CloneTree();
		listView.bindItem = (e, i) =>
		{
			var playerColor = Color.blue;
			playerColor.a = 0.9f;
			e.Q("icon").style.unityBackgroundImageTintColor = playerColor;
			e.Q<Label>("session-name").text = statisticsEmployee[i].name;
		};
		listView.itemsSource = statisticsEmployee;
		listView.Refresh();

		//set button function create game
		root.Q<Button>("back_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Statistic_manager, Screen_Menu));
		};
	}
}
