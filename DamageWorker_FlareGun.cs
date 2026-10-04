using RimWorld;
using Verse;

public class DamageWorker_FlareGun : DamageWorker_AddInjury {
	public override DamageResult Apply(DamageInfo dinfo, Thing victim) {
		if (victim.IsBurning())
		{
			dinfo.SetAmount((float) (dinfo.Amount * 2.5));
		}
		return base.Apply(dinfo, victim);
	}
}
