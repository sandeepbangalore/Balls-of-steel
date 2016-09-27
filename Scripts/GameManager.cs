using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Game Manager States
public enum GameState { GetReady, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour {
	//Public variables
	public List<GameObject> BrickPrefab = new List<GameObject>();
	public List<GameObject> BonusPrefab = new List<GameObject> ();
	public float BonusChance = 5.5f;
	public GameObject BallPrefab = null;
	public Transform BallSpawnPoint = null;


	public TextMesh ScoreText = null;
	public TextMesh HiScoreText = null;
	public TextMesh BallsLeftText = null;
	public TextMesh GetReadyText = null;
	public TextMesh GameOverText = null;

	public float WallSpeed = 0.0f;
	public float MaxWallSpeed = 0.3f;
	public float fadeTime = 2.0f;

	public AudioSource BrickLoweringAudio = null;
	public float WallLoweringVolume = 0.8f;
	public GameState CurrentState = GameState.GetReady;
	public GUISkin GUISkin_user = null;

	//Private variables
	private int BallsInPlay = 0;
	private int BallCount = 6;
	private int Score = 0;
	private int HiScore = 0;

	private float[] BrickWeightTable = null;
	private float[] BonusWeightTable = null;
	private int[] HalfRow = new int[5];

	private Rect PauseBox_ScreenRect = new Rect (700,450,520,320);
	private Rect Continue_ScreenRect = new Rect (800,550,320,60);
	private Rect Quit_ScreenRect = new Rect (800,650,320,60);


	private float RowInterpolator = 0.0f;
	private Dictionary<string,Timer> Timers = new Dictionary<string, Timer>();

	private static GameManager _Instance = null;
	private Screen_Manager_Base MyScreenManager = null;
	//Singleton representation of Game Manager
	public static GameManager Instance{
		get{ 
			if (_Instance == null) {
				_Instance = (GameManager)FindObjectOfType (typeof(GameManager));
			}
			return _Instance;	
		}
	}

	public void RegisterTimer (string TimerName) {
		if (!Timers.ContainsKey (TimerName)) {
			Timers.Add (TimerName, new Timer ());
		}
	}

	public void UpdateTimer (string TimerName, float t ){
		Timer timer;
		if (Timers.TryGetValue (TimerName, out timer))
			timer.AddTime (t);
	}

	public float GetTime (string TimerName) {
		Timer timer; 
		if (Timers.TryGetValue (TimerName, out timer)) {
			return timer.GetTime();
		}
		return -1.0f;
	}

	void Awake () {
		int bi = 0, i;

		MyScreenManager = Screen_Manager_Base.Instance;

		BuildBrickWeightTable ();
		BonusBrickWeightTable ();

		if (BrickPrefab.Count > 0) {
				for (int row = 12; row > 5; row--) {
					for (i = 0; i < 5; i++) {
							HalfRow [i] = NextBrickIndex ();
					}
					int direction = 1; 
					i = 0;
					for (int col = -10; col < 12; col += 2) {
					if (Chance () < BonusChance && BonusPrefab.Count > 0) {
						bi = NextBonusIndex ();
						Instantiate (BonusPrefab [bi], new Vector3 ((float)col, (float)row + 0.5f, 0.0f), Quaternion.identity);
						if (i == 5)
							direction = -1;
					} else {
						if (i == 5) {
							bi = NextBrickIndex ();
							direction = -1;
						} else {
							bi = HalfRow [i];
						}
						Instantiate (BrickPrefab [bi], new Vector3 ((float)col, (float)row, -0.5f), Quaternion.identity);
					}
					i += direction;
					}
				}
		}
		HiScore = PlayerPrefs.GetInt ("HiScore", 0);

		if (ScoreText != null) 
			ScoreText.text = "Score: " + Score.ToString ();
		if (HiScoreText != null)
			HiScoreText.text = "HiScore: " + HiScore.ToString ();
		if (BallsLeftText != null)
			BallsLeftText.text = "Balls Left: " + BallCount.ToString ();
	
		RegisterTimer ("Wall Stop Timer");
		if (BrickLoweringAudio) {
			BrickLoweringAudio.volume = WallLoweringVolume;
		}
	}

