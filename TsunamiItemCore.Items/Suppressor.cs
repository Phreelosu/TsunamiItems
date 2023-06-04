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

	public class Suppressor : ItemBase<Suppressor>
	{
		public float shieldArmor;

		public override string ItemName => "Suppressor";

		public override string ItemLangTokenName => "TSUNAMI_SUPPRESSOR";

		public override string ItemPickupDesc => "Killing an elite or boss grants a temporary speed, damage, and regen boost.";

		public override string ItemFullDescription => "Upon killing an <style=cIsUtility>elite</style> or <style=cIsUtility>boss</style>, increase <style=cIsHealing>regen</style>, <style=cIsUtility>movement speed</style>, and <style=cIsUtility>damage</style> by 45% for 5 <style=cStack>(+2.5 per stack)</style> seconds for elites and 10 <style=cStack>(+5 per stack)</style>.";

		public override string ItemLore => "It’ll be fine, it should be quick, quiet, and clean.\n<style=cMono>You do realize that there’s more to stealth than not being audible, correct?</style>\nLook man, have some faith in me, you know my quality of work! You know how I roll.\n<style=cMono>Yes, but you do know who you’re going up against here. You are absolutely mentally damaged if if you think this will be as simple and easy as you appear to.</style>\nListen, putting more doubt in myself is the last thing I need right now. I know I’m probably gonna have some complications here, that’s a given, I’m not stupid.\n<style=cMono>Then why agree to it if you understand the risk?</style>\nHave you SEEN how much they’re paying me for this one job? You know how rough it’s been for me, I don’t know when I’ll next have an opportunity like this. I've got one shot at this, and if I don't take it, you know i'll never forgive myself for it.\n<style=cMono>Money. Hm. That’s just like you.</style>\nI don’t need you judging me, I know it’s worth the risk.\n<style=cMono>...Please stay safe out there. And a word of advice, stay on full alert at all times. That... rather ugly little thing may muffle the sound of your gun but it will not keep you from being seen.</style>";

		public override ItemTier Tier => ItemTier.Tier2;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Damage };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("SuppressorDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("SuppressorIcon.png");

		public BuffDef SuppressorBuff { get; private set; }

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
			SuppressorBuff = ScriptableObject.CreateInstance<BuffDef>();
			SuppressorBuff.name = "SuppressorBoostBuff";
			SuppressorBuff.buffColor = Color.black;
			SuppressorBuff.canStack = false;
			SuppressorBuff.isDebuff = false;
			SuppressorBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Cloak").iconSprite;
			ContentAddition.AddBuffDef(SuppressorBuff);
		}

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
			RecalculateStatsAPI.GetStatCoefficients += new RecalculateStatsAPI.StatHookEventHandler(AddSuppressorBuff);
			On.RoR2.GlobalEventManager.OnCharacterDeath += new On.RoR2.GlobalEventManager.hook_OnCharacterDeath(OnKillStuff);
			On.RoR2.GlobalEventManager.OnCharacterDeath += new On.RoR2.GlobalEventManager.hook_OnCharacterDeath(OnKillStuff2);
		}

		private void OnKillStuff(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
		{
			orig.Invoke(self, damageReport);
			int count = GetCount(damageReport.attackerBody);
			CharacterBody victimBody = damageReport.victimBody;
			CharacterBody attackerBody = damageReport.attackerBody;
			CharacterMaster attackerMaster = damageReport.attackerMaster;
			if (count > 0 && victimBody.isElite && (bool)(UnityEngine.Object)(object)attackerBody && (bool)(UnityEngine.Object)(object)attackerMaster)
			{
				ItemHelpers.RefreshTimedBuffs(attackerBody, SuppressorBuff, 5f + 2.5f * (float)(count - 1));
				attackerBody.AddTimedBuffAuthority(SuppressorBuff.buffIndex, 5f + 2.5f * (float)(count - 1));
				ItemHelpers.RefreshTimedBuffs(attackerBody, SuppressorBuff, 5f + 2.5f * (float)(count - 1));
			}
		}

		private void OnKillStuff2(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
		{
			orig.Invoke(self, damageReport);
			int count = GetCount(damageReport.attackerBody);
			CharacterBody victimBody = damageReport.victimBody;
			CharacterBody attackerBody = damageReport.attackerBody;
			CharacterMaster attackerMaster = damageReport.attackerMaster;
			if (count > 0 && victimBody.isChampion && (bool)(UnityEngine.Object)(object)attackerBody && (bool)(UnityEngine.Object)(object)attackerMaster)
			{
				ItemHelpers.RefreshTimedBuffs(attackerBody, SuppressorBuff, 10f + 5f * (float)(count - 1));
				attackerBody.AddTimedBuffAuthority(SuppressorBuff.buffIndex, 10f + 5f * (float)(count - 1));
				ItemHelpers.RefreshTimedBuffs(attackerBody, SuppressorBuff, 10f + 5f * (float)(count - 1));
			}
		}

		private void AddSuppressorBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			int count = GetCount(sender);
			if (sender.HasBuff(SuppressorBuff))
			{
				args.damageMultAdd += 0.45f * (float)sender.GetBuffCount(SuppressorBuff) + 1f * (float)sender.GetBuffCount(SuppressorBuff) * (float)(count - 1);
				args.moveSpeedMultAdd += 0.45f * (float)sender.GetBuffCount(SuppressorBuff) + 1f * (float)sender.GetBuffCount(SuppressorBuff) * (float)(count - 1);
				args.regenMultAdd += 5f * (float)sender.GetBuffCount(SuppressorBuff) + 1f * (float)sender.GetBuffCount(SuppressorBuff) * (float)(count - 1);
			}
		}
	}
}
