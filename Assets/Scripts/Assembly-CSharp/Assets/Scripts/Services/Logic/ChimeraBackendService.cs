using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ABH.Shared.DTOs;
using ABH.Shared.Generic;
using Chimera.Library.Components.Interfaces;
using Chimera.Library.Components.Services;
using UnityEngine;

namespace Assets.Scripts.Services.Logic
{
	public class ChimeraBackendService
	{
		private string m_serverUrlTemplate = "https://{environment}.abh.chimeraws.com/api/{api_version}/";

		private string m_apiVersion;

		private string m_serverEnvironment;

		private ISerializer m_serializer;

		private string m_authToken;

		private bool m_initialized;

		private List<ServerCallQueueInfo> m_serverCallQueue;

		private string m_cachedServerBaseUrl;

		private string m_serverBaseUrl
		{
			get
			{
				if (!string.IsNullOrEmpty(m_cachedServerBaseUrl))
				{
					return m_cachedServerBaseUrl;
				}
				string newValue = m_serverEnvironment;
				if (m_serverEnvironment.ToLower().Contains("live"))
				{
					newValue = m_serverEnvironment + "-" + m_apiVersion.Replace('.', '-');
				}
				m_cachedServerBaseUrl = m_serverUrlTemplate.Replace("{environment}", newValue).Replace("{api_version}", m_apiVersion);
				return m_cachedServerBaseUrl;
			}
		}

		public string AuthToken
		{
			get
			{
				return m_authToken;
			}
		}

		private string GetServiceRequestEndpoint(BaseRequestDto request)
		{
			DebugLog.Log(GetType(), "GetServiceRequestEndpoint for : " + request.GetType());
			StringBuilder stringBuilder = new StringBuilder();
			if (request.GetType() == typeof(GetEventLeaderboardRequestDto))
			{
				return "event/leaderboard";
			}
			if (request.GetType() == typeof(GetBossDefeatLogRequestDto))
			{
				return "event/boss";
			}
			if (request.GetType() == typeof(AuthRequestDto))
			{
				return "auth";
			}
			if (request.GetType() == typeof(AddEventScoreRequestDto))
			{
				return "event/score";
			}
			if (request.GetType() == typeof(TrackBossDefeatRequestDto))
			{
				return "event/boss";
			}
			if (request.GetType() == typeof(AddPvpScoreRequestDto))
			{
				return "pvp/score";
			}
			if (request.GetType() == typeof(GetPvpLeaderboardRequestDto))
			{
				return "pvp/leaderboard";
			}
			return "status/";
		}

		public bool Init()
		{
			DebugLog.Log(GetType(), "Init: reading api version file and setting endpoint...");
			TextAsset textAsset = Resources.Load("server_api_version") as TextAsset;
			if (textAsset != null)
			{
				m_apiVersion = textAsset.text.Trim().TrimEnd('\r', '\n');
				m_serverEnvironment = "live";
				if (m_serverEnvironment == "dev" || m_serverEnvironment == "test")
				{
					m_serializer = new StringSerializerNewtonSoftImpl();
				}
				else
				{
					m_serializer = DIContainerInfrastructure.GetBinarySerializer();
				}
				m_serverCallQueue = new List<ServerCallQueueInfo>();
				m_initialized = true;
				return true;
			}
			DebugLog.Error(GetType(), "Init: server_api_version.txt file not found!");
			return false;
		}

