using System;
using UnityEngine;

namespace MJM
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static event EventHandler<OnTickArgs> OnTick;

        public WorldSystem WorldSystem { get; private set; }
        public EntitySystem EntitySystem { get; private set; }

        private int _tick;
        private float _tickTimer;

        // Awake is called on object creation
        void Awake()
        {
            EnforceSingleInstance();

            WorldSystem = new WorldSystem();
            EntitySystem = new EntitySystem();

            _tick = 0;
            _tickTimer = 0f;
        }

        private void EnforceSingleInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            WorldSystem.Init();
            EntitySystem.Init();
        }
            
        // Update is called once per frame
        void Update()
        {
            _tickTimer += Time.deltaTime;

            if (_tickTimer >= GameInfo.TickDuration)
            {
                _tick++;

                _tickTimer -= GameInfo.TickDuration;

                OnTick?.Invoke(this, new OnTickArgs { Tick = _tick });
            }
        }

        void OnDisable()
        {
            WorldSystem.Quit();
            EntitySystem.Quit();
        }
    }
}
