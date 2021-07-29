using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

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
                StartCoroutine(PlayOpenChestSFX());
                HasSpawnedLoot = true;
                chestAnimator.SetBool("playOpen", true);
                Invoke("SpawnLoot", 2);
                Invoke("CloseChest", 4);
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
            //Instantiate(LootPrefab, LootSpawnPoint.position, Quaternion.identity);
        }

        private void CloseChest()
        {

            if (HasSpawnedLoot && !ChestIsClosed)
            {
                StartCoroutine(PlayCloseChestSFX());
                chestAnimator.SetBool("playClose", true);
                ChestIsClosed = true;
                StartCoroutine(DarkenChest());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ChestWithinRange = false;
                chestAnimator.SetBool("playShake", false);
            }
        }

        IEnumerator PlayOpenChestSFX()
        {
            yield return new WaitForSeconds(0f);
            AudioSource.PlayOneShot(ChestOpenSFX, 1);
        }

        IEnumerator PlayCloseChestSFX()
        {
            yield return new WaitForSeconds(0.2f);
            AudioSource.PlayOneShot(ChestCloseSFX, 1);
        }

        IEnumerator DarkenChest()
        {
            yield return new WaitForSeconds(1.1f);
            material.DisableKeyword("_EMISSION");
            for (int i = 0; i < transform.childCount; i++)
            {
                material = transform.GetChild(i).gameObject.GetComponent<Renderer>().material;
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}