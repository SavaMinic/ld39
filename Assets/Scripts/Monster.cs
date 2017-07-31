using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{

	private Rigidbody2D rb2D;
	private Animator animator;

	public GameObject carryHitArea;
	public ParticleSystem explodeParticles;
	public SpriteRenderer monsterRenderer;

	public bool goingRight;
	public bool wentRightInLastFrame;
	public float speed = 10f;
	public float maxSpeed = 20f;

	public int health = 3;
	public bool canBeHit;
	public bool isKO = false;
	public float powerWhenBurned = 25f;

	public float kickPower = 50f;
	public float kickUp = 10f;

	public bool isCarried;
	public bool isDestroyed;

	void Awake()
	{
		rb2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		goingRight = Random.value > 0.5f;

		speed += Random.value * 0.2f - 0.1f;
		animator.SetBool("isDead", false);
		canBeHit = true;
	}

	public void Hit(bool fallRight, float hitPower, float hitUp)
	{
		rb2D.AddForce((fallRight ? Vector2.right : Vector2.left) * hitPower + Vector2.up * hitUp);
		if (!isKO && canBeHit)
		{
			canBeHit = false;
			health--;
			if (health == 0)
			{
				KO();
			}
			else
			{
				StartCoroutine(WasHit());
			}
		}
	}

	void Update()
	{
		if (isKO)
			return;
		
		var scaleX = Mathf.Abs(transform.localScale.x);
		transform.localScale = new Vector2 (scaleX * (wentRightInLastFrame ? -1f : 1f), scaleX);

		animator.SetBool("isRunning", Mathf.Abs(rb2D.velocity.x) > 0.3f);
	}

	void FixedUpdate()
	{
		if (!GameManager.Instance.IsPlaying)
			return;

		if (isKO)
			return;

		rb2D.AddForce(Vector2.right * speed * (goingRight ? 1f: -1f));
		wentRightInLastFrame = goingRight;

		if(rb2D.velocity.x > maxSpeed)
		{
			rb2D.velocity =  new Vector2(maxSpeed, rb2D.velocity.y);
		}

		if(rb2D.velocity.x < -maxSpeed)
		{
			rb2D.velocity =  new Vector2(-maxSpeed, rb2D.velocity.y);
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Ground")
		{
			Die();
		}
		else if (other.gameObject.tag == "Crate")
		{
			goingRight = !goingRight;
		}
		else if (other.gameObject.tag == "Gentleman" && !isKO)
		{
			GameManager.Instance.player.TakeDamage();
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "TrainEngine")
		{
			Die(true);
		}
		else if (other.tag == "Ground")
		{
			Die();
		}
		else if (other.gameObject.tag == "MonsterLimit")
		{
			goingRight = !goingRight;
		}
	}

	void OnCollisionStay2D(Collision2D other)
	{
		if (other.gameObject.tag == "Gentleman")
		{
			GameManager.Instance.player.TouchingMonster(gameObject);
		}
	}

	public void KO()
	{
		monsterRenderer.color = Color.white;
		isKO = true;
		animator.SetBool("isDead", true);
		carryHitArea.SetActive(true);
	}

	public void Die(bool addPower = false)
	{
		if (isDestroyed)
			return;
		isDestroyed = true;

		if (isCarried)
		{
			GameManager.Instance.player.ReleaseCarriedObject();
		}
		StartCoroutine(Explode());
		if (addPower)
		{
			GameManager.Instance.AddToPower(powerWhenBurned);
		}
	}

	public void CarryMonster(Transform parentTransform)
	{
		isCarried = true;

		//rb2D.simulated = false;
		rb2D.mass = 0f;
		rb2D.gravityScale = 0f;
		rb2D.drag = 100f;
		rb2D.angularDrag = 1f;

		transform.SetParent(parentTransform);
		var v = parentTransform.localScale;
		transform.localScale = new Vector3(0.2f/v.x, 0.2f/v.y);
	}

	public void ReleaseMonster(bool kickIt = false, bool wentRightInLastFrame = false)
	{
		isCarried = false;

		//rb2D.simulated = true;
		rb2D.mass = 5f;
		rb2D.gravityScale = 1f;
		rb2D.drag = 0f;
		rb2D.angularDrag = 0.05f;

		transform.SetParent(GameManager.Instance.CratesContainer);
		transform.localScale = Vector3.one * 0.2f;

		if (kickIt)
		{
			rb2D.AddForce((wentRightInLastFrame ? Vector2.right : Vector2.left) * kickPower + (Vector2.up * kickUp));
		}
	}

	public IEnumerator Explode()
	{
		monsterRenderer.enabled = false;
		explodeParticles.Play();

		yield return new WaitForSeconds(1f);

		GameObject.Destroy(gameObject);
	}

	public IEnumerator WasHit()
	{
		monsterRenderer.color = Color.white;

#if UNITY_WEBGL
		monsterRenderer.color = Color.red;
#else
		Go.to(monsterRenderer, 0.3f, new GoTweenConfig()
			.colorProp("color", Color.red)
			.setIterations(2, GoLoopType.PingPong)
		);
#endif

		yield return new WaitForSeconds(.3f);

#if UNITY_WEBGL
		monsterRenderer.color = Color.white;
#endif
		canBeHit = true;
	}
}
