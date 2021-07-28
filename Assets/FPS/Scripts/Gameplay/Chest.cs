using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Gameplay
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private Animator chestAnimator;

        [Tooltip("The object this chest drops")]
        public GameObject LootPrefab;

        [Tooltip("The point at which the loot spawns")]
        public Transform LootSpawnPoint;

        PlayerInputHandler m_InputHandler;
        bool HasSpawnedLoot;
        bool ChestWithinRange;
        bool ChestIsClosed;
        Material material;
        public GameObject sparkles;

        [Header("SFX")]
        public AudioSource AudioSource;

        [Tooltip("Sound chest makes when opening")]
        public AudioClip ChestOpenSFX;

        [Tooltip("Sound chest makes when closing")]
        public AudioClip ChestCloseSFX;

        // Start is called before the first frame update
        void Start()
        {
            chestAnimator = gameObject.GetComponent<Animator>();
            HasSpawnedLoot = false;
            ChestWithinRange = false;
            material = GetComponent<Renderer>().material;
            sparkles = this.transform.Find("VFX_Sparkles").gameObject;
            LootPrefab.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (ChestWithinRange && m_InputHandler.GetInteractButtonDown() && !HasSpawnedLoot)
            {
                chestAnimator.SetBool("playOpen", true);
                AudioSource.PlayOneShot(ChestOpenSFX, 1);
                Invoke("SpawnLoot", 2);
            }    
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_InputHandler = other.GetComponent<PlayerInputHandler>();
                ChestWithinRange = true;
                chestAnimator.SetBool("playShake", true);
                sparkles.SetActive(false);
            }
        }

        private void SpawnLoot()
        {
            LootPrefab.SetActive(true);
            HasSpawnedLoot = true;
            //Instantiate(LootPrefab, LootSpawnPoint.position, Quaternion.identity);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ChestWithinRange = false;
                chestAnimator.SetBool("playShake", false);
                Invoke("CloseChest", 0);
            }
        }

        private void CloseChest()
        {
            
            if (HasSpawnedLoot && !ChestIsClosed)
            {
                material.DisableKeyword("_EMISSION");
                chestAnimator.SetBool("playClose", true);
                for (int i = 0; i < transform.childCount; i++)
                {
                    material = transform.GetChild(i).gameObject.GetComponent<Renderer>().material;
                    material.DisableKeyword("_EMISSION");
                }
                AudioSource.PlayOneShot(ChestCloseSFX, 1);
                ChestIsClosed = true;
            }
            
            
        }
    }
}