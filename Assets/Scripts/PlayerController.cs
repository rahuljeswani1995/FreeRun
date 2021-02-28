using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed = 10;
    public float maxForwardSpeed = 20;

    private int desiredLane = 1;
    public float laneDistance = 4;

    // amount of jump on pressing up arrow
    public float jumpForce = 10;
    public float Gravity = -20;

    public Animator animator;
    private bool isSliding = false;


    static int s_DeadHash = Animator.StringToHash ("Dead");
	static int s_RunStartHash = Animator.StringToHash("runStart");
	static int s_MovingHash = Animator.StringToHash("Moving");
	static int s_JumpingHash = Animator.StringToHash("Jumping");
	static int s_JumpingSpeedHash = Animator.StringToHash("JumpSpeed");
	static int s_SlidingHash = Animator.StringToHash("Sliding");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator.SetInteger("RandomIdle", Random.Range(0, 5));

    }

    void dataCollection(string Type) {
        
        float timeChangeInMillis = Time.deltaTime * 1000;

        // Put your location here 
        // location/<<user-name>>
        // Make sure to change this
        ScreenCapture.CaptureScreenshot("C:\\Extras\\MS\\529-MLG\\FreeRun\\vai-"+timeChangeInMillis+"-"+Type+".jpg");

    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.isGameStarted)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.W)) {
            dataCollection("idle");
        } else if(Input.GetKeyDown(KeyCode.UpArrow)) {
            dataCollection("jump");
        } else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            dataCollection("duck");
        } else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            dataCollection("left");
        } else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            dataCollection("right");
        }
            

        //if (forwardSpeed < maxForwardSpeed)
        //    forwardSpeed += 0.1f * Time.deltaTime;


        animator.SetBool("isGameStarted", true);

        if (PlayerManager.isGameStarted && !animator.GetBool(s_MovingHash) && !animator.GetBool(s_SlidingHash) && !animator.GetBool(s_JumpingHash))
            StartRunning();

        //animator.SetBool("isGrounded", controller.isGrounded);

        // controller.Move(direction * Time.deltaTime);
        direction.z = forwardSpeed;

        // introduce gravity after every deltaTime interval (brings player back down on jumping)
        direction.y += Gravity * Time.deltaTime;

        // jump on pressing up key only when player is on ground (to avoid multiple jumps)
        if (controller.isGrounded)
        {
            // no gravity needed on ground
            //direction.y = -1;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Jump();
                StartCoroutine(Jump());
            }
        }
        //else
        //{

        //}

        if (Input.GetKeyDown(KeyCode.DownArrow) && !isSliding)
        {
            StartCoroutine(Slide());
        }


        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane == 3)
            {
                desiredLane = 2;
            }
            //else
            //{
            //    direction.x += laneDistance;
            //}
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane == -1)
            {
                desiredLane = 0;
            }
            //else
            //{
            //    direction.x -= laneDistance;
            //}
        }



        // Calculate next position
        //Vector3 targetPostion = transform.position.z * transform.forward + transform.position.y * transform.up;
        Vector3 targetPostion = transform.position.z * transform.forward + transform.position.y * transform.up;

        if (desiredLane == 0)
        {
            targetPostion += Vector3.left * laneDistance;
        }
        else if (desiredLane == 2)
        {
            targetPostion += Vector3.right * laneDistance;
        }

        // transform.position = Vector3.Lerp(transform.position, targetPostion, 70 * Time.deltaTime);
        // controller.center = controller.center;

        if(transform.position == targetPostion)
            return;

        Vector3 diff = targetPostion - transform.position;
        Vector3 moveDir = diff.normalized * 12 * Time.deltaTime;
        if(moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
    }

    private void FixedUpdate()
    {
        //if (!PlayerManager.isGameStarted)
        //    return;
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private IEnumerator Jump()
    {
        // animator.SetBool("Jumping", true);

        animator.SetBool(s_JumpingHash, true);
        direction.y = jumpForce;
        yield return new WaitForSeconds(1.1f);
        
        StopJump();
    }

    private void StopJump()
    {
        animator.SetBool(s_JumpingHash, false);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool(s_SlidingHash, true);
        animator.SetFloat(s_JumpingSpeedHash, 0.8f);
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;
        yield return new WaitForSeconds(1.1f);
        controller.center = new Vector3(0, 0, 0);
        controller.height = 2;
        animator.SetBool(s_SlidingHash, false);
        isSliding = false;
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(1.0f);
    }


    // ##########################################

    public void StartRunning()
    {   
        if (animator)
        {
            animator.Play(s_RunStartHash);
            animator.SetBool(s_MovingHash, true);
        }
    }

	

}