using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Vector3 movement;
    private bool isMoving = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            isMoving = true;
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            isMoving = false;
        }
        Vector2 inputVector = context.ReadValue<Vector2>();
        movement = new Vector3(inputVector.x, inputVector.y, 0).normalized;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime);
        }
    }
}
