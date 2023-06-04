using System;
using System.Collections.Generic;
using UnityEngine;

namespace TsunamiItemCore.Utils {

	public static class MathHelpers
	{
		public static string FloatToPercentageString(float number, float numberBase = 100f)
		{
			return (number * numberBase).ToString("##0") + "%";
		}

		public static Vector3 ClosestPointOnSphereToPoint(Vector3 origin, float radius, Vector3 targetPosition)
		{
			Vector3 value = targetPosition - origin;
			value = Vector3.Normalize(value);
			value *= radius;
			return origin + value;
		}

		public static List<Vector3> DistributePointsEvenlyAroundSphere(int points, float radius, Vector3 origin)
		{
			List<Vector3> list = new List<Vector3>();
			double num = Math.PI * (3.0 - Math.Sqrt(5.0));
			for (int i = 0; i < points; i++)
			{
				int num2 = 1 - i / (points - 1) * 2;
				double num3 = Math.Sqrt(1 - num2 * num2);
				double num4 = num * (double)i;
				float x = (float)(Math.Cos(num4) * num3);
				float z = (float)(Math.Sin(num4) * num3);
				Vector3 vector = origin + new Vector3(x, num2, z);
				list.Add(vector * radius);
			}
			return list;
		}

		public static List<Vector3> DistributePointsEvenlyAroundCircle(int points, float radius, Vector3 origin, float angleOffset = 0f)
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < points; i++)
			{
				double num = Math.PI * 2.0 / (double)points;
				double num2 = num * (double)i + (double)angleOffset;
				Vector3 item = new Vector3((float)((double)radius * Math.Cos(num2) + (double)origin.x), origin.y, (float)((double)radius * Math.Sin(num2) + (double)origin.z));
				list.Add(item);
			}
			return list;
		}

		public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
		{
			float f = UnityEngine.Random.Range(0f, angle) * ((float)Math.PI / 180f);
			Vector2 vector = UnityEngine.Random.insideUnitCircle.normalized * Mathf.Sin(f);
			Vector3 vector2 = new Vector3(vector.x, vector.y, Mathf.Cos(f));
			return targetDirection * vector2;
		}

		public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
		{
			return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
		}

		public static Vector3 RandomPointOnCircle(Vector3 origin, float radius, Xoroshiro128Plus random)
		{
			float f = random.RangeFloat(0f, (float)Math.PI * 2f);
			return origin + new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * radius;
		}

		public static float InverseHyperbolicScaling(float baseValue, float additionalValue, float maxValue, int itemCount)
		{
			return baseValue + (maxValue - baseValue) * (1f - 1f / (1f + additionalValue * (float)(itemCount - 1)));
		}
	}
}
