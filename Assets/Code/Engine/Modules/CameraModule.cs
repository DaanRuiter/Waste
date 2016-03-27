using DG.Tweening;
using UnityEngine;

namespace Mythic.Modules
{
    public class CameraModule : ModuleBase
    {
        private PlayerModule m_playerModule;
		private Rigidbody2D m_rigidBody;
        private Camera m_camera;

        private Vector3 m_velocity = Vector3.zero;
        private Vector3 m_lastFramePosition;
        private Vector3 m_framePosition;

        private Timer m_shakeTimer;

        private float m_damp = 0.55f;
        private float m_shakeStrength = 0f;

        public CameraModule()
        {
            m_camera = Camera.main;

            FINISH_INITIALISATION("CameraModule");
        }

        internal override void Start()
        {
            base.Start();
            m_playerModule = GET_MODULE<PlayerModule>();
            m_playerModule.ScriptWrapper.GetScript<PlayerController>().OnCollisionEnterEvent += OnPlayerCollision;
            m_lastFramePosition = m_camera.transform.position;
			m_rigidBody = m_playerModule.ScriptWrapper.GetScript<PlayerController>().RigidBody;
        }

        internal override void FixedUpdate()
        {
            Transform target = m_playerModule.GameObject.transform;
            Vector3 point = m_camera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - m_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = m_camera.transform.position + delta;
			m_framePosition = Vector3.SmoothDamp(m_lastFramePosition, destination, ref m_velocity, m_damp);
        }

        internal override void LateUpdate()
        {
            base.LateUpdate();

            m_lastFramePosition = m_camera.transform.position;

            if (m_shakeTimer != null)
            {
                if (!m_shakeTimer.IsDone)
                {
                    m_framePosition += new Vector3(Random.Range(-m_shakeStrength, m_shakeStrength), Random.Range(-m_shakeStrength, m_shakeStrength), 0f);
                }
            }
            m_camera.transform.position = m_framePosition;
        }

        private void OnPlayerCollision(Rigidbody2D player, Collision2D collision)
        {
            StartScreenShake(0.045f, player.velocity.magnitude / 45f);
        }

        public void StartScreenShake(float duration, float strength)
        {
            m_shakeTimer = new Timer(duration);
            m_shakeStrength = strength;
        }
    }
}