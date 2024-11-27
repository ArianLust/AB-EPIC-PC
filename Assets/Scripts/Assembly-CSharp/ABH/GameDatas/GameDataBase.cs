using ABH.GameDatas.Interfaces;
using ABH.Shared.Interfaces;
using Chimera.Library.Components.Interfaces;

namespace ABH.GameDatas
{
	public abstract class GameDataBase<TBalancingData, TData> : IGameData<TBalancingData, TData> where TBalancingData : class, IBalancingData where TData : IData
	{
		protected TBalancingData _balancingData;

		protected TData _instancedData;

		public TBalancingData BalancingData
		{
			get
			{
				return _balancingData;
			}
		}

		public TData Data
		{
			get
			{
				return _instancedData;
			}
		}

		public GameDataBase()
		{
		}

		public GameDataBase(TData instancedData)
		{
			_instancedData = instancedData;
			if (!string.IsNullOrEmpty(instancedData.NameId))
			{
				_balancingData = DIContainerBalancing.Service.GetBalancingData<TBalancingData>(instancedData.NameId);
			}
		}

		public GameDataBase(string nameId)
		{
			_balancingData = DIContainerBalancing.Service.GetBalancingData<TBalancingData>(nameId);
			_instancedData = CreateNewInstance(nameId);
		}

		public GameDataBase(TBalancingData balancingData)
		{
			_balancingData = balancingData;
			_instancedData = CreateNewInstance(balancingData.NameId);
		}

		public GameDataBase(TBalancingData balancingData, TData instancedData)
		{
			_instancedData = instancedData;
			_balancingData = balancingData;
		}

		protected abstract TData CreateNewInstance(string nameId);
	}
	public abstract class GameDataBase<TData> : GameDataBase<IBalancingData, TData> where TData : IData
	{
	}
}
