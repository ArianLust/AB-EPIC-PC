using System;
using System.Collections.Generic;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Interfaces
{
	public interface ICharacter
	{
		float BaseAttack { get; }

		float BaseHealth { get; }

		ClassItemGameData ClassItem { get; }

		InventoryGameData InventoryGameData { get; }

		EquipmentGameData MainHandItem { get; }

		EquipmentGameData OffHandItem { get; }

		List<SkillGameData> Skills { get; }

		Dictionary<string, LootInfoData> DefeatLoot { get; }

		float Scale { get; }

		string Name { get; }

		string AssetName { get; }

		CharacterSizeType CharacterSize { get; }

		Faction CharacterFaction { get; }

		int Level { get; }

		List<AiCombo> SkillCombos { get; }

		bool IsNPC { get; set; }

		bool IsPvPBird { get; set; }

		event Action ClassItemChanged;

		event Action<int, int> LevelChanged;

		void RaiseLevelChanged(int oldLevel, int newLevel);
	}
}
