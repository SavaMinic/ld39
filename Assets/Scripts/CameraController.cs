using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

	public float minPosition = -5.6f;
	public float maxPosition = 6.25f;

	public float speed = 10f;

	private Vector3 offset;
	private Vector3 playerPosition { get { return GameManager.Instance.player.transform.position; } }

	void Start()
	{
		offset = transform.position - playerPosition;
	}

	void LateUpdate () 
	{
		var newPosition = playerPosition + offset;
		var x = Mathf.Clamp(newPosition.x, minPosition, maxPosition);
		newPosition = new Vector3(x, newPosition.y / 10f, transform.position.z);

		transform.position = Vector3.Lerp(transform.position, newPosition, speed * Time.deltaTime);
	}
}
