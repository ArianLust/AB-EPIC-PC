using ABH.Shared.Interfaces;
using Chimera.Library.Components.Interfaces;
using ProtoBuf;

namespace ABH.GameDatas.Interfaces
{
	[ProtoContract]
	public interface IGameData<out TBalancingData, out TInstancedData> where TBalancingData : IBalancingData where TInstancedData : IData
	{
		TBalancingData BalancingData { get; }

		TInstancedData Data { get; }
	}
}
