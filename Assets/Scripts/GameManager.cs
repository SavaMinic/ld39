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

	public PlayerController player;

	void Awake()
	{
		Application.targetFrameRate = 60;
	}
}
