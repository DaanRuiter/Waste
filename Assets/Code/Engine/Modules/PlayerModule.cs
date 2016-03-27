using System;
using DG.Tweening;
using UnityEngine;
using ParticlePlayground;
using Mythic.Engine;
using Mythic.VFX;

namespace Mythic.Modules
{
    public class PlayerModule : ModuleBase
    {
        private PlayerController m_controller;
        private PlayerGraphics m_graphics;

        public PlayerModule()
        {
            GameObject playerParent = new GameObject("Player");
            m_graphics = ScriptWrapper.AddScript(Unity.Initialize<PlayerGraphics>()) as PlayerGraphics;

            m_controller = ScriptWrapper.AddScript(Unity.Initialize<PlayerController>(playerParent)) as PlayerController;

            m_graphics.transform.SetParent(playerParent.transform);
            m_graphics.transform.localPosition = new Vector3(0f, 0f, -0.1f);

            FINISH_INITIALISATION("PlayerModule");
        }

        internal override void Start()
        {
            base.Start();
        }

        public GameObject GameObject
        {
            get
            {
                return m_controller.gameObject;
            }
        }
    }
    
    public class PlayerController : MonoBehaviour, IUnityScript
    {
        /*//Movement
        private float m_maxMovementSpeed = 35f;
        private float m_currentMovementSpeed = 0f;
        private float m_accelerationTime = 6.25f;
        private float m_deaccelerationTime = 3.5f;
        //Rotation
        private float m_rotationSpeed = 115f;
        private float m_movementRotationMultiplier = 1.15f;

        private bool m_isMoving = false;

        private Tweener m_accelerationTweener;

        private Rigidbody2D m_rigidBody;

        public string ObjectName
        {
            get
            {
                return "Controller";
            }
        }

        public void SetCurrentMovementSpeed(float speed)
        {
            m_currentMovementSpeed = speed;
        }

        public void AddComponents()
        {
            m_rigidBody = transform.parent.gameObject.AddComponent<Rigidbody2D>();
            transform.parent.gameObject.AddComponent<BoxCollider2D>();
        }

        public void OnStart()
        {
            m_rigidBody.gravityScale = 0f;

            BoxCollider2D collider = transform.parent.gameObject.GetComponent<BoxCollider2D>();
            Vector2 size = transform.parent.FindChild("Graphics").GetComponent<SpriteRenderer>().sprite.rect.size;
            collider.size = size / 100f;
        }

        public void OnTick()
        {
            //Moving
            float forward = Input.GetAxis("Vertical");
            if (!m_isMoving)
            {
                if (forward >= 0.05f)
                {
                    OnMovementStart();
                }
            }else if (forward < 0.05f)
            {
                OnMovementEnd();
            }
            Vector3 velocity = transform.parent.right * Time.deltaTime * m_currentMovementSpeed;

            //Turning
            float horizontal = -Input.GetAxis("Horizontal");
            float currentSpeedPercentage = m_currentMovementSpeed / (m_maxMovementSpeed / 100f);
            horizontal = horizontal * (m_movementRotationMultiplier / 100f * currentSpeedPercentage);

            transform.parent.Rotate(new Vector3(0f, 0f, horizontal * Time.deltaTime * m_rotationSpeed));
            m_rigidBody.MoveRotation(m_rigidBody.rotation + horizontal * Time.deltaTime * m_rotationSpeed);
            m_rigidBody.MovePosition(m_rigidBody.position + (Vector2)velocity);
        }

        private void OnMovementStart()
        {
            float currentSpeedPercentage = (m_maxMovementSpeed -  m_currentMovementSpeed) / (m_maxMovementSpeed / 100f);
            if (m_accelerationTweener != null)
                m_accelerationTweener.Kill();
            m_accelerationTweener = DOTween.To(SetCurrentMovementSpeed, m_currentMovementSpeed, m_maxMovementSpeed, m_accelerationTime / 100f * currentSpeedPercentage);
            m_isMoving = true;
        }
        private void OnMovementEnd()
        {
            float currentSpeedPercentage = m_currentMovementSpeed / (m_maxMovementSpeed / 100f);
            if (m_accelerationTweener != null)
                m_accelerationTweener.Kill();
            m_accelerationTweener = DOTween.To(SetCurrentMovementSpeed, m_currentMovementSpeed, 0f, m_deaccelerationTime / 100f * currentSpeedPercentage);
            m_isMoving = false;
        }*/

        public delegate void OnCollisionEventHandler(Rigidbody2D player, Collision2D collision);
        public OnCollisionEventHandler OnCollisionEnterEvent;

