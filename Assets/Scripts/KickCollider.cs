using UnityEngine;
using System.Collections;

public class KickCollider : MonoBehaviour {

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Monster")
		{
			GameManager.Instance.player.TouchingMonster(other.gameObject);
		}
	}
}
