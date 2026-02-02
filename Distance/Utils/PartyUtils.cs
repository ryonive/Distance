using Dalamud.Game.ClientState.Conditions;

using FFXIVClientStructs.FFXIV.Client.Game.Group;

namespace Distance;

internal static unsafe class PartyUtils
{
	internal static bool ObjectIsPartyMember( uint entityID )
	{
		if( entityID is 0 or 0xE0000000 ) return false;
		if( GroupManager.Instance() is null ) return false;
		if( Service.Condition[ConditionFlag.DutyRecorderPlayback] )
		{
			return GroupManager.Instance()->ReplayGroup.IsEntityIdInParty( entityID );
		}
		else
		{
			return GroupManager.Instance()->MainGroup.IsEntityIdInParty( entityID );
		}
	}

	internal static bool ObjectIsAllianceMember( uint entityID )
	{
		if( entityID is 0 or 0xE0000000 ) return false;
		if( GroupManager.Instance() is null ) return false;
		if( Service.Condition[ConditionFlag.DutyRecorderPlayback] )
		{
			if( GroupManager.Instance()->ReplayGroup.IsEntityIdInParty( entityID ) ) return false;
			return GroupManager.Instance()->ReplayGroup.IsEntityIdInAlliance( entityID );
		}
		else
		{
			if( GroupManager.Instance()->MainGroup.IsEntityIdInParty( entityID ) ) return false;
			return GroupManager.Instance()->MainGroup.IsEntityIdInAlliance( entityID );
		}
	}
}
