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

	public class DoctorsBag : ItemBase<DoctorsBag>
	{
		public static float shieldArmor;

		public override string ItemName => "Doctor’s Bag";

		public override string ItemLangTokenName => "TSUNAMI_DOCTORS_BAG";

		public override string ItemPickupDesc => "Gain a chance to double incoming healing.";

		public override string ItemFullDescription => "Gain a <style=cIsUtility>10%</style><style=cStack> (+10% per stack)</style> to '<style=cIsHealing>Critically Heal</style>', doubling your <style=cIsHealing>healing</style> received.";

		public override string ItemLore => "Gunfire rains heavy as the man lets out a cry of pain.\n''I'm hurt bad out here!''\nThe well armored man hastily retreated back into the vault, with his LMG hanging loosely off his back.\n''Damn it- how many bags do we got left?''\nWet spots are starting to appear on the masked man’s armor.\n''Got enough for another man, get over here Daryl!''\nThe last few needles and bandages clinked inside the case, whilst the symphony of gunfire rings outside the bank vault.\nThe man takes the entire case, and starts to frantically use everything that was left. Bandaging, methadone, everything.\nSoon after, he feels his vigor returning, his will to keep going, to finish off this disaster of a heist.\n''…''\n''Much better.''";

		public override ItemTier Tier => ItemTier.Tier1;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Healing };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("DoctorsBagDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("DoctorsBagIcon.png");

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
			On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
		}

		private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
		{
			int count = GetCount(self.body);
			if (self && self.body && self.body.inventory && GetCount(self.body) > 0)
			{
                if (Util.CheckRoll(10f * (float)count, self.body.master))
                {
					amount *= 2f;
                }
			}
			return orig(self, amount, procChainMask, nonRegen);
		}

		/*private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			orig.Invoke(self);
			int count = GetCount(self);
			self.critHeal += count * 10f;
		}*/
	}
}
