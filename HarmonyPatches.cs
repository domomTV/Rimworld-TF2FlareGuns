

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

[StaticConstructorOnStartup]
public static class HarmonyPatches {
	private static readonly Type patchType = typeof(HarmonyPatches);
	
	static HarmonyPatches() {
		Harmony harmony = new Harmony("domom_FlareGuns");
		harmony.Patch(AccessTools.Method(typeof(Bullet_Flare), "Impact"), transpiler: new HarmonyMethod(patchType, nameof(FlareDepthTranspiler)));
		harmony.Patch(AccessTools.Method(typeof(Bullet_Flare), "Impact"), postfix: new HarmonyMethod(patchType, nameof(Bullet_Hello)));
	}

	public static void Bullet_Hello() {
		Log.Message("Hello");
	}

	public static IEnumerable<CodeInstruction> FlareDepthTranspiler(IEnumerable<CodeInstruction> instructions) {
		MethodInfo qualityMethod = AccessTools.Method(typeof(DamageInfo), nameof(DamageInfo.SetWeaponQuality));
		bool called = false;
		
		foreach (CodeInstruction instruction in instructions)
		{
			yield return instruction;
			if (instruction.)
			{
				Log.Message(instruction.operand != null ? instruction.operand.ToString() : "null");
				called = true;
			}
		}

		if (!called)
			Log.Message("not even called");
		// dinfo1.SetBodyRegion(depth: BodyPartDepth.Outside);
	}
}