using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public InputAction moveAction;
    public InputAction jumpAction;

    private Rigidbody rb;

    public float speed = 1f;
    public float jumpForce = 5f;
    private bool isGrounded = true;
    private bool isPowerUp = false;
    private bool capsuleSpawned = false;

    private int numCollected = 0;
    public int total = 0;

    public GameObject youWinGameObject;
    public GameObject powerUpGameObject;
    public TextMeshProUGUI counterText;
    
    private bool gameStarted = false;
    public Button startButton;
    public GameObject startPanel;
    

    public GameObject capsule;
    public GameObject BreakWall;


    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        rb = GetComponent<Rigidbody>();
        
        rb.useGravity = false;
        rb.isKinematic = true;

        youWinGameObject.SetActive(false);
        powerUpGameObject.SetActive(false);
        startPanel.SetActive(true);
        capsule.SetActive(false);

        counterText.text = numCollected + " / " + total;

        Debug.Log(counterText.text = numCollected + " / " + total);
        
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }

    private void FixedUpdate(){
        if (!gameStarted) return;
        
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);

        rb.AddForce(movement * speed);
    }
    
    private void Update(){
        if (!gameStarted) return;
        
        if (jumpAction.WasPressedThisFrame() && isGrounded){
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    
    private void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Ground")){
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("BreakWall") && isPowerUp){
            Debug.Log("Wall broken!");
            collision.gameObject.SetActive(false);
        }
    }
    
    public void StartGame()
    {
        gameStarted = true;
        rb.useGravity = true;
        rb.isKinematic = false;
        startPanel.SetActive(false);
        startButton.gameObject.SetActive(false);
        Debug.Log("Game Started!");
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Collectible")){
            other.gameObject.SetActive(false);
            Debug.Log("Collision with " + other.gameObject.name);

            numCollected++;

            counterText.text = numCollected + " / " + total;

            Debug.Log(counterText.text = numCollected + " / " + total);

            if (numCollected >= total && !capsuleSpawned){
                Debug.Log("All cubes collected, capsule spawned");
                SpawnPowerUp();
                capsuleSpawned = true;
            }
        }
        else if(other.CompareTag("Capsule")){
            Debug.Log("Capsule collected");
            ActivatePowerUp();
            other.gameObject.SetActive(false);
        }
        else if(other.CompareTag("GoalZone")){
            Debug.Log("You won!");
            youWinGameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    
    private void SpawnPowerUp()
    {
        capsule.SetActive(true);
        Debug.Log("Capsule activated");
    }
    
    private void ActivatePowerUp()
    {
        Debug.Log("Power-up activated!");
        powerUpGameObject.SetActive(true);
        

        Renderer playerRenderer = GetComponent<Renderer>();
        playerRenderer.material.color = Color.green;

        speed = speed * 2f;

        isPowerUp = true;
    }
}

