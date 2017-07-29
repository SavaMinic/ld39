using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	protected KeyCode LeftKey { get { return KeyCode.A; } }
	protected KeyCode RightKey { get { return KeyCode.D; } }
	protected KeyCode JumpKey { get { return KeyCode.W; } }
	protected KeyCode KickKey { get { return KeyCode.Space; } }
	protected KeyCode PickUpKey { get { return KeyCode.S; } }



	public float speed = 50f;
	public float jumpPower = 150f;
	public float maxSpeed = 100f;

	public float kickPower = 50f;
	public float kickUp = 10f;
	public float kickDuration = 0.4f;

	public float hitPower = 50f;
	public float hitUp = 10f;
	public float hitDuration = 0.4f;

	private Animator animator;
	private Rigidbody2D rb2D;

	public bool isJumping;
	public bool isKicking;
	public bool goingRight;
	public bool wentRightInLastFrame;

	void Awake()
	{
		rb2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		// control the rotation
		goingRight = !Input.GetKey(LeftKey);

		var scaleX = Mathf.Abs(transform.localScale.x);
		transform.localScale = new Vector2 (scaleX * (wentRightInLastFrame ? 1f : -1f), scaleX);
		//animator.SetBool("isRunning", !isJumping && !isKicking && (Input.GetKey (LeftKey) || Input.GetKey (RightKey)));

		//animator.SetBool("isJumping", !isKicking && Mathf.Abs(rb2D.velocity.y) > 0.05f);

		// dont touch this, it's working
		if ((!isJumping || Mathf.Abs(rb2D.velocity.y) < 0.05f) && Input.GetKeyDown(JumpKey))
		{
			isJumping = true;
			rb2D.AddForce(Vector2.up * jumpPower);
		}

		if (!isKicking && Input.GetKeyDown (KickKey))
		{
			//animator.SetTrigger("kick");
			isKicking = true;
			rb2D.AddForce ((wentRightInLastFrame ? Vector2.right : Vector2.left) * kickPower + (Vector2.up * kickUp));
			Invoke("FinishKick", kickDuration);
		}

		// just animation
		if (!isJumping && !isKicking && Input.GetKeyDown(PickUpKey)) {
			//animator.SetTrigger("punch");
		}
	}

	protected virtual void FixedUpdate()
	{

		bool left = Input.GetKey(LeftKey);
		bool right = Input.GetKey(RightKey);

		if (left || right)
		{
			wentRightInLastFrame = right;

			// check if we can move at all
			rb2D.AddForce(Vector2.right * speed * (left ? -1f: 1f));
		}

		if(rb2D.velocity.x > maxSpeed)
		{
			rb2D.velocity =  new Vector2(maxSpeed, rb2D.velocity.y);
		}

		if(rb2D.velocity.x < -maxSpeed)
		{
			rb2D.velocity =  new Vector2(-maxSpeed, rb2D.velocity.y);
		}
	}

	private void FinishKick()
	{
		isKicking = false;
	}
}
