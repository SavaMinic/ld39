using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour
{

	public Rigidbody2D normalBody;
	public SpriteRenderer crateRenderer;
	public ParticleSystem explodeParticles;

	public float kickPower = 50f;
	public float kickUp = 10f;

	public float powerWhenBurned = 25f;

	private bool isDestroyed;
	private bool isCarried;

	public void CarryCrate(Transform parentTransform)
	{
		isCarried = true;

		//normalBody.simulated = false;
		normalBody.mass = 0f;
		normalBody.gravityScale = 0f;
		normalBody.drag = 100f;
		normalBody.angularDrag = 1f;

		transform.SetParent(parentTransform);
		var v = parentTransform.localScale;
		transform.localScale = new Vector3(1f/v.x, 1f/v.y);

		crateRenderer.sortingOrder = 15;
	}

	public void ReleaseCrate(bool kickIt = false, bool wentRightInLastFrame = false)
	{
		isCarried = false;

		//normalBody.simulated = true;
		normalBody.mass = 5f;
		normalBody.gravityScale = 1f;
		normalBody.drag = 0.5f;
		normalBody.angularDrag = 0.05f;

		transform.SetParent(GameManager.Instance.CratesContainer);
		transform.localScale = Vector3.one;

		crateRenderer.sortingOrder = 10;

		if (kickIt)
		{
			normalBody.AddForce((wentRightInLastFrame ? Vector2.right : Vector2.left) * kickPower + (Vector2.up * kickUp));
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "TrainEngine" || other.tag == "Ground")
		{
			if (isDestroyed)
				return;

			isDestroyed = true;
			if (isCarried)
			{
				GameManager.Instance.player.ReleaseCrate();
			}
			StartCoroutine(Explode());
			if (other.tag == "TrainEngine")
			{
				GameManager.Instance.AddToPower(powerWhenBurned);
			}
		}
	}

	public IEnumerator Explode()
	{
		crateRenderer.enabled = false;
		explodeParticles.Play();

		yield return new WaitForSeconds(1f);

		GameObject.Destroy(gameObject);
	}

}
