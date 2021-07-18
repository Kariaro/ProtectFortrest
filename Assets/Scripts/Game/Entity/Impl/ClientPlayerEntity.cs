using ProjectFortrest.Game.Entities;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Game.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProjectFortrest.Game.Entity.Impl {
	public class ClientPlayerEntity : PlayerEntity {
		
		// This is the MouseSelectionBox
		public GameObject mouseBox;
		[HideInInspector] public SpriteRenderer MouseBoxRenderer;

		// This is the PlayerLocationBox
		public GameObject locationBox;
		[HideInInspector] public SpriteRenderer LocationBoxRenderer;

		public SpriteRenderer rend;
		public Sprite sprite_x;
		public Sprite sprite_up;
		public Sprite sprite_down;


		public override void OnStart() {
			MouseBoxRenderer = mouseBox.GetComponent<SpriteRenderer>();
			LocationBoxRenderer = locationBox.GetComponent<SpriteRenderer>();
			rend = GetComponent<SpriteRenderer>();
		}

		public new void SetLayer(int layer) {
			if(layer < 0 || layer > 31) return;
			int lastLayer = Layer;
			base.SetLayer(layer);
			GetComponent<SpriteRenderer>().sortingOrder = (layer + 1) * 2;

			if(lastLayer != layer) {
				LevelManager level = GameManager.Instance.LevelManager;
				level.GetLevel(lastLayer).SetCollision(false);
				level.GetLevel(layer).SetCollision(true);

				for(int i = 0; i < 32; i++) {
					int intensity = i - layer;
					float curve = Mathf.Abs(intensity) / 4.0f;
					if(curve > 0.75) curve = 0.75f;
					Color layerColor = new Color(1 - curve, 1 - curve, 1 - curve, 1 - curve);

					TileLevel tileLevel = level.GetLevel(i);
					tileLevel.tilemap.color = layerColor;

					
					if(i > layer) {
						tileLevel.tilemap_render.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
					} else {
						tileLevel.tilemap_render.maskInteraction = SpriteMaskInteraction.None;
					}
				}
			}
		}

		public override void Tick() {
			bool up = Input.GetKey(KeyBindingsManager.Up);
			bool down = Input.GetKey(KeyBindingsManager.Down);
			bool left = Input.GetKey(KeyBindingsManager.Left);
			bool right = Input.GetKey(KeyBindingsManager.Right);
			bool shift = Input.GetKey(KeyBindingsManager.Shift);

			int xx = (left ? 1:0) - (right ? 1:0);
			int yy = (up ? 1:0) - (down ? 1:0);

			if(xx != 0 || yy != 0) {
				float denom = Mathf.Sqrt(xx * xx + yy * yy);
				float speed = shift ? 3f:1.5f;
				Velocity += new Vector2(xx, yy) * speed / denom;
			}
		}

		void Update() {
			if(Input.GetKeyDown(KeyCode.I)) {
				SetLayer(Layer + 1);
			}
			if(Input.GetKeyDown(KeyCode.J)) {
				SetLayer(Layer - 1);
			}
		}

		public override void AfterTick() {
			Camera current = Camera.main;
			Vector3 camPos = current.transform.position;
			Vector2 newPos = ((Vector2)camPos + Position) / 2.0f;

			current.transform.position = new Vector3(newPos.x, newPos.y, camPos.z);
			/*{
				float ppx = Mathf.FloorToInt(newPos.x * 16) / 16.0f + 0.0001f;
				float ppy = Mathf.FloorToInt(newPos.y * 16) / 16.0f + 0.0001f;
				current.transform.position = new Vector3(ppx, ppy, camPos.z);
			}*/

			int px = Mathf.FloorToInt(Position.x);
			int py = Mathf.FloorToInt(Position.y - 0.25f);
			locationBox.transform.position = new Vector3(px + 0.5f, py + 0.5f, -Layer);

			if(Velocity.sqrMagnitude > 0) {
				if(Velocity.x > 0 && !rend.flipX) rend.flipX = true;
				if(Velocity.x < 0 && rend.flipX) rend.flipX = false;
				if(Mathf.Abs(Velocity.x) > Mathf.Abs(Velocity.y)) {
					rend.sprite = sprite_x;
				} else {
					if(Velocity.y > 0) rend.sprite = sprite_up;
					if(Velocity.y < 0) rend.sprite = sprite_down;
				}
			}
			
			{
				Ray ray = current.ScreenPointToRay(Input.mousePosition);
				Vector3 rayPos = ray.origin;

				int mx = Mathf.FloorToInt(rayPos.x);
				int my = Mathf.FloorToInt(rayPos.y);
				mouseBox.transform.position = new Vector3(mx + 0.5f, my + 0.5f, -Layer);

				TileLevel level = GameManager.Instance.LevelManager.GetLevel(Layer);
				if(Input.GetMouseButton(0)) {
					//Debug.Log(level.GetFloor(mx, my));
					//level.SetWall(mx, my, GameManager.Instance.BlockManager.WOOD_WALL);
					//level.SetBlock(mx, my, GameManager.Instance.BlockManager.CHEST);
				} else if(Input.GetMouseButton(1)) {
					//level.RemoveBlock(mx, my, 2);
				}

				{
					Sprite sprite = level.tilemap.GetSprite(new Vector3Int(mx, my, TileLevel.INTER));
					Bounds bounds = GetSpritePhysicsShapeBounds(sprite);
					MouseBoxRenderer.size = new Vector2(bounds.size.x, bounds.size.y);
					mouseBox.transform.position = new Vector3(mx + 0.5f + bounds.center.x, my + 0.5f + bounds.center.y, -Layer);

					if(sprite == null) {
						MouseBoxRenderer.color = new Color(0, 0, 0, 0.5f);
					} else {
						MouseBoxRenderer.color = new Color(0, 0, 0);
					}

					{
						Color color = MouseBoxRenderer.color;
						float val = (Mathf.Sin(Time.realtimeSinceStartup * 4) + 2) / 3.0f;
						color.a *= val;

						MouseBoxRenderer.color = color;
					}
				}
			}
		}

		private Bounds GetSpritePhysicsShapeBounds(Sprite sprite) {
			if(sprite == null || sprite.GetPhysicsShapeCount() == 0) {
				return new Bounds(Vector3.zero, Vector3.one);
			}

			List<Vector2> physicsShape = new List<Vector2>();
			int count = sprite.GetPhysicsShape(0, physicsShape);

			float x_min = 1, x_max = 0;
			float y_min = 1, y_max = 0;
			foreach(Vector2 point in physicsShape) {
				if(x_min > point.x) x_min = point.x;
				if(x_max < point.x) x_max = point.x;
				if(y_min > point.y) y_min = point.y;
				if(y_max < point.y) y_max = point.y;
			}

			return new Bounds(new Vector3(
				(x_max + x_min) / 2.0f,
				(y_max + y_min) / 2.0f,
				0
			), new Vector3(
				(x_max - x_min),
				(y_max - y_min),
				1
			));
		}
	}
}
