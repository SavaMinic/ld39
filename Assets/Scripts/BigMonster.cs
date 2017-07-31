using UnityEngine;
using System.Collections;

public class BigMonster : MonoBehaviour
{

	void Start()
	{
		GetComponent<Animator>().SetBool("isRunning", true);
	}
}
