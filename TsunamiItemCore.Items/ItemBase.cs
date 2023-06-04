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

public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
{
	public static T instance { get; private set; }

	public ItemBase()
	{
		if (instance != null)
		{
			throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBase was instantiated twice");
		}
		instance = this as T;
	}
}
	public abstract class ItemBase
	{
		public ItemDef ItemDef;

		public virtual string Name { get; }

		public abstract string ItemName { get; }

		public abstract string ItemLangTokenName { get; }

		public abstract string ItemPickupDesc { get; }

		public abstract string ItemFullDescription { get; }

		public abstract string ItemLore { get; }

		public abstract ItemTier Tier { get; }

		public virtual ItemTag[] ItemTags { get; set; } = new ItemTag[0];


		public abstract GameObject ItemModel { get; }

		public abstract Sprite ItemIcon { get; }

		public virtual bool CanRemove { get; } = true;


		public virtual bool AIBlacklisted { get; set; } = false;


		public abstract void Init(ConfigFile config);

		public virtual void CreateConfig(ConfigFile config)
		{
		}

		protected virtual void CreateLang()
		{
			LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_NAME", ItemName);
			LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_PICKUP", ItemPickupDesc);
			LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_DESCRIPTION", ItemFullDescription);
			LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_LORE", ItemLore);
		}

		public abstract ItemDisplayRuleDict CreateItemDisplayRules();

		protected void CreateItem()
		{
			if (AIBlacklisted)
			{
				ItemTags = new List<ItemTag>(ItemTags) { ItemTag.AIBlacklist }.ToArray();
			}

			ItemDef = ScriptableObject.CreateInstance<ItemDef>();
			ItemDef.name = "ITEM_" + ItemLangTokenName;
			ItemDef.nameToken = "ITEM_" + ItemLangTokenName + "_NAME";
			ItemDef.pickupToken = "ITEM_" + ItemLangTokenName + "_PICKUP";
			ItemDef.descriptionToken = "ITEM_" + ItemLangTokenName + "_DESCRIPTION";
			ItemDef.loreToken = "ITEM_" + ItemLangTokenName + "_LORE";
			ItemDef.pickupModelPrefab = ItemModel;
			ItemDef.pickupIconSprite = ItemIcon;
			ItemDef.hidden = false;
			ItemDef.canRemove = CanRemove;
			ItemDef.deprecatedTier = Tier;

			if (ItemTags.Length > 0) { ItemDef.tags = ItemTags; }
			ItemAPI.Add(new CustomItem(ItemDef, CreateItemDisplayRules()));
		}

		public virtual void Hooks()
		{
		}

		public int GetCount(CharacterBody body)
		{
			if (!(UnityEngine.Object)(object)body || !(UnityEngine.Object)(object)body.inventory)
			{
				return 0;
			}
			return body.inventory.GetItemCount(ItemDef);
		}

		public int GetCount(CharacterMaster master)
		{
			if (!(UnityEngine.Object)(object)master || !(UnityEngine.Object)(object)master.inventory)
			{
				return 0;
			}
			return master.inventory.GetItemCount(ItemDef);
		}

		public int GetCountSpecific(CharacterBody body, ItemDef itemDef)
		{
			if (!(UnityEngine.Object)(object)body || !(UnityEngine.Object)(object)body.inventory)
			{
				return 0;
			}
			return body.inventory.GetItemCount(itemDef);
		}
	}
}