		public IEnumerator SendRequest<TResponse>(BaseRequestDto dto, HttpMethods method, Action<TResponse> successHandler = null, Action<int> errorHandler = null) where TResponse : BaseResponseDto
		{
			DebugLog.Log(GetType(), string.Concat("SendRequest: ", method, " ", dto.GetType()));
			if (!m_initialized)
			{
				DebugLog.Error(GetType(), "SendRequest: Not yet initialized!");
			}
			else
			{
				if (m_serializer == null)
				{
					yield break;
				}
				StringBuilder serviceRequestUrl = new StringBuilder(m_serverBaseUrl).Append(GetServiceRequestEndpoint(dto));
				byte[] postArg = null;
				switch (method)
				{
				case HttpMethods.POST:
					try
					{
						if (m_serverEnvironment == "dev" || m_serverEnvironment == "test")
						{
							string banana = m_serializer.Serialize(dto);
							postArg = Encoding.ASCII.GetBytes(banana.ToCharArray());
						}
						else
						{
							postArg = m_serializer.SerializeToBytes(dto);
						}
					}
					catch (Exception ex)
					{
						Exception e = ex;
						DebugLog.Error(e);
						yield break;
					}
					break;
				case HttpMethods.GET:
					AppendGetParameters(dto, ref serviceRequestUrl);
					break;
				}
				Dictionary<string, string> headers = GetHeaders();
				if (string.IsNullOrEmpty(m_authToken) && dto.GetType() != typeof(AuthRequestDto))
				{
					DebugLog.Warn(GetType(), "GetHeaders: No auth token for backend found!! Need to authenticate user first!");
					yield break;
				}
				if (headers != null)
				{
					Stopwatch swatch = new Stopwatch();
					swatch.Start();
					using (WWW www = new WWW(serviceRequestUrl.ToString(), postArg, headers))
					{
						yield return www;
						swatch.Stop();
						Dictionary<string, string> responseTimeParams = new Dictionary<string, string>
						{
							{
								"ResponseTime",
								swatch.ElapsedMilliseconds.ToString()
							},
							{
								"ServiceEndpoint",
								serviceRequestUrl.ToString()
							}
						};
						DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("ServerRequest", responseTimeParams);
						if (!string.IsNullOrEmpty(www.error))
						{
							DebugLog.Error(GetType(), www.error);
							if (errorHandler != null)
							{
								errorHandler(1);
							}
						}
						else
						{
							DeserializeResponse(successHandler, errorHandler, www);
						}
						yield break;
					}
				}
				DebugLog.Error(GetType(), "SendRequest: Could not create request headers!");
			}
		}

