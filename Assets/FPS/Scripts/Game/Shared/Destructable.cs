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

        [Header("FX")]
        [Tooltip("The VFX prefab spawned when the object is destroyed")]
        public GameObject DestroyVfx;

        [Tooltip("The point at which the destroy VFX is spawned")]
        public Transform DestroyVfxSpawnPoint;

        [Tooltip("The sound played when the object is damaged")]
        public AudioClip DamageTick;

        [Tooltip("The sound played when the object is destroyed")]
        public AudioClip DestroySound;

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

        void Start()
        {
            m_Health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, Destructable>(m_Health, this, gameObject);

            // Subscribe to damage & death actions
            m_Health.OnDie += OnDie;
            m_Health.OnDamaged += OnDamaged;
        }

        private void Update()
        {
            Debug.Log(m_Health.CurrentHealth);
            m_WasDamagedThisFrame = false;
        }

        void OnDamaged(float damage, GameObject damageSource)
        {
            if (damageSource && !damageSource.GetComponent<Destructable>()) //checks if damage was from player
            {

                // play the damage tick sound
                if (DamageTick && !m_WasDamagedThisFrame)
                    AudioUtility.CreateSFX(DamageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

                m_WasDamagedThisFrame = true;
            }
        }

        void OnDie()
        {
            if (TryDropItem())
            {
                Instantiate(LootPrefab, transform.position, Quaternion.identity);
            }
            var vfx = Instantiate(DestroyVfx, DestroyVfxSpawnPoint.position, Quaternion.identity);
            AudioUtility.CreateSFX(DestroySound, transform.position, AudioUtility.AudioGroups.DamageTick, 1f);
            // this will call the OnDestroy function
            Destroy(gameObject);
        }
        
    }
}