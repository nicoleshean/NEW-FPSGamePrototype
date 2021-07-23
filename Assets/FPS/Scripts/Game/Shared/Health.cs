using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    public class Health : MonoBehaviour //applies to player, enemies, etc.
    {
        [Tooltip("Maximum amount of health")] public float MaxHealth = 10f; //sets max health to 10 by default

        [Tooltip("Health ratio at which the critical health vignette starts appearing")]
        public float CriticalHealthRatio = 0.3f; //sets critical health ratio to 0.3f by default

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float> OnHealed;
        public UnityAction OnDie;

        public float CurrentHealth { get; set; } 
        public bool Invincible { get; set; }
        public bool CanPickup() => CurrentHealth < MaxHealth;

        public float GetRatio() => CurrentHealth / MaxHealth; //ratio of current/max health
        public bool IsCritical() => GetRatio() <= CriticalHealthRatio; //true if current/maxhealth is less than the critical ratio specified

        bool m_IsDead;

        void Start()
        {
            CurrentHealth = MaxHealth; //starts game with current health at 100
        }

        public void Heal(float healAmount) //takes input for heal amount
        {
            float healthBefore = CurrentHealth; //sets new healthBefore float to current health
            CurrentHealth += healAmount; //adds heal amount to current health, potentially resulting in CurrentHealth over the max
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth); //returns CurrentHealth value between 0f and MaxHealth

            // call OnHeal action
            float trueHealAmount = CurrentHealth - healthBefore; //finds the true amount healed (not counting the overheal)
            if (trueHealAmount > 0f) //if health needed healing
            {
                OnHealed?.Invoke(trueHealAmount); //invokes delegate with heal amount
            }
        }

        public void TakeDamage(float damage, GameObject damageSource) //takes input of damage, as well as source object that caused damaged
        {
            if (Invincible) //do not take any damage if invincible
                return;

            float healthBefore = CurrentHealth; 
            CurrentHealth -= damage; //subtracts damage amount from current health
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth); //returns CurrentHealth value between 0f and MaxHealth

            // call OnDamage action
            float trueDamageAmount = healthBefore - CurrentHealth; //finds true damage taken (not counting damage that put health below 0)
            if (trueDamageAmount > 0f) //if damage taken
            {
                OnDamaged?.Invoke(trueDamageAmount, damageSource); //invokes delegate with damage amount and damage source
            }

            HandleDeath();
        }

        public void Kill()
        {
            CurrentHealth = 0f; //immediately sets current health to 0

            // call OnDamage action
            OnDamaged?.Invoke(MaxHealth, null); //invokes delegate with damage amount and null source (died by falling)

            HandleDeath();
        }

        void HandleDeath()
        {
            if (m_IsDead) //if already dead, do nothing
                return;

            // call OnDie action
            if (CurrentHealth <= 0f) //if health is less than or equal to zero
            {
                m_IsDead = true; //dead is true
                OnDie?.Invoke(); //invokes OnDie delegate(s)
            }
        }
    }
}