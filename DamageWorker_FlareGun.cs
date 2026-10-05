using RimWorld;
using Verse;

public class DamageWorker_FlareGun : DamageWorker_AddInjury {
	public virtual float Mult => 2.5f;
	
	public override DamageResult Apply(DamageInfo dinfo, Thing victim) {
		if (victim.IsBurning())
			dinfo.SetAmount(dinfo.Amount * Mult);
		
		return base.Apply(dinfo, victim);
	}
}
