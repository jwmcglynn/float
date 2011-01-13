using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sputnik.Game {
	public class GameEntity : Entity {
		public GameEnvironment Environment;
		public SpawnPoint SpawnPoint;

        public static int TRACK_NUM = 10;
        public static float TRACK_DISTANCE = 90.0f;
        public static float TRACK_0 = 50.0f;

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
		
		/*********************************************************************/
		// Wind/pressure reaction.

		public virtual void OnTempChange(float amount) {
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
			int halfwidth = (int) (GameEnvironment.k_maxVirtualSize.X / 2 + GameEnvironment.k_cullRadius);
			int halfheight = (int) (GameEnvironment.k_maxVirtualSize.Y / 2 + GameEnvironment.k_cullRadius);

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