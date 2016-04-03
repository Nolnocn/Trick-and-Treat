using UnityEngine;
using System.Collections;

public class LoseScreenUIManager : MonoBehaviour {

	public Texture2D CandyCorn_NormalUp;
	public Texture2D CandyCorn_HoverUp;
	public Texture2D CandyCorn_ActiveUp;
	public Texture2D CandyCorn_NormalDown;
	public Texture2D CandyCorn_HoverDown;
	public Texture2D CandyCorn_ActiveDown;

	public Texture2D scoreButton;
	public Texture2D menuButton;

	public Texture2D Button_Normal;
	public Texture2D Button_Hover;
	public Texture2D Button_Active;

	public Texture Background;

	public float xTextOffset; //.22
	public float yTextOffset; //.555
	public float xTextOffset2; //.74
	public float yTextOffset2; //.19
	public float xTextOffset3;//.69
	public float yTextOffset3;//.82
	public float xTextOffset4;//.74
	public float yTextOffset4;//.36
	public float xBoxTransform; //.92
	public float yBoxTransform; //.16
	public float xButtonOffset;//.705
	public float yButtonOffset;//.463
	public float yButtonOffset2;//.7

	int name1 = 65;
	int name2 = 65;
	int name3 = 65;

	int score = 0;

	public Font deathScreenFont;
	public Font ghostFont;
	public Color fontcolor;

	//GUI Skin
	public GUISkin skin;

	void Start()
	{
		Cursor.visible = true;
		score = PlayerPrefs.GetInt("currScore");
	}

	void OnGUI()
	{
		GUI.skin = skin;
		var buttonOverlay = GUI.skin.GetStyle("Button");
		var labelOverlay = GUI.skin.GetStyle("Label");

		labelOverlay.font = ghostFont;
		labelOverlay.normal.textColor = Color.black;
		labelOverlay.fontStyle = FontStyle.Bold;

		GUI.DrawTexture(new Rect(0, 0 , 960, 600),Background);

		GUI.Box(new Rect (Screen.width * xBoxTransform - 275, Screen.height * yBoxTransform, 350, 400), "");

		labelOverlay.fontSize = 64;

		GUI.Label(new Rect (Screen.width * xTextOffset,Screen.height * yTextOffset, Screen.width , Screen.height * .1f), "" + System.Convert.ToChar(name1));
		GUI.Label(new Rect (Screen.width * xTextOffset + 85,Screen.height * yTextOffset, Screen.width , Screen.height * .1f), "" + System.Convert.ToChar(name2));
		GUI.Label(new Rect (Screen.width * xTextOffset + 170,Screen.height * yTextOffset, Screen.width , Screen.height * .1f), "" + System.Convert.ToChar(name3));


		buttonOverlay.normal.background = CandyCorn_NormalUp;
		buttonOverlay.hover.background = CandyCorn_HoverUp;
		buttonOverlay.active.background = CandyCorn_ActiveUp;

		//Top Three
		if(GUI.Button (new Rect (Screen.width * xButtonOffset, Screen.height * yButtonOffset, 35, 50), "")) {

			if(name1 < 66)
			{
				name1 = 90;
			}
			else
			{
				name1--;
			}
		}
		if(GUI.Button (new Rect (Screen.width * xButtonOffset + 85, Screen.height * yButtonOffset, 35, 50), "")) {

			if(name2 < 66)
			{
				name2 = 90;
			}
			else
			{
				name2--;
			}
		}
		if(GUI.Button (new Rect (Screen.width * xButtonOffset + 170, Screen.height * yButtonOffset, 35, 50), "")) {

			if(name3 < 66)
			{
				name3 = 90;
			}
			else
			{
				name3--;
			}
		}

		//Bottom Three
		buttonOverlay.normal.background = CandyCorn_NormalDown;
		buttonOverlay.hover.background = CandyCorn_HoverDown;
		buttonOverlay.active.background = CandyCorn_ActiveDown;

		if(GUI.Button (new Rect (Screen.width * xButtonOffset, Screen.height * yButtonOffset2, 35, 50), "")) {

			if(name1 > 89)
			{
				name1 = 65;
			}
			else
			{
				name1++;
			}
		}
		if(GUI.Button (new Rect (Screen.width * xButtonOffset + 85, Screen.height * yButtonOffset2, 35, 50), "")) {

			if(name2 > 89)
			{
				name2 = 65;
			}
			else
			{
				name2++;
			}
		}
		if(GUI.Button (new Rect (Screen.width * xButtonOffset + 170, Screen.height * yButtonOffset2, 35, 50), "")) {
			
			if(name3 > 89)
			{
				name3 = 65;
			}
			else
			{
				name3++;
			}
		}

		labelOverlay.font = deathScreenFont;
		labelOverlay.normal.textColor = new Color(.75f,.42f,.12f,1.0f);
		labelOverlay.fontStyle = FontStyle.Normal;
		labelOverlay.fontSize = 30;

		//score = 10000;

		GUI.Label(new Rect (Screen.width * xBoxTransform - 275,Screen.height * yTextOffset2, 350, Screen.height * .1f), "Score:");
		GUI.Label(new Rect (Screen.width * xBoxTransform - 275,Screen.height * yTextOffset2 + 40, 350 , Screen.height * .1f), "" + score);

		GUI.Label(new Rect (Screen.width * xBoxTransform - 275,Screen.height * yTextOffset4, 350, Screen.height * .1f), "Name:");

		buttonOverlay.normal.background = Button_Normal;
		buttonOverlay.hover.background = Button_Hover;
		buttonOverlay.active.background = Button_Active;

		if(GUI.Button (new Rect (Screen.width * xTextOffset3, Screen.height * yTextOffset3, 250, 50), scoreButton)) {
			DoScoreStuff();
			if(PlayerPrefs.HasKey("currScore"))
			{
				PlayerPrefs.DeleteKey("currScore");
			}
			PlayerPrefs.SetInt("goToScore", 1);
			Application.LoadLevel("MainMenu");
		}
		if(GUI.Button (new Rect (Screen.width * xTextOffset3, Screen.height * yTextOffset3 + 55, 250, 50), menuButton)) {
			Application.LoadLevel("MainMenu");
		}

	}

	private void DoScoreStuff()
	{
		char letter1 = System.Convert.ToChar(name1);
		char letter2 = System.Convert.ToChar(name2);
		char letter3 =  System.Convert.ToChar(name3);
		string name = "" + letter1 + letter2 + letter3;
		int tmpScore;
		string tmpName;
		for(int i = 0; i < 10; i++)
		{
			if(PlayerPrefs.HasKey("score" + i))
			{
				if(PlayerPrefs.GetInt("score" + i) < score)
				{
					tmpScore = PlayerPrefs.GetInt("score" + i);
					tmpName = PlayerPrefs.GetString("name" + i);
					PlayerPrefs.SetInt("score" + i, score);
					PlayerPrefs.SetString("name" + i, name);

					for(int j = i + 1; j < 10; j++)
					{
						string tmpName2 = tmpName;
						int tmpScore2 = tmpScore;
						tmpScore = PlayerPrefs.GetInt("score" + j);
						tmpName = PlayerPrefs.GetString("name" + j);
						PlayerPrefs.SetInt("score" + j, tmpScore2);
						PlayerPrefs.SetString("name" + j, tmpName2);
					}

					return;
				}
			}
			else
			{
				PlayerPrefs.SetString("name" + i, name);
				PlayerPrefs.SetInt("score" + i, score);
			}
		}
	}
}
