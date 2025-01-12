using UnityEngine;
using UnityEngine.Events;

namespace Etra.StarterAssets.Combat
{

    public sealed class HealthSystem : MonoBehaviour
    {
        #region Variables
        [Header("Variables")]
        [SerializeField, Tooltip("The health of the GameObject")]
        float _health = 100;
        /// <summary>
        ///   The health of the GameObject.
        /// </summary>
        /// <remarks>
        ///   This variable's setter isn't public because it should be set by using the <see cref="Damage"/> and <see cref="Heal"/> methods
        ///   or by using the inspector.
        /// </remarks>
        public float health
        {
            get
            {
                return _health;
            }
            private set
            {
                float lastHealth = _health;
                _health = Mathf.Clamp(value, 0, maxHealth);
                float diff = Mathf.Abs(_health - lastHealth);
                if (diff != 0)
                    OnChange?.Invoke(diff);
                if (manualDeath)
                    return;

                bool last = isAlive;
                isAlive = _health > 0;
                if (last && !isAlive) OnDeath?.Invoke();
            }
        }
        [Tooltip("The maximum health of the object")] public float maxHealth = 100;
        /// <summary>
        ///   If true, <see cref="OnDeath"/> won't be called.
        /// </summary>
        [Tooltip("If true, the OnDeath event won't be called")] public bool manualDeath;
        public bool isAlive { get; private set; }

        #endregion

        #region Events
        /// <summary>
        ///   Called when the object is damaged.
        /// </summary>
        /// <remarks>
        ///   If the health is set to the value it already has, this event won't be executed.
        /// </remarks>
        [Header("Events")]
        [Tooltip("Called when the health changes")] public UnityEvent<float> OnChange;
        /// <summary>
        ///   Called when the object is damaged.
        /// </summary>
        /// <remarks>
        ///   If the damage dealed by <see cref="Damage"/> is equal to 0 or the <paramref name="skipEvents"/> parameter is true, this event won't be executed.
        /// </remarks>
        [Tooltip("Called when the object is damaged")] public UnityEvent<float> OnDamage;
        /// <summary>
        ///   Called when the object is healed.
        /// </summary>
        /// <remarks>
        ///   If the health healed by <see cref="Heal"/> is equal to 0 or the <paramref name="skipEvents"/> parameter is true, this event won't be executed.
        /// </remarks>
        [Tooltip("Called when the object is healed")] public UnityEvent<float> OnHeal;
        /// <summary>
        ///   Called when the health becomes 0.
        /// </summary>
        [Tooltip("Called when the health becomes 0")] public UnityEvent OnDeath;

        #endregion

        #region Methods

        /// <summary>
        ///   Decreases the amount of health
        /// </summary>
        /// <param name="hp">Amount of health to decrease. Negative values will be evaluated as positive.</param>
        /// <param name="skipEvents">If true, <see cref="OnDamage"/> won't be called</param>
        public void Damage(float hp, bool skipEvents = false)
        {
            float lastHealth = health;
            health -= Mathf.Abs(hp);
            float diff = Mathf.Abs(_health - lastHealth);
            if (diff != 0 || !skipEvents)
                OnDamage?.Invoke(diff);
        }

        /// <summary>
        ///   Increases the amount of health
        /// </summary>
        /// <param name="hp">Amount of health to increase. Negative values will be evaluated as positive.</param>
        /// <param name="skipEvents">If true, <see cref="OnHeal"/> won't be called</param>
        public void Heal(float hp, bool skipEvents = false)
        {
            float lastHealth = health;
            health += Mathf.Abs(hp);
            float diff = Mathf.Abs(_health - lastHealth);
            if (diff != 0 || !skipEvents)
                OnHeal?.Invoke(diff);
        }

        #endregion

        #region Unity Methods

        void OnValidate()
        {
            _health = Mathf.Clamp(health, 0, maxHealth);
            maxHealth = Mathf.Max(maxHealth, 0);
        }

        #endregion

    }


}
