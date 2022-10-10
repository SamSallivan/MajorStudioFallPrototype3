using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Input/Output.
// Makes no decisions: always asks the controller.
public class View : MonoBehaviour
{
    // Unity components
    CharacterController characterController;
    public Transform myCamera;
    public GameObject myFish;
    public GameObject bullet;

    // MVC
    public Model model;
    private Controller controller;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        characterController = GetComponent<CharacterController>();

        controller = new Controller(model);
    }


    // Game controller changes should always be applied in FixedUpdate because they are physics operations.
    void FixedUpdate()
    {
        //Allows move and jump if on ground/
        if (controller.IsGrounded(characterController.isGrounded))
        {
            controller.CalcMoveDirection();

            controller.Jump();
        }

        controller.ApplyGravity(Time.deltaTime);

        // Output
        characterController.Move(model.MoveDirection * Time.deltaTime);

        controller.CalcRotation();
        transform.localRotation = Quaternion.Euler(0, model.horizontalRotation, 0);
        myCamera.localRotation = Quaternion.Euler(model.verticalRotation, 0, 0);
    }

    // Control input should always be read in Update because it is frame-dependent.
    void Update()
    {
        // Input
        model.XMove = Input.GetAxis("Horizontal") * transform.right;
        model.YMove = Input.GetAxis("Vertical") * transform.forward;
        model.Jump = Input.GetButton("Jump");

        model.MouseX = Input.GetAxis("Mouse X") * Time.deltaTime;
        model.MouseY = Input.GetAxis("Mouse Y") * Time.deltaTime;
        model.Shift = Input.GetKey(KeyCode.LeftShift);

    }
}
