using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
	public PanelRenderer Screen_Start;
	public PanelRenderer Screen_JoinSession;
	public PanelRenderer Screen_Menu;
	public PanelRenderer Screen_CreateRoom;
	public PanelRenderer Screen_StartGame;
	public PanelRenderer Screen_GameLobby;
	public PanelRenderer Screen_Settings;
	public PanelRenderer Screen_Statistic_manager;
	public PanelRenderer Screen_Statistic_employee;

	public PanelRenderer Screen_GamePuzzle_Picture;
	public PanelRenderer Screen_GamePuzzle;

	//list view items
	public VisualTreeAsset player_ready_item;
	public VisualTreeAsset statistic_employee_item;
	public VisualTreeAsset statistic_manager_item;
	public VisualTreeAsset overview_item;

	//Puzzles
	public Puzzle[] puzzles;

	//Listen für die ListViews
	private List<StatisticEmployee> statisticsEmployee = new List<StatisticEmployee>();
	private List<StatisticEmployee> statisticsManager = new List<StatisticEmployee>();
	private List<OverViewListData> overviewGamePuzzle = new List<OverViewListData>();                         //ist dort schon ein PuzzleTeil gesetzt?

	private const int updateInterval = 2000;
	//Logic
	private string playerId;
	private string roomId;
	private bool isManager;

	//PuzzleGame
	public List<Sprite> wholePuzzlePicture = new List<Sprite>();
	private List<(bool, bool)> informationPicture = new List<(bool, bool)>();  //(owning, placed)
	private List<int> placedPictures = new List<int>();            //pictures that got placed
	private int index = 0;
	private int currentPicture = -1;

	private void OnEnable()
	{
		//Bind Screens
		Screen_Start.postUxmlReload = BindStartScreen;
		Screen_JoinSession.postUxmlReload = BindJoinSessionScreen;
		Screen_Menu.postUxmlReload = BindMenuScreen;
		Screen_CreateRoom.postUxmlReload = BindCreateRoomScreen;
		Screen_StartGame.postUxmlReload = BindStartGameScreen;
		Screen_GameLobby.postUxmlReload = BindGameLobbyScreen;
		Screen_Settings.postUxmlReload = BindSettingsScreen;
		Screen_Statistic_employee.postUxmlReload = BindEmployeeStatisticScreen;
		Screen_Statistic_manager.postUxmlReload = BindManagerStatisticScreen;
		Screen_GamePuzzle.postUxmlReload = BindGamePuzzleScreen;
		Screen_GamePuzzle_Picture.postUxmlReload = BindGamePuzzlePictureScreen;

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
		//Update für das PuzzleSpiel
		//TODO erst wenn das spiel gestartet ist immer actualisieren
		updateInformationPictureList();
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
		SetScreenEnableState(Screen_Start, true);
		SetScreenEnableState(Screen_Menu, false);
		SetScreenEnableState(Screen_JoinSession, false);
		SetScreenEnableState(Screen_CreateRoom, false);
		SetScreenEnableState(Screen_Settings, false);
		SetScreenEnableState(Screen_GameLobby, false);
		SetScreenEnableState(Screen_StartGame, false);
		SetScreenEnableState(Screen_Statistic_employee, false);
		SetScreenEnableState(Screen_Statistic_manager, false);

		SetScreenEnableState(Screen_GamePuzzle, false);
		SetScreenEnableState(Screen_GamePuzzle_Picture, false);
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
				StartCoroutine(ScreenChange(Screen_Start, Screen_Menu));
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
				StartCoroutine(ScreenChange(Screen_Menu, Screen_JoinSession));
			};
		}

		joinButton = root.Q<Button>("create_game_button");
		if (joinButton != null)
		{
			//button function
			joinButton.clickable.clicked += () =>
			{
				StartCoroutine(ScreenChange(Screen_Menu, Screen_CreateRoom));
			};
		}

		joinButton = root.Q<Button>("statistics_button");
		if (joinButton != null)
		{
			//button function
			joinButton.clickable.clicked += () =>
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

	private IEnumerable<Object> BindCreateRoomScreen()
	{
		var root = Screen_CreateRoom.visualTree;

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

		return null;
	}

	private IEnumerable<Object> BindStartGameScreen()
	{
		var root = Screen_StartGame.visualTree;

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

		return null;
	}

	private IEnumerable<Object> BindJoinSessionScreen()
	{
		//bind root 
		var root = Screen_JoinSession.visualTree;

		// Join room
		root.Q<Button>("join_session_button").clickable.clicked += () =>
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

			StartCoroutine(ScreenChange(Screen_JoinSession, Screen_GameLobby));
		};

		// Zum Menue
		root.Q<Button>("back_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_JoinSession, Screen_Menu));

		};

		return null;
	}

	private IEnumerable<Object> BindGameLobbyScreen()
	{
		var root = Screen_GameLobby.visualTree;

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
						StartCoroutine(ScreenChange(Screen_GameLobby, Screen_GamePuzzle));
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

		//set button function create game
		var backButton = root.Q<Button>("back_button");
		if (backButton != null)
		{
			//button function
			backButton.clickable.clicked += () =>
			{
				StartCoroutine(ScreenChange(Screen_Settings, Screen_Menu));

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

		//set button function create game
		var backButton = root.Q<Button>("back_button");
		if (backButton != null)
		{
			//button function
			backButton.clickable.clicked += () =>
			{
				StartCoroutine(ScreenChange(Screen_Statistic_employee, Screen_Menu));

			};
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

		//set button function create game
		var backButton = root.Q<Button>("back_button");
		if (backButton != null)
		{
			//button function
			backButton.clickable.clicked += () =>
			{
				StartCoroutine(ScreenChange(Screen_Statistic_manager, Screen_Menu));

			};
		}

		return null;
	}

	private IEnumerable<Object> BindGamePuzzleScreen()
	{
		//test nur
		for (int i = 0; i < wholePuzzlePicture.Count; ++i)
		{
			informationPicture.Add((true, false));
			placedPictures.Add(-1);
		}

		//welches bild wird in dem spiel verwendet (holt es sich)
		inizializeLists();

		//bind root 
		var root = Screen_GamePuzzle.visualTree;

		var screenPuzzle = root.Q<VisualElement>("puzzle_piece");
		if (placedPictures[index] != -1)
		{
			screenPuzzle.style.backgroundImage = Background.FromTexture2D(wholePuzzlePicture[placedPictures[index]].texture);
		}

		var indexlabel = root.Q<Label>("index_label");
		if (indexlabel != null)
		{
			indexlabel.text = "Seite " + (index + 1) + " von " + wholePuzzlePicture.Count;
		}

		//set left button function
		var joinButton = root.Q<Button>("left_button");
		if (joinButton != null)
		{
			//button function
			joinButton.clickable.clicked += () =>
			{
				Debug.Log("Links gedrückt");
				//schauen dass das bild nicht aus dem array geht 
				if (index > 0)
				{
					index--;
					if (placedPictures[index] != -1)
					{
						screenPuzzle.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(wholePuzzlePicture[placedPictures[index]].texture);
					}
					else
					{
						screenPuzzle.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(null);
					}
				}
				else if (index == 0)
				{
					if (placedPictures[index] != -1)
					{
						screenPuzzle.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(wholePuzzlePicture[placedPictures[index]].texture);
					}
					else
					{
						screenPuzzle.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(null);
					}
				}
				//text updaten
				indexlabel.text = "Seite " + (index + 1) + " von " + wholePuzzlePicture.Count;
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
				if (index < wholePuzzlePicture.Count - 1)
				{
					index++;
					if (placedPictures[index] != -1)
					{
						screenPuzzle.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(wholePuzzlePicture[placedPictures[index]].texture);
					}
					else
					{
						screenPuzzle.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(null);
					}
				}
				//update text
				indexlabel.text = "Seite " + (index + 1) + " von " + wholePuzzlePicture.Count;
			};
		}

		//Owned PuzzlePieces
		(int, int, int) pieces = getIndexOfOwnPuzzlePieces();
		var part1 = root.Q<Button>("part1");
		if (part1 != null)
		{
			if (pieces.Item1 != -1 && informationPicture[pieces.Item1].Item2 == false)
			{
				part1.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(wholePuzzlePicture[pieces.Item1].texture);
				//button function
				part1.clickable.clicked += () =>
				{
					Debug.Log("part1 gedrückt");
					currentPicture = pieces.Item1;
					StartCoroutine(ScreenChange(Screen_GamePuzzle, Screen_GamePuzzle_Picture));
				};
			}
		}
		var part2 = root.Q<Button>("part2");
		if (part2 != null)
		{
			if (pieces.Item2 != -1 && informationPicture[pieces.Item2].Item2 == false)
			{
				part2.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(wholePuzzlePicture[pieces.Item2].texture);
				//button function
				part2.clickable.clicked += () =>
				{
					Debug.Log("part2 gedrückt");
					currentPicture = pieces.Item2;
					StartCoroutine(ScreenChange(Screen_GamePuzzle, Screen_GamePuzzle_Picture));
				};
			}
		}
		var part3 = root.Q<Button>("part3");
		if (part3 != null)
		{
			if (pieces.Item3 != -1 && informationPicture[pieces.Item3].Item2 == false)
			{
				part3.style.backgroundImage = UnityEngine.UIElements.Background.FromTexture2D(wholePuzzlePicture[pieces.Item3].texture);
				//button function
				part3.clickable.clicked += () =>
				{
					Debug.Log("part3 gedrückt");
					currentPicture = pieces.Item3;
					StartCoroutine(ScreenChange(Screen_GamePuzzle, Screen_GamePuzzle_Picture));
				};
			}
		}

		return null;
	}

	private IEnumerable<Object> BindGamePuzzlePictureScreen()
	{
		//bind root 
		var root = Screen_GamePuzzle_Picture.visualTree;

		var screenPuzzle = root.Q<VisualElement>("puzzle_piece");
		if (screenPuzzle != null && currentPicture != -1)
		{
			screenPuzzle.style.backgroundImage = Background.FromTexture2D(wholePuzzlePicture[currentPicture].texture);
		}

		//set left button function
		var placebutton = root.Q<Button>("place_button");
		if (placebutton != null)
		{
			//button function
			placebutton.clickable.clicked += () =>
			{
				if (placedPictures[index] == -1)
				{
					informationPicture[currentPicture] = (true, true);
					placedPictures[index] = currentPicture;
				}
				currentPicture = -1;
				StartCoroutine(ScreenChange(Screen_GamePuzzle_Picture, Screen_GamePuzzle));
			};
		}

		//set left button function
		var backbutton = root.Q<Button>("back_button");
		if (backbutton != null)
		{
			//button function
			backbutton.clickable.clicked += () =>
			{
				currentPicture = -1;
				StartCoroutine(ScreenChange(Screen_GamePuzzle_Picture, Screen_GamePuzzle));
			};
		}

		return null;
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Game Logic

	//Überprüft ob die Eingabe korrekt zum LogIn ist und logt dann in die entsprechende Session ein.
	private bool checkLogIn(string session_key, string name)
	{
		//TODO Serverdaten abgleichen
		return true;
	}

	//PuzzleGame

	//Gibt die 3 Indices, der Teile die einem selbst gehören zurück, wenn man weniger als 3 Teile hat, dann steht -1 dort
	private (int, int, int) getIndexOfOwnPuzzlePieces()
	{
		return (0, 1, 2);
	}

	private void inizializeLists()
	{
		//TODO 
		//inizialisiert die Listen für das Spiel 
		//circus und circusPicture
		//holt vom server die information welches bild benutzt wird, weißt dann das bild (list<sprites>) dem circusPicture zu 
		//und holt vom server immer die Informationen für circus ob was schon gesetzt wurde.
	}

	private void updateInformationPictureList()
	{
		//TODO
		//hol vom server die liste InformationPicture und überschrieb sie auch im server 
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////
	// In-Game Virtualized ListView Implementation

	//LIST VIEW JOIN GAME /////////////////////////////////////////////////////////////////////////////

	private void BindItemJoinGame(VisualElement element, int index)
	{
		var playerColor = Color.blue;
		playerColor.a = 0.9f;
		element.Q("icon").style.unityBackgroundImageTintColor = playerColor;
	}

	//LIST VIEW STATISTIC EMPLOYEE ///////////////////////////////////////////////////////////////////////////
	private VisualElement MakeItemStatisticEmployee()
	{
		var element = statistic_employee_item.CloneTree();
		return element;
	}

	private void BindItemStatisticEmployee(VisualElement element, int index)
	{
		element.Q<Label>("session-name").text = statisticsEmployee[index].name;

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
}
