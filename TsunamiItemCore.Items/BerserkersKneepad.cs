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

	public class BerserkersKneepad : ItemBase<BerserkersKneepad>
	{
		public static float shieldArmor;

		public override string ItemName => "Berzerker’s Kneepad";

		public override string ItemLangTokenName => "TSUNAMI_BERZERKERS_KNEEPAD";

		public override string ItemPickupDesc => "Go berserk upon entering combat.";

		public override string ItemFullDescription => "Upon entering combat, enter a battle frenzy for 3 seconds, dealing <style=cIsDamage>10%</style><style=cStack> (+10% per stack)</style> more <style=cIsDamage>damage</style>, moving <style=cIsUtility>20%</style><style=cStack> (+10% per stack)</style> <style=cIsUtility>faster</style>, and attacking <style=cIsUtility>20%</style><style=cStack> (+10% per stack)</style> faster. Recharges every <style=cIsUtility>13</style> seconds.";

		public override string ItemLore => "THEY GON RUMBLE\nTHEY GON TAKE YO FACE OFF\nIT'S ABOUT RICE, IT'S ABOUT FLOUR\nYOU STAY HUNGRY, I DEVOUR";

		public override ItemTier Tier => ItemTier.Tier1;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Utility };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("BerzerkersKneepadDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("BerserkersKneepad.png");

		public BuffDef BerserkBuff { get; private set; }

		public BuffDef BerserkDebuff { get; private set; }

		public BuffIndex BerserkDebuffIndex { get; private set; }

		public BuffIndex BerserkBuffIndex { get; private set; }

		public override void Init(ConfigFile config)
		{
			CreateConfig(config);
			CreateLang();
			CreateBuffs();
			CreateItem();
			Hooks();
		}

		public override void CreateConfig(ConfigFile config)
		{
			shieldArmor = config.Bind("Item: " + ItemName, "Armor Per Shield", 5f, "armor added per shield.").Value;
		}

		private void CreateBuffs()
		{
			BerserkBuff = ScriptableObject.CreateInstance<BuffDef>();
			BerserkBuff.name = "Berserk Buff";
			BerserkBuff.buffColor = new Color(0.5f, 0f, 0f);
			BerserkBuff.canStack = false;
			BerserkBuff.isDebuff = false;
			BerserkBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Cripple").iconSprite;
			BerserkBuff.buffIndex = BerserkBuffIndex;
			ContentAddition.AddBuffDef(BerserkBuff);
			BerserkDebuff = ScriptableObject.CreateInstance<BuffDef>();
			BerserkDebuff.name = "Berserk Cooldown";
			BerserkDebuff.buffColor = Color.gray;
			BerserkDebuff.canStack = false;
			BerserkDebuff.isDebuff = false;
			BerserkDebuff.isCooldown = true;
			BerserkDebuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Cripple").iconSprite;
			BerserkDebuff.buffIndex = BerserkDebuffIndex;
			ContentAddition.AddBuffDef(BerserkDebuff);
		}

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
			On.RoR2.CharacterBody.OnSkillActivated += new On.RoR2.CharacterBody.hook_OnSkillActivated(GoBananas);
			RecalculateStatsAPI.GetStatCoefficients += new RecalculateStatsAPI.StatHookEventHandler(AddBananasBuff);
		}

		private void GoBananas(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
		{
			orig.Invoke(self, skill);
			int count = GetCount(self);
			CharacterBody component = ((Component)(object)self).GetComponent<CharacterBody>();
			if (count > 0 && !component.HasBuff(BerserkDebuff))
			{
				component.AddTimedBuff(BerserkDebuff, 13f);
				component.AddTimedBuff(BerserkBuff, 3f);
			}
		}

		private void AddBananasBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			int count = GetCount(sender);
			if (sender.HasBuff(BerserkBuff))
			{
				args.damageMultAdd += 0.1f * (float)count;
				args.attackSpeedMultAdd += 0.1f + 0.1f * (float)count;
				args.moveSpeedMultAdd += 0.1f + 0.1f * (float)count;
			}
		}
	}
}
