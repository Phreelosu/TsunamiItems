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

	public class IonicCola : ItemBase<IonicCola>
	{
		public static float shieldArmor;

		public override string ItemName => "Ionic Cola";

		public override string ItemLangTokenName => "TSUNAMI_IONIC_COLA";

		public override string ItemPickupDesc => "Increase attack speed and movement speed. Kills grant a stacking attack speed and movement speed boost.";

		public override string ItemFullDescription => "Increase <style=cIsUtility>movement and attack speed by 50%</style><style=cStack> (+50% per stack)</style>. Killing an enemy will give you a stack of <style=cIsUtility>Sugar Rush</style>, which temporarily increases <style=cIsUtility>attack speed and movement speed</style> by <style=cIsUtility>10%</style><style=cStack> (+10% per stack)</style>.";

		public override string ItemLore => "HOW MUCH!?\nThe showroom around them quieted. The two businessmen stood just off the center of the room, one delivering the most dumbfounded gaze of his life, the other simply standing, their hands folded together. Around them, the buzz of the room dimmed, distant conversation and networking becoming ambience. The other one held their hand out, and signaled him over to walk. Breaking from his trance, the man stepped alongside him, with the waves of chatter returning to the floor as they left.\n<style=cMono>Clarification: 25 Credits</style>\nLet me get this straight. You want to sell this, ungodly concoction of jetfuel, energy drinks, PLURAL, and nuclear fissile material, for 25 credits a pop!?\n<style=cMono>Such Materials Are Highly Subsidized By Several UES Departments</style>\nI know, I know, but, god, listen. You and I both know how things work around here. Regulations are more like suggestions, laws are more like guidelines, blah blah. But who in their right mind is going to BUY this crap? Engineers? This stuff is so radically impure and mixed to hell that no being, organic or otherwise, could consume it without having their component parts break down in mere hours!\n<style=cMono>Energizing Colas Are Highly Popular These Days, You Know</style>\nI KNOW, THEY'RE POPULAR. Everyone knows this, I know you're just in on the cash grab, but come ON, this has to be the lowest form of cash grab ever conceived! This isn't just snake oil, it's Ifrit blood! This has no right to exist, and you're selling liquified death for the same price of a standard storage container! I ask again, who in their right, sane mind would EVER use this?\n<style=cMono>Human, You Do Not Understand</style>\nWhat?! What could I possibly be misunderstanding about the fact you want to sell the most energy efficient poison on the planet?!\n<style=cMono>Requesting Position In Conversation For Elaboration</style>\n*Sigh,* granted.\n<style=cMono>All Worlds Needs Energy, Human. You Require It As Much As I. The Universe Is A Desperate Place\nAnd When Desperate People Need To Survive, They Need Energy. Clarification: High Quantities Of Energy\nDesperate People [Found, Created, Fabricated, Otherwise] Designate All Protocols To Locate What They Require And Utilize It\nI Give People What They Need, And In Exchange The Conglomerate Receives 25 Credits. Clarification: Relative Average Price Given Current Market Circumstances\nSimple Transaction. We Are Both Programmed For Business, You Should Understand Now</style>\n...I wasn't programmed, I studied, and I'm sure as hell didn't need any of *that* garbage to get my degree.\n<style=cMono>Unsubstantiated Claim. Clarification: UES Policy Fails To Distinct Between The Processes Of Programming And The Human Activity Of Studyi-</style>\nOh shove it up your nuts and bolts.";

		public override ItemTier Tier => ItemTier.Tier3;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[2]
		{
		ItemTag.Utility,
		ItemTag.Damage
		};


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("IonicColaDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("IonicColaIcon.png");

		public BuffDef ColaBuff { get; private set; }

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
			ColaBuff = ScriptableObject.CreateInstance<BuffDef>();
			ColaBuff.name = "ColaBoostBuff";
			ColaBuff.buffColor = Color.cyan;
			ColaBuff.canStack = true;
			ColaBuff.isDebuff = false;
			ColaBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Bleeding").iconSprite;
			ContentAddition.AddBuffDef(ColaBuff);
		}

		public override ItemDisplayRuleDict CreateItemDisplayRules()
		{
			ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
			return rules;
		}

		public override void Hooks()
		{
			On.RoR2.CharacterBody.RecalculateStats += new On.RoR2.CharacterBody.hook_RecalculateStats(CharacterBody_RecalculateStats);
			RecalculateStatsAPI.GetStatCoefficients += new RecalculateStatsAPI.StatHookEventHandler(AddColaBuff);
			On.RoR2.GlobalEventManager.OnCharacterDeath += new On.RoR2.GlobalEventManager.hook_OnCharacterDeath(OnKillStuff);
		}

		private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			orig.Invoke(self);
			int count = GetCount(self);
			self.attackSpeed += (float)count * 0.5f;
			self.moveSpeed += self.moveSpeed * 0.5f * (float)count;
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
					ItemHelpers.RefreshTimedBuffs(attackerBody, ColaBuff, 3.5f);
					attackerBody.AddTimedBuffAuthority(ColaBuff.buffIndex, 3.5f);
					ItemHelpers.RefreshTimedBuffs(attackerBody, ColaBuff, 3.5f);
				}
			}
		}

		private void AddColaBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			int count = GetCount(sender);
			if (sender.HasBuff(ColaBuff))
			{
				args.attackSpeedMultAdd += 0.1f * (float)sender.GetBuffCount(ColaBuff) + 0.1f * (float)sender.GetBuffCount(ColaBuff) * (float)(count - 1);
				args.moveSpeedMultAdd += 0.1f * (float)sender.GetBuffCount(ColaBuff) + 0.1f * (float)sender.GetBuffCount(ColaBuff) * (float)(count - 1);
			}
		}
	}
}
