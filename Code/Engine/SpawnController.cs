using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik {
	public class SpawnController {
		internal GameEnvironment Environment;
		public List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

		public SpawnController(GameEnvironment env) {
			Environment = env;
		}
		
		public void CreateSpawnPoints(IList<Squared.Tiled.ObjectGroup> objectGroupList, Vector2? offset = null, bool createPlayerSpawn = false) {
			bool spawnedPlayer = false;
			Vector2 realOffset = offset == null ? Vector2.Zero : (Vector2) offset;
			
			// Load spawn points.
			foreach (Squared.Tiled.ObjectGroup objGroup in objectGroupList) {
				foreach (List<Squared.Tiled.Object> objList in objGroup.Objects.Values) {
					foreach (Squared.Tiled.Object obj in objList) {
						obj.X += (int) realOffset.X;
						obj.Y += (int) realOffset.Y;
						SpawnPoint sp = new SpawnPoint(this, obj);

						// Immediately spawn some entities.
						switch (sp.EntityType) {
							case "spawn":
								if (createPlayerSpawn) {
									spawnedPlayer = true;
									sp.AlwaysSpawned = true;
									sp.Spawn();
								}
								break;
							default:
								SpawnPoints.Add(sp);
								break;
						}
					}
				}
			}

			if (!spawnedPlayer && createPlayerSpawn) throw new InvalidOperationException("Level loaded does not contain player spawn point.");
		}

		public void Update(float elapsedTime) {
			int halfwidth = (int) (GameEnvironment.k_idealScreenSize.X / 2 + GameEnvironment.k_spawnRadius);
			int halfheight = (int) (GameEnvironment.k_idealScreenSize.Y / 2 + GameEnvironment.k_spawnRadius);

			int x = (int) Environment.Camera.Position.X;
			int y = (int) Environment.Camera.Position.Y;

			Rectangle spawnRect = new Rectangle(x - halfwidth, y - halfheight, 2 * halfwidth, 2 * halfheight);

			SpawnPoints.ForEach((SpawnPoint sp) => sp.Update(elapsedTime, spawnRect));
		}
	}
}
