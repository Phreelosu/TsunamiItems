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

	public class AshSandwich : ItemBase<AshSandwich>
	{
		public static float shieldArmor;

		public override string ItemName => "Sandwich For Ash";

		public override string ItemLangTokenName => "TSUNAMI_ASH_SANDWICH";

		public override string ItemPickupDesc => "Killing enemies grant a stacking heal consumed by using Special.";

		public override string ItemFullDescription => "Killing an enemy will grant you a stacking buff. Upon using your <style=cIsUtility>Special</style>, <style=cIsHealing>heal</style> for <style=cIsHealing>4</style><style=cStack> (+4 per stack)</style> <style=cIsHealing>health</style> multiplied by <style=cIsUtility>every stack</style> of the buff you have.";

		public override string ItemLore => "Sounds of keys clacking and occasional groans can be heard outside of the door. A man enters the room and speaks to the diligent woman on the computer.\n''Hey Ash, whatcha doing?''\n''Coding.''\n''Coding…?''\n''Coding... something.''\n''Uh, right. Well...''\n\nThe sound of a plate being slid onto her desk can be heard. A rather filled sandwich is located on the plate.\n\n''Here. Keep up the good work.''\n''Oh, nice! Thanks man, I was getting a little peckish.''\n\nThe sounds of clacking has been temporarily replaced with the sound of Ash scarfing the sandwich down.\n\n''Yeah no problem, might as well return the favor.''\n''Oh, for that little thing I did for you back then? It was only a kind gesture.''\n''And for that, you get a sandwich! Good luck on your nerd stuff, and thanks for your work.''\n\nThe man gives a quick thumbs up before exiting the room.";

		public override ItemTier Tier => ItemTier.Tier2;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Healing };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("SandwichForAshDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("SandwichForAshIcon.png");

		public BuffDef SandwichBuff { get; private set; }

		public BuffIndex SandwichBuffIndex { get; private set; }

		private void CreateBuff()
		{
			SandwichBuff = ScriptableObject.CreateInstance<BuffDef>();
			SandwichBuff.name = "SandwichHealBuff";
			SandwichBuff.buffColor = new Color(0f, 0.5f, 0f);
			SandwichBuff.canStack = true;
			SandwichBuff.isDebuff = false;
			SandwichBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/AttackSpeedOnCrit").iconSprite;
			SandwichBuff.buffIndex = SandwichBuffIndex;
			ContentAddition.AddBuffDef(SandwichBuff);
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
			On.RoR2.GlobalEventManager.OnCharacterDeath += new On.RoR2.GlobalEventManager.hook_OnCharacterDeath(OnKillStuff);
			On.RoR2.CharacterBody.OnSkillActivated += new On.RoR2.CharacterBody.hook_OnSkillActivated(DoSandwichTask);
		}

		private void OnKillStuff(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
		{
			orig.Invoke(self, damageReport);
			CharacterBody victimBody = damageReport.victimBody;
			CharacterBody attackerBody = damageReport.attackerBody;
			if (!victimBody.healthComponent.alive)
			{
				int count = GetCount(attackerBody);
				if (count > 0)
				{
					ItemHelpers.RefreshTimedBuffs(attackerBody, SandwichBuff, 60f);
					attackerBody.AddTimedBuffAuthority(SandwichBuff.buffIndex, 60f);
					ItemHelpers.RefreshTimedBuffs(attackerBody, SandwichBuff, 60f);
				}
			}
		}

		private void DoSandwichTask(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
		{
			orig.Invoke(self, skill);
			int count = GetCount(self);
			bool flag;
			if (count > 0)
			{
				SkillLocator skillLocator = self.skillLocator;
				flag = (((UnityEngine.Object)(object)skillLocator != null) ? skillLocator.special : null) == skill;
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				if (NetworkServer.active)
				{
					self.healthComponent.Heal(4 * self.GetBuffCount(SandwichBuff) * count, default(ProcChainMask));
					self.ClearTimedBuffs(SandwichBuff);
				}
				self.ClearTimedBuffs(SandwichBuff);
			}
		}
	}
}
