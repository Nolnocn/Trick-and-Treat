using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUIManager : MonoBehaviour {

	//Pause Menu UI
	
	//Box Attributes
	public float xBoxTransform; //.5
	public float yBoxTransform; //.04
	//Logo
	public float yLogoTransform;//.09
	//Offset for all Buttons (Except Return button on Score Screen)
	public float yButtonOffset; //.49
	//Return Button Attributes
	public float xReturnButtonOffset; //.38
	public float yReturnButtonOffset; //.69
	//Tom Image (Score Screen)
	public float xTomImageOffset; //.04
	public float yTomImageOffset; //.31
	public float tomImageScale; //300
	//NW Image (Score Screen)
	public float xNWImageOffset; //.65
	public float yNWImageOffset; //.31
	public float NWImageScale; //300
	//Scores
	public float xScoresOffset;
	public float yScoresOffset;
	//UI Logic
	public bool main = true;
	public bool scores = false;
	public bool howToPlay = false;
	public int howToPlayPage = 1;
	//GUI Skin
	public GUISkin skin;
	//UI Sprites
	public Texture tomSprite;
	public Texture NWSprite;
	public Texture ghostSprite;
	public Texture parentSprite;
	
	//Menu and Button Texture Overlays
	public Texture menuLogo;
	public Texture2D pauseMenuOverlay;
	public Texture2D highscoreMenuOverlay;
	public Texture2D scoreButton;
	public Texture2D playButton;
	public Texture2D quitButton;
	public Texture2D pauseMenuButton_Normal;
	public Texture2D pauseMenuButton_Hover;
	public Texture2D pauseMenuButton_Active;
	public Texture2D CandyCorn_Normal;
	public Texture2D CandyCorn_Hover;
	public Texture2D CandyCorn_Active;
	public Texture2D loadingText;
	public Texture2D howToPlay1;
	public Texture2D howToPlay2;
	public Texture2D howToPlayButton;
	public Texture2D CandyCorn_Normal_JoeSucks;
	public Texture2D CandyCorn_Hover_JoeSucks;
	public Texture2D CandyCorn_Active_JoeSucks;

	private AsyncOperation sceneLoad;
	private bool isWebBuild;

	void Start()
	{
		isWebBuild = (Application.platform == RuntimePlatform.OSXWebPlayer || 
			Application.platform == RuntimePlatform.WindowsWebPlayer ||
			Application.platform == RuntimePlatform.WebGLPlayer);
		
		Cursor.visible = true;

		if(PlayerPrefs.HasKey("goToScore"))
		{
			if(PlayerPrefs.GetInt("goToScore") != 0)
			{
				scores = true;
				main = false;
				PlayerPrefs.SetInt("goToScore", 0);
			}
		}

		if(!PlayerPrefs.HasKey("score0"))
		{
			PlayerPrefs.SetString("name0", "JOE");
			PlayerPrefs.SetInt("score0", 10000);
			PlayerPrefs.SetString("name1", "SUX");
			PlayerPrefs.SetInt("score1", 9000);
			PlayerPrefs.SetString("name2", "SRS");
			PlayerPrefs.SetInt("score2", 8000);
			PlayerPrefs.SetString("name3", "AND");
			PlayerPrefs.SetInt("score3", 7000);
			PlayerPrefs.SetString("name4", "DRE");
			PlayerPrefs.SetInt("score4", 6000);
			PlayerPrefs.SetString("name5", "WSM");
			PlayerPrefs.SetInt("score5", 5000);
			PlayerPrefs.SetString("name6", "ELL");
			PlayerPrefs.SetInt("score6", 4000);
			PlayerPrefs.SetString("name7", "SBA");
			PlayerPrefs.SetInt("score7", 3000);
			PlayerPrefs.SetString("name8", "DDY");
			PlayerPrefs.SetInt("score8", 2000);
			PlayerPrefs.SetString("name9", "AAA");
			PlayerPrefs.SetInt("score9", 1000);
		}
	}

	void OnGUI()
	{
		GUI.skin = skin;
		var menuOverlay = GUI.skin.GetStyle("Box");
		var buttonOverlay = GUI.skin.GetStyle("Button");
		var labelOverlay = GUI.skin.GetStyle("Label");
		
		//At the pause menu
		if (main) {
			
			menuOverlay.normal.background = pauseMenuOverlay;
			buttonOverlay.normal.background = pauseMenuButton_Normal;
			buttonOverlay.hover.background = pauseMenuButton_Hover;
			buttonOverlay.active.background = pauseMenuButton_Active;
			
			GUI.Box (new Rect (Screen.width * xBoxTransform - 275, Screen.height * yBoxTransform, 550, 550), "");
			GUI.DrawTexture (new Rect (Screen.width * xTomImageOffset, Screen.height * yTomImageOffset, tomImageScale, tomImageScale), tomSprite);
			GUI.DrawTexture (new Rect (Screen.width * xNWImageOffset, Screen.height * yNWImageOffset, NWImageScale, NWImageScale), NWSprite);
			GUI.DrawTexture(new Rect(Screen.width * xBoxTransform - 200, Screen.height * yLogoTransform, 400, 245),menuLogo);

			//if(!Application.isLoadingLevel)
			if(sceneLoad == null)
			{
				//PLAY
				if(GUI.Button (new Rect (Screen.width * xBoxTransform - 125, Screen.height * yButtonOffset, 250, 50), playButton)) {
					if(Application.CanStreamedLevelBeLoaded("Game"))
					{
						//Application.LoadLevel("Game");
						sceneLoad = SceneManager.LoadSceneAsync("Game");
					}
				}

				//HOW TO PLAY
				if (GUI.Button (new Rect (Screen.width * xBoxTransform - 125, Screen.height * yButtonOffset + 60, 250, 50), howToPlayButton)) {
					main = false;
					scores = false;
					howToPlay = true;
					howToPlayPage = 1;
				}

				//HIGH SCORES
				if (GUI.Button (new Rect (Screen.width * xBoxTransform - 125, Screen.height * yButtonOffset + 120, 250, 50), scoreButton)) {
					scores = true;
					main = false;
				}

				//QUIT
				if(!isWebBuild)
				{
					if (GUI.Button (new Rect (Screen.width * xBoxTransform - 125, Screen.height * yButtonOffset + 180, 250, 50), quitButton)) {
						Application.Quit();
					}
				}
			}
			else
			{
				GUI.DrawTexture(new Rect (Screen.width * xBoxTransform - 125, Screen.height * yButtonOffset + 40, 250, 50), loadingText);
			}
			//At the highscore screen
		}
		else if (scores) 
		{
			menuOverlay.normal.background = highscoreMenuOverlay;
			buttonOverlay.normal.background = CandyCorn_Normal;
			buttonOverlay.hover.background = CandyCorn_Hover;
			buttonOverlay.active.background = CandyCorn_Active;
			
			GUI.Box (new Rect (Screen.width * xBoxTransform - 225, Screen.height * .2f, 450, 350), "");
			
			//Draw Characters
			GUI.DrawTexture (new Rect (Screen.width * .13f, Screen.height * .32f, 217, 215), parentSprite);
			GUI.DrawTexture (new Rect (Screen.width * xNWImageOffset, Screen.height * .36f, 150, 180), ghostSprite);
			
			//Return Button
			if (GUI.Button (new Rect (Screen.width * xReturnButtonOffset - 50, Screen.height * yReturnButtonOffset, 50, 35), "")) 
			{
				main = true;
				scores = false;
			}

			labelOverlay.fontSize = 16;

			for(int i = 0; i < 10; i++)
			{
				if(PlayerPrefs.HasKey("score" + i))
				{
					int hiScore = PlayerPrefs.GetInt("score" + i);
					string hiName = PlayerPrefs.GetString("name" + i);

					GUI.Label(new Rect(Screen.width * xScoresOffset, yScoresOffset + (i*20), Screen.width, Screen.height), "" + (i + 1) + ": " + hiName + " - " + hiScore);
				}
				else
				{
					return;
				}
			}
		}
		else if(howToPlay)
		{
			menuOverlay.normal.background = highscoreMenuOverlay;
			buttonOverlay.normal.background = CandyCorn_Normal;
			buttonOverlay.hover.background = CandyCorn_Hover;
			buttonOverlay.active.background = CandyCorn_Active;

			if(howToPlayPage == 1)
			{
				GUI.DrawTexture (new Rect (Screen.width * xBoxTransform - 225, Screen.height * .1f, 450, 450), howToPlay1);

				if (GUI.Button (new Rect (Screen.width * xReturnButtonOffset - 50, Screen.height * yReturnButtonOffset + 45, 50, 35), "")) 
				{
					main = true;
					scores = false;
					howToPlay = false;
				}

				buttonOverlay.normal.background = CandyCorn_Normal_JoeSucks;
				buttonOverlay.hover.background = CandyCorn_Hover_JoeSucks;
				buttonOverlay.active.background = CandyCorn_Active_JoeSucks;

				if (GUI.Button (new Rect (Screen.width * xReturnButtonOffset + 225, Screen.height * yReturnButtonOffset + 40, 50, 35), "")) 
				{
					howToPlayPage = 2;
				}
			}
			else
			{
				GUI.DrawTexture (new Rect (Screen.width * xBoxTransform - 225, Screen.height * .1f, 450, 450), howToPlay2);

				if (GUI.Button (new Rect (Screen.width * xReturnButtonOffset - 50, Screen.height * yReturnButtonOffset + 45, 50, 35), "")) 
				{
					howToPlayPage = 1;
				}
			}
		}
	}
}
