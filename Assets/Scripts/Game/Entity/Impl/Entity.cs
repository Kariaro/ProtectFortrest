using ProjectFortrest.Game.Entities;
using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Entity.Impl {
	[RequireComponent(typeof(Rigidbody2D))]
	public class Entity : MonoBehaviour, IEntity {
		[SerializeField] private Vector2 _position = new Vector2();
		[SerializeField] private Vector2 _velocity = new Vector2();
		[SerializeField] private int _layer = 0;
		[SerializeField] private EntityStatus _status = new EntityStatus();

		public Vector2 Position { get => _position; set => SetPosition(value); }
		public Vector2 Velocity { get => _velocity; set => SetVelocity(value); }
		public int Layer { get => _layer; set => SetLayer(value); }
		public EntityStatus Status { get => _status; }

		public Collider2D coll;
		public Rigidbody2D rb2d;

		void Start() {
			coll = GetComponent<Collider2D>();
			rb2d = GetComponent<Rigidbody2D>();
			OnStart();
		}

		public virtual void OnStart() {

		}

		public virtual void Deserialize(BinaryReader reader) {
			_position = reader.ReadVector2();
			_velocity = reader.ReadVector2();
			_layer = reader.ReadInt32();
			_status.Deserialize(reader);
		}
		
		public virtual void Serialize(BinaryWriter writer) {
			writer.WriteVector2(_position);
			writer.WriteVector2(_velocity);
			writer.Write(_layer);
			_status.Serialize(writer);
		}

		public EntityStatus GetStatus() {
			return _status;
		}

		public void SetPosition(Vector2 position) {
			_position = position;
		}
		
		public void SetPosition(Vector2 position, int layer) {
			_position = position;
			_layer = layer;
		}
		
		public Vector2 GetPosition() {
			return _position;
		}

		public void SetVelocity(Vector2 velocity) {
			_velocity = velocity;
		}

		public Vector2 GetVelocity() {
			return _velocity;
		}

		public void SetLayer(int layer) {
			if(layer < 0 || layer > 31) return;
			_layer = layer;
		}

		public int GetLayer() {
			return _layer;
		}

		// Called before each internal physics update
		void FixedUpdate() {
			Tick();

			// Apply velocity
			
			rb2d.velocity = _velocity;
			_position = rb2d.position;

			//_position += _velocity * Time.fixedDeltaTime;
			// Dampen velocity
			_velocity *= 0.6f;

			// If velocity is small enough make it zero
			if(_velocity.magnitude < 0.0001) {
				_velocity = Vector2.zero;
			}
			
			// Apply position
			transform.position = new Vector3(_position.x, _position.y, -_layer);

			AfterTick();
		}

		public virtual void Tick() {}
		public virtual void AfterTick() {}
	}
}
