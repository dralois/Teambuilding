using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using InputField = UnityEngine.UI.InputField;

public class UIManager : MonoBehaviour
{

	public Canvas Screen_TextInput;
	public InputField Input_TextInput;
	[Space]
	public UIDocument Screen_Start;
	public UIDocument Screen_End;
	public UIDocument Screen_Menu;
	public UIDocument Screen_Settings;
	public UIDocument Screen_JoinRoom;
	public UIDocument Screen_RoomLobby;
	public UIDocument Screen_CreateRoom;
	public UIDocument Screen_StartGame;
	public UIDocument Screen_GamePlayer;
	public UIDocument Screen_GameManager;
	public UIDocument Screen_Statistic_manager;
	public UIDocument Screen_Statistic_employee;
	[Space]
	public VisualTreeAsset player_ready_item;
	public VisualTreeAsset puzzle_piece_item;
	public VisualTreeAsset statistic_employee_item;
	public VisualTreeAsset statistic_manager_item;
	[Space]
	public Puzzle[] puzzles;

	// Listen für die ListViews
	private List<StatisticEmployee> statisticsEmployee = new List<StatisticEmployee>();
	private List<StatisticEmployee> statisticsManager = new List<StatisticEmployee>();

	// Logic
	private const int updateInterval = 2000;
	private System.Action<string> onText = null;

	private string playerId;
	private string roomId;
	private int pictureId;
	private bool isManager;
	private int myPiece;

	private List<Piece> puzzlePieces = new List<Piece>();

	private int[] selected = { };
	private int[] places = { };
	private int[] range = { };

	///////////////////////////////////////////////////////////////////////////////////////////////////
	// Unity Callbacks
	///////////////////////////////////////////////////////////////////////////////////////////////////

	private void OnEnable()
	{
		//Bind Screens
		BindTextInput();
		BindStartScreen();
		BindEndScreen();
		BindMenuScreen();
		BindSettingsScreen();
		BindJoinRoomScreen();
		BindRoomLobbyScreen();
		BindCreateRoomScreen();
		BindStartGameScreen();
		BindGamePlayerScreen();
		BindGameManagerScreen();
		BindEmployeeStatisticScreen();
		BindManagerStatisticScreen();
	}

	void Start()
	{

#if !UNITY_EDITOR
				if (Screen.fullScreen)
						Screen.fullScreen = false;
#endif
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

	private void TextChange(UIDocument doc, bool enableText)
	{
		if (enableText)
		{
			Screen_TextInput.gameObject.SetActive(true);
			doc.gameObject.SetActive(false);
		}
		else
		{
			Screen_TextInput.gameObject.SetActive(false);
			doc.gameObject.SetActive(true);
		}
	}

	private void GoToStartScreen()
	{
		SetScreenEnableState(Screen_Start, true);
		SetScreenEnableState(Screen_End, false);
		SetScreenEnableState(Screen_Menu, false);
		SetScreenEnableState(Screen_JoinRoom, false);
		SetScreenEnableState(Screen_CreateRoom, false);
		SetScreenEnableState(Screen_Settings, false);
		SetScreenEnableState(Screen_RoomLobby, false);
		SetScreenEnableState(Screen_StartGame, false);
		SetScreenEnableState(Screen_GamePlayer, false);
		SetScreenEnableState(Screen_GameManager, false);
		SetScreenEnableState(Screen_Statistic_employee, false);
		SetScreenEnableState(Screen_Statistic_manager, false);
	}

	private void SetScreenEnableState(UIDocument screen, bool state)
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

	private void BindTextInput()
	{
		Input_TextInput.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>((s) =>
		{
			onText?.Invoke(s);
			onText = null;
			Input_TextInput.text = "";
		}));
	}

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

