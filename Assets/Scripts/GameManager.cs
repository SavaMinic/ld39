using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	public float DistanceRemaining { get; private set; }

	public PlayerController player;
	public Transform CratesContainer;

	public float maxPower = 100f;
	public float powerDecreaseOverTime = 3f;
	public Text powerLabel;
	public Color normalPowerColor;
	public Color warningPowerColor;
	public Color dangerPowerColor;
	public Text powerIncreaseLabel;


	public float destinationDistance = 15000f;
	public float distanceCoveredOverTime = 100f;
	public Text destinationLabel;

	void Awake()
	{
		Application.targetFrameRate = 60;
		powerIncreaseLabel.GetComponent<CanvasRenderer>().SetAlpha(0f);

		StartNewGame();
	}

	public void StartNewGame()
	{
		State = GameState.Playing;
		Power = maxPower;
		DistanceRemaining = destinationDistance;
	}

	public static string ToRGBHex(Color c)
	{
		return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
	}

	private static byte ToByte(float f)
	{
		f = Mathf.Clamp01(f);
		return (byte)(f * 255);
	}

	void Update()
	{
		if (IsPlaying)
		{
			Power -= powerDecreaseOverTime * Time.deltaTime;
			if (Power <= 0f)
			{
				Power = 0f;
				EndGame(false);
			}
			DistanceRemaining -= distanceCoveredOverTime * Time.deltaTime;
			if (DistanceRemaining <= 0f)
			{
				DistanceRemaining = 0f;
				EndGame(true);
			}
		}

		// update ui
		var powerPercent = Power / maxPower;
		Color powerColor;
		if (powerPercent > 0.85f)
		{
			powerColor = normalPowerColor;
		}
		else if (powerPercent > 0.6f)
		{
			powerColor = Color.Lerp(warningPowerColor, normalPowerColor, powerPercent - 0.6f);
		}
		else if (powerPercent > 0.3f)
		{
			powerColor = Color.Lerp(dangerPowerColor, warningPowerColor, powerPercent - 0.3f);
		}
		else
		{
			powerColor = dangerPowerColor;
		}
		powerLabel.text = string.Format("Power: <color={1}><b>{0:P1}</b></color>", Power / maxPower, ToRGBHex(powerColor));

		destinationLabel.text = string.Format("{0:0.00}km to Belgrade", DistanceRemaining / 1000f);
	}

	public void AddToPower(float amount)
	{
		Power = Mathf.Min(Power + amount, maxPower);
		StartCoroutine(ShowIncrease(amount));
	}

	public void EndGame(bool isWon)
	{
		State = GameState.End;
		Debug.Log("END");
	}

	private IEnumerator ShowIncrease(float amount)
	{
		powerIncreaseLabel.text = "+" + amount;
		powerIncreaseLabel.CrossFadeAlpha(1f, .15f, false);

		yield return new WaitForSeconds(0.5f);

		powerIncreaseLabel.CrossFadeAlpha(0f, .2f, false);
	}
}
