using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

public class Bullet_Flare : Bullet {
	// Stolen from Bullet
	protected override void Impact(Thing hitThing, bool blockedByShield = false) {
		Map map = this.Map;
		IntVec3 position = this.Position;
		this.ProjectileImpact(hitThing, blockedByShield);
		BattleLogEntry_RangedImpact entryRangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing,
			this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
		Find.BattleLog.Add((LogEntry) entryRangedImpact);
		this.NotifyImpact(hitThing, map, position);
		if (hitThing != null)
		{
			bool instigatorGuilty = !(this.launcher is Pawn launcher) || !launcher.Drafted;
			DamageInfo dinfo1 = new DamageInfo(this.DamageDef, (float) this.DamageAmount, this.ArmorPenetration,
				this.ExactRotation.eulerAngles.y, this.launcher, weapon: this.equipmentDef,
				intendedTarget: this.intendedTarget.Thing, instigatorGuilty: instigatorGuilty);
			dinfo1.SetWeaponQuality(this.equipmentQuality);
			dinfo1.SetBodyRegion(depth: BodyPartDepth.Outside); // Added to prevent bullets from hitting inside parts
			hitThing.TakeDamage(dinfo1).AssociateWithLog((LogEntry_DamageResult) entryRangedImpact);
			if (hitThing is Pawn pawn)
				pawn.stances?.stagger.Notify_BulletImpact(this);
			if (this.ExtraDamages == null)
				return;
			foreach (ExtraDamage extraDamage in this.ExtraDamages)
			{
				if (Rand.Chance(extraDamage.chance))
				{
					DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount,
						extraDamage.AdjustedArmorPenetration(), this.ExactRotation.eulerAngles.y, this.launcher,
						weapon: this.equipmentDef, intendedTarget: this.intendedTarget.Thing,
						instigatorGuilty: instigatorGuilty);
					hitThing.TakeDamage(dinfo2).AssociateWithLog((LogEntry_DamageResult) entryRangedImpact);
				}
			}
		}
		else
		{
			if (!blockedByShield)
			{
				SoundDefOf.BulletImpact_Ground.PlayOneShot((SoundInfo) new TargetInfo(this.Position, map));
				if (this.Position.GetTerrain(map).takeSplashes)
					FleckMaker.WaterSplash(this.ExactPosition, map, Mathf.Sqrt((float) this.DamageAmount) * 1f, 4f);
				else
					FleckMaker.Static(this.ExactPosition, map, FleckDefOf.ShotHit_Dirt);
			}

			if (!Rand.Chance(this.DamageDef.igniteCellChance))
				return;
			FireUtility.TryStartFireIn(this.Position, map, Rand.Range(0.55f, 0.85f), this.launcher);
		}
	}

	// Stolen from Bullet
	private void NotifyImpact(Thing hitThing, Map map, IntVec3 position) {
		BulletImpactData impactData = new BulletImpactData() {
			bullet = this,
			hitThing = hitThing,
			impactPosition = position
		};
		hitThing?.Notify_BulletImpactNearby(impactData);
		int num = 9;
		for (int index1 = 0; index1 < num; ++index1)
		{
			IntVec3 c = position + GenRadial.RadialPattern[index1];
			if (c.InBounds(map))
			{
				List<Thing> thingList = c.GetThingList(map);
				for (int index2 = 0; index2 < thingList.Count; ++index2)
				{
					if (thingList[index2] != hitThing)
						thingList[index2].Notify_BulletImpactNearby(impactData);
				}
			}
		}
	}

	// Stolen from Projectile
	private void ProjectileImpact(Thing hitThing, bool blockedByShield = false) {
		GenClamor.DoClamor((Thing) this, 12f, ClamorDefOf.Impact);
		if (!blockedByShield && this.def.projectile.landedEffecter != null)
			this.def.projectile.landedEffecter.Spawn(this.ExactPosition.ToIntVec3(), this.Map).Cleanup();
		this.Destroy(DestroyMode.Vanish);
	}
}