	private void BindEndScreen()
	{
		var root = Screen_End.rootVisualElement;

		var result = root.Q<Label>("result");

		bool playersWon = true;
		for(int i = 0; i < places.Length; i++)
		{
			playersWon = selected[i] == places[i] ? playersWon : false;
		}

		result.text = playersWon ? "solved!" : "oof";

		result.schedule.Execute(() =>
		{
			ScreenChange(Screen_End, Screen_Menu);
		}).ExecuteLater(5000);
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

		// Name
		root.Q<Button>("input_name").clickable.clicked += () =>
		{
			onText = (s) =>
			{
				root.Q<Button>("input_name").text = s;
				TextChange(Screen_Settings, false);
			};
			TextChange(Screen_Settings, true);
		};

		root.Q<Button>("logout_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Settings, Screen_Menu));
		};

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

		// Room code
		root.Q<Button>("input_roomid").clickable.clicked += () =>
		{
			onText = (s) =>
			{
				root.Q<Button>("input_roomid").text = s;
				TextChange(Screen_CreateRoom, false);
			};
			TextChange(Screen_CreateRoom, true);
		};

		// Room Erstellung
		root.Q<Button>("create_room_button").clickable.clicked += () =>
		{
			// Room Code
			roomId = root.Q<Button>("input_roomid").text;
			pictureId = lv.selectedIndex + 1;
			// Sende Befehl
			StartCoroutine(HttpClient.CallMethod("CREATEROOM",
				new Headers().AddHeader("picture", pictureId.ToString()).AddHeader("room_id", roomId),
				(result, worked) =>
				{
					if (worked)
					{
						if (result.GetHeader<bool>("success"))
						{
							isManager = true;
							playerId = result.GetHeader<string>("pers_id");
							StartCoroutine(ScreenChange(Screen_CreateRoom, Screen_StartGame));
						}
					}
				}
			));
		};

		// Zum Menue
		root.Q<Button>("back_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_CreateRoom, Screen_Menu));
		};
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
				(result, worked) =>
				{
					if (worked)
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
				}
			));
		}).Every(updateInterval).Until(() => startGame);

		// Game start
		root.Q<Button>("start-game-button").clickable.clicked += () =>
		{
			if (startPossible)
			{
				StartCoroutine(HttpClient.CallMethod("UPDATE",
				new Headers().AddHeader("pers_id", playerId).AddHeader("room_id", roomId),
				(result, worked) =>
				{
					if (worked)
					{
						if (result.GetHeader<bool>("success"))
						{
							startGame = true;

							places = JSONArray.FromJson<int>(result.GetHeader<string>("places"));
							range = JSONArray.FromJson<int>(result.GetHeader<string>("range"));

							// Pre generate pieces
							List<Piece> puzzlePieces = new List<Piece>(range[1] - range[0]);
							for (int i = range[0]; i < range[1]; ++i)
							{
								puzzlePieces.Add(new Piece());
							}

							StartCoroutine(ScreenChange(Screen_StartGame, Screen_GameManager));
						}
					}
				}));
			}
		};
	}

	private void BindJoinRoomScreen()
	{
		//bind root 
		var root = Screen_JoinRoom.rootVisualElement;

		// Room code
		root.Q<Button>("input_roomkey").clickable.clicked += () =>
		{
			onText = (s) =>
			{
				root.Q<Button>("input_roomkey").text = s;
				TextChange(Screen_JoinRoom, false);
			};
			TextChange(Screen_JoinRoom, true);
		};

		// Name
		root.Q<Button>("input_name").clickable.clicked += () =>
		{
			onText = (s) =>
			{
				root.Q<Button>("input_name").text = s;
				TextChange(Screen_JoinRoom, false);
			};
			TextChange(Screen_JoinRoom, true);
		};

		// Join room
		root.Q<Button>("join-room-button").clickable.clicked += () =>
		{
			//test
			roomId = root.Q<Button>("input_roomkey").text;
			var name = root.Q<Button>("input_name").text;

			StartCoroutine(HttpClient.CallMethod("JOINROOM",
				new Headers().AddHeader("room_id", roomId).AddHeader("name", name),
				(result, worked) =>
				{
					if (worked)
					{
						playerId = result.GetHeader<string>("pers_id");
						StartCoroutine(ScreenChange(Screen_JoinRoom, Screen_RoomLobby));
					}
				}
			));
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
				(result, worked) =>
				{
					if (worked)
					{
						startGame = result.GetHeader<int>("gamestate") == 2;

						if (startGame)
						{
							StartCoroutine(HttpClient.CallMethod("GETSTARTINFO",
								new Headers().AddHeader("pers_id", playerId).AddHeader("room_id", roomId),
								(startinfo, gotinfo) =>
								{
									if (gotinfo)
									{
										pictureId = startinfo.GetHeader<int>("picture") - 1;
										places = JSONArray.FromJson<int>(startinfo.GetHeader<string>("places"));
										range = JSONArray.FromJson<int>(startinfo.GetHeader<string>("range"));

										// Pre generate pieces
										puzzlePieces = new List<Piece>();
										for (int i = range[0]; i < range[1]; ++i)
										{
											puzzlePieces.Add(new Piece());
										}

										// Set correct picture
										myPiece = places.First(p => p == int.Parse(playerId));
										Screen_GamePlayer.rootVisualElement.Q<VisualElement>("puzzle-piece").style.backgroundImage =
											Background.FromTexture2D(puzzles[pictureId].allPics[range[0] + myPiece].texture);

										StartCoroutine(ScreenChange(Screen_RoomLobby, Screen_GamePlayer));
									}
								}
							));
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
				}
			));
		}).Every(updateInterval).Until(() => startGame);

		var ready = root.Q<VisualElement>("ready");
		var notready = root.Q<VisualElement>("not-ready");

		// Ready setzen
		ready.Q<Button>("ready-button").clickable.clicked += () =>
		{
			StartCoroutine(HttpClient.CallMethod("READY",
				new Headers().AddHeader("room_id", roomId).AddHeader("pers_id", playerId).AddHeader("ready", true.ToString()),
				(result, worked) =>
				{
					if (worked)
					{
						notready.style.display = DisplayStyle.Flex;
						ready.style.display = DisplayStyle.None;
					}
				}
			));
		};

		// Unready setzen
		notready.Q<Button>("not-ready-button").clickable.clicked += () =>
		{
			StartCoroutine(HttpClient.CallMethod("READY",
				new Headers().AddHeader("room_id", roomId).AddHeader("pers_id", playerId).AddHeader("ready", false.ToString()),
				(result, worked) =>
				{
					if (worked)
					{
						notready.style.display = DisplayStyle.None;
						ready.style.display = DisplayStyle.Flex;
					}
				}
			));
		};
	}

	private void BindGamePlayerScreen()
	{
		var root = Screen_GamePlayer.rootVisualElement;

		bool endGame = false;

		// Setup pieces list view
		var lv = root.Q<ListView>("puzzle-overview");
		lv.horizontalScrollingEnabled = true;
		lv.itemsSource = puzzlePieces;

		lv.makeItem = () =>
		{
			var newEle = puzzle_piece_item.CloneTree();
			return newEle;
		};

		lv.bindItem = (e, i) =>
		{
			var visual = e.Q<VisualElement>("select-visual");
			var btn = e.Q<Button>("select-button");

			btn.style.backgroundImage = puzzlePieces[i].isOwn ?
				Background.FromTexture2D(puzzles[pictureId].allPics[range[0] + myPiece].texture) :
				new StyleBackground();

			btn.clickable.clicked += () =>
			{
				var place = puzzlePieces[i].isOwn ? "-1" : i.ToString();
				StartCoroutine(HttpClient.CallMethod("SUBMITPLACE",
				new Headers().AddHeader("room_id", roomId).AddHeader("pers_id", playerId).AddHeader("place", place),
				(result, worked) =>
				{
					if (worked)
					{
						var selectionPositive = result.GetHeader<bool>("success");

						btn.style.backgroundImage = puzzlePieces[i].isOwn ?
							Background.FromTexture2D(puzzles[pictureId].allPics[range[0] + myPiece].texture) :
							new StyleBackground();

						visual.style.backgroundColor = selectionPositive ? new StyleColor(Color.green) : new StyleColor(Color.red);

						visual.schedule.Execute(() =>
						{
							visual.style.backgroundColor = new StyleColor(Color.clear);
						}).ExecuteLater(1000);
					}
				}
			));
			};
		};

		// Server update loop
		lv.schedule.Execute(() =>
		{
			lv.Refresh();
			StartCoroutine(HttpClient.CallMethod("UPDATE",
				new Headers(),
				(result, worked) =>
				{
					if (worked)
					{
						var gamestate = result.GetHeader<int>("gamestate");
						endGame = gamestate == 3;

						places = JSONArray.FromJson<int>(result.GetHeader<string>("places"));
						selected = JSONArray.FromJson<int>(result.GetHeader<string>("selected"));

						if (endGame)
						{
							ScreenChange(Screen_GamePlayer, Screen_End);
						}
					}
				}
			));
		}).Every(updateInterval).Until(() => endGame);
	}

	private void BindGameManagerScreen()
	{
		var root = Screen_GameManager.rootVisualElement;

		bool endGame = false;

		// Setup pieces list view
		var lv = root.Q<ListView>("puzzle-overview");
		lv.horizontalScrollingEnabled = true;
		lv.itemsSource = puzzlePieces;

		lv.makeItem = () =>
		{
			var newEle = puzzle_piece_item.CloneTree();
			return newEle;
		};

		lv.bindItem = (e, i) =>
		{
			var visual = e.Q<VisualElement>("select-visual");
			var btn = e.Q<Button>("select-button");

			var isSelected = selected[i] != -1;

			var picIndex = places.First(p => p == selected[i]);

			btn.style.backgroundImage = isSelected ?
				Background.FromTexture2D(puzzles[pictureId].allPics[range[0] + picIndex].texture) :
				new StyleBackground();
		};

		// Server update loop
		lv.schedule.Execute(() =>
		{
			lv.Refresh();
			StartCoroutine(HttpClient.CallMethod("UPDATE",
				new Headers(),
				(result, worked) =>
				{
					if (worked)
					{
						places = JSONArray.FromJson<int>(result.GetHeader<string>("places"));
						selected = JSONArray.FromJson<int>(result.GetHeader<string>("selected"));
					}
				}
			));
		}).Every(updateInterval).Until(() => endGame);

		// End game
		root.Q<Button>("end-button").clickable.clicked += () =>
		{
			StartCoroutine(HttpClient.CallMethod("END",
				new Headers().AddHeader("pers_id", playerId).AddHeader("room_id", roomId),
				(result, worked) =>
				{
					if (worked)
					{
						if (result.GetHeader<bool>("success"))
						{
							endGame = true;

							places = JSONArray.FromJson<int>(result.GetHeader<string>("places"));
							selected = JSONArray.FromJson<int>(result.GetHeader<string>("selected"));

							ScreenChange(Screen_GameManager, Screen_End);
						}
					}
				}
			));
		};
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
		var listView = root.Q<ListView>("statistic");
		listView.makeItem = () =>
		{
			var newEle = statistic_employee_item.CloneTree();
			return newEle;
		};
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
		var listView = root.Q<ListView>("statistic");
		listView.makeItem = () =>
		{
			var newEle = statistic_manager_item.CloneTree();
			return newEle;
		};
		listView.bindItem = (e, i) =>
		{
			var playerColor = Color.blue;
			playerColor.a = 0.9f;
			e.Q("icon").style.unityBackgroundImageTintColor = playerColor;
			e.Q<Label>("session-name").text = statisticsEmployee[i].name;
		};
		listView.itemsSource = statisticsManager;
		listView.Refresh();

		//set button function create game
		root.Q<Button>("back_button").clickable.clicked += () =>
		{
			StartCoroutine(ScreenChange(Screen_Statistic_manager, Screen_Menu));
		};
	}
}