	private void BuildBrickWeightTable () {
		int NoOfPrefabs = BrickPrefab.Count;
		int sum = 0;

		BrickWeightTable = new float[NoOfPrefabs];
		for (int i = 0; i < NoOfPrefabs; i++) {
			DestructableItem script = BrickPrefab [i].GetComponent<DestructableItem> ();
			if (script != null) {
				sum += script.Weight;
				BrickWeightTable [i] = (float)script.Weight;
			}
		}
		for (int i = 0; i < NoOfPrefabs; i++) {
			BrickWeightTable[i] /= sum;
		}
	}

	private void BonusBrickWeightTable () {
		int NoOfPrefabs = BonusPrefab.Count;
		int sum = 0;

		BonusWeightTable = new float[NoOfPrefabs];
		for (int i = 0; i < NoOfPrefabs; i++) {
			DestructableItem script = BonusPrefab [i].GetComponent<DestructableItem> ();
			if (script != null) {
				sum += script.Weight;
				BonusWeightTable [i] = (float)script.Weight;
			}
		}
		if (sum > 0) {
			for (int i = 0; i < NoOfPrefabs; i++) {
				BonusWeightTable [i] /= sum;
			}
		}
	}

	private float Chance () {
		float t = Random.value*100.0f;
		return t;
	}

	private int NextBrickIndex () { 
		float t = Random.value;
		float q = 0.0f;
		for (int i = 0; i < BrickPrefab.Count; i++) {
			q += BrickWeightTable [i];
			if (t <= q)
				return i;
		}
		return 0;
	}

	private int NextBonusIndex () {
	float t = Random.value;
	float q = 0.0f;
	for (int i = 0; i < BonusPrefab.Count; i++) {
		q += BonusWeightTable [i];
		if (t <= q)
			return i;
	}
	return 0;
}
	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		StartCoroutine (StartGame ());
	}

	private IEnumerator StartGame () {
		float timer = 2.0f;
		while (timer >= 0.0f) {
			timer -= Time.deltaTime;
			if (MyScreenManager != null)
				MyScreenManager.SetScreenFade (timer / 2.0f);
			AudioListener.volume = 1.0f - timer/2.0f;
			yield return null;

		}
		Color FadeColor = Color.yellow;
		Material FadeMaterial = null;
		timer = 2.0f;
		if (GetReadyText != null && GetReadyText.GetComponent<Renderer> () != null) {
			GetReadyText.gameObject.SetActive(true);
			FadeMaterial = GetReadyText.GetComponent<Renderer> ().material;
		}
		while (timer >= 0.0f) {
			timer -= Time.deltaTime;
			if (GetReadyText != null && FadeMaterial != null) {
				FadeColor.a = 2.0f - timer;
				FadeMaterial.color = FadeColor;
			}
			yield return null;
		}
		GetReadyText.gameObject.SetActive(false);
		CurrentState = GameState.Playing;
		if (BrickLoweringAudio)
			BrickLoweringAudio.Play ();
	}

