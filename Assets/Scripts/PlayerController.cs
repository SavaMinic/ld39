using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

	#region READ ME FIRST

	// THIS SHOULD BE DONE WITH SOME STATE MACHINE, BUT AINT NOBODY GOT TIME FOR THAT
	/*
			............--:------....................................```````````.../ddhddsyyyssoysssoo+ooo+++hd/
			```````````````...........-........-.....-````````````                   -osooo+/+/------..:-...--yd
			.```````````````````````````````````````````..`````.`.   `..``.`     `.    `/:..```````````````...:o
			.`.`````````..`````````````````````````````....`..``` `-/+++++++ohhyo+//-.` `.-:/:.````````````...-:
			......``.`.`....``..............................--../yhdddddmmmmdhdNNmhyo:``.//so+.````..`.....----:
			-.--..``.`........----................-......-..--ommyyhdmddhdNNNmdhdhhyyhy+:+hdh+.``.````.....-:+`.
			---:--.-----------------------------------------:dmmdysdmmNmhydNNNmmmddhyshso+yh:-..`.`.``.----:+o..
			-----:------------------------------------------hmmNhyysydmmdooymNNMNNNmdhydo+o/:-------------:/+shh
			::::::-:::::::::::::::::::::::::::::::::::::::::mNNhsyhhhyyyyyyyyssydNMNNNmdmy+o//::/:/:::::::///+oo
			::/::::::::::/::/://::/:::://::/::::/:::///::///mmdsydmNNmdhmNNNdyo+//ohMMMNmd/y++//////////:://////
			////:///////:::::::::////::/::/:////////////////ydhmmNNNmms/odNNNmdyys/:omMMNNmmhysyssssooo+++++++/+
			++++/+/++/////////////////////////++++/+++++++oooyydmdhhhs:-/+syhhyo/----/dmmNNmmmddddddhhhyyysssoos
			sosoossossooooooooo++++++++oo++++oo+yhhhhdssssyyyo/++osso:-.::+o+/:-....::/+hddNdmNmNNmmmmmmdddhhhyy
			hyyhyyyhyyysssssssssosssssoosssossoosssssyyyyyhhhsosydhyyyyssyyymdhyo+////:-/ooNNNNNNNmmmmmmmmddddhh
			hhhhhhhhhyyyyyyyyyyyyyyyyyssssssssssssssssyyhhhddhddmNNNNNNmdyssymNNmhyo+///:+oNNNNNmmNNmmmmmmmddddd
			hhhhhdhhhhhhysyhhhhyhhyyhyyhyhhhyhhyhyyyyyyhhhhhhhmNNNdhyyyso+//+shNNmdsoo+ooddNmNmmNNNmmmmddddhdddd
			yhhhhhddddmmdhhhhhhhyhhyyyhhhhyyyysss+yssyyyyhhhhydNNmmmdyss/ooymNddmmhyssosshydmmmmmmmmNmdhhhhhhhhh
			mmmddmNNNNNNNNNNNNNNNNNNNNNNNNNNmmmmmmmmmmmmmmmmmmddmmhmmsso+:-:/+oydhhyysssdNNNmmmmmmmmmmmmmmmmmmmm
			mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmddddddhyssoosoooossyhhhhhysymmmmmmmmmmmddmddmmmmmmmd
			dddddddddddddddddddmmmmdmmmmmmmmmmmmmddddddddddhdhhhhhddmmNmmmdhyyyhhdddhyoshdddddddddhhhhhhhhhhhhhh
			yyyyyyyhhhhhhhhhhhhhhhdddddddmmmmdddddddddddddddddddddddddmmddhhhdmmmmddhhosyyyyyyyyyyyysssssssoooos
			ooooooosssssyyyyssooossssyyyyyyyyyyyyyyyysssssssssyyo/odmmNNNNNNNNNNmmddhyo--:/++//////++////////+oo
			//://++//://///////::://///:://///://:::::::::---.../+odmmNNMMMMNNNNmdhysss++`+/os.-://///:::::::::/
			::///+++++++++++ooooosssssssssssssoooo+:..``.....--/s+ydmmmNNNNNNNmdhhhyyyyyo+/:/:-```.-/+oooosooooo
			ssssssssssssssssyyyyyyyyyyysooooooooo/....--:::::..oooyhdmmNNNNNmmmdddhhsosyyys-/+-`````````.-//+ooo
			sooooooo++++++ooo++++oooo++++///++++/:::::/++++/.`.yosyhhdmmNNmmmmmddhhsooooo+:-/:/-`....-..```` `.:
			::::::::::-::::::::/:::::::::::::::/+++++++oo++:`..sosyyhhddmmmddddhhhyso+++//:-+-:/...-::::-.```` .
			----------.----.---.--------------:+ooooooooo+/...-ysosyyhhddmmdddhhyyysso+///:::::/:..--///:-..```-

		 * */

	#endregion

	#region Fields/Properties

	protected KeyCode LeftKey { get { return KeyCode.A; } }
	protected KeyCode RightKey { get { return KeyCode.D; } }
	protected KeyCode JumpKey { get { return KeyCode.W; } }
	protected KeyCode KickKey { get { return KeyCode.Space; } }
	protected KeyCode DownKey { get { return KeyCode.S; } }

	public float speed = 50f;
	public float jumpPower = 150f;
	public float maxSpeed = 100f;
	public float speedWhileCarry = 50f;

	public float kickPower = 50f;
	public float kickUp = 10f;

	public float hitPower = 50f;
	public float hitUp = 10f;
	public float hitDuration = 0.4f;

	public float ladderSpeed = 10f;
	public float maxClimbVelocity = 10f;

	public Collider2D kickCollider;

	private Animator animator;
	private Rigidbody2D rb2D;

	public bool isJumping;
	public bool isKicking;
	public bool goingRight;
	public bool wentRightInLastFrame;

	public Ladder touchingLadders;
	public bool onLadders;

	public GameObject canCarryObject;
	public GameObject carriedObject;
	public bool isCarrying;
	public bool isThrowing;

	#endregion Fields/Properties

	#region Unity methods

	void Awake()
	{
		rb2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		isJumping = isKicking = false;
		kickCollider.enabled = false;
	}

	void Update()
	{
		if (!GameManager.Instance.IsPlaying)
			return;

		// control the rotation
		goingRight = !Input.GetKey(LeftKey);

		var moving = Input.GetKey(LeftKey) || Input.GetKey(RightKey);

		if (moving && onLadders)
		{
			EndLadderClimb();
		}

		// just animation
		if (!isJumping && !isKicking && !onLadders && Input.GetKeyDown(DownKey))
		{
			if (!isCarrying && canCarryObject != null)
			{
				isCarrying = true;
				animator.SetBool("isCarrying", true);

				carriedObject = canCarryObject;
				carriedObject.GetComponent<Crate>().CarryCrate(transform);
			}
			else if (isCarrying && carriedObject != null)
			{
				ReleaseCrate();
			}
		}

		if (onLadders)
		{
			if (Input.GetKey(JumpKey))
			{
				rb2D.AddForce(Vector2.up * ladderSpeed);
				rb2D.velocity.Set(Mathf.Min(rb2D.velocity.x, maxClimbVelocity), rb2D.velocity.y);
			} else if (Input.GetKey(DownKey))
			{
				rb2D.AddForce(Vector2.up * -ladderSpeed);
			}
			animator.speed = Mathf.Lerp(0f, 8f, Mathf.Abs(rb2D.velocity.y) / maxClimbVelocity);
		}
		else if (isCarrying && !isThrowing)
		{
			// i expected x velocity to be normalized, but it looks fine even like this
			animator.speed = Mathf.Lerp(0f, 8f, Mathf.Abs(rb2D.velocity.x));
		}
		else
		{
			animator.speed = 1f;
		}

		var scaleX = Mathf.Abs(transform.localScale.x);
		transform.localScale = new Vector2 (scaleX * (wentRightInLastFrame ? 1f : -1f), scaleX);
		animator.SetBool("isRunning", !isJumping && !isKicking && moving && !isCarrying);

		animator.SetBool("isJumping", !isKicking && Mathf.Abs(rb2D.velocity.y) > 0.05f);
		animator.SetBool("isFallingDown", !isKicking && isJumping && rb2D.velocity.y < 0.5f);

		// dont touch this, it's working
		if (touchingLadders != null && !isCarrying && (Input.GetKeyDown(JumpKey) || Input.GetKeyDown(DownKey)))
		{
			onLadders = true;
			rb2D.gravityScale = 0f;
			isJumping = isKicking = false;
			touchingLadders.SetCeilingActive(false);
		}
		else
		{
			if ((!isJumping || Mathf.Abs(rb2D.velocity.y) < 0.05f) && !isCarrying && Input.GetKeyDown(JumpKey))
			{
				isJumping = true;
				rb2D.AddForce(Vector2.up * jumpPower);
			}
		}

		if (!isKicking && Input.GetKeyDown(KickKey))
		{
			if (!isCarrying)
			{
				animator.SetTrigger("kick");
				isKicking = true;
				kickCollider.gameObject.transform.localScale = new Vector3((wentRightInLastFrame ? -1f : 1f), 1f, 1f);
				rb2D.AddForce((wentRightInLastFrame ? Vector2.right : Vector2.left) * kickPower + (Vector2.up * kickUp));
			}
			else if (carriedObject != null)
			{
				isThrowing = true;
				animator.speed = 1f;
				animator.SetTrigger("throws");
			}
		}


		animator.SetBool("onLadders", onLadders);

	}

	void FixedUpdate()
	{
		if (!GameManager.Instance.IsPlaying)
			return;
		
		bool left = Input.GetKey(LeftKey);
		bool right = Input.GetKey(RightKey);

		if (left || right)
		{
			wentRightInLastFrame = right;

			// check if we can move at all
			var currentSpeed = isCarrying ? speedWhileCarry : speed;
			rb2D.AddForce(Vector2.right * currentSpeed * (left ? -1f: 1f));
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

	#endregion Unity methods

	#region Collisions

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Ground")
		{
			GameManager.Instance.EndGame(GameManager.EndType.DeathByRailroad);
		}
	}

	void OnCollisionStay2D(Collision2D other)
	{
		if (other.gameObject.tag == "TrainFloor")
		{
			// TODO: THIS IS CALLED ALL THE TIME, SOMETHING MESSED UP WITH COLLIDERS
			// changed so jumping animation depends on vertical velocity
			isJumping = false;
		}
		else if (other.gameObject.tag == "Monster" && isKicking)
		{
			var monster = other.gameObject.GetComponent<Monster>();
			if (monster != null)
			{
				monster.Hit(wentRightInLastFrame, hitPower, hitUp);
			}
		}
		else if (other.gameObject.tag == "Crate")
		{
			canCarryObject = other.gameObject;
		}
	}

	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag == "Crate")
		{
			canCarryObject = null;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Ladder")
		{
			touchingLadders = other.gameObject.GetComponent<Ladder>();
		}
		else if (other.tag == "TrainEngine")
		{
			GameManager.Instance.EndGame(GameManager.EndType.DeathInEngine);
		}
		else if (other.tag == "Ground")
		{
			GameManager.Instance.EndGame(GameManager.EndType.DeathByRailroad);
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Ladder")
		{
			touchingLadders = other.gameObject.GetComponent<Ladder>();
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Ladder")
		{
			EndLadderClimb();
		}
	}

	#endregion Collisions

	#region Animation events

	// called from animation
	private void DoKick()
	{
		kickCollider.enabled = true;
	}

	// called from animation
	private void FinishKick()
	{
		isKicking = false;
		kickCollider.enabled = false;
	}

	// called from animation
	private void ThrowCrate()
	{
		isThrowing = false;
		ReleaseCrate(true);
	}

	#endregion Animation events

	#region Helpers

	private void EndLadderClimb()
	{
		onLadders = false;
		rb2D.gravityScale = 1.5f;
		if (touchingLadders != null)
		{
			touchingLadders.SetCeilingActive(true);
		}
		touchingLadders = null;
	}

	public void ReleaseCrate(bool kickIt = false)
	{
		isCarrying = false;
		animator.SetBool("isCarrying", false);
		carriedObject.GetComponent<Crate>().ReleaseCrate(kickIt, wentRightInLastFrame);
		carriedObject = null;
	}

	#endregion Helpers
}
