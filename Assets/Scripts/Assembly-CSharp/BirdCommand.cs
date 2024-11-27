using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;

public class BirdCommand
{
	public ICombatant m_Source;

	public SkillBattleDataBase m_UsedSkill;

	public ICombatant m_Target;

	public int m_Sortorder;
}
