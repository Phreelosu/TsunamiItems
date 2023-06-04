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

	public class TacticiansManual : ItemBase<TacticiansManual>
	{
		public static float shieldArmor;

		public override string ItemName => "Tactician’s Manual";

		public override string ItemLangTokenName => "TSUNAMI_TACTICIANS_MANUAL";

		public override string ItemPickupDesc => "Empower your next strike.";

		public override string ItemFullDescription => "Your <style=cIsDamage>next strike</style> deals <style=cIsDamage>20%</style><style=cStack> (+20% per stack)</style> more <style=cIsDamage>damage</style>. Has a cooldown of <style=cIsUtility>10</style> seconds.";

		public override string ItemLore => "<style=cMono>[Excerpt from Chapter 7, page 105]</style>\n\n\n\n...So when you have a huge cluster of guys in your way, you wanna try and blow them up in a way where the explosion’s right about in the middle, or, centered on the most dangerous target. That way you can get the most bang for your buck. Make sure that upon detonation, you have anything you’d like to stay clean out of the splash zone. If you do it right, you should be left with nothing but red rain and a few bodies, it’s as simple as that.\n\nAnd for the record, just so you know I’m not pulling your leg, I can name several times I’ve even used the environment to my advantage. For example, there was the time I was on this planet that was practically coated in ice, so I blew a hole in the ice under these creatures and they fell in and drowned, simple as that!\n\nThere was also that time I blew an entire chunk off of a mountain and caused an avalanche made of these chumps.\n\nThe sky’s the limit when it comes to explosives, so make sure you get creative with how you blow stuff up. And make sure to have some fun with it!\n\n- Elizabeth Richard's Tactician's Manual";

		public override ItemTier Tier => ItemTier.Tier1;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Damage };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("TacticiansManualDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("TacticiansManualIcon.png");

		public BuffDef TacticianBuff { get; private set; }

		public BuffDef TacticianDebuff { get; private set; }

		public override void Init(ConfigFile config)
		{
			CreateConfig(config);
			CreateBuffs();
			CreateLang();
			CreateItem();
			Hooks();
		}

		public override void CreateConfig(ConfigFile config)
		{
			shieldArmor = config.Bind("Item: " + ItemName, "Armor Per Shield", 5f, "armor added per shield.").Value;
		}

		private void CreateBuffs()
		{
			TacticianBuff = ScriptableObject.CreateInstance<BuffDef>();
			TacticianBuff.name = "ManualReadyBuff";
			TacticianBuff.buffColor = Color.red;
			TacticianBuff.canStack = false;
			TacticianBuff.isDebuff = false;
			TacticianBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/FullCrit").iconSprite;
			ContentAddition.AddBuffDef(TacticianBuff);
			TacticianDebuff = ScriptableObject.CreateInstance<BuffDef>();
			TacticianDebuff.name = "ManualCooldownDebuff";
			TacticianDebuff.buffColor = Color.black;
			TacticianDebuff.canStack = false;
			TacticianDebuff.isDebuff = false;
			TacticianDebuff.isCooldown = true;
			TacticianDebuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/FullCrit").iconSprite;
			ContentAddition.AddBuffDef(TacticianDebuff);
		}

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
			On.RoR2.CharacterBody.OnInventoryChanged += new On.RoR2.CharacterBody.hook_OnInventoryChanged(AddItemBehavior);
			RecalculateStatsAPI.GetStatCoefficients += new RecalculateStatsAPI.StatHookEventHandler(AddManualBuff);
			On.RoR2.GlobalEventManager.OnHitEnemy += new On.RoR2.GlobalEventManager.hook_OnHitEnemy(TacticianCheck);
		}

		private void AddItemBehavior(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
		{
			orig.Invoke(self);
			if (NetworkServer.active)
			{
				self.AddItemBehavior<ManualBehavior>(GetCount(self));
			}
		}

		private void TacticianCheck(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
		{
			GameObject attacker = damageInfo.attacker;
			if ((bool)attacker)
			{
				CharacterBody component = attacker.GetComponent<CharacterBody>();
				CharacterBody component2 = victim.GetComponent<CharacterBody>();
				if ((bool)(UnityEngine.Object)(object)component && (bool)(UnityEngine.Object)(object)component2)
				{
					int count = GetCount(component);
					if (count > 0 && component.HasBuff(TacticianBuff))
					{
						component.AddTimedBuff(TacticianDebuff, 10f);
					}
				}
			}
			orig.Invoke(self, damageInfo, victim);
		}

		private void AddManualBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			int count = GetCount(sender);
			if (sender.HasBuff(TacticianBuff))
			{
				args.damageMultAdd += 0.2f * (float)count;
			}
		}
	}
}
