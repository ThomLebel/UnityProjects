using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public Transform groundCheck;

	public float originalSpeed = 150f;
	public float jumpTakeOff = 40f;
	public float speed;
	public Vector3 lastDir;
	public LayerMask groundLayerMask;
	public LayerMask jumpThroughLayerMask;

	public bool canMove = true;
	public bool canJump = true;

	protected float horizontal;
	protected float vertical;
	protected float safeSpot = 0.2f;

	[SerializeField]
	protected bool grounded = false;
	[SerializeField]
	protected bool isJumping = false;
	[SerializeField]
	protected bool hasJump = false;
	[SerializeField]
	protected bool isFalling = false;

	protected Collider2D platformCollider;
	protected CapsuleCollider2D playerCollider;
	protected Animator animator;
	protected PlayerInfo playerInfo;

	private void Awake()
	{
		playerCollider = gameObject.GetComponent<CapsuleCollider2D>();
		animator = gameObject.GetComponentInChildren<Animator>();
	}

	// Start is called before the first frame update
	public virtual void Start()
    {
		playerInfo = gameObject.GetComponent<PlayerInfo>();
		lastDir = new Vector3(1, 0, 0);
		speed = originalSpeed;
		jumpThroughLayerMask = LayerMask.GetMask("jumpThrough");
	}

    // Update is called once per frame
    public virtual void Update()
    {
		if (canMove)
			Move();

		if(canJump)
			Jump();

		Fall();
    }

	public virtual void Move()
	{
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, groundLayerMask);

		animator.SetBool("playerMove", false);

		horizontal = Input.GetAxisRaw("Horizontal_P" + playerInfo.playerController);

		if (Math.Abs(horizontal) > safeSpot)
		{
			if (grounded && !isFalling)
				animator.SetBool("playerMove", true);

			if (horizontal > safeSpot)
			{
				lastDir = new Vector3(1, 0, 0);
			}
			else if (horizontal < safeSpot * -1)
			{
				lastDir = new Vector3(-1, 0, 0);
			}
		}
	}

	public virtual void Jump()
	{
		vertical = Input.GetAxisRaw("Vertical_P" + playerInfo.playerController);

		if (playerInfo.rb2d.velocity.y > 0)
		{
			animator.SetBool("playerJump", true);
		}
		else if (playerInfo.rb2d.velocity.y < 0)
		{
			animator.SetBool("playerJump", false);
			animator.SetBool("playerFall", true);
		}
		else
		{
			animator.SetBool("playerJump", false);
			animator.SetBool("playerFall", false);
		}

		if (vertical > safeSpot && !isJumping && !hasJump)
		{
			hasJump = true;
			isJumping = true;
			//canJump = false;
			playerInfo.rb2d.AddForce(new Vector2(0f, jumpTakeOff));
		}
		if (vertical > safeSpot && isJumping && hasJump && grounded)
		{
			isJumping = false;
		}
		if (vertical < safeSpot)
		{
			if (grounded)
			{
				isJumping = false;
				hasJump = false;
			}
			if (isJumping && playerInfo.rb2d.velocity.y > 0)
			{
				playerInfo.rb2d.velocity = new Vector2(playerInfo.rb2d.velocity.x, playerInfo.rb2d.velocity.y * 0.5f);
			}
			if (vertical < safeSpot * -1 && grounded)
			{
				RaycastHit2D ray = Physics2D.Linecast(transform.position, groundCheck.position, jumpThroughLayerMask);

				if (ray.collider != null)
				{
					platformCollider = ray.collider;
					playerCollider.isTrigger = true;
					isFalling = true;
					canJump = false;
					animator.SetBool("playerFall", true);
				}
			}
		}
	}

	private void Fall()
	{
		if (isFalling)
		{
			if (!playerCollider.IsTouching(platformCollider))
			{
				canJump = true;
				isFalling = false;
				platformCollider = null;
				playerCollider.isTrigger = false;
				animator.SetBool("playerFall", false);
			}
			//Detect if we are touching a wall
		}
	}

	void FixedUpdate()
	{
		//Move character
		if (canMove || canJump)
			playerInfo.rb2d.velocity = new Vector2(horizontal * speed * Time.fixedDeltaTime, playerInfo.rb2d.velocity.y);

		//Change character orientation
		if (lastDir.x != 0)
			playerInfo.playerBody.transform.localScale = new Vector3(playerInfo.originalScale * lastDir.x, transform.localScale.y, transform.localScale.z);
	}
}
