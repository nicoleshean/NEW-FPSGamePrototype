using UnityEngine;

namespace Unity.FPS.Game
{
    public class Destructable : MonoBehaviour
    {
        [Header("Loot")]
        [Tooltip("The object this object can drop when being destroyed")]
        public GameObject LootPrefab;

        [Tooltip("The chance the object has to drop loot")] [Range(0, 1)]
        public float DropRate = 1f;

        [Header("VFX")]
        [Tooltip("The VFX prefab spawned when the object is destroyed")]
        public GameObject DestroyVfx;

        [Tooltip("The point at which the destroy VFX is spawned")]
        public Transform DestroyVfxSpawnPoint;

        [Header("SFX")]
        [Tooltip("Audio Source to play from")]
        public AudioSource AudioSource;

        [Tooltip("The sound played when the object is damaged")]
        public AudioClip DamageTick;

        [Tooltip("The sound played when the object is destroyed")]
        public AudioClip DestroySound;

        [Tooltip("The volume at which the sounds play")] [Range(0,1)]
        public float Volume = 1.0f;

        Health m_Health;
        bool m_WasDamagedThisFrame;

        public bool TryDropItem()
        {
            if (DropRate == 0 || LootPrefab == null)
                return false;
            else if (DropRate == 1)
                return true;
            else
                return (Random.value <= DropRate);
        }

        public void Update()
        {
            m_WasDamagedThisFrame = false;
        }

        void Start()
        {
            m_Health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, Destructable>(m_Health, this, gameObject);

            // Subscribe to damage & death actions
            m_Health.OnDie += OnDie;
            m_Health.OnDamaged += OnDamaged;
        }

        void OnDamaged(float damage, GameObject damageSource)
        {
            if (damageSource && !damageSource.GetComponent<Destructable>()) //checks if damage was from player
            {

                //play the damage tick sound
                if (DamageTick && !m_WasDamagedThisFrame)
                {
                    AudioSource.PlayOneShot(DamageTick);
                    
                }

            }
            m_WasDamagedThisFrame = true;
        }

        void OnDie()
        {

            if (TryDropItem())
            {
                Instantiate(LootPrefab, transform.position, Quaternion.identity);
            }

            var vfx = Instantiate(DestroyVfx, DestroyVfxSpawnPoint.position, Quaternion.identity);
            AudioSource.PlayOneShot(DestroySound);

            //make object appear to be destroyed to player
            gameObject.GetComponent<MeshRenderer>().enabled = false;

            if (gameObject.GetComponent<BoxCollider>() != null)
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }

            if (gameObject.GetComponent<MeshCollider>() != null)
            {
                gameObject.GetComponent<MeshCollider>().enabled = false;
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            // this will call the OnDestroy function
            Destroy(gameObject, 1);
        }

    }
}