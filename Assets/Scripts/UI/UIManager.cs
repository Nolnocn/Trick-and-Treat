using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour 
{
	
	//Pause Menu UI
	
	//Box Attributes
	public float xBoxTransform; //.5
	public float yBoxTransform; //.2
	//Offset for all Buttons (Except Return button on Score Screen)
	public float yButtonOffset; //.34
	//Return Button Attributes
	public float xReturnButtonOffset; //.38
	public float yReturnButtonOffset; //.69
	//Tom Image (Score Screen)
	public float xTomImageOffset; //.1
	public float yTomImageOffset; //.31
	public float tomImageScale; //225
	//NW Image (Score Screen)
	public float xNWImageOffset; //.65
	public float yNWImageOffset; //.31
	public float NWImageScale; //225
	// Score
	public float xScoresOffset;//.37
	public float yScoresOffset;//-95.7
	//UI Logic
	public bool paused = false;
	public bool scores = false;
	//GUI Skin
	public GUISkin skin;
	//UI Sprites
	public Texture tomSprite;
	public Texture NWSprite;
	
	//Game play UI
	public float xFaceTomOffset;//.0125
	public float yFaceTomOffset;//.02
	public float xFaceScale;//85
	public float yFaceScale;//98
	
	public Texture tomFaceDamagedSprite;
	public Texture tomFaceDamagedSprite2;
	public Texture tomFaceSprite;
	
	public int lives = 3;
	public bool shocked = false;
	int shockCounter = 0;
	
	//Menu and Button Texture Overlays
	public Texture2D pauseMenuOverlay;
	public Texture2D highscoreMenuOverlay;
	public Texture2D scoreButton;
	public Texture2D resumeButton;
	public Texture2D menuButton;
	public Texture2D pauseMenuButton_Normal;
	public Texture2D pauseMenuButton_Hover;
	public Texture2D pauseMenuButton_Active;
	public Texture2D CandyCorn_Normal;
	public Texture2D CandyCorn_Hover;
	public Texture2D CandyCorn_Active;

	public int score = 0;

	public Font ghostGuys;
	public Font alternateFont;
	
	
	void OnGUI()
	{
		GUI.skin = skin;
		var menuOverlay = GUI.skin.GetStyle("Box");
		var buttonOverlay = GUI.skin.GetStyle("Button");
		var labelOverlay = GUI.skin.GetStyle("Label");
		labelOverlay.fontSize = 54;
		labelOverlay.font = ghostGuys;
		labelOverlay.normal.textColor = Color.white;
		
		//At the pause menu
		if (paused) {

			if(!Cursor.visible)
			{
				Cursor.visible = true;
			}

			menuOverlay.normal.background = pauseMenuOverlay;
			buttonOverlay.normal.background = pauseMenuButton_Normal;
			buttonOverlay.hover.background = pauseMenuButton_Hover;
			buttonOverlay.active.background = pauseMenuButton_Active;
			
			GUI.Box (new Rect (Screen.width * xBoxTransform - 175, Screen.height * yBoxTransform, 350, 350), "");
			
			//PLAY
			if(GUI.Button (new Rect (Screen.width * xBoxTransform - 125, Screen.height * yButtonOffset, 250, 50), resumeButton)) {
				TogglePause();
			}
			//HIGH SCORES
			if (GUI.Button (new Rect (Screen.width * xBoxTransform - 125, Screen.height * yButtonOffset + 80, 250, 50), scoreButton)) {
				scores = true;
				paused = false;
			}
			//MAIN MENU
			if (GUI.Button (new Rect (Screen.width * xBoxTransform - 125, Screen.height * yButtonOffset + 160, 250, 50), menuButton)) {
				TogglePause();
				//Application.LoadLevel("MainMenu");
				SceneManager.LoadScene("Game");
			}
			//At the highscore screen
		} 
		else if (scores) 
		{
			if(!Cursor.visible)
			{
				Cursor.visible = true;
			}

			menuOverlay.normal.background = highscoreMenuOverlay;
			buttonOverlay.normal.background = CandyCorn_Normal;
			buttonOverlay.hover.background = CandyCorn_Hover;
			buttonOverlay.active.background = CandyCorn_Active;
			
			GUI.Box (new Rect (Screen.width * xBoxTransform - 225, Screen.height * yBoxTransform, 450, 350), "");
			
			//Draw Characters
			GUI.DrawTexture (new Rect (Screen.width * xTomImageOffset, Screen.height * yTomImageOffset, tomImageScale, tomImageScale), tomSprite);
			GUI.DrawTexture (new Rect (Screen.width * xNWImageOffset, Screen.height * yNWImageOffset, tomImageScale, NWImageScale), NWSprite);

			labelOverlay.fontSize = 16;
			labelOverlay.font = alternateFont;
			labelOverlay.normal.textColor = new Color(.75f,.42f,.12f,1.0f);
			
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

			//Return Button
			if (GUI.Button (new Rect (Screen.width * xReturnButtonOffset - 50, Screen.height * yReturnButtonOffset, 50, 35), "")) 
			{
				paused = true;
				scores = false;
			}
			
			//Write to screen the high scores
			
		} 
		else  //While the game is playing
		{
			if(Cursor.visible)
			{
				Cursor.visible = false;
			}

			for(int i = 0; i < lives; i++)
			{
				//If shocked, alternate between two sprites
				if(i == lives - 1 && shocked)
				{
					if(shockCounter >= 0 && shockCounter <= 10)
					{
						GUI.DrawTexture (new Rect (Screen.width * xFaceTomOffset + (i * xFaceScale), Screen.height * yFaceTomOffset, xFaceScale, yFaceScale), tomFaceDamagedSprite);
						shockCounter++;
					}
					else if(shockCounter >= 11 && shockCounter <= 20) //Switch to the next one
					{
						GUI.DrawTexture (new Rect (Screen.width * xFaceTomOffset + (i * xFaceScale), Screen.height * yFaceTomOffset, xFaceScale, yFaceScale), tomFaceDamagedSprite2);
						shockCounter++;
					}
					else //Reset the counter
					{
						GUI.DrawTexture (new Rect (Screen.width * xFaceTomOffset + (i * xFaceScale), Screen.height * yFaceTomOffset, xFaceScale, yFaceScale), tomFaceDamagedSprite);
						shockCounter = 0;
					}
					
				}
				else
				{
					GUI.DrawTexture (new Rect (Screen.width * xFaceTomOffset + (i * xFaceScale), Screen.height * yFaceTomOffset, xFaceScale, yFaceScale), tomFaceSprite);
				}
			}
			
			//Set the alignment for the score label
			var centeredStyle = GUI.skin.GetStyle("Label");
			centeredStyle.alignment = TextAnchor.MiddleRight;
			
			//Score Label
			GUI.Label(new Rect (0,Screen.height * .9f, Screen.width * .5f, Screen.height * .1f), "Score:");
			
			centeredStyle.alignment = TextAnchor.MiddleLeft;
			
			GUI.Label(new Rect (Screen.width * .5f,Screen.height * .9f, Screen.width * .5f, Screen.height * .1f), " " + score);
		}
	}

	public void ShockedUI()
	{
		shocked = !shocked;
	}

	public void LoseLife()
	{
		lives--;
	}

	public void SetScore(int amt)
	{
		score = amt;
	}

	// Joe smells
	public void TogglePause()
	{
		paused = ! paused;
		if(paused)
		{
			Time.timeScale = 0.0f;
		}
		else
		{
			Time.timeScale = 1.0f;
			scores = false;
		}
	}

	public void GameOver()
	{
		PlayerPrefs.SetInt("currScore", score);
		//Application.LoadLevel("LoseScreen");
		SceneManager.LoadSceneAsync("LoseScreen");
	}
}

