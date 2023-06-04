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

	public class ForeignFlower : ItemBase<ForeignFlower>
	{
		public static float shieldArmor;

		public override string ItemName => "Foreign Flower";

		public override string ItemLangTokenName => "TSUNAMI_FOREIGN_FLOWER";

		public override string ItemPickupDesc => "Regenerate upon using your equipment.";

		public override string ItemFullDescription => "Upon using your <style=cIsUtility>equipment</style>, increase your <style=cIsHealing>regen</style> heavily for <style=cIsUtility>3</style><style=cStack> (+1 per stack)</style> seconds.";

		public override string ItemLore => "A Nutapple's bloom may not be as nutritious as it's fully grown fruit form, but it's unique herbal flavor and lighter properties still lead it to have it's own vague yet distinct flavor. Among Titan's other natural trees including the Delliux, Bezacon, Lexaquill, and the Aacororo, the Xelphum Tree is easily the most popular.\n\n- Dietary Sciences Digest";

		public override ItemTier Tier => ItemTier.Tier1;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Healing };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("ForeignFlowerDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("ForeignFlowerIcon.png");

		public BuffDef FlowerBuff { get; private set; }

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
			FlowerBuff = ScriptableObject.CreateInstance<BuffDef>();
			FlowerBuff.name = "FlowerRegenBuff";
			FlowerBuff.buffColor = Color.green;
			FlowerBuff.canStack = false;
			FlowerBuff.isDebuff = false;
			FlowerBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Fruiting").iconSprite;
			ContentAddition.AddBuffDef(FlowerBuff);
		}

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
			On.RoR2.EquipmentSlot.PerformEquipmentAction += new On.RoR2.EquipmentSlot.hook_PerformEquipmentAction(EquipmentSlot_PerformEquipmentAction);
			RecalculateStatsAPI.GetStatCoefficients += new RecalculateStatsAPI.StatHookEventHandler(AddFlowerBuff);
		}

		private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
		{
			bool flag = orig.Invoke(self, equipmentDef);
			if (flag && (bool)(UnityEngine.Object)(object)self)
			{
				CharacterBody characterBody = self.characterBody;
				if ((bool)(UnityEngine.Object)(object)characterBody)
				{
					int count = GetCount(characterBody);
					if (count > 0)
					{
						characterBody.AddTimedBuff(FlowerBuff.buffIndex, 3f + 1f * (float)(count - 1));
					}
				}
			}
			return flag;
		}

		private void AddFlowerBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			int count = GetCount(sender);
			if (sender.HasBuff(FlowerBuff))
			{
				args.regenMultAdd += 7f * (float)sender.GetBuffCount(FlowerBuff);
			}
		}
	}
}
