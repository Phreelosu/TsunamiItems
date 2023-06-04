using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TsunamiItemCore.Equipment { 

public abstract class EquipmentBase<T> : EquipmentBase where T : EquipmentBase<T>
{
	public static T instance { get; private set; }

	public EquipmentBase()
	{
		if (instance != null)
		{
			throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting EquipmentBoilerplate/Equipment was instantiated twice");
		}
		instance = this as T;
	}
}
	public abstract class EquipmentBase
	{
		public enum TargetingType
		{
			Enemies,
			Friendlies
		}

		public class TargetingControllerComponent : MonoBehaviour
		{
			public GameObject TargetObject;

			public GameObject VisualizerPrefab;

			public RoR2.Indicator Indicator;

			public RoR2.BullseyeSearch TargetFinder;

			public Action<RoR2.BullseyeSearch> AdditionalBullseyeFunctionality = delegate
			{
			};

			public void Awake()
			{
				Indicator = new RoR2.Indicator(base.gameObject, null);
			}

			public void OnDestroy()
			{
				Invalidate();
			}

			public void Invalidate()
			{
				TargetObject = null;
				Indicator.targetTransform = null;
			}

			public void ConfigureTargetFinderBase(RoR2.EquipmentSlot self)
			{
				if (TargetFinder == null)
				{
					TargetFinder = new RoR2.BullseyeSearch();
				}
				TargetFinder.teamMaskFilter = RoR2.TeamMask.allButNeutral;
				TargetFinder.teamMaskFilter.RemoveTeam(self.characterBody.teamComponent.teamIndex);
				TargetFinder.sortMode = RoR2.BullseyeSearch.SortMode.Angle;
				TargetFinder.filterByLoS = true;
				float extraRaycastDistance;
				Ray ray = RoR2.CameraRigController.ModifyAimRayIfApplicable(self.GetAimRay(), ((Component)(object)self).gameObject, out extraRaycastDistance);
				TargetFinder.searchOrigin = ray.origin;
				TargetFinder.searchDirection = ray.direction;
				TargetFinder.maxAngleFilter = 10f;
				TargetFinder.viewer = self.characterBody;
			}

			public void ConfigureTargetFinderForEnemies(RoR2.EquipmentSlot self)
			{
				ConfigureTargetFinderBase(self);
				TargetFinder.teamMaskFilter = RoR2.TeamMask.GetUnprotectedTeams(self.characterBody.teamComponent.teamIndex);
				TargetFinder.RefreshCandidates();
				TargetFinder.FilterOutGameObject(((Component)(object)self).gameObject);
				AdditionalBullseyeFunctionality(TargetFinder);
				PlaceTargetingIndicator(TargetFinder.GetResults());
			}

			public void ConfigureTargetFinderForFriendlies(RoR2.EquipmentSlot self)
			{
				ConfigureTargetFinderBase(self);
				TargetFinder.teamMaskFilter = RoR2.TeamMask.none;
				TargetFinder.teamMaskFilter.AddTeam(self.characterBody.teamComponent.teamIndex);
				TargetFinder.RefreshCandidates();
				TargetFinder.FilterOutGameObject(((Component)(object)self).gameObject);
				AdditionalBullseyeFunctionality(TargetFinder);
				PlaceTargetingIndicator(TargetFinder.GetResults());
			}

			public void PlaceTargetingIndicator(IEnumerable<RoR2.HurtBox> TargetFinderResults)
			{
				RoR2.HurtBox hurtBox = (TargetFinderResults.Any() ? TargetFinderResults.First() : null);
				if ((bool)hurtBox)
				{
					TargetObject = ((Component)(object)hurtBox.healthComponent).gameObject;
					Indicator.visualizerPrefab = VisualizerPrefab;
					Indicator.targetTransform = hurtBox.transform;
				}
				else
				{
					Invalidate();
				}
				Indicator.active = hurtBox;
			}
		}

		public RoR2.EquipmentDef EquipmentDef;

		public GameObject TargetingIndicatorPrefabBase = null;

		public abstract string EquipmentName { get; }

		public abstract string EquipmentLangTokenName { get; }

		public abstract string EquipmentPickupDesc { get; }

		public abstract string EquipmentFullDescription { get; }

		public abstract string EquipmentLore { get; }

		public abstract GameObject EquipmentModel { get; }

		public abstract Sprite EquipmentIcon { get; }

		public virtual bool AppearsInSinglePlayer { get; } = true;


		public virtual bool AppearsInMultiPlayer { get; } = true;


		public virtual bool CanDrop { get; } = true;


		public virtual float Cooldown { get; } = 60f;


		public virtual bool EnigmaCompatible { get; } = true;


		public virtual bool IsBoss { get; } = false;


		public virtual bool IsLunar { get; } = false;


		public virtual bool UseTargeting { get; } = false;


		public virtual TargetingType TargetingTypeEnum { get; } = TargetingType.Enemies;


		public abstract ItemDisplayRuleDict CreateItemDisplayRules();

		public abstract void Init(ConfigFile config);

		protected virtual void CreateConfig(ConfigFile config)
		{
		}

		protected virtual void CreateLang()
		{
			LanguageAPI.Add("EQUIPMENT_" + EquipmentLangTokenName + "_NAME", EquipmentName);
			LanguageAPI.Add("EQUIPMENT_" + EquipmentLangTokenName + "_PICKUP", EquipmentPickupDesc);
			LanguageAPI.Add("EQUIPMENT_" + EquipmentLangTokenName + "_DESCRIPTION", EquipmentFullDescription);
			LanguageAPI.Add("EQUIPMENT_" + EquipmentLangTokenName + "_LORE", EquipmentLore);
		}

		protected void CreateEquipment()
		{
			EquipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
			EquipmentDef.name = "EQUIPMENT_" + EquipmentLangTokenName;
			EquipmentDef.nameToken = "EQUIPMENT_" + EquipmentLangTokenName + "_NAME";
			EquipmentDef.pickupToken = "EQUIPMENT_" + EquipmentLangTokenName + "_PICKUP";
			EquipmentDef.descriptionToken = "EQUIPMENT_" + EquipmentLangTokenName + "_DESCRIPTION";
			EquipmentDef.loreToken = "EQUIPMENT_" + EquipmentLangTokenName + "_LORE";
			EquipmentDef.pickupModelPrefab = EquipmentModel;
			EquipmentDef.pickupIconSprite = EquipmentIcon;
			EquipmentDef.appearsInSinglePlayer = AppearsInSinglePlayer;
			EquipmentDef.appearsInMultiPlayer = AppearsInMultiPlayer;
			EquipmentDef.canDrop = CanDrop;
			EquipmentDef.cooldown = Cooldown;
			EquipmentDef.enigmaCompatible = EnigmaCompatible;
			EquipmentDef.isBoss = IsBoss;
			EquipmentDef.isLunar = IsLunar;
			ItemAPI.Add(new CustomEquipment(EquipmentDef, CreateItemDisplayRules()));
            On.RoR2.EquipmentSlot.PerformEquipmentAction += PerformEquipmentAction;
		}

		private bool PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, RoR2.EquipmentSlot self, RoR2.EquipmentDef equipmentDef)
		{
			if (equipmentDef == EquipmentDef)
			{
				return ActivateEquipment(self);
			}
			return orig.Invoke(self, equipmentDef);
		}

		protected abstract bool ActivateEquipment(RoR2.EquipmentSlot slot);

		public virtual void Hooks()
		{
		}

		protected void UpdateTargeting(On.RoR2.EquipmentSlot.orig_Update orig, RoR2.EquipmentSlot self)
		{
			orig.Invoke(self);
			if (self.equipmentIndex != EquipmentDef.equipmentIndex)
			{
				return;
			}
			TargetingControllerComponent targetingControllerComponent = ((Component)(object)self).GetComponent<TargetingControllerComponent>();
			if (!targetingControllerComponent)
			{
				targetingControllerComponent = ((Component)(object)self).gameObject.AddComponent<TargetingControllerComponent>();
				targetingControllerComponent.VisualizerPrefab = TargetingIndicatorPrefabBase;
			}
			if (self.stock > 0)
			{
				switch (TargetingTypeEnum)
				{
					case TargetingType.Enemies:
						targetingControllerComponent.ConfigureTargetFinderForEnemies(self);
						break;
					case TargetingType.Friendlies:
						targetingControllerComponent.ConfigureTargetFinderForFriendlies(self);
						break;
				}
			}
			else
			{
				targetingControllerComponent.Invalidate();
				targetingControllerComponent.Indicator.active = false;
			}
		}
	}
}