	void FixedUpdate () {
		if (CurrentState == GameState.Playing) {
			if (GetTime ("Wall Stop Timer") > 0) {
				if (BrickLoweringAudio != null)
					BrickLoweringAudio.volume = 0;
			}
			else {
				WallSpeed *= 1.001f;
				if (BrickLoweringAudio != null)
					BrickLoweringAudio.volume = WallLoweringVolume;
			
				RowInterpolator += Time.deltaTime * WallSpeed;
				int bi = 0, i;
				if (RowInterpolator >= 1.0f) {
					float ypos = 12.0f + (RowInterpolator - 1.0f);
					for (i = 0; i < 5; i++) {
						HalfRow [i] = NextBrickIndex ();
					}
					int direction = 1; 
					i = 0;
					for (int col = -10; col < 12; col += 2) {
						if (i == 5) {
							bi = NextBrickIndex ();
							direction = -1;
						} else {
							bi = HalfRow [i];
						}
						if(Chance() < BonusChance)
							Instantiate (BonusPrefab [NextBonusIndex()], new Vector3 ((float)col, ypos + 0.5f, 0.0f), Quaternion.identity);
						else
							Instantiate (BrickPrefab [bi], new Vector3 ((float)col, ypos, -0.5f), Quaternion.identity);
						i += direction;
					}
					RowInterpolator = 0.0f;
				}
				if (WallSpeed > MaxWallSpeed) {
					WallSpeed = MaxWallSpeed; 
				}
			}
		}
	}
 
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (CurrentState == GameState.Playing) {
				CurrentState = GameState.Paused;
				Time.timeScale = 0.0f;
				Screen.lockCursor = false;
				if (MyScreenManager)
					MyScreenManager.SetScreenFade (0.7f);
				if (BrickLoweringAudio)
					BrickLoweringAudio.volume = 0.0f;
			}
			//else if (CurrentState == GameState.Paused)
			//	CurrentState = GameState.Playing;
		}
		else if (CurrentState == GameState.Playing)
			Screen.lockCursor = true;

		if (BallCount + BallsInPlay < 1) {
			EndGame ();
			return;
		}
		foreach ( KeyValuePair<string, Timer> entry in Timers ) {
			entry.Value.Tick (Time.deltaTime);
		}
}

	void OnGUI(){
		if (CurrentState != GameState.Paused)
			return;
		if (GUISkin_user)
			GUI.skin = GUISkin_user;
		float x = Screen.width / 1920.0f;
		float y = Screen.height / 1280.0f;

		Matrix4x4 oldMat = GUI.matrix;
		GUI.matrix = Matrix4x4.TRS (new Vector3 (0,0,0), Quaternion.identity, new Vector3 (x, y, 1.0f));
		GUI.Box (PauseBox_ScreenRect, "Game Paused");
		if(GUI.Button(Continue_ScreenRect, "Continue")){
			CurrentState = GameState.Playing;
			Screen.lockCursor = true;
			Time.timeScale = 1.0f;
			if(MyScreenManager)
				MyScreenManager.SetScreenFade(0.0f);
			if (BrickLoweringAudio)
				BrickLoweringAudio.volume = WallLoweringVolume;
		}
		if (GUI.Button (Quit_ScreenRect, "Quit")) {
			Time.timeScale = 1.0f;
			EndGame ();
		}
		GUI.matrix = oldMat;

	}
	public void EndGame() {
		if (CurrentState != GameState.GameOver) {
			CurrentState = GameState.GameOver;
			if (Score > HiScore)
				PlayerPrefs.SetInt ("HiScore", Score);
			StartCoroutine (GoToMenu (fadeTime));
		}	
}

	private IEnumerator GoToMenu(float duration = 2.0f){
		float timer = duration;
		Color FadeColor = Color.yellow;
		Material FadeMaterial = null;

		if (GameOverText != null && GameOverText.GetComponent<Renderer> () != null) {
			GameOverText.gameObject.SetActive (true);
			FadeMaterial = GameOverText.GetComponent<Renderer> ().material;
		}

		while (timer >= 0.0f) {
			timer -= Time.deltaTime;
			if (GameOverText != null && FadeMaterial != null) {
				FadeColor.a = duration - timer;
				FadeMaterial.color = FadeColor;
			}
			AudioListener.volume = timer/duration;
			yield return null;
		}
		GameOverText.gameObject.SetActive(false);

		timer = duration;
		while (timer >= 0.0f) {
			timer -= Time.deltaTime;
			if (MyScreenManager != null ) {
				MyScreenManager.SetScreenFade (1.0f - (timer / duration));
			}
			yield return null;
		}
		Screen.lockCursor = false;
		Application.LoadLevel ("MainMenu");
	}

	public void ReleaseBall() {
		if (BallCount > 0 && CurrentState == GameState.Playing && BallSpawnPoint != null) {
			Instantiate (BallPrefab, BallSpawnPoint.position, BallSpawnPoint.rotation);
			BallCount--;
		}
		if (BallsLeftText != null)
			BallsLeftText.text = "Balls Left: " + BallCount.ToString ();
	}

	public void AddPoints(int points) {
		if (CurrentState == GameState.Playing) {
			Score += points;
			if (ScoreText != null)
				ScoreText.text = "Score: " + Score.ToString ();
			if (Score > HiScore)
				HiScoreText.text = "HiScore: " + Score.ToString ();
		}
	}

	public void RegisterBall() {
		BallsInPlay++;
	}

	public void UnregisterBall() {
		BallsInPlay--;
	}
}







