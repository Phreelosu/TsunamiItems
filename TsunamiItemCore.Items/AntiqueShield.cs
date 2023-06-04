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

namespace TsunamiItemCore.Items {

	public class AntiqueShield : ItemBase<AntiqueShield>
	{
		public static float shieldArmor;

		public override string ItemName => "Antique Shield";

		public override string ItemLangTokenName => "TSUNAMI_ANTIQUE_SHIELD";

		public override string ItemPickupDesc => "Gain extra armor.";

		public override string ItemFullDescription => "Gain an extra <style=cIsUtility>5 armor</style> <style=cStack>(+5 per stack)</style>.";

		public override string ItemLore => "Man, this work is backbreaking.\nYou could say that again, pal. I mean what the hell are we doing out here? We’ve barely found anything even remotely interesting!\nWhat about this old shield?\nHuh? What shield? Lemme take a look.\n... \nThat's... <style=cIsHealth>suspicious</style>.\nWhat do you mean? How is it <style=cIsHealth>suspicious</style>? It’s just a shield, man.\nI don’t know, it’s just… it just is. I can’t help but be filled with this overwhelming sense in my mind that it’s fully and absolutely <style=cIsHealth>suspicious</style>, and have this strong urge to make it known.\nYou feeling alright man? You get a good sleep last night?\nGet rid of it.\nWhat?\nThat shield, it’s dangerous. It was left here for a reason. We need to destroy it.\nDude calm the hell down, it’s just a shield.";

		public override ItemTier Tier => ItemTier.Tier1;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Utility };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("AntiqueShieldDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("AntiqueShieldIcon.png");

		public override void Init(ConfigFile config)
		{
			CreateConfig(config);
			CreateLang();
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
			On.RoR2.CharacterBody.RecalculateStats += new On.RoR2.CharacterBody.hook_RecalculateStats(CharacterBody_RecalculateStats);
		}

		private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			orig.Invoke(self);
			int count = GetCount(self);
			self.armor += (float)count * shieldArmor;
		}
	}
}