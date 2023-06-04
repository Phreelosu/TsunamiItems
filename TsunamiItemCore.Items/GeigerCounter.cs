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
using EntityStates.Croco;

namespace TsunamiItemCore.Items { 

public class GeigerCounter : ItemBase<GeigerCounter>
{
	public static float shieldArmor;

	private int attackCounter;

	private static int geigerExplosionCharge = 10;

	public static GameObject geigerExplosionEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/CrocoLeapExplosion");

	public override string ItemName => "Geiger Counter";

	public override string ItemLangTokenName => "TSUNAMI_GEIGER_COUNTER";

	public override string ItemPickupDesc => "Every 10 skill uses causes a blighted explosion.";

	public override string ItemFullDescription => "Every <style=cIsUtility>10</style> non-primary skill uses cause you to explode for <style=cIsDamage>1600%</style> <style=cStack>(+400% damage per stack)</style> damage, applying <style=cIsDamage>Blight</style> to enemies.";

	public override string ItemLore => "Hello, sir. I'm assuming you're one of the hired prospectors?\nNo time to talk, I have some very important cargo on board and I need to get this to the grand prospector ASAP.\n...very well, please allow me to scan your luggage for contraband and other miscellaneous hazards.\nMake it quick lady, I don't got all day.\n\n<style=cMono>[KRRRRRKRRKZZRRRKRRRRRR]</style>\n\nExcuse me sir, it appears that you have radioactive hazards located somewhere on your ship. Please allow our automated search drones to inspect your ship.\n*sigh* Yeah yeah, alright. Hurry up with it already!\n\n<style=cMono>.     .     .</style>\n\nIt's been 20 damn minutes lady, hurry this up!\nIt appears that our scanners are having trouble detecting the hazard. This may take a bit longer than expected.\nGrrrrrr...\n\n<style=cMono>.     .     .</style>\n\nOh my! we seem to have found the problem! The hazard is not on your ship, but rather... on you.\nWhat the hell are you talking about, lady?! Quit spouting bull and open up the damn port!\nSorry sir! I don't make the rules, I simply work here. I do recommend that next time you want to smuggle a deadly nuclear weapon somewhere, you don't put it so close to yourself.";

	public override ItemTier Tier => ItemTier.Tier3;

	public override ItemTag[] ItemTags { get; set; } = new ItemTag[1] { ItemTag.Damage };


	public override GameObject ItemModel => Main.MainAssets.LoadAsset<GameObject>("GeigerCounterDisplay.prefab");

	public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("GeigerCounterIcon.png");

	public BuffDef GeigerBuff { get; private set; }

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
		GeigerBuff = ScriptableObject.CreateInstance<BuffDef>();
		GeigerBuff.name = "GeigerBuff";
		GeigerBuff.buffColor = Color.green;
		GeigerBuff.canStack = true;
		GeigerBuff.isDebuff = false;
		GeigerBuff.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffHealingDisabledIcon");
		ContentAddition.AddBuffDef(GeigerBuff);
	}

	public override ItemDisplayRuleDict CreateItemDisplayRules()
	{
		ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
		return rules;
	}

	public override void Hooks()
	{
		On.RoR2.CharacterBody.OnSkillActivated += new On.RoR2.CharacterBody.hook_OnSkillActivated(DoGeigerTask);
	}

		private void DoGeigerTask(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
		{
			orig.Invoke(self, skill);
			int count = GetCount(self);
			if (count > 0)
			{
                //SkillLocator skillLocator = self.skillLocator;
                //flag = (((UnityEngine.Object)(object)skillLocator != null) ? skillLocator.primary : null) == skill;
                if (self.skillLocator.primary != skill)
                {
					attackCounter++;
				}
			}
			
			if (attackCounter >= geigerExplosionCharge)
			{
				attackCounter = 0;
				Util.PlayAttackSpeedSound(BaseLeap.leapSoundString, ((Component)(object)self).gameObject, self.attackSpeed);
				if (NetworkServer.active)
				{
					float baseDamage = self.damage * (16f + 4f * (float)(count - 1));
					float num = 25f;
					EffectManager.SpawnEffect(geigerExplosionEffectPrefab, new EffectData
					{
						origin = self.transform.position,
						scale = num
					}, transmit: true);
					BlastAttack blastAttack = new BlastAttack
					{
						baseDamage = baseDamage,
						radius = num,
						procCoefficient = 1f,
						position = self.transform.position,
						attacker = ((Component)(object)self).gameObject,
						crit = Util.CheckRoll(self.crit, self.master),
						falloffModel = BlastAttack.FalloffModel.None,
						damageType = DamageType.BlightOnHit,
						teamIndex = TeamComponent.GetObjectTeam(((Component)(object)self).gameObject)
					};
					blastAttack.Fire();
				}
			}
		}
	}
}