		private void AppendGetParameters(BaseRequestDto dto, ref StringBuilder serviceRequestUrl)
		{
			if (serviceRequestUrl == null)
			{
				return;
			}
			Type type = dto.GetType();
			serviceRequestUrl.Append("?");
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanRead)
				{
					if (propertyInfo.GetIndexParameters().Length == 0)
					{
						string text = WWW.EscapeURL(propertyInfo.Name);
						string text2 = WWW.EscapeURL(propertyInfo.GetValue(dto, null) as string);
						serviceRequestUrl.Append(text + "=" + text2 + "&");
					}
					else
					{
						DebugLog.Error("INDEXED TYPE: " + propertyInfo.Name + " not added to GET parameters.");
					}
				}
			}
		}

		private void DeserializeResponse<TResponse>(Action<TResponse> successHandler, Action<int> errorHandler, WWW response) where TResponse : BaseResponseDto
		{
			TResponse val = (TResponse)null;
			try
			{
				val = m_serializer.Deserialize<TResponse>(response.bytes);
			}
			catch (Exception ex)
			{
				StringBuilder stringBuilder = new StringBuilder(GetType().ToString());
				stringBuilder.Append(" SendRequest: Failed to deserialize content! ");
				stringBuilder.AppendLine(string.Concat(ex.GetType(), " ", ex.Message));
				DebugLog.Error(stringBuilder);
			}
			if (val != null)
			{
				DebugLog.Log(GetType(), "SendRequest: Server Response acquired: " + val.Result);
				if (val.Result == RESTResultEnum.Success && successHandler != null)
				{
					successHandler(val);
					return;
				}
				ServerFailHandler(val.Result);
				if (errorHandler != null)
				{
					errorHandler((int)val.Result);
				}
			}
			else
			{
				DebugLog.Warn(GetType(), "SendRequest: Empty Response from server!");
			}
		}

		private void ServerFailHandler(RESTResultEnum responseResult)
		{
			RESTResultEnum rESTResultEnum = responseResult;
			if (rESTResultEnum != RESTResultEnum.Success)
			{
				DebugLog.Error(GetType(), "ServerFailHandler: " + responseResult);
			}
		}

		private void StartAsynchPostCall<TRequest, TResponse>(TRequest request, Action<TResponse> onSuccess, Action<int> onError) where TRequest : BaseRequestDto where TResponse : BaseResponseDto
		{
			DebugLog.Log(GetType(), "StartAsynchPostCall");
			CoreStateMgr coreStateMgr = DIContainerInfrastructure.GetCoreStateMgr();
			coreStateMgr.StartCoroutine(SendRequest(request, HttpMethods.POST, onSuccess, onError));
		}

		private void StartAsynchGetCall<TRequest, TResponse>(TRequest request, Action<TResponse> onSuccess, Action<int> onError) where TRequest : BaseRequestDto where TResponse : BaseResponseDto
		{
			DebugLog.Log(GetType(), "StartAsynchGetCall");
			CoreStateMgr coreStateMgr = DIContainerInfrastructure.GetCoreStateMgr();
			coreStateMgr.StartCoroutine(SendRequest(request, HttpMethods.GET, onSuccess, onError));
		}

		private TRequest CreateDto<TRequest>(TRequest dto) where TRequest : BaseRequestDto
		{
			dto.v = m_apiVersion;
			return dto;
		}

		private Dictionary<string, string> GetHeaders()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Content-Type", "application/x-protobuf");
			dictionary.Add("PlayerId", DIContainerInfrastructure.IdentityService.SharedId);
			// dictionary.Add("RovioAccessToken", ContentLoader.Instance.m_BeaconConnectionMgr.Identiy.GetAccessToken());
			dictionary.Add("RovioAccessToken", "access-token");
			dictionary.Add("AuthToken", m_authToken ?? string.Empty);
			return dictionary;
		}

		public void Authenticate(Action<AuthResponseDto> onSuccess, Action<int> onError)
		{
			AuthRequestDto request = CreateDto(new AuthRequestDto());
			StartAsynchGetCall(request, delegate(AuthResponseDto response)
			{
				OnAuthSuccess(response);
				if (onSuccess != null)
				{
					onSuccess(response);
				}
			}, onError);
		}

		private void OnAuthSuccess(AuthResponseDto response)
		{
			foreach (ServerCallQueueInfo item in m_serverCallQueue)
			{
				DebugLog.Log(GetType(), "OnAuthSuccess: Found queued server request for " + item.requestDto.GetType());
			}
			DIContainerInfrastructure.IdentityService.OnLoggedIn -= InvalidateAndRefreshAuth;
			DIContainerInfrastructure.IdentityService.OnLoggedIn += InvalidateAndRefreshAuth;
			m_authToken = response.PlayerToken;
		}

		private void InvalidateAndRefreshAuth()
		{
			m_authToken = string.Empty;
			Authenticate(null, null);
		}

		public void AddEventScore(string boardName, long score, Action onSuccess, Action<int> onError)
		{
			AddEventScoreRequestDto request = CreateDto(new AddEventScoreRequestDto
			{
				Score = (int)score
			});
			StartAsynchPostCall<AddEventScoreRequestDto, AddEventScoreResponseDto>(request, delegate
			{
				onSuccess();
			}, onError);
		}

		public void AddEventScore(string boardName, long score, int matchmakingScore, bool isBossEvent, uint eventEndTime, Action<AddEventScoreResponseDto> onSuccess, Action<int> onError)
		{
			AddEventScoreRequestDto request = CreateDto(new AddEventScoreRequestDto
			{
				Score = (int)score,
				IsBossEvent = isBossEvent,
				HatchEventLevel = boardName,
				MatchMakingScore = matchmakingScore
			});
			StartAsynchPostCall(request, onSuccess, onError);
		}

		public void GetBossDefeatLog(string leaderboardId, Action<GetBossDefeatLogResponseDto> onSuccess, Action<int> onError)
		{
			GetBossDefeatLogRequestDto request = CreateDto(new GetBossDefeatLogRequestDto
			{
				EventLeaderboardId = leaderboardId
			});
			StartAsynchGetCall(request, onSuccess, onError);
		}

		public void GetEventLeaderboard(string leaderboardId, Action<GetLeaderboardResponseDto> onSuccess, Action<int> onError)
		{
			GetEventLeaderboardRequestDto request = CreateDto(new GetEventLeaderboardRequestDto
			{
				LeaderboardId = leaderboardId
			});
			StartAsynchGetCall(request, onSuccess, onError);
		}

		public void GetPvpLeaderboard(string leaderboardId, Action<GetLeaderboardResponseDto> onSuccess, Action<int> onError)
		{
			GetPvpLeaderboardRequestDto request = CreateDto(new GetPvpLeaderboardRequestDto
			{
				LeaderboardId = leaderboardId
			});
			StartAsynchGetCall(request, onSuccess, onError);
		}

		public void AddPvpScore(string seasonName, int turn, PvpLeague league, long score, int matchmakingScore, Action<AddPvpScoreResponseDto> onSuccess, Action<int> onError)
		{
			AddPvpScoreRequestDto request = CreateDto(new AddPvpScoreRequestDto
			{
				Score = (int)score,
				HatchScoreLevel = GetPvpHatchScorelevelName(seasonName, turn),
				MatchMakingScore = matchmakingScore,
				League = league
			});
			StartAsynchPostCall(request, onSuccess, onError);
		}

		public void TrackBossDefeat(string leaderboardId, Action<TrackBossDefeatResponseDto> onSuccess, Action<int> onError)
		{
			TrackBossDefeatRequestDto request = CreateDto(new TrackBossDefeatRequestDto
			{
				EventLeaderboardId = leaderboardId
			});
			StartAsynchPostCall(request, onSuccess, onError);
		}

		public string GetPvpHatchScorelevelName(string seasonName, int turn)
		{
			return string.Format("{0}_turn_{1}", seasonName, turn.ToString("00"));
		}
	}
}
