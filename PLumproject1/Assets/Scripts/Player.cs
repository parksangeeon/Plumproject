using System;
using Unity.VisualScripting;
using UnityEngine;

namespace ClearSky
{
    public class Player : MonoBehaviour
    {
        public static bool isControlBlocked = false;
        public float movePower = 10f;
        public SceneEntrance pendingEntrance = SceneEntrance.None;
        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        private int direction = 1;
        
        private bool alive = true;
        private static Player instance;
        public Vector2 spawnPosition = new Vector2(0, 0); // 원하는 좌표 입력
        public Inventory inventory;
        public GameObject Hudinventory;


        // Start is called before the first frame update
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

            }
            else
            {
                Destroy(gameObject);

            }

        }
        void Start()
        {
            if (instance != this) return; // 씬에 배치된 복제본이 이 줄까지 실행되지 않도록
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            Hudinventory.SetActive(false);

            if (pendingEntrance == SceneEntrance.None)
            {
                transform.position = spawnPosition;
                var rb2d = GetComponent<Rigidbody2D>();
                if (rb2d != null) rb2d.position = spawnPosition;
            }
        }
        

        private void Update()
        {
            if (isControlBlocked) {
                anim.SetBool("isRun",false);
                return; 
                }
            Restart();
            if (alive)
            {
                OpenInventory();
          
                Run();

                if (isControlBlocked) { return; }
            }
        }
        void OpenInventory()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Hudinventory.SetActive(!Hudinventory.activeSelf);
            }
        }



        private void OnTriggerStay2D(Collider2D other)
        {
            // LanternPickup처럼 자체적으로 줍기 로직(대화/이펙트 등)을 처리하는
            // 오브젝트는 여기서 또 AddItem을 호출하면 중복 픽업 경쟁이 생기므로 제외
            if (other.gameObject.GetComponent<LanternPickup>() != null) return;

            if (other.gameObject.CompareTag("Item") && Input.GetKeyDown(KeyCode.Z))
            {
                IInventoryItem item = other.gameObject.GetComponent<IInventoryItem>();
                if (item != null)
                {
                    inventory.AddItem(item);
                }
            }
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

                transform.localScale = new Vector3(direction*Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                anim.SetBool("isRun", true);

            }
            if (Input.GetAxisRaw("Horizontal") > 0 && !isAttacking)
            {
                direction = 1;
                moveVelocity = Vector3.right;

                transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                anim.SetBool("isRun", true);

            }
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))  // 바닥 태그 필요
            {
                
                anim.SetBool("isJump", false);
            }

            if (collision.gameObject.CompareTag("Enemy"))
            {
                Die();
                Debug.Log("죽음");
            }
            
        }
        void Die()
        {
            anim.SetTrigger("die");
            alive = false;

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

