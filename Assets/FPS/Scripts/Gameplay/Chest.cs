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
        Material material;

        // Start is called before the first frame update
        void Start()
        {
            chestAnimator = gameObject.GetComponent<Animator>();
            HasSpawnedLoot = false;
            ChestWithinRange = false;
            material = GetComponent<Renderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            if (ChestWithinRange && m_InputHandler.GetInteractButtonDown() && !HasSpawnedLoot)
            {
                chestAnimator.SetBool("playOpen", true);
                Invoke("SpawnLoot", 1.5f);
                HasSpawnedLoot = true;
            }    
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_InputHandler = other.GetComponent<PlayerInputHandler>();
                ChestWithinRange = true;
                chestAnimator.SetBool("playShake", true);
            }
        }

        private void SpawnLoot()
        {
            Instantiate(LootPrefab, LootSpawnPoint.position, Quaternion.identity);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ChestWithinRange = false;
                chestAnimator.SetBool("playShake", false);
                if (HasSpawnedLoot)
                {
                    chestAnimator.SetBool("playClose", true);
                    Invoke("DisableGlow", 1);
                }
            }
        }

        private void DisableGlow()
        {
            material.DisableKeyword("_EMISSION");
            for (int i = 0; i < transform.childCount; i++)
            {
                material = transform.GetChild(i).gameObject.GetComponent<Renderer>().material;
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}