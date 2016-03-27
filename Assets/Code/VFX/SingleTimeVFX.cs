using UnityEngine;

namespace Mythic.VFX
{
    public class SingleTimeVFX : MonoBehaviour
    {
        private ParticleSystem m_particleSystem;

        private void Start()
        {
            m_particleSystem = GetComponent<ParticleSystem>();
            if(m_particleSystem != null)
                new Timer(m_particleSystem.startLifetime, Kill);
        }

        private void Kill()
        {
            Destroy(gameObject);
        }

        public ParticleSystem System
        {
            get
            {
                return m_particleSystem;
            }
        }
    }
}