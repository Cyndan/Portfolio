using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Standard stuff.
    private bool grounded;
    private float lastVelX, lastVelY;
    private Rigidbody2D rb;

    // As it's a metroidvania, we can only do certain things if we possess the abilities. These are denoted here.
    // We default to 0 jumps, and unlock up to 2.
    public bool testing;
    private bool abilDash, abilPuddle;
    private int jumpCountMax;

    // Used for editing the sprite.
    private SpriteRenderer sprite;
    public Sprite idleSprite, runSprite;

    // Used for Coyote Time.
    //private bool grndBack, grndFor, grndCen;
    public Transform castBack, castFor, castCen;
    public float coyTimeMax;
    private float coyTimeLeft;

    // Used for Dashing.
    public float dashTimeMax, dashSpeed, dashResetTime;
    [SerializeField] private float dashTimeLeft, dashResetLeft;
    private bool dashReleased;
    [HideInInspector] public bool canDash;
    [HideInInspector] public bool dashing;
    // the becameGrounded var may look unnecessary, but this allows us to recharge our dash by just tapping the ground while it's cooling down rather than being grounded AS it becomes available.
    private bool becameGrounded;

    // Used for Jumping.
    public float jumpForceGrnd, jumpForceAir, jumpTimeMax, gravBase, gravMult;
    private bool canJump, jumpReleased, fallen, jumping;
    private float jumpTimeLeft;
    private int jumpCountLeft;


    // Raycast shenanigans for being grounded.
    private RaycastHit2D hitCen, hitRear, hitFor;
    private float rayDist = 1.0f;
    private Vector2 rayDir = new Vector2(0, -1);

    // Nyoom.
    public float runSpeed, floatSpeed, floatCap;
    private int facing;

    // Puddle of Grue
    private CapsuleCollider2D baseColl;
    private CapsuleCollider2D puddleColl;
    [HideInInspector] public bool puddle;
    public PhysicsMaterial2D puddleColliderMaterial;
    public float puddleSpeedMult;

    // Gotta have those sweet, sweet animations, bro
    private Animator anim;

    void Start ()
    {
        // By ticking off Testing in the editor, we will start with all abilities unlocked.
        if (testing)
        {
            abilDash = true;
            abilPuddle = true;
            jumpCountMax = 2;
        }

        // Let's initialize like, a shit-ton of values, here.
        canJump = true;
        canDash = true;

        jumpReleased = true;
        dashReleased = true;

        // This is just setting up the puddle collider.
        baseColl = GetComponent<CapsuleCollider2D>();
        puddleColl = gameObject.AddComponent<CapsuleCollider2D>();
        puddleColl.direction = CapsuleDirection2D.Horizontal;
        puddleColl.size = new Vector2(1.47f, 0.26f);
        puddleColl.offset = new Vector2(0, -1.12f);
        puddleColl.sharedMaterial = puddleColliderMaterial;
        

        sprite = gameObject.GetComponent<SpriteRenderer>();
        facing = -1;

        jumpTimeLeft = 0.0f;
        coyTimeLeft = 0.0f;
        dashResetLeft = 1;

        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

            
	}


    void Update()
    {
        // Before moving, jumping, etc, we initiate the dash, as this will interrupt pretty much any action.
        // We'll follow a similar process to jumping and take care of inputs first.
        if (Input.GetButtonDown("Dash"))
            dashReleased = false;
        if (Input.GetButtonUp("Dash"))
            dashReleased = true;

        // Are we able to initiate a dash at this point? It only resets once we touch the ground and our recharge timer has run out.
        if (becameGrounded && dashResetLeft >= 1 && dashReleased)
        {
            canDash = true;
            dashResetLeft = 1;
            becameGrounded = false;
        }

        // Timer time, and our dash function.
        if (dashing)
        {
            rb.gravityScale = 0;
        }

            if (dashReleased == false && canDash && abilDash)
            {
                Dash();
                dashTimeLeft = dashTimeMax;
                dashResetLeft = 0;
                StartCoroutine(DashTimer());
                StartCoroutine(DashReset());
            }


        if (dashing && dashTimeLeft <= 0)
        {
            rb.gravityScale = gravBase;
            dashTimeLeft = dashTimeMax;
            dashing = false;
            if (!puddle)
                rb.velocity = Vector2.Lerp(new Vector2(lastVelX, 0), rb.velocity, 0.1f);
            else
            {
                rb.velocity = Vector2.Lerp(new Vector2(lastVelX, 0), rb.velocity, 0.5f);
            }

        }


        // This is used to make checking for jumping input simpler.
        if (Input.GetButtonDown("Jump"))
            jumpReleased = false;
        if (Input.GetButtonUp("Jump"))
            jumpReleased = true;


        // This variable is used to see if we can still initiate a jump.
        if (jumpReleased && jumpCountLeft > 0)
            canJump = true;

        // Next we fiddle with timers for jumping, allowing us to hold the jump button longer to jump higher.
        if (jumpReleased)
            jumpTimeLeft = jumpTimeMax;
        else if (!jumpReleased && jumpTimeLeft >= 0)
            jumpTimeLeft -= 0.1f;



        // Use a raycast to check if the player is grounded. This commented out block is a basic version for a single raycast. The following after is accounting for coyote time.
        hitCen = Physics2D.Raycast(castCen.position, rayDir, rayDist);
        if (hitCen.distance <= 0.3f && hitCen.distance > 0)
            grounded = true;
        else if (hitCen.distance > 0.3f || hitCen.collider == null)
            grounded = false;

        hitRear = Physics2D.Raycast(castBack.position, rayDir, rayDist);
        hitCen = Physics2D.Raycast(castCen.position, rayDir, rayDist);
        hitFor = Physics2D.Raycast(castFor.position, rayDir, rayDist);
        
        // If we perform the grounded check, it resets our gravity even if we're dashing, soooo that's why we have the if check!
        if (!dashing)
            grounded = GroundCheck(hitRear, hitCen, hitFor);



        // Before we puddle, we need to make a raycast from the bottom of the player up, to make sure that the player can actually un-puddle!
        var puddleHit = PuddleRaycast();


        // A quick thing for animations, just to make sure we stay in puddle form (visually!) if there's something above us.
        if (puddleHit.collider != null)
            anim.SetBool("AnimPuddleRayHit", true);
        else
            anim.SetBool("AnimPuddleRayHit", false);

        // We need to be grounded to puddle, so we'll check that here, before we move.
        if (Input.GetAxis("Vertical") < 0 && grounded)
        {
            puddle = true;
            anim.SetBool("AnimPuddle", true);
            baseColl.enabled = false;
            puddleColl.enabled = true;
        }
        else if ((Input.GetAxis("Vertical") == 0 || !grounded) && puddleHit.collider == null)
        {
            puddle = false;
            anim.SetBool("AnimPuddle", false);
            baseColl.enabled = true;
            puddleColl.enabled = false;
        }



        // With being grounded determined, now we can replace our current jump count if we're grounded and not holding the jump button. We should also make it so that we are not considered fallen.
        if (grounded && jumpReleased)
        {
            jumpCountLeft = jumpCountMax;
            fallen = false;
        }

        // Check if we have walked off a ledge or otherwise fallen, so that we lose a single jump.
        if (!grounded && !fallen && jumpCountLeft == jumpCountMax)
        {
            fallen = true;
            jumpCountLeft -= 1;
        }

        // Now we actually try to jump, so long as we're holding down the jump button and not currently dashing.
        if (!jumpReleased && !dashing)
            Jump();

        // If we stop jumping, our current jump dies off.
        if (jumpTimeLeft <= 0 || jumpReleased)
            jumping = false;


        // Oh boy, we're a moving lad.
        if (Input.GetAxis("Horizontal") != 0 && !dashing)
        {
            Walking();
            anim.SetFloat("AnimRunSpeed", Input.GetAxis("Horizontal"));
        }
        else
        {
            anim.SetFloat("AnimRunSpeed", Input.GetAxis("Horizontal"));
        }



        // Set the direction we're facing based on our horizontal movement.
        if (rb.velocity.x > 0.1f)
        {
            sprite.flipX = true;
            facing = 1;
        }
        else if (rb.velocity.x < -0.1f)
        {
            sprite.flipX = false;
            facing = -1;
        }



        if (!grounded && !dashing)
        {

            // Cap our speed if we try to apply too much force while in the air, yet preserve previous speed.

            if (rb.velocity.x > lastVelX && rb.velocity.x > floatCap)
                rb.velocity = new Vector2(lastVelX, rb.velocity.y);
            else if (rb.velocity.x < lastVelX && rb.velocity.x < -floatCap)
                rb.velocity = new Vector2(lastVelX, rb.velocity.y);

            // While we're doing stuff in the air...let's make our downward air velocity greater than our upward.
            if (rb.velocity.y < 0)
                rb.gravityScale = gravBase * gravMult;
        }




        // Set our lastVelX and lastVelY at the very end of Update().
        if (!dashing)
        {
            lastVelX = rb.velocity.x;
            lastVelY = rb.velocity.y;
        }
        
    }


    private void Jump()
    {
        if (canJump)
        {
            jumpCountLeft--;
            canJump = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            jumping = true;
        }

        if (jumpTimeLeft > 0 && jumping && jumpCountLeft >= 0)
        {
            
            if (grounded)
                rb.AddForce(new Vector2(0, 1) * jumpForceGrnd, ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(0, 1) * jumpForceAir, ForceMode2D.Impulse);
        }


    }

    private void Walking()
    {
        // If we're grounded, we have a fairly set momentum depending on if we're a puddle or not.
        // If we're in the air, we instead apply a force that can't add any power beyond a cap; this allows us to preserve momentum.
        if (grounded)
        {
            var localPuddMult = 1.0f;
            if (puddle)
                localPuddMult = puddleSpeedMult;
            else
                localPuddMult = 1.0f;
            var move = new Vector2(Input.GetAxis("Horizontal") * runSpeed * localPuddMult, rb.velocity.y);
            rb.velocity = move;
        }
        else
        {
            rb.AddForce(new Vector2(Input.GetAxis("Horizontal") * floatSpeed, 0));
        }
    }

    private bool GroundCheck(RaycastHit2D rear, RaycastHit2D cen, RaycastHit2D forw)
    {
        // This is all actually simpler than it looks! It's just kinda convoluted to work around programming logic. But basically, for every raycast that returns as grounded, our ground count goes up by 1.
        // If even one returns true, we are considered grounded! If not, we'll initiate coyote time, and once that runs out or we jump, we'll be considered NOT grounded.
        // For the record, coyote time is when you walk off a ledge and can still jump for a brief period of time.
        int groundCount;
        int rearCheck = 1, cenCheck = 1, forwCheck = 1;

        if (rear.distance <= 0.3f && rear.distance > 0)
            rearCheck = 1;
        else if (rear.distance > 0.3f || rear.collider == null)
            rearCheck = 0;

        if (cen.distance <= 0.3f && cen.distance > 0)
            cenCheck = 1;
        else if (cen.distance > 0.3f || cen.collider == null)
            cenCheck = 0;

        if (cen.distance <= 0.3f && cen.distance > 0)
            forwCheck = 1;
        else if (cen.distance > 0.3f || cen.collider == null)
            forwCheck = 0;

        groundCount = rearCheck + cenCheck + forwCheck;

        if (!becameGrounded && groundCount >= 1)
        {
            becameGrounded = true;
        }

        if (groundCount >= 1)
        {
            coyTimeLeft = coyTimeMax;
            rb.gravityScale = gravBase;
            anim.SetBool("AnimGrounded", true);
            return true;
        }
        else if (coyTimeLeft < 0 || jumpCountLeft < jumpCountMax)
        {
            coyTimeLeft = 0;
            rb.gravityScale = gravBase;
            anim.SetBool("AnimGrounded", false);
            return false;
        }
        else if (coyTimeLeft >= 0)
        {
            coyTimeLeft -= 0.1f;
            rb.gravityScale = 0;
            anim.SetBool("AnimGrounded", true);
            return true;
        }

        anim.SetBool("AnimGrounded", false);
        return false;
    }

    private RaycastHit2D PuddleRaycast()
    {
        return Physics2D.Raycast(castCen.position, new Vector2(0, 1), 2.0f);
    }

    private IEnumerator DashTimer()
    {
        while (dashTimeLeft >= 0)
        {
            dashTimeLeft -= Time.deltaTime / dashTimeMax;
            yield return null;
        }

    }

    private IEnumerator DashReset()
    {
        while (dashResetLeft <= 1)
        {
            dashResetLeft += Time.deltaTime / (dashTimeMax + dashResetTime);
            if (dashResetLeft > 0.4f)
            {
                float r, g, b;
                r = sprite.color.r;
                g = sprite.color.g;
                b = sprite.color.b;
                sprite.color = new Color(r, r, b, dashResetLeft);
            }
            yield return null;
        }
    }

    private void Dash()
    {
        // For the dash itself, we want to kill momentum and gravity, make sure we're invulnerable (determined by another script), make ourselves a bit translucent, start our timers, and then dash.

        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0;
        dashing = true;

        float r, g, b;
        r = sprite.color.r;
        g = sprite.color.g;
        b = sprite.color.b;


        sprite.color = new Color(r, g, b, 0.4f);

        // Quickly grab the direction we're inputting, but only on a scale of 1 OR -1.
        float input = -1;
        input = Input.GetAxisRaw("Horizontal");
        if (input == 0)
            input = facing;

        // Now, fetch the direction that the mouse is from Grue. We need to convert the mouse position from it's place in the world to where it is in screen relative to the world as it's part of the UI...
        // ...and then normalize it, otherwise dashSpeed will be exponentially multiplied by the distance.
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position;
        direction = direction / direction.magnitude;

        // Dash behavious differently in the air than on the ground.
        if (grounded)
        {
            rb.AddForce(new Vector2(1 * input, 0) * dashSpeed, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(direction * dashSpeed, ForceMode2D.Impulse);
        }
        canDash = false;
    }
}
