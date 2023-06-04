using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace TsunamiItemCore.Utils {

	public static class MiscUtils
	{
		public static Vector3? RaycastToDirection(Vector3 position, float maxDistance, Vector3 direction, int layer)
		{
			if (Physics.Raycast(new Ray(position, direction), out var hitInfo, maxDistance, layer, QueryTriggerInteraction.Ignore))
			{
				return hitInfo.point;
			}
			return null;
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> toShuffle, Xoroshiro128Plus random)
		{
			List<T> list = new List<T>();
			foreach (T item in toShuffle)
			{
				list.Insert(random.RangeInt(0, list.Count + 1), item);
			}
			return list;
		}

		public static Vector3 FindClosestNodeToPosition(Vector3 position, HullClassification hullClassification, bool checkAirNodes = false)
		{
			NodeGraph nodeGraph = (checkAirNodes ? SceneInfo.instance.airNodes : SceneInfo.instance.groundNodes);
			NodeGraph.NodeIndex nodeIndex = nodeGraph.FindClosestNode(position, hullClassification);
			if (nodeIndex != NodeGraph.NodeIndex.invalid)
			{
				nodeGraph.GetNodePosition(nodeIndex, out var position2);
				return position2;
			}
			Main.ModLogger.LogInfo($"No closest node to be found for XYZ: {position}, returning 0,0,0");
			return Vector3.zero;
		}

		public static bool TeleportBody(CharacterBody characterBody, GameObject target, GameObject teleportEffect, HullClassification hullClassification, Xoroshiro128Plus rng, float minDistance = 20f, float maxDistance = 45f, bool teleportAir = false)
		{
			if (!(Object)(object)characterBody)
			{
				return false;
			}
			SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
			spawnCard.hullSize = hullClassification;
			spawnCard.nodeGraphType = (teleportAir ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground);
			spawnCard.prefab = Resources.Load<GameObject>("SpawnCards/HelperPrefab");
			GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
			{
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				position = target.transform.position,
				minDistance = minDistance,
				maxDistance = maxDistance
			}, rng));
			if ((bool)gameObject)
			{
				TeleportHelper.TeleportBody(characterBody, gameObject.transform.position);
				if ((bool)teleportEffect)
				{
					EffectManager.SimpleEffect(teleportEffect, gameObject.transform.position, Quaternion.identity, transmit: true);
				}
				Object.Destroy(gameObject);
				Object.Destroy(spawnCard);
				return true;
			}
			Object.Destroy(spawnCard);
			return false;
		}

		public static Vector3? AboveTargetVectorFromDamageInfo(DamageInfo damageInfo, float distanceAboveTarget)
		{
			if (damageInfo.rejected || !damageInfo.attacker)
			{
				return null;
			}
			CharacterBody component = damageInfo.attacker.GetComponent<CharacterBody>();
			if ((bool)(Object)(object)component)
			{
				TeamMask enemyTeams = TeamMask.GetEnemyTeams(component.teamComponent.teamIndex);
				HurtBox hurtBox = new SphereSearch
				{
					radius = 1f,
					mask = LayerIndex.entityPrecise.mask,
					origin = damageInfo.position
				}.RefreshCandidates().FilterCandidatesByHurtBoxTeam(enemyTeams).OrderCandidatesByDistance()
					.FilterCandidatesByDistinctHurtBoxEntities()
					.GetHurtBoxes()
					.FirstOrDefault();
				if ((bool)hurtBox && (bool)(Object)(object)hurtBox.healthComponent && (bool)(Object)(object)hurtBox.healthComponent.body)
				{
					CharacterBody body = hurtBox.healthComponent.body;
					Vector3 vector = body.mainHurtBox.collider.ClosestPointOnBounds(body.transform.position + new Vector3(0f, 10000f, 0f));
					Vector3? vector2 = RaycastToDirection(vector, distanceAboveTarget, Vector3.up, LayerIndex.world.mask);
					if (vector2.HasValue)
					{
						return vector2.Value;
					}
					return vector + Vector3.up * distanceAboveTarget;
				}
			}
			return null;
		}

		public static Vector3? AboveTargetBody(CharacterBody body, float distanceAbove)
		{
			if (!(Object)(object)body)
			{
				return null;
			}
			Vector3 vector = body.mainHurtBox.collider.ClosestPointOnBounds(body.transform.position + new Vector3(0f, 10000f, 0f));
			Vector3? vector2 = RaycastToDirection(vector, distanceAbove, Vector3.up, LayerIndex.world.mask);
			if (vector2.HasValue)
			{
				return vector2.Value;
			}
			return vector + Vector3.up * distanceAbove;
		}

		public static Dictionary<string, Vector3> GetAimSurfaceAlignmentInfo(Ray ray, int layerMask, float distance)
		{
			Dictionary<string, Vector3> dictionary = new Dictionary<string, Vector3>();
			if (!Physics.Raycast(ray, out var hitInfo, distance, layerMask, QueryTriggerInteraction.Ignore))
			{
				Main.ModLogger.LogInfo($"GetAimSurfaceAlignmentInfo did not hit anything in the aim direction on the specified layer ({layerMask}).");
				return null;
			}
			Vector3 point = hitInfo.point;
			Vector3 vector = Vector3.Cross(ray.direction, Vector3.up);
			Vector3 vector2 = Vector3.ProjectOnPlane(hitInfo.normal, vector);
			Vector3 value = Vector3.Cross(vector, vector2);
			dictionary.Add("Position", point);
			dictionary.Add("Right", vector);
			dictionary.Add("Forward", value);
			dictionary.Add("Up", vector2);
			return dictionary;
		}
	}
}
