using System;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace TsunamiItemCore.Equipment {

	public class ExperimentalVoidStrikeDevice : EquipmentBase
	{
		public static GameObject voidFuckEffectPrefab = Resources.Load<GameObject>("prefabs/effects/nullifierdeathexplosion");

		private float stopwatch;

		private int riskyCounter;

		public override string EquipmentName => "Experimental Void Strike Device";

		public override string EquipmentLangTokenName => "TSUNAMI_EVSD";

		public override string EquipmentPickupDesc => "Fire off 3 explosions, dealing heavy damage. May summon Void Reavers.";

		public override string EquipmentFullDescription => "Create three small <style=cIsUtility>Void Reaver</style> explosions, dealing <style=cIsUtility>1250% damage</style> to enemies caught inside them… <style=cIsHealth>BUT</style>, each use has an <style=cIsHealth>increasing</style> chance to <style=cIsHealth>summon Void Reavers</style> to your location.";

		public override string EquipmentLore => "dark samus isn't real im sorry";

		public override float Cooldown => 40f;

		public override bool IsLunar => true;

		public override GameObject EquipmentModel => Main.MainAssets.LoadAsset<GameObject>("ExperimentalVoidStrikeDeviceDisplay.prefab");

		public override Sprite EquipmentIcon => Main.MainAssets.LoadAsset<Sprite>("ExperimentalVoidStrikeDeviceIcon.png");

		public override void Init(ConfigFile config)
		{
			CreateConfig(config);
			CreateLang();
			CreateEquipment();
			Hooks();
		}

		protected override void CreateConfig(ConfigFile config)
		{
		}

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
		}

		protected override bool ActivateEquipment(EquipmentSlot slot)
		{
			if (NetworkServer.active)
			{
				if (Util.CheckRoll(75f - (float)riskyCounter, slot.characterBody.master))
				{
					riskyCounter++;
					riskyCounter++;
					riskyCounter++;
					riskyCounter++;
					riskyCounter++;
					BlastAttack blastAttack = new BlastAttack
					{
						baseDamage = 12.5f * slot.characterBody.damage,
						radius = 15f,
						procCoefficient = 1f,
						position = slot.characterBody.transform.position,
						attacker = ((Component)(object)slot.characterBody).gameObject,
						crit = Util.CheckRoll(slot.characterBody.crit, slot.characterBody.master),
						falloffModel = BlastAttack.FalloffModel.None,
						damageType = DamageType.Nullify,
						teamIndex = TeamComponent.GetObjectTeam(((Component)(object)slot.characterBody).gameObject)
					};
					BlastAttack blastAttack2 = new BlastAttack
					{
						baseDamage = 12.5f * slot.characterBody.damage,
						radius = 25f,
						procCoefficient = 1f,
						position = slot.characterBody.transform.position,
						attacker = ((Component)(object)slot.characterBody).gameObject,
						crit = Util.CheckRoll(slot.characterBody.crit, slot.characterBody.master),
						falloffModel = BlastAttack.FalloffModel.None,
						damageType = DamageType.Nullify,
						teamIndex = TeamComponent.GetObjectTeam(((Component)(object)slot.characterBody).gameObject)
					};
					BlastAttack blastAttack3 = new BlastAttack
					{
						baseDamage = 12.5f * slot.characterBody.damage,
						radius = 35f,
						procCoefficient = 1f,
						position = slot.characterBody.transform.position,
						attacker = ((Component)(object)slot.characterBody).gameObject,
						crit = Util.CheckRoll(slot.characterBody.crit, slot.characterBody.master),
						falloffModel = BlastAttack.FalloffModel.None,
						damageType = DamageType.Nullify,
						teamIndex = TeamComponent.GetObjectTeam(((Component)(object)slot.characterBody).gameObject)
					};
					Util.PlayAttackSpeedSound("Play_nullifier_death", ((Component)(object)slot.characterBody).gameObject, 1.5f);
					EffectManager.SpawnEffect(voidFuckEffectPrefab, new EffectData
					{
						origin = slot.characterBody.transform.position,
						scale = 15f
					}, transmit: true);
					blastAttack.Fire();
					Util.PlayAttackSpeedSound("Play_nullifier_death", ((Component)(object)slot.characterBody).gameObject, 1f);
					EffectManager.SpawnEffect(voidFuckEffectPrefab, new EffectData
					{
						origin = slot.characterBody.transform.position,
						scale = 25f
					}, transmit: true);
					blastAttack2.Fire();
					Util.PlayAttackSpeedSound("Play_nullifier_death", ((Component)(object)slot.characterBody).gameObject, 0.5f);
					EffectManager.SpawnEffect(voidFuckEffectPrefab, new EffectData
					{
						origin = slot.characterBody.transform.position,
						scale = 35f
					}, transmit: true);
					blastAttack3.Fire();
				}
				else
				{
					DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscNullifier"), new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.Approximate,
						minDistance = 2f,
						maxDistance = 20f,
						spawnOnTarget = slot.characterBody.transform
					}, RoR2Application.rng);
					directorSpawnRequest.teamIndexOverride = TeamIndex.Monster;
					DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
					directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, (Action<SpawnCard.SpawnResult>)delegate
					{
					});
					DirectorCore instance = DirectorCore.instance;
					if (instance == null)
					{
					}
					instance.TrySpawnObject(directorSpawnRequest);
					instance.TrySpawnObject(directorSpawnRequest);
					instance.TrySpawnObject(directorSpawnRequest);
					riskyCounter = 0;
				}
			}
			return true;
		}
	}
}
