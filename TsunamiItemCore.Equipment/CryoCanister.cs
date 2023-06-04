using System;
using BepInEx.Configuration;
using EntityStates.Mage.Weapon;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace TsunamiItemCore.Equipment {

	public class CryoCanister : EquipmentBase
	{
		public static GameObject cryoCanisterEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/AffixWhiteExplosion");

		public override string EquipmentName => "Cryo Canister";

		public override string EquipmentLangTokenName => "TSUNAMI_CRYO_CANISTER";

		public override string EquipmentPickupDesc => "Release a freezing wave.";

		public override string EquipmentFullDescription => "Release a shockwave of <style=cIsUtility>frost</style>, dealing <style=cIsDamage>500% damage</style> and <style=cIsUtility>freezing</style> enemies hit by it for <style=cIsUtility>2</style> seconds.";

		public override string EquipmentLore => "Report #22A796\nDescription: Police were called about a noise complaint from a nearby frat house at Jimbuson College. Once authorities arrived at the scene, many people were seen drinking alcohol. Additionally, marijuana was being smoked by multiple of the attendees. During a search of the house, authorities found a strange device in a cooler. The device felt extremely cold to the touch, and gloves were needed to retrieve it.\nInterview Transcript #22A796-A: Hello Michael. Now, I’m not going to beat around the bush here. You’re in some hot water, with all the underage drinking and possession of marijuana. However, we’ve received a call from the National Guard, saying that if you stated how you got the canister, they would drop all charges.\nCanister? What do you mean man?\nThe canister was about a foot tall, and it was extremely cold to the touch.\nOH! That thing? We found it on the side of the road when we were heading back to the house. We checked it out and it was super cold, so we decided to use it to cool our drinks without having to spend money on ice.\n…\nSomething wrong, officer?\nNo, Michael. I’m just a bit confused on how you were able to even grab it, as our team had to get insulated gloves to carry it.\nOh! Bill, James, Brad, and I took off our shirts and made little shirt gloves to protect our hands. Still was colder than ice though.\nHrm. Okay, I think we’re done here.";

		public override float Cooldown => 20f;

		public override GameObject EquipmentModel => Main.MainAssets.LoadAsset<GameObject>("CryoCanisterDisplay.prefab");

		public override Sprite EquipmentIcon => Main.MainAssets.LoadAsset<Sprite>("CryoCanisterIcon.png");

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
			Util.PlayAttackSpeedSound(IceNova.attackString, ((Component)(object)slot.characterBody).gameObject, slot.characterBody.attackSpeed);
			if (NetworkServer.active)
			{
				float baseDamage = slot.characterBody.damage * 4f;
				float num = 25f;
				EffectManager.SpawnEffect(cryoCanisterEffectPrefab, new EffectData
				{
					origin = slot.characterBody.transform.position,
					scale = num
				}, transmit: true);
				BlastAttack blastAttack = new BlastAttack
				{
					baseDamage = baseDamage,
					radius = num,
					procCoefficient = 1f,
					position = slot.characterBody.transform.position,
					attacker = ((Component)(object)slot.characterBody).gameObject,
					crit = Util.CheckRoll(slot.characterBody.crit, slot.characterBody.master),
					falloffModel = BlastAttack.FalloffModel.None,
					damageType = DamageType.Freeze2s,
					teamIndex = TeamComponent.GetObjectTeam(((Component)(object)slot.characterBody).gameObject)
				};
				blastAttack.Fire();
			}
			return true;
		}
	}
}
