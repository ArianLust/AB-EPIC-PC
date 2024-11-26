using System;
using ABH.Shared.DTOs;

namespace Assets.Scripts.Services.Logic
{
	public struct ServerCallQueueInfo
	{
		public BaseRequestDto requestDto;

		public HttpMethods method;

		public Action<BaseRequestDto> successHandler;

		public Action<int> errorHandler;
	}
}
