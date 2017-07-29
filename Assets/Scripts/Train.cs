using UnityEngine;
using System.Collections;

public class Train : MonoBehaviour
{
	public BoxCollider2D ceilingCollider;
	public Vector2 ceilingOffset;
	public Vector2 ceilingSize;

	public Vector2 ladderCeilingOffset;
	public Vector2 ladderCeilingSize;

	public bool ladder;

	public PolygonCollider2D leftWall;
	public PolygonCollider2D rightWall;
	public float disableWallsIfPlayerAboveY;

	void Awake()
	{
		Refresh(ladder);
	}

	public void Refresh(bool hasLadder)
	{
		ceilingCollider.size = hasLadder ? ladderCeilingSize : ceilingSize;
		ceilingCollider.offset = hasLadder ? ladderCeilingOffset : ceilingOffset;
	}

	void Update()
	{
		var playerPosition = GameManager.Instance.player.transform.position;
		leftWall.enabled = rightWall.enabled = playerPosition.y < disableWallsIfPlayerAboveY;
	}
}
