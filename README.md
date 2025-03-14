<summary><b>ScriptToInspectorExample - how scripts affect the Unity Inspector and what values you can manipulate.</b></summary>

  ```csharp

        // Serializable basic types
        public int intValue = 42;
        public float floatValue = 3.14f;
        public string stringValue = "Hello, Unity!";
        public bool boolValue = true;

        // Arrays and Lists are also serializable
        public int[] intArray = { 1, 2, 3, 4, 5 };
        public List<string> stringList = new List<string>() { "Apple", "Banana", "Cherry" };

        // Unity-specific types
        public Vector3 vectorValue = new Vector3(1.0f, 2.0f, 3.0f);
        public Color colorValue = Color.red;
        public GameObject gameObjectReference;

        // Custom Serializable Class
        [System.Serializable]
        public class CustomData
        {
            public int customInt;
            public string customString;
        }

        public CustomData customData;

        // Inspector-related expressions
        [Header("Inspector Header")]
        public float headerValue;

        [Tooltip("This is a tooltip for the inspector.")]
        public string tooltipValue = "Hover over me!";

        // Example of using space in the inspector
        [Space(20)]
        public bool separateSection;

        // Example of a read-only field in the inspector
        [Header("Read-Only Field")]
        [SerializeField]
        private int readOnlyField = 100;

        // Example of a Range slider in the inspector
        [Header("Range Slider")]
        [Range(0, 1)]
        public float rangeValue = 0.5f;

        // Example of using a LayerMask in the inspector
        [Header("Layer Mask")]
        public LayerMask layerMaskValue;

 ```

<summary><b>Interactables - implements an Interactable interface and example how to use it for changing materials, sprites, sounds</b></summary>

  ```csharp

    public interface IInteractable
    {
        public void Enter();
        public void Exit();
    }

 ```
<summary><b>InteractableChangeMaterial - change 3D mesh material when triggered.</b></summary>

  ```csharp
    public class InteractableChangeMaterial : MonoBehaviour, IInteractable
    {
        public Material OnEnterMaterial;
        public MeshRenderer renderer;

        private Material defaultMaterial;

        private void Awake()
        {
            if (renderer == null)
            {
                renderer = GetComponentInChildren<MeshRenderer>();
            }
            defaultMaterial = renderer.material;
        }

        public void Enter()
        {
            UIManager.Instance.DebugText.text = "Enter Interactable";
            renderer.material = OnEnterMaterial;
        }

        public void Exit()
        {
            UIManager.Instance.DebugText.text = "Exit Interactable";
            renderer.material = defaultMaterial;
        }
    }


 ```
<summary><b>InteractableChangeSprite - change 2D sprite when triggered.</b></summary>

  ```csharp

    public class InteractableChangeSprite : MonoBehaviour, IInteractable
    {
        public Sprite OnEnterSprite;
        public SpriteRenderer spriteRenderer;

        private Sprite defaultSprite;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            defaultSprite = spriteRenderer.sprite;
        }

        public void Enter()
        {
            UIManager.Instance.DebugText.text = "Enter Interactable";
            
            spriteRenderer.sprite = OnEnterSprite;
        }

        public void Exit()
        {
            UIManager.Instance.DebugText.text = "Exit Interactable";

            spriteRenderer.sprite = defaultSprite;
        }
    }


 ```
<summary><b>InteractablePlaySound - playing a sound source when triggered.</b></summary>

  ```csharp

    public class InteractablePlaySound : MonoBehaviour, IInteractable
    {
        public AudioSource source;

        public void Enter()
        {
            UIManager.Instance.DebugText.text = "Play Sound";
            source.Play();
        }

        public void Exit()
        {
            source.Stop();
        }
    }

 ```
<summary><b>InteractableSpawnParticles - creating new objects from script</b></summary>

  ```csharp

    public class InteractableSpawnParticles : MonoBehaviour, IInteractable
    {
        public GameObject prefab;


        public void Enter()
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }

        public void Exit()
        {
        }
    }

  ```

<summary><b>LookAtPlayerBehaviour - simple look at another object demonstration</b></summary>

  ```csharp
    public class LookAtPlayerBehaviour : MonoBehaviour
    {
        private Transform target;

        private void Awake()
        {
            target = FindObjectOfType<PlayerMoveController>().transform;
        }

        void Update()
        {
            if (transform != null)
            {
                transform.LookAt(target);
            }
        }
    }
 ```
<summary><b>PlayerCollisionController - demonstrates OnTriggerEnter and calling function on collision object.</b></summary>

  ```csharp
    public class PlayerCollisionController : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponents<IInteractable>() != null)
            {
                foreach (var interactive in other.GetComponents<IInteractable>())
                {
                    interactive.Enter();
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponents<IInteractable>() != null)
            {
                foreach (var interactive in other.GetComponents<IInteractable>())
                {
                    interactive.Exit();
                }
            }
        }
    }
 ```
