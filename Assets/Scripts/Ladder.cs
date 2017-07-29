using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{

	public Collider2D ceilingCollider;

	public void SetCeilingActive(bool isActive)
	{
		ceilingCollider.enabled = isActive;
	}

}
