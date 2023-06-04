/*using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace TsunamiItemCore.Equipment {

	public class VoidClam : EquipmentBase
	{
		public List<ItemDef> sickitemList = new List<ItemDef> { RoR2Content.Items.ShinyPearl };

		public static GameObject voidFuckEffectPrefab = Resources.Load<GameObject>("prefabs/effects/nullifierdeathexplosion");

		public EquipmentDef equipmentDef;

		private Xoroshiro128Plus rng = new Xoroshiro128Plus(0uL);

		private int clamCounter;

		public override string EquipmentName => "Void Clam";

		public override string EquipmentLangTokenName => "TSUNAMI_VOID_CLAM";

		public override string EquipmentPickupDesc => "Attempt to break into a Void Clam. Failing may have serious consequences...";

		public override string EquipmentFullDescription => "Upon use, you have a <style=cIsUtility>40%</style> chance to be <style=cIsHealth>detained</style>. If you survive, obtain <style=cIsUtility>5</style> random <style=cIsHealing>Uncommon</style> items and an <style=cIsDamage>Irradiant Pearl</style>. This equipment can only be used <style=cIsHealth>ONCE</style>. If found again, the chance to <style=cIsHealth>fail</style> will increase by <style=cIsHealth>10%</style>.";

		public override string EquipmentLore => "Two void reavers come across a strange organism. A clam, seemingly native to the void itself.\nIt points to its partner, and then at the clam.\nThe partner backs away, hastily.\nThe reaver gestures to its partner again, with more intensity. It then points back at the clam.\nThe reaver's partner shakes its head.\nA pause between the two occurs.\n\n\nThe reaver strides confidently towards the clam, and wrenches its jaws open.\n\n\n\n\nThe partner is alone.";

		public override float Cooldown => 0f;

		public override bool IsLunar => true;

		public override GameObject EquipmentModel => Main.MainAssets.LoadAsset<GameObject>("VoidClamDisplay.prefab");

		public override Sprite EquipmentIcon => Main.MainAssets.LoadAsset<Sprite>("VoidClamIcon.png");

		public ItemDef GetRandomItem(List<ItemDef> Item)
		{
			int index = UnityEngine.Random.Range(0, Item.Count);
			return Item[index];
		}

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
				if (Util.CheckRoll(60f - (float)clamCounter, slot.characterBody.master))
				{
					clamCounter++;
					clamCounter++;
					clamCounter++;
					clamCounter++;
					clamCounter++;
					clamCounter++;
					clamCounter++;
					clamCounter++;
					clamCounter++;
					clamCounter++;
					int num = 1;
					int num2 = 1;
					List<PickupIndex> availableTier2DropList = Run.instance.availableTier2DropList;
					PickupDropletController.CreatePickupDroplet(availableTier2DropList[rng.RangeInt(0, availableTier2DropList.Count)], slot.characterBody.transform.position, new Vector3(UnityEngine.Random.Range(-5f, 5f), 20f, UnityEngine.Random.Range(-5f, 5f)));
					PickupDropletController.CreatePickupDroplet(availableTier2DropList[rng.RangeInt(0, availableTier2DropList.Count)], slot.characterBody.transform.position, new Vector3(UnityEngine.Random.Range(-5f, 5f), 20f, UnityEngine.Random.Range(-5f, 5f)));
					PickupDropletController.CreatePickupDroplet(availableTier2DropList[rng.RangeInt(0, availableTier2DropList.Count)], slot.characterBody.transform.position, new Vector3(UnityEngine.Random.Range(-5f, 5f), 20f, UnityEngine.Random.Range(-5f, 5f)));
					PickupDropletController.CreatePickupDroplet(availableTier2DropList[rng.RangeInt(0, availableTier2DropList.Count)], slot.characterBody.transform.position, new Vector3(UnityEngine.Random.Range(-5f, 5f), 20f, UnityEngine.Random.Range(-5f, 5f)));
					PickupDropletController.CreatePickupDroplet(availableTier2DropList[rng.RangeInt(0, availableTier2DropList.Count)], slot.characterBody.transform.position, new Vector3(UnityEngine.Random.Range(-5f, 5f), 20f, UnityEngine.Random.Range(-5f, 5f)));
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(GetRandomItem(sickitemList).itemIndex), slot.characterBody.transform.position, new Vector3(0f, 0f, 0f));
					slot.characterBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
				}
				else
				{
					Util.PlayAttackSpeedSound("Play_nullifier_death", ((Component)(object)slot.characterBody).gameObject, 0.65f);
					if (NetworkServer.active)
					{
						float num3 = slot.characterBody.damage * 10000f;
						float num4 = 65f;
						EffectManager.SpawnEffect(voidFuckEffectPrefab, new EffectData
						{
							origin = slot.characterBody.transform.position,
							scale = 65f
						}, transmit: true);
						BlastAttack blastAttack = new BlastAttack
						{
							baseDamage = 1E+37f,
							radius = 65f,
							procCoefficient = 1f,
							position = slot.characterBody.transform.position,
							attacker = ((Component)(object)slot.characterBody).gameObject,
							crit = true,
							falloffModel = BlastAttack.FalloffModel.None,
							damageType = DamageType.VoidDeath,
							teamIndex = TeamIndex.None
						};
						slot.characterBody.healthComponent.Suicide(null, null, DamageType.VoidDeath);
						blastAttack.Fire();
						blastAttack.Fire();
						blastAttack.Fire();
						blastAttack.Fire();
						blastAttack.Fire();
						slot.characterBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
					}
				}
			}
			slot.characterBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
			return true;
		}
	}
}
*/