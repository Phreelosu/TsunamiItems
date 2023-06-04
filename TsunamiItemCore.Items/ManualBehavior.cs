using RoR2;

namespace TsunamiItemCore.Items {

	public class ManualBehavior : CharacterBody.ItemBehavior
	{
		private void FixedUpdate()
		{
			bool flag = body.HasBuff(ItemBase<TacticiansManual>.instance.TacticianBuff);
			bool flag2 = body.HasBuff(ItemBase<TacticiansManual>.instance.TacticianDebuff);
			if (!flag && !flag2)
			{
				body.AddBuff(ItemBase<TacticiansManual>.instance.TacticianBuff);
			}
			if (flag && flag2)
			{
				body.RemoveBuff(ItemBase<TacticiansManual>.instance.TacticianBuff);
			}
		}
	}
}
