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

	public class DeckofCards : ItemBase<DeckofCards>
	{
		public static float shieldArmor;

		public override string ItemName => "Deck of Cards";

		public override string ItemLangTokenName => "TSUNAMI_CARD_DECK";

		public override string ItemPickupDesc => "Gain a chance to get a random base game buff on killing an enemy.";

		public override string ItemFullDescription => "Upon killing an enemy, you have a 5%<style=cStack> (+5% per stack)</style> chance to gain a <style=cIsUtility>random buff</style> for <style=cIsUtility>3</style><style=cStack> (+3 per stack)</style> seconds.";

		public override string ItemLore => "You know, I've heard some... interesting things about those cards of yours.\n\nThe younger player shuffled his deck around idly, almost completely brushing off the words of the older player.\n\nYeah, so what? My cards are awesome! Never lost me a game. Now are we playing or what?\n\nThe older man begin to chuckle oddly at him. He would get up slowly approach him.\n\nOh, they are quite... awesome, as you'd describe them.\nBut you see, I know your secret... you didn't even make this deck.\n\nThe younger would recoil in obvious shock, as no one he had played against knew his secret.\n\nT-the hell?! Who even are you?\n\nOh, you see... I'm the original holder of those very special cards. I made them with my...friends. And I want them back. Now. hand them over. The young man hopped out of his chair and began to run away.\nYou can have them back if you can catch me! Good luck with that, coffin dodger!\n\nThe old man began to take out a very ornate looking camera.\nA fast one. You'll make a nice addition...\n\n\n...An echoing click rang out through the ship.";

		public override ItemTier Tier => ItemTier.Tier2;

		public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Utility };


		public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("DeckOfCardsDisplay.prefab");

		public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("DeckOfCardsIcon.png");

		public Xoroshiro128Plus rng { get; internal set; }

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
			On.RoR2.GlobalEventManager.OnCharacterDeath += new On.RoR2.GlobalEventManager.hook_OnCharacterDeath(OnKillStuff);
		}

		private void OnKillStuff(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
		{
			orig.Invoke(self, damageReport);
			CharacterBody victimBody = damageReport.victimBody;
			CharacterBody attackerBody = damageReport.attackerBody;
			if (!victimBody.healthComponent.alive)
			{
				int count = GetCount(attackerBody);
				if (count > 0 && Util.CheckRoll(5f * (float)count, attackerBody.master))
				{
					attackerBody.AddTimedBuff(ChooseRandomBuff(), 3.5f * (float)count);
				}
			}
		}

		private BuffDef ChooseRandomBuff()
		{
			BuffDef result = null;
			switch (Run.instance.runRNG.RangeInt(1, 34))
			{
				case 1:
					result = RoR2Content.Buffs.ArmorBoost;
					break;
				case 2:
					result = RoR2Content.Buffs.AttackSpeedOnCrit;
					break;
				case 3:
					result = RoR2Content.Buffs.BanditSkull;
					break;
				case 4:
					result = RoR2Content.Buffs.Cloak;
					break;
				case 5:
					result = RoR2Content.Buffs.CloakSpeed;
					break;
				case 6:
					result = RoR2Content.Buffs.CrocoRegen;
					break;
				case 7:
					result = RoR2Content.Buffs.ElephantArmorBoost;
					break;
				case 8:
					result = RoR2Content.Buffs.Energized;
					break;
				case 9:
					result = RoR2Content.Buffs.EngiShield;
					break;
				case 10:
					result = RoR2Content.Buffs.FullCrit;
					break;
				case 11:
					result = RoR2Content.Buffs.HiddenInvincibility;
					break;
				case 12:
					result = RoR2Content.Buffs.Immune;
					break;
				case 13:
					result = RoR2Content.Buffs.LifeSteal;
					break;
				case 14:
					result = RoR2Content.Buffs.MedkitHeal;
					break;
				case 15:
					result = RoR2Content.Buffs.NoCooldowns;
					break;
				case 16:
					result = RoR2Content.Buffs.PowerBuff;
					break;
				case 17:
					result = RoR2Content.Buffs.SmallArmorBoost;
					break;
				case 18:
					result = RoR2Content.Buffs.TeamWarCry;
					break;
				case 19:
					result = RoR2Content.Buffs.TonicBuff;
					break;
				case 20:
					result = RoR2Content.Buffs.Warbanner;
					break;
				case 21:
					result = RoR2Content.Buffs.WarCryBuff;
					break;
				case 22:
					result = RoR2Content.Buffs.WhipBoost;
					break;
				case 23:
					result = DLC1Content.Buffs.KillMoveSpeed;
					break;
				case 24:
					result = DLC1Content.Buffs.OutOfCombatArmorBuff;
					break;
				case 25:
					result = DLC1Content.Buffs.PrimarySkillShurikenBuff;
					break;
				case 26:
					result = RoR2Content.Buffs.TeslaField;
					break;
				case 27:
					result = RoR2Content.Buffs.AffixHauntedRecipient;
					break;
				case 28:
					result = DLC1Content.Buffs.MushroomVoidActive;
					break;
				case 29:
					result = ItemBase<Suppressor>.instance.SuppressorBuff;
					break;
				case 30:
					result = ItemBase<ScoutsRifle>.instance.ScoutBuff;
					break;
				case 31:
					result = ItemBase<BerserkersKneepad>.instance.BerserkBuff;
					break;
				case 32:
					result = ItemBase<IonicCola>.instance.ColaBuff;
					break;
				case 33:
					result = ItemBase<ForeignFlower>.instance.FlowerBuff;
					break;
				case 34:
					result = ItemBase<ObsidianFragment>.instance.ObsidianBuff;
					break;
			}
			return result;
		}
	}
}
