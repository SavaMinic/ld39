using UnityEngine;
using System.Collections;

public class SpawnerManager : MonoBehaviour
{

	#region Singleton

	private static SpawnerManager instance;

	public static SpawnerManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<SpawnerManager>();
			}
			return instance;
		}
	}

	#endregion

	public GameObject monsterPrefab;

	public void SpawnMonster()
	{
		var childCount = transform.childCount;
		var spawner = transform.GetChild(Random.Range(0, childCount - 1));

		var monster = Instantiate(monsterPrefab, transform) as GameObject;
		monster.transform.position = spawner.transform.position;
	}
}