        private float power = 15;
        private float maxspeed = 222;
        private float turnpower = 2;
        private float friction = 0.85f;
        private Vector2 curspeed;
        private Rigidbody2D m_rigidbody;

        public string ObjectName
        {
            get
            {
                return "Controller";
            }
        }        
        public Rigidbody2D RigidBody
        {
            get
            {
                return m_rigidbody;
            }
        }

        private void FixedUpdate()
        {
            curspeed = new Vector2(m_rigidbody.velocity.x, m_rigidbody.velocity.y);

            if (curspeed.magnitude > maxspeed)
            {
                curspeed = curspeed.normalized;
                curspeed *= maxspeed;
            }

            if (Input.GetKey(KeyCode.W))
            {
                m_rigidbody.AddForce(transform.right * power);
                m_rigidbody.drag = friction;
            }
            if (Input.GetKey(KeyCode.S))
            {
                m_rigidbody.AddForce(-(transform.right) * (power / 2));
                m_rigidbody.drag = friction;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.forward * turnpower);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.forward * -turnpower);
            }
            if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                m_rigidbody.drag = friction * 2f;
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (OnCollisionEnterEvent != null)
                OnCollisionEnterEvent(m_rigidbody, coll);
        }

        //I_UNITY_SCRIPT
        public void AddComponents()
        {
            m_rigidbody = transform.gameObject.AddComponent<Rigidbody2D>();
            m_rigidbody.angularDrag = 100f;

            transform.gameObject.AddComponent<BoxCollider2D>();
        }
        public void OnStart()
        {
            BoxCollider2D collider = transform.gameObject.GetComponent<BoxCollider2D>();
            Vector2 size = transform.FindChild("Graphics").GetComponent<SpriteRenderer>().sprite.rect.size;
            collider.size = size / 100f;
        }
        public void OnTick()
        {

        }
    }
    
    public class PlayerGraphics : MonoBehaviour, IUnityScript
    {
        private SpriteRenderer m_sr;
        private Rigidbody2D m_rigidBody;

        //VFX
        private ParticleSystem m_carTrail;
		private PlaygroundParticlesC m_carDust;

        //Timers
        private Timer m_carTrailTimer;

        public string ObjectName
        {
            get
            {
                return "Graphics";
            }
        }

        public void AddComponents()
        {
            m_sr = gameObject.AddComponent<SpriteRenderer>();
        }

        public void OnStart()
        {
            PlayerModule playerModule = Main.FindModule<PlayerModule>();

            //Sprite
            m_sr.sprite = Resources.Load<Sprite>("Graphics/Player/player");

            //Rigidbody
            m_rigidBody = playerModule.ScriptWrapper.GetScript<PlayerController>().RigidBody;

            //Trail
            m_carTrail = Instantiate(Resources.Load<GameObject>("VFX/Particle_Trail_Car")).GetComponent<ParticleSystem>();
			m_carTrail.transform.SetParent(transform.parent);

			//Dust
			m_carDust = Instantiate(Resources.Load<GameObject>("VFX/Particle_Dust_Car")).GetComponent<PlaygroundParticlesC>();
			m_carDust.transform.SetParent(transform.parent);
			m_carDust.transform.localPosition = Vector3.zero;

            m_carTrailTimer = new Timer(0.08f, EmitDustParticles);

            //Collision
            playerModule.ScriptWrapper.GetScript<PlayerController>().OnCollisionEnterEvent += CollisionParticles;            
        }

        public void OnTick()
        {

        }

        //Particles
        private void CollisionParticles(Rigidbody2D player, Collision2D collision)
        {
            foreach (var contact in collision.contacts)
            {
                GameObject vfx = Instantiate(Resources.Load<GameObject>("VFX/Particle_Collision_Junk"));
                vfx.AddComponent<SingleTimeVFX>();
                vfx.GetComponent<ParticleSystem>().Emit((int)player.velocity.magnitude);

                vfx.transform.position = contact.point;

                Quaternion originalRottaion = vfx.transform.rotation;
                vfx.transform.LookAt(player.transform.position);
            }
        }
        private void EmitDustParticles()
        {
            if(m_rigidBody.velocity.magnitude > 3f)
            {
                m_carTrail.transform.rotation = Quaternion.LookRotation(m_rigidBody.velocity);
				m_carTrail.transform.eulerAngles +=  new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(0f, 8f), 0);
                m_carTrail.startSpeed = m_rigidBody.velocity.magnitude / 3f;
				m_carTrail.Emit(UnityEngine.Random.Range(1, 3));
				m_carDust.particleCount = (int)(m_rigidBody.velocity.magnitude * 10);
			}else{
				m_carDust.particleCount = 0;
			}
            m_carTrailTimer.Start();
        }
    }
}