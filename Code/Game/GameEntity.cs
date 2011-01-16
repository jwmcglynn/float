using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sputnik.Game {
	public class GameEntity : Entity {
		public GameEnvironment Environment;
		public SpawnPoint SpawnPoint;

		public const float LEFT_POSITION_BUFFER = 100;
		public const float RIGHT_POSITION_BUFFER = 100;
		public const float TOP_POSITION_BUFFER = 50;
		public const float BOTTOM_POSITION_BUFFER = -150;

		public const int NUMBER_OF_RUNGS = 12;
		public const float RUNG_HEIGHT = (1024
									- TOP_POSITION_BUFFER
									- BOTTOM_POSITION_BUFFER)
									/ (1 + NUMBER_OF_RUNGS);
		
		static protected float[] tracks;

		static GameEntity()
		{

			tracks = new float[NUMBER_OF_RUNGS];

			for (int i = 0; i < tracks.Length; i++)
			{
				tracks[i] = TOP_POSITION_BUFFER + i * RUNG_HEIGHT;
			}
		}

		public GameEntity(GameEnvironment env) {
			Environment = env;
		}

		public GameEntity(GameEnvironment env, SpawnPoint sp) {
			Environment = env;
			SpawnPoint = sp;
		}

		public override void Update(float elapsedTime) {
			if (ShouldCull()) Dispose();
			base.Update(elapsedTime);
		}

		public override void Dispose() {
			if (SpawnPoint != null) {
				// FIXME: Do we want to update the spawnpoint position when entities are culled?
				// SpawnPoint.Position = Position;
				OnCull();
			}

			base.Dispose();
		}

		public void SnapToRung()
		{
			int closestRung = -1;
			float closestDistance = float.PositiveInfinity;
			for (int i = 0; i < tracks.Length - 1; i++)
			{
				if (Math.Abs(Position.Y - tracks[i]) < closestDistance)
				{
					closestDistance = Math.Abs(Position.Y - tracks[i]);
					closestRung = i;
				}
			}
			Position = new Vector2(Position.X, tracks[closestRung]);
		}

		public int PositionToRung(float pos) {
			for (int i = 0; i < tracks.Length; i++) {
				if (tracks[i] >= pos - 10.0f) return i;
			}

			return tracks.Length;
		}

		/*********************************************************************/
		// Wind/pressure reaction.

		public virtual void OnTempChange(float amount)
		{
			// Call for children.
			Children.ForEach((Entity ent) => { if (ent is GameEntity) ((GameEntity) ent).OnTempChange(amount); });
		}

		public virtual void OnPressureChange(float amount) {
			// Call for children.
			Children.ForEach((Entity ent) => { if (ent is GameEntity) ((GameEntity) ent).OnPressureChange(amount); });
		}

		/*********************************************************************/
		// Culling.

		/// <summary>
		/// Should this Entity cull right now?  Called within Update.
		/// 
		/// If an Entity should not cull override this and return false.
		/// </summary>
		/// <returns>[true] if Entity should cull, [false] if not.</returns>
		public virtual bool ShouldCull() {
			return !InsideCullRect(VisibleRect);
		}

		/// <summary>
		/// Does the provided rectangle intersect the cull rect?
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		protected bool InsideCullRect(Rectangle rect) {
			int halfwidth = (int) (GameEnvironment.k_idealScreenSize.X / 2 + GameEnvironment.k_cullRadius);
			int halfheight = (int) (GameEnvironment.k_idealScreenSize.Y / 2 + GameEnvironment.k_cullRadius);

			int x = (int) Environment.Camera.Position.X;
			int y = (int) Environment.Camera.Position.Y;

			Rectangle cullRect = new Rectangle(x - halfwidth, y - halfheight, 2 * halfwidth, 2 * halfheight);

			return cullRect.Intersects(rect);
		}

		/// <summary>
		/// Called immediately before Entity is culled this and update the SpawnPoint before an object is culled.
		/// </summary>
		public virtual void OnCull() {
			// Currently does nothing.  Update SpawnPoint.
			SpawnPoint.Reset();
			SpawnPoint = null;
		}
	}
}