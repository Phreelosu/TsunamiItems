/*using RoR2;

this is unused because I rewrote Red Skull partially, so it doesn't need a buff atm

namespace TsunamiItemCore.Items { 

public class SkullBehavior : CharacterBody.ItemBehavior
{
		private void FixedUpdate()
		{
			float combinedHealthFraction = body.healthComponent.combinedHealthFraction;
			int buffCount = body.GetBuffCount(ItemBase<RedSkull>.instance.SkullBuff);
			int num = stack - buffCount;
			if (combinedHealthFraction <= RedSkull.maxHealthThreshold && num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					body.AddBuff(ItemBase<RedSkull>.instance.SkullBuff);
				}
			}
			if (combinedHealthFraction > RedSkull.maxHealthThreshold && buffCount != 0)
			{
				while (body.GetBuffCount(ItemBase<RedSkull>.instance.SkullBuff) > 0)
				{
					body.RemoveBuff(ItemBase<RedSkull>.instance.SkullBuff);
				}
			}
		}
	}
}
*/