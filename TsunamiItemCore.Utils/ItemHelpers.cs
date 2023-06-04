using System;
using System.Collections.Generic;
using RoR2;
using TsunamiItemCore.Utils.Components;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace TsunamiItemCore.Utils {

	public static class ItemHelpers
	{
		public static CharacterModel.RendererInfo[] ItemDisplaySetup(GameObject obj, bool debugmode = false)
		{
			List<Renderer> list = new List<Renderer>();
			MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
			if (componentsInChildren.Length != 0)
			{
				list.AddRange(componentsInChildren);
			}
			SkinnedMeshRenderer[] componentsInChildren2 = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (componentsInChildren2.Length != 0)
			{
				list.AddRange(componentsInChildren2);
			}
			CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				if (debugmode)
				{
					MaterialControllerComponents.HGControllerFinder hGControllerFinder = list[i].gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
					hGControllerFinder.Renderer = list[i];
				}
				array[i] = new CharacterModel.RendererInfo
				{
					defaultMaterial = ((list[i] is SkinnedMeshRenderer) ? list[i].sharedMaterial : list[i].material),
					renderer = list[i],
					defaultShadowCastingMode = ShadowCastingMode.On,
					ignoreOverlays = false
				};
			}
			return array;
		}

		public static string OrderManifestLoreFormatter(string deviceName, string estimatedDelivery, string sentTo, string trackingNumber, string devicePickupDesc, string shippingMethod, string orderDetails)
		{
			string[] value = new string[19]
			{
			"<align=left>Estimated Delivery:<indent=70%>Sent To:</indent></align>",
			"<align=left>" + estimatedDelivery + "<indent=70%>" + sentTo + "</indent></align>",
			"",
			"<indent=1%><style=cIsDamage><size=125%><u>  Shipping Details:\u00a0\u00a0\u00a0\u00a0\u00a0\u00a0\u00a0</u></size></style></indent>",
			"",
			"<indent=2%>-Order: <style=cIsUtility>" + deviceName + "</style></indent>",
			"<indent=4%><style=cStack>Tracking Number:  " + trackingNumber + "</style></indent>",
			"",
			"<indent=2%>-Order Description: " + devicePickupDesc + "</indent>",
			"",
			"<indent=2%>-Shipping Method: <style=cIsHealth>" + shippingMethod + "</style></indent>",
			"",
			"",
			"",
			"<indent=2%>-Order Details: " + orderDetails + "</indent>",
			"",
			"",
			"",
			"<style=cStack>Delivery being brought to you by the brand new </style><style=cIsUtility>Orbital Drop-Crate System (TM)</style>. <style=cStack><u>No refunds.</u></style>"
			};
			return string.Join("\n", value);
		}

		public static void RefreshTimedBuffs(CharacterBody body, BuffDef buffDef, float duration)
		{
			if (!(UnityEngine.Object)(object)body || body.GetBuffCount(buffDef) <= 0)
			{
				return;
			}
			foreach (CharacterBody.TimedBuff timedBuff in body.timedBuffs)
			{
				if (buffDef.buffIndex == timedBuff.buffIndex)
				{
					timedBuff.timer = duration;
				}
			}
		}

		public static void RefreshTimedBuffs(CharacterBody body, BuffDef buffDef, float taperStart, float taperDuration)
		{
			if (!(UnityEngine.Object)(object)body || body.GetBuffCount(buffDef) <= 0)
			{
				return;
			}
			int num = 0;
			foreach (CharacterBody.TimedBuff timedBuff in body.timedBuffs)
			{
				if (buffDef.buffIndex == timedBuff.buffIndex)
				{
					timedBuff.timer = taperStart + (float)num * taperDuration;
					num++;
				}
			}
		}

		public static void AddBuffAndDot(BuffDef buff, float duration, int stackCount, CharacterBody body)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			DotController.DotIndex dotIndex = (DotController.DotIndex)Array.FindIndex(DotController.dotDefs, (DotController.DotDef dotDef) => dotDef.associatedBuff == buff);
			for (int i = 0; i < stackCount; i++)
			{
				if (dotIndex != DotController.DotIndex.None)
				{
					DotController.InflictDot(((Component)(object)body).gameObject, ((Component)(object)body).gameObject, dotIndex, duration, 0.25f);
				}
				else
				{
					body.AddTimedBuff(buff.buffIndex, duration);
				}
			}
		}

		public static DotController.DotIndex FindAssociatedDotForBuff(BuffDef buff)
		{
			return (DotController.DotIndex)Array.FindIndex(DotController.dotDefs, (DotController.DotDef dotDef) => dotDef.associatedBuff == buff);
		}
	}
}
