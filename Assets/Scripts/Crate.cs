using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour
{

	public Rigidbody2D normalBody;
	public SpriteRenderer crateRenderer;

	public void CarryCrate(Transform parentTransform)
	{
		normalBody.simulated = false;

		transform.SetParent(parentTransform);
		var v = parentTransform.localScale;
		transform.localScale = new Vector3(1f/v.x, 1f/v.y);

		crateRenderer.sortingOrder = 15;
	}

	public void ReleaseCrate()
	{
		normalBody.simulated = true;

		transform.SetParent(GameManager.Instance.CratesContainer);
		transform.localScale = Vector3.one;

		crateRenderer.sortingOrder = 10;
	}

}
