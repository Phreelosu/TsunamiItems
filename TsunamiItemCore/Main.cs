using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using TsunamiItemCore.Equipment;
using TsunamiItemCore.Items;
using UnityEngine;

namespace TsunamiItemCore {

	[BepInPlugin("com.Phreel.TsunamiItemsRevived", "TsunamiItems", "1.0.0")]
	[BepInDependency("com.bepis.r2api")]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	[R2APISubmoduleDependency(/*new string[] { "ItemAPI", "LanguageAPI", "ArtifactAPI", "EliteAPI", "RecalculateStatsAPI", "BuffAPI" })*/nameof(ItemAPI), nameof(LanguageAPI))]
	public class Main : BaseUnityPlugin
	{
		public const string ModGuid = "com.Phreel.TsunamiItemsRevived";

		public const string ModName = "TsunamiItemsRevived";

		public const string ModVer = "1.0.0";

		public static AssetBundle MainAssets;

		public List<ItemBase> Items = new List<ItemBase>();

		public List<EquipmentBase> Equipments = new List<EquipmentBase>();

		public static ManualLogSource ModLogger;

		private void Awake()
		{
			ModLogger = base.Logger;
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TsunamiItemCore.tsunamiitemcoreassets"))
			{
				MainAssets = AssetBundle.LoadFromStream(stream);
			}
			IEnumerable<Type> enumerable2 = from type in Assembly.GetExecutingAssembly().GetTypes()
											where !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase))
											select type;
			foreach (Type item2 in enumerable2)
			{
				ItemBase itemBase = (ItemBase)Activator.CreateInstance(item2);
				if (ValidateItem(itemBase, Items))
				{
					itemBase.Init(base.Config);
				}
			}
			IEnumerable<Type> enumerable3 = from type in Assembly.GetExecutingAssembly().GetTypes()
											where !type.IsAbstract && type.IsSubclassOf(typeof(EquipmentBase))
											select type;
			foreach (Type item3 in enumerable3)
			{
				EquipmentBase equipmentBase = (EquipmentBase)Activator.CreateInstance(item3);
				if (ValidateEquipment(equipmentBase, Equipments))
				{
					equipmentBase.Init(base.Config);
				}
			}
		}

		public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
		{
			bool value = base.Config.Bind("Item: " + item.ItemName, "Enable Item?", defaultValue: true, "Should this item appear in runs?").Value;
			bool value2 = base.Config.Bind("Item: " + item.ItemName, "Blacklist Item from AI Use?", defaultValue: false, "Should the AI not be able to obtain this item?").Value;
			if (value)
			{
				itemList.Add(item);
				if (value2)
				{
					item.AIBlacklisted = true;
				}
			}
			return value;
		}

		public bool ValidateEquipment(EquipmentBase equipment, List<EquipmentBase> equipmentList)
		{
			if (base.Config.Bind("Equipment: " + equipment.EquipmentName, "Enable Equipment?", defaultValue: true, "Should this equipment appear in runs?").Value)
			{
				equipmentList.Add(equipment);
				return true;
			}
			return false;
		}
	}
}
