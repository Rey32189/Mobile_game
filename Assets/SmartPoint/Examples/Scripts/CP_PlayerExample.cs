using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SmartPointExample
{
    public class CP_PlayerExample : MonoBehaviour
    {
        public enum PlayerMode
        {
            Standard,
            IdleShooter
        }

        private GameObject bulletPrefab;
        #if ENABLE_LEGACY_INPUT_MANAGER
        private float speed = 5f;
        #endif
        private CharacterController controller;
        [SerializeField]
        private PlayerMode pMode = PlayerMode.Standard;

        void Start()
        {
            if (pMode == PlayerMode.Standard)
            {
                controller = GetComponent<CharacterController>();
            }
            else
            {
                bulletPrefab = Resources.Load("CP_Bullet") as GameObject;
            }
        }

        // Update is called once per frame
        void Update()
        {
        #if ENABLE_LEGACY_INPUT_MANAGER
            switch (pMode)
            {
                //basic low-effort movement using legacy input system
                case PlayerMode.Standard:
                    float yVel = 0f;
                    //Gravity
                    if (!controller.isGrounded)
                    {
                        yVel = -2f;
                    }

                    // Old input backends are enabled.
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    Vector3 moveVector = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).normalized;
                    moveVector += new Vector3(0f, yVel, 0f);

                    controller.Move(moveVector * speed * Time.deltaTime);
                    break;

                //For multicollision example. No movement other than rotation
                case PlayerMode.IdleShooter:
                    Vector3 mousePos = Input.mousePosition;
                    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.y - 1));
                    Vector3 relativePos = targetPosition - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    transform.rotation = rotation;
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject go = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                        go.GetComponent<CP_BulletExample>().PassDirection(transform.eulerAngles);
                    }
                    break;
            }
        #endif
        }

    //Maybe in a later update
    #if ENABLE_INPUT_SYSTEM
    // New input system backends are enabled.
    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
    }
    #endif
    }
}