<summary><b>MouseClickInteractor shows how to implement Raycasting from left mouse button. Place on Main Camera, will select any interactables in the scene.</b></summary>

  ```csharp
    public class MouseClickInteractor : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponents<IInteractable>() != null)
                    {
                        foreach (var interactive in hit.transform.GetComponents<IInteractable>())
                        {
                            interactive.Enter();
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponents<IInteractable>() != null)
                    {
                        foreach (var interactive in hit.transform.GetComponents<IInteractable>())
                        {
                            interactive.Exit();
                        }
                    }
                }
            }
        }
    }
 ```
<summary><b>PlayerMoveController. Pres WASD or arrow keys to move object. Place on main player object.</b></summary>

  ```csharp
    public class PlayerMoveController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public Camera mainCamera; // Assign your camera in the Inspector

        void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Get camera's forward and right vectors
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            // Project forward and right vectors onto the horizontal plane (remove Y-axis)
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Calculate desired movement direction
            Vector3 movementDirection = forward * verticalInput + right * horizontalInput;

            // Apply movement
            transform.Translate(movementDirection * moveSpeed * Time.deltaTime);
        }
    }

 ```
<summary><b>Camera Orbit - press right mouse button to rotate camera around target object. Place script on main camera.</b></summary>

  ```csharp

    public class CameraOrbit : MonoBehaviour
    {
        public Transform target; // The object to rotate around
        public float rotationSpeed = 10f;
        public bool invertY = false; // Option to invert vertical rotation

        private Vector3 _offset;

        void Start()
        {
            // Calculate initial offset based on camera's starting position
            _offset = transform.position - target.position;
        }

        void LateUpdate() // Use LateUpdate for smoother camera movement
        {
            if (Input.GetMouseButton(1))
            {
                float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
                float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;

                if (invertY) vertical *= -1;

                // Rotate offset around the target object
                _offset = Quaternion.AngleAxis(horizontal, Vector3.up) * _offset;
                _offset = Quaternion.AngleAxis(vertical, Vector3.right) * _offset;

                // Update the camera position with the rotated offset
                transform.position = target.position + _offset;

                // Always make the camera look at the target
                transform.LookAt(target);
            }
        }
    }


  ```
<summary><b>AutoDestroySelf</b></summary>

  ```csharp
    public class AutoDestroySelf : MonoBehaviour
    {
        public float DestroyTime;

        void Awake()
        {
            Invoke(nameof(SelfDestroy), DestroyTime);
        }

        private void SelfDestroy()
        {
            Destroy(gameObject);
        }
    }
  ```

<summary><b>Singleton Managers - simple implementation of the singleton pattern, shows basic UI interaction and accessing manager scripts from any other script. Singletons make sure the script exists only once in the scene.</b></summary>


  ```csharp
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                    if (_instance == null)
                    {
                        // Create a new GameManager GameObject
                        GameObject gameManagerObject = new GameObject("UIManager");
                        _instance = gameManagerObject.AddComponent<UIManager>();
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        private static UIManager _instance;

        [Header("UI Elements")]
        public Button button1;
        public Button button2;
        public TextMeshProUGUI DebugText;
        public TextMeshProUGUI CollisionsText;


        [Header("Tests")] 
        public string button1Text;
        public string button2Text;

        private void Awake()
        {
            button1.onClick.AddListener(delegate
            {
                DebugText.text = button1Text;
            });

            button2.onClick.AddListener(delegate
            {
                DebugText.text = button2Text;
            });
        }
    }



    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        // Create a new GameManager GameObject
                        GameObject gameManagerObject = new GameObject("GameManager");
                        _instance = gameManagerObject.AddComponent<GameManager>();
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        private static GameManager _instance;

        private int collisions;

        public void RegisterCollision()
        {
            collisions += 1;
            UIManager.Instance.CollisionsText.text = $"Collisions: {collisions}";
        }
    }
  ```
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Color blue;
    private InputActions actions;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody2D rb;
    private float movementX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        actions = new InputActions();
        actions.Basic.Enable();
        actions.Basic.Jump.performed += Jump;
        actions.Basic.MoveX.performed += MoveXOnperformed;
        actions.Basic.MoveX.canceled += MoveXOnperformed;
    }

    private void MoveXOnperformed(InputAction.CallbackContext context)
    {
        movementX = context.ReadValue<float>();
        Debug.Log($"{movementX}");
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log($"Jump");
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movementX * moveSpeed, rb.linearVelocity.y);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with Enemy!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Health"))
        {
            Destroy(other.gameObject); // Collect the pickup
            Debug.Log("Health Collected!");
        }

        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject); // Collect the pickup
            Debug.Log("Coin Collected!");
        }
    }
}
```
