using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{

	private Rigidbody2D rb2D;

	void Awake()
	{
		rb2D = GetComponent<Rigidbody2D>();
	}

	public void Hit(bool fallRight, float hitPower, float hitUp)
	{
		rb2D.AddForce((fallRight ? Vector2.right : Vector2.left) * hitPower + Vector2.up * hitUp);
	}
}
