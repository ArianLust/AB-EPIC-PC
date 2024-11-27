using System;
using System.Linq;
using System.Threading;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Services.Logic
{
	public class BattleServiceNullImpl
	{
		public class BattleAsyncResult : IAsyncResult
		{
			private object state;

			private Type returnType;

			public bool IsCompleted
			{
				get
				{
					return false;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public Type ReturnType
			{
				get
				{
					return returnType;
				}
			}

			public object AsyncState
			{
				get
				{
					return state;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public BattleAsyncResult(object state, Type returnType)
			{
				this.state = state;
				this.returnType = returnType;
			}
		}

		public BattleGameData GenerateBattleAtHotspot(PlayerGameData player, HotspotGameData hotspot)
		{
			return GenerateBattle(player, hotspot.BalancingData.BattleId.FirstOrDefault());
		}

		public BattleGameData GenerateBattle(PlayerGameData player, string battleId)
		{
			BattleGameData battleGameData = new BattleGameData(battleId);
			battleGameData.m_BattleEndData = new BattleEndGameData
			{
				m_WinnerFaction = Faction.Birds,
				m_BattlePerformanceStars = UnityEngine.Random.Range(1, 4)
			};
			return battleGameData;
		}

		public IAsyncResult BeginBattle(BattleGameData battledata, AsyncCallback callback)
		{
			IAsyncResult asyncResult = new BattleAsyncResult(battledata.m_BattleEndData, typeof(BattleEndGameData));
			battledata.CallbackWhenDone = callback;
			battledata.CallbackWhenDone(asyncResult);
			return asyncResult;
		}

		public BattleEndGameData EndBattle(IAsyncResult result)
		{
			return result.AsyncState as BattleEndGameData;
		}
	}
}
