using BepInEx.Configuration;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RoR2.Achievements;
using HarmonyLib;
using On.RoR2.Items;
using TsunamiItemCore.Utils;
using UnityEngine.Networking;

namespace TsunamiItemCore.Items {

	public class ObsidianFragment : ItemBase<ObsidianFragment>
	{
		public static float shieldArmor;

		public override string ItemName => "Obsidian Fragment";

		public override string ItemLangTokenName => "TSUNAMI_OBSIDIAN_FRAGMENT";

		public override string ItemPickupDesc => "Gain a chance to burn enemies. Attacks against burning enemies increase crit chance.";

		public override string ItemFullDescription => "Gain a 10% chance to <style=cIsDamage>ignite</style> enemies on hit for <style=cIsUtility>3</style><style=cStack> (+1 per stack)</style> seconds. Attacks against <style=cIsDamage>ignited</style> enemies give you a buff that increases your <style=cIsDamage>critical chance</style> by <style=cIsUtility>20%</style><style=cStack> (+10% per stack)</style>.";

		public override string ItemLore => "Order: Unexplainable Volcanic Fragment\nTracking Number: 119**\nEstimated Delivery: 9/24/2056\nShipping Method: Fragile\nShipping Address: High Court Research Institute, Tranquility Base, Sunstorm Quadrant\nShipping Details:\n\nOur guys found a whole bunch of these about 20 kilometers under Olympus Mons. Real pretty glowing chunks of obsidian. We tried cracking them open to see what gave 'em their glow, expecting to find magma inside or… something. We really can't figure these out - the heat they're putting off doesn't seem to die out at all. We even tried dipping a few in hydrogen, and while they cooled temporarily, they reheated within the hour.\n\nCould potentially revolutionize energy generation. This is just a small bit of what we've found of 'em this far; overall we've dug up about two to three tons of these, and are estimating it's just the tip of the iceberg. Thanks for the help. Keep pushing humanity forward.";

		public override ItemTier Tier => ItemTier.Tier2;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Damage };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("ObsidianShardDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("ObsidianShardIcon.png");

		public RoR2.BuffDef ObsidianBuff { get; private set; }

		public override void Init(ConfigFile config)
		{
			CreateConfig(config);
			CreateLang();
			CreateBuff();
			CreateItem();
			Hooks();
		}

		public override void CreateConfig(ConfigFile config)
		{
			shieldArmor = config.Bind("Item: " + ItemName, "Armor Per Shield", 5f, "armor added per shield.").Value;
		}

		private void CreateBuff()
		{
			ObsidianBuff = ScriptableObject.CreateInstance<RoR2.BuffDef>();
			ObsidianBuff.name = "ObsidianCritBuff";
			ObsidianBuff.buffColor = new Color(0.25f, 0f, 0.25f);
			ObsidianBuff.canStack = false;
			ObsidianBuff.isDebuff = false;
			ObsidianBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/OnFire").iconSprite;
			ContentAddition.AddBuffDef(ObsidianBuff);
		}

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
			RecalculateStatsAPI.GetStatCoefficients += new RecalculateStatsAPI.StatHookEventHandler(AddObsidianBuff);
			On.RoR2.GlobalEventManager.OnHitEnemy += new On.RoR2.GlobalEventManager.hook_OnHitEnemy(BurnOnHit);
			On.RoR2.GlobalEventManager.OnHitEnemy += new On.RoR2.GlobalEventManager.hook_OnHitEnemy(CritBurnOnHit);
		}

		private void BurnOnHit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
		{
			orig.Invoke(self, damageInfo, victim);
			if (!damageInfo.attacker || !(damageInfo.procCoefficient > 0f))
			{
				return;
			}
			CharacterBody component = damageInfo.attacker.GetComponent<CharacterBody>();
			CharacterBody characterBody = (victim ? victim.GetComponent<CharacterBody>() : null);
			if (!(UnityEngine.Object)(object)component || !(UnityEngine.Object)(object)characterBody)
			{
				return;
			}
			int count = GetCount(component);
			if (count > 0)
			{
				var dotInfo = new InflictDotInfo()
				{
					attackerObject = component.gameObject,
					victimObject = characterBody.gameObject,
					dotIndex = DotController.DotIndex.Burn,
					duration = damageInfo.procCoefficient * 3f + (float)count,
					damageMultiplier = count * 1f
				};
				float num = 10f;
				float num2 = Mathf.Sqrt(damageInfo.procCoefficient);
				int num3 = Mathf.FloorToInt(num * num2 / 100f);
				for (int i = 0; i < num3; i++)
				{
					StrengthenBurnUtils.CheckDotForUpgrade(component.inventory, ref dotInfo);
					DotController.InflictDot(ref dotInfo);
				}
				if (Util.CheckRoll(num * num2 - (float)(num3 * 100), component.master))
				{
					StrengthenBurnUtils.CheckDotForUpgrade(component.inventory, ref dotInfo);
					DotController.InflictDot(ref dotInfo);
				}
			}
		}

		private void CritBurnOnHit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
		{
			GameObject attacker = damageInfo.attacker;
			if ((bool)attacker)
			{
				CharacterBody component = attacker.GetComponent<CharacterBody>();
				CharacterBody component2 = victim.GetComponent<CharacterBody>();
				if ((bool)(UnityEngine.Object)(object)component && (bool)(UnityEngine.Object)(object)component2)
				{
					int count = GetCount(component);
					if (count > 0 && component2.HasBuff(RoR2Content.Buffs.OnFire))
					{
						component.AddTimedBuff(ObsidianBuff, 2.5f);
					}
				}
			}
			orig.Invoke(self, damageInfo, victim);
		}

		private void AddObsidianBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			int count = GetCount(sender);
			if (sender.HasBuff(ObsidianBuff))
			{
				args.critAdd += 20f + 10f * (float)(count - 1);
			}
		}
	}
}
