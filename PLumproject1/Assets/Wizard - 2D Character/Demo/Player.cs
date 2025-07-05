using UnityEngine;

namespace ClearSky
{
    public class Player : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 15f; //Set Gravity Scale in Rigidbody2D Component to 5
        public string currentMapName;
        public string transferMapName;
        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        private int direction = 1;
        bool isJumping = false;
        private bool alive = true;
        private static Player instance;


        // Start is called before the first frame update
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("DOntDestroy succesece");
            }
            else
            {
                Destroy(gameObject);
                Debug.Log("Destroy succesece");
            }
            
        }
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            Restart();
            if (alive)
            {
                Hurt();
                Die();
                Attack();
                Jump();
                Run();

            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            anim.SetBool("isJump", false);
        }


        void Run()
        {

            Vector3 moveVelocity = Vector3.zero;
            anim.SetBool("isRun", false);
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            bool isAttacking = currentState.IsName("Attack");

            if (Input.GetAxisRaw("Horizontal") < 0 && !isAttacking)
            {
                direction = -1;
                moveVelocity = Vector3.left;

                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);

            }
            if (Input.GetAxisRaw("Horizontal") > 0 && !isAttacking)
            {
                direction = 1;
                moveVelocity = Vector3.right;

                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);

            }
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }
        void Jump()
        {
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            bool isAttacking = currentState.IsName("Attack");

            // 점프 입력 감지
            if (!isAttacking && (Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0))
            {
                if (!isJumping)
                {
                    // 점프 실행
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // 수직 속도만 리셋
                    rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);

                    isJumping = true;
                    anim.SetBool("isJump", true);
                }
            }
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))  // 바닥 태그 필요
            {
                isJumping = false;
                anim.SetBool("isJump", false);
            }
        }


        void Attack()
            {
                bool isJumping = anim.GetBool("isJump");
                if (Input.GetKeyDown(KeyCode.Alpha1) && !isJumping)
                {
                    anim.SetTrigger("attack");


                }
            }
         void Hurt()
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    anim.SetTrigger("hurt");
                    if (direction == 1)
                        rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
                    else
                        rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
                }
            }
         void Die()
            {
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    anim.SetTrigger("die");
                    alive = false;
                }
            }
         void Restart()
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    anim.SetTrigger("idle");
                    alive = true;
                }
            }
        }
    }

