using System;

using Dalamud.Game.ClientState.Objects.Types;

using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace Distance;

internal static unsafe class TargetResolver
{
	public static void Init( Configuration configuration )
	{
		mConfiguration = configuration;
	}

	public static void Uninit()
	{
		mConfiguration = null;
	}

	public static IGameObject GetTarget( TargetType targetType )
	{
		return targetType switch
		{
			TargetType.Target_And_Soft_Target => Service.TargetManager.SoftTarget ?? Service.TargetManager.Target,
			TargetType.FocusTarget => Service.TargetManager.FocusTarget,
			TargetType.MouseOver_And_UIMouseOver_Target => GetUIMouseoverTarget() ?? Service.TargetManager.MouseOverTarget,
			TargetType.Target => Service.TargetManager.Target,
			TargetType.SoftTarget => Service.TargetManager.SoftTarget,
			TargetType.MouseOverTarget => Service.TargetManager.MouseOverTarget,
			TargetType.UIMouseOverTarget => GetUIMouseoverTarget(),
			TargetType.TargetOfTarget => GetTargetOfTarget(),
			_ => throw new Exception( $"Request to resolve unknown target type: \"{targetType}\"." ),
		};
	}

	//	Technically not a target, but may be based on target, so this is probably a reasonable place for it.
	public static IGameObject EffectiveLocalPlayer
	{
		get
		{
			return mConfiguration?.DutyRecorderTreatSelectedCharacterAsPlayer == true &&
				Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.DutyRecorderPlayback] ?
				SelectedReplayPlayer :
				Service.ObjectTable.LocalPlayer;
		}
	}

	public static IGameObject SelectedReplayPlayer
	{
		get
		{
			var pTargetSystemInstance = TargetSystem.Instance();
			if( Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.DutyRecorderPlayback] &&
				pTargetSystemInstance is not null )
			{
				//***** TODO: Idle cam target works for this, but there is another target that also matches at 0xC8 (Idle cam is 0xA0 at time of writing).  Should look into this to see if that is actually the correct target.
				var pActor = pTargetSystemInstance->IdleCamTarget;
				if( pActor is not null )
				{
					return Service.ObjectTable.CreateObjectReference( (nint)pActor );
				}
			}

			return null;
		}
	}

	private static IGameObject GetTargetOfTarget()
	{
		var target = Service.TargetManager.SoftTarget ?? Service.TargetManager.Target;
		if( target != null && target.TargetObjectId != 0xE000000 )
		{
			return target.TargetObject;
		}
		else
		{
			return null;
		}
	}

	private static IGameObject GetUIMouseoverTarget()
	{
		if( PronounModule.Instance() != null )
		{
			var pActor = (IntPtr)PronounModule.Instance()->UiMouseOverTarget;
			if( pActor != IntPtr.Zero )
			{
				return Service.ObjectTable.CreateObjectReference( (IntPtr)pActor );
			}
		}

		return null;
	}

	private static Configuration mConfiguration = null;
}
