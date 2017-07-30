using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

	#region Singleton

	private static GameManager instance;

	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<GameManager>();
			}
			return instance;
		}
	}

	#endregion

	public enum GameState
	{
		MainMenu,
		Playing,
		End
	}

	public GameState State { get; private set; }
	public bool IsPlaying { get { return State == GameState.Playing; } }

	public float Power { get; private set; }

	public PlayerController player;
	public Transform CratesContainer;

	public float maxPower = 100f;
	public float powerDecreaseOverTime = 3f;

	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	public void StartNewGame()
	{
		State = GameState.Playing;
		Power = maxPower;
	}

	void Update()
	{
		if (IsPlaying)
		{
			maxPower -= powerDecreaseOverTime * Time.deltaTime;
			if (maxPower <= 0)
			{
				EndGame();
			}
		}
	}

	public void AddToPower(float amount)
	{
		Power = Mathf.Min(Power + amount, maxPower);
	}

	public void EndGame()
	{
		State = GameState.End;
	}
}
