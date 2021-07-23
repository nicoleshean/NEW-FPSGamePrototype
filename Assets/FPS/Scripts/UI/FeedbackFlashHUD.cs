using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class FeedbackFlashHUD : MonoBehaviour //flash that pops on screen when taking damage or gaining health
    {
        [Header("References")] [Tooltip("Image component of the flash")]
        public Image FlashImage;

        [Tooltip("CanvasGroup to fade the damage flash, used when recieving damage end healing")]
        public CanvasGroup FlashCanvasGroup;

        [Tooltip("CanvasGroup to fade the critical health vignette")]
        public CanvasGroup VignetteCanvasGroup;

        [Header("Damage")] [Tooltip("Color of the damage flash")]
        public Color DamageFlashColor;

        [Tooltip("Duration of the damage flash")]
        public float DamageFlashDuration;

        [Tooltip("Max alpha of the damage flash")]
        public float DamageFlashMaxAlpha = 1f;

        [Header("Critical health")] [Tooltip("Max alpha of the critical vignette")]
        public float CriticaHealthVignetteMaxAlpha = .8f;

        [Tooltip("Frequency at which the vignette will pulse when at critical health")]
        public float PulsatingVignetteFrequency = 4f;

        [Header("Heal")] [Tooltip("Color of the heal flash")]
        public Color HealFlashColor;

        [Tooltip("Duration of the heal flash")]
        public float HealFlashDuration;

        [Tooltip("Max alpha of the heal flash")]
        public float HealFlashMaxAlpha = 1f;

        bool m_FlashActive;
        float m_LastTimeFlashStarted = Mathf.NegativeInfinity;
        Health m_PlayerHealth;
        GameFlowManager m_GameFlowManager;

        void Start()
        {
            // Subscribe to player damage events
            PlayerCharacterController playerCharacterController = FindObjectOfType<PlayerCharacterController>(); 
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, FeedbackFlashHUD>(
                playerCharacterController, this);

            m_PlayerHealth = playerCharacterController.GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, FeedbackFlashHUD>(m_PlayerHealth, this,
                playerCharacterController.gameObject);

            m_GameFlowManager = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, FeedbackFlashHUD>(m_GameFlowManager, this);

            m_PlayerHealth.OnDamaged += OnTakeDamage; //adds local OnTakeDamage function to OnDamaged delegate
            m_PlayerHealth.OnHealed += OnHealed; //adds local OnHealed function to OnHealed delegate
        }

        void Update()
        {
            if (m_PlayerHealth.IsCritical()) //if health has reached critical threshold 
            {
                VignetteCanvasGroup.gameObject.SetActive(true); //turns on critical health vignette
                float vignetteAlpha =
                    (1 - (m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth /
                          m_PlayerHealth.CriticalHealthRatio)) * CriticaHealthVignetteMaxAlpha; //sets vignetteAlpha based on how critical the health is

                if (m_GameFlowManager.GameIsEnding)
                    VignetteCanvasGroup.alpha = vignetteAlpha; //holds vignette on screen if dead
                else
                    VignetteCanvasGroup.alpha =
                        ((Mathf.Sin(Time.time * PulsatingVignetteFrequency) / 2) + 0.5f) * vignetteAlpha; //pulsates the vignette if still alive
            }
            else
            {
                VignetteCanvasGroup.gameObject.SetActive(false); //if health isn't critical, turn off vignette
            }


            if (m_FlashActive)
            {
                float normalizedTimeSinceDamage = (Time.time - m_LastTimeFlashStarted) / DamageFlashDuration; //time between now and last flash divided by the duration

                if (normalizedTimeSinceDamage < 1f)
                {
                    float flashAmount = DamageFlashMaxAlpha * (1f - normalizedTimeSinceDamage); //determines the alpha of the flash depending on the max alpha specified in inspector
                    FlashCanvasGroup.alpha = flashAmount;
                }
                else
                {
                    FlashCanvasGroup.gameObject.SetActive(false); //turns off flash
                    m_FlashActive = false; //sets flash active back to false
                }
            }
        }

        void ResetFlash() //records last flash time and turns on flash
        {
            m_LastTimeFlashStarted = Time.time; //sets the last time the flash started as the current time
            m_FlashActive = true; 
            FlashCanvasGroup.alpha = 0f; //sets flash alpha back to 0
            FlashCanvasGroup.gameObject.SetActive(true); //turns on flash
        }

        void OnTakeDamage(float dmg, GameObject damageSource) //triggers damage flash 
        {
            ResetFlash();
            FlashImage.color = DamageFlashColor; //sets flash color to Damage color
        }

        void OnHealed(float amount) //triggers heal flash
        {
            ResetFlash();
            FlashImage.color = HealFlashColor; //sets flash color to Heal color
        }
    }
}