using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private Camera eyes;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float smooth;
    float _smoothValX, _smoothValZ; // for input smoothing

    [SerializeField]
    private float gravity = -9.81f;
    private Vector3 playerVelocity;
    private bool grounded;
    float mRotationY = 0f;

    [SerializeField]
    private float lookSensitivity;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        grounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Camera();
    }

    public void Movement()
    {
        // walking
        grounded = controller.isGrounded;
        Vector2 smoothedInput = SmoothedInput();
        float horizontal = smoothedInput.x;
        float vertical = smoothedInput.y;
        Vector3 move = transform.forward * vertical * speed + transform.right * horizontal * speed;

        move.y = 0f;
        controller.Move(move * Time.deltaTime);

        if (grounded)
        {
            playerVelocity.y = -1f; // keeps player tied to ground
        }
    }

    public void Camera()
    {
        float mouse_x = Input.GetAxis("Mouse X");
        float mouse_y = Input.GetAxis("Mouse Y");
        transform.Rotate(0f, mouse_x * lookSensitivity, 0f);
        mRotationY += mouse_y * lookSensitivity;
        mRotationY = Mathf.Clamp(mRotationY, -90f, 90f);
        eyes.transform.localEulerAngles = new Vector3(-mRotationY, eyes.transform.localEulerAngles.y, eyes.transform.localEulerAngles.z);
    }

    public Vector2 SmoothedInput()
    {
        float dead = 0.001f;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        float rX, ry;

        // Horizontal
        float targetx = input.x;
        _smoothValX = Mathf.MoveTowards(_smoothValX, targetx, smooth * Time.unscaledDeltaTime);
        rX = (Mathf.Abs(_smoothValX) < dead) ? 0f : _smoothValX;

        // Vertical
        float targety = input.y;
        _smoothValZ = Mathf.MoveTowards(_smoothValZ, targety, smooth * Time.unscaledDeltaTime);
        ry = (Mathf.Abs(_smoothValZ) < dead) ? 0f : _smoothValZ;

        return new Vector2(rX, ry);
    }
}
