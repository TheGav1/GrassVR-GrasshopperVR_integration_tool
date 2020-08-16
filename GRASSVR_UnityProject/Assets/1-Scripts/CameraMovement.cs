using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CameraMovement : MonoBehaviour
{
    public float MouseSensitiviy = 100f;
    public float MovementSpeed = 15f;
    public Transform CameraTransform;
    private CharacterController controller;
    private Transform PlayerTransform;
    private float Xrotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        PlayerTransform = this.transform;
        //hide cursor
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
    #region Rotation
        //normal mouse rotation for camera
        float mouseX = Input.GetAxis("Mouse X")*MouseSensitiviy*Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitiviy * Time.deltaTime;
        
        Xrotation -= mouseY;//- for rotation diration (lefthanded space)
        Xrotation = Mathf.Clamp(Xrotation, -90, 90);//limit up down rotation

        CameraTransform.localRotation = Quaternion.Euler(Xrotation, 0f, 0f);
        PlayerTransform.Rotate(Vector3.up * mouseX);
        #endregion
        #region Movement
        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");

        Vector3 move = transform.right*X+transform.forward*Z;
        controller.Move(move*MovementSpeed*Time.deltaTime);
    #endregion
    }
}
