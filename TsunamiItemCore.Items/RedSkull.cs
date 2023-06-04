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

	public class RedSkull : ItemBase<RedSkull>
	{
		public static float shieldArmor;

		internal static float maxHealthThreshold = 0.5f;

		public override string ItemName => "Red Skull";

		public override string ItemLangTokenName => "TSUNAMI_RED_SKULL";

		public override string ItemPickupDesc => "Gain extra damage at half health.";

		public override string ItemFullDescription => "While under <style=cIsUtility>50%</style> <style=cIsHealth>health</style>, deal <style=cIsDamage>30%</style><style=cStack> (+10% per stack)</style> more <style=cIsDamage>damage</style>.";

		public override string ItemLore => "Nothing is more exhilarating then being backed into the corner like a wild animal. It’s the most terrifying thing you’ll ever feel, but at the same time, it’s freeing. There’s nothing else to do, nowhere to run. Your enemy has made themselves the <style=cIsDamage>path of least resistance</style>.\n\nThese words pounded in Je-ton’s mind as he fought for his freedom.\nThey stripped him of everything he had: his dignity, his pride, even the clothes off his back. However, no matter how hard they beat him, no matter how long they worked him, no matter how brutally they treated him, they would not take his mind. And when they finally got him on the ropes, cornered, he stood tall.\nHe had won.\nFor they were the <style=cIsDamage>path of least resistance</style>.";

		public override ItemTier Tier => ItemTier.Tier2;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Damage };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("RedSkullDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("RedSkullIcon.png");

		public BuffDef SkullBuff { get; private set; }

		public BuffIndex SkullBuffIndex { get; private set; }

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
			SkullBuff = ScriptableObject.CreateInstance<BuffDef>();
			SkullBuff.name = "SkullBuff";
			SkullBuff.buffColor = new Color(0.75f, 0f, 0f);
			SkullBuff.canStack = false;
			SkullBuff.isDebuff = false;
			SkullBuff.iconSprite = Resources.Load<Sprite>("textures/bufficons/texbuffbanditskullicon");
			SkullBuff.buffIndex = SkullBuffIndex;
			ContentAddition.AddBuffDef(SkullBuff);
		}

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
			RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
		}

		private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (GetCount(sender) > 0)
            {
				var hc = sender.GetComponent<HealthComponent>();
				var lessThanHalf = hc.fullHealth / 2 >= hc.combinedHealth;
				if ((sender.healthComponent.health + sender.healthComponent.shield) / sender.healthComponent.fullCombinedHealth < maxHealthThreshold)
                {
					args.damageMultAdd += 0.3f + ((GetCount(sender) - 1) * 0.1f);
                }
            }
        }

		/*private void On_HCTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo di)
        {
            if (di == null || di.rejected || di.attacker.GetComponent<HealthComponent>() == null || di.attacker.GetComponent<HealthComponent>().body == null)
            {
				orig(self, di);
				return;
            }
			var body = self.body;
			var hc = di.attacker.GetComponent<HealthComponent>();
			var lessThanHalf = hc.fullHealth / 2 >= hc.combinedHealth;
			var inv = hc.body.inventory;

			if (inv)
			{
				int rsCount = GetCount(body);
				if (rsCount == 1 && lessThanHalf)
				{
					di.damage *= 1f + (0.3f * rsCount);
				}
                if (rsCount > 1 && lessThanHalf)
                {
					di.damage *= 1f + 0.3f + (0.1f * rsCount);
                }
			}
			orig(self, di);
		}*/
	}
}
