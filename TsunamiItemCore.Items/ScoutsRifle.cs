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

	public class ScoutsRifle : ItemBase<ScoutsRifle>
	{
		public static float shieldArmor;

		public override string ItemName => "Scout’s Rifle";

		public override string ItemLangTokenName => "TSUNAMI_SCOUT_RIFLE";

		public override string ItemPickupDesc => "Gain a speed boost upon entering a stage.";

		public override string ItemFullDescription => "Entering a new stage will boost your <style=cIsUtility>speed</style> by <style=cIsUtility>30%</style> for <style=cIsUtility>20</style><style=cStack> (+5 per stack)</style> seconds.";

		public override string ItemLore => "I'm not even winded!";

		public override ItemTier Tier => ItemTier.Tier1;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Utility };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("ScoutsRifleDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("ScoutsRifleIcon.png");

		public BuffDef ScoutBuff { get; private set; }

		private void CreateBuff()
		{
			ScoutBuff = ScriptableObject.CreateInstance<BuffDef>();
			ScoutBuff.name = "ScoutSpeedBuff";
			ScoutBuff.buffColor = new Color(0.5f, 0f, 0.7f);
			ScoutBuff.canStack = false;
			ScoutBuff.isDebuff = false;
			ScoutBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite;
			ContentAddition.AddBuffDef(ScoutBuff);
		}

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

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
			On.RoR2.CharacterBody.Start += new On.RoR2.CharacterBody.hook_Start(ScoutStart);
			RecalculateStatsAPI.GetStatCoefficients += new RecalculateStatsAPI.StatHookEventHandler(AddScoutBuff);
		}

		public void ScoutStart(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
		{
			orig.Invoke(self);
			if (GetCount(self) > 0)
			{
				self.AddTimedBuff(ScoutBuff, 20 + (5 * GetCount(self) - 1));
			}
		}

		private void AddScoutBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			int count = GetCount(sender);
			if (sender.HasBuff(ScoutBuff))
			{
				args.moveSpeedMultAdd += 0.3f;
			}
		}
	}
}
