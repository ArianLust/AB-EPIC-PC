using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using Interfaces.Identity;
using UnityEngine;

namespace ABH.GameDatas
{
	public class SocialEnvironmentGameData : GameDataBase<SocialEnvironmentBalancingData, SocialEnvironmentData>
	{
		public Dictionary<string, PublicPlayerData> PublicPlayerDatas = new Dictionary<string, PublicPlayerData>();

		public Dictionary<int, string> StoryProgressHotspotIds = new Dictionary<int, string>();

		public Dictionary<string, FriendGameData> Friends = new Dictionary<string, FriendGameData>();

		public Dictionary<string, int> FailedMessagesCount = new Dictionary<string, int>();

		public Dictionary<string, IMailboxMessageGameData> MailboxMessages = new Dictionary<string, IMailboxMessageGameData>();

		public List<IMailboxMessageGameData> PendingMailboxMessages = new List<IMailboxMessageGameData>();

		public bool ReceivedPlayersOnce;

		private IdentityCredentials m_CurrentCredentials;

		public string MatchmakingPlayerName
		{
			get
			{
				if (string.IsNullOrEmpty(Data.MatchmakingPlayerName))
				{
					return "unknown";
				}
				return Data.MatchmakingPlayerName;
			}
			set
			{
				Data.MatchmakingPlayerName = value;
			}
		}

		public string IdLoginEmail
		{
			get
			{
				return Data.IdLoginEmail;
			}
			set
			{
				Data.IdLoginEmail = value;
			}
		}

		public string IdPassword
		{
			get
			{
				return DecryptPassword(Data.IdPassword);
			}
			set
			{
				Data.IdPassword = EncryptPassword(value);
			}
		}

		public IdentityCredentials CurrentCredentials
		{
			get
			{
				if (m_CurrentCredentials != null && !m_CurrentCredentials.IsEmpty() && m_CurrentCredentials.UserName == IdLoginEmail)
				{
					return m_CurrentCredentials;
				}
				m_CurrentCredentials = new IdentityCredentials
				{
					UserName = IdLoginEmail,
					Password = IdPassword
				};
				return m_CurrentCredentials;
			}
			set
			{
				m_CurrentCredentials = value;
				IdLoginEmail = m_CurrentCredentials.UserName;
				IdPassword = m_CurrentCredentials.Password;
			}
		}

		[method: MethodImpl(32)]
		public event Action<IMailboxMessageGameData> MessageAdded;

		[method: MethodImpl(32)]
		public event Action<IMailboxMessageGameData> MessageRemoved;

		[method: MethodImpl(32)]
		public event Action<FriendGameData> FriendAdded;

		[method: MethodImpl(32)]
		public event Action<List<FriendGameData>> AcceptedFriendsReceived;

		[method: MethodImpl(32)]
		public event Action<List<FriendData>> InvitableFriendsReceived;

		[method: MethodImpl(32)]
		public event Action<FriendGameData> FriendRemoved;

		[method: MethodImpl(32)]
		public event Action<List<string>> FriendIdsReceived;

		[method: MethodImpl(32)]
		public event Action PublicPlayersRefreshed;

		public SocialEnvironmentGameData(string nameId)
			: base(nameId)
		{
			InitGameData(Data);
		}

		public SocialEnvironmentGameData(SocialEnvironmentData instance)
			: base(instance)
		{
			InitGameData(instance);
		}

		private void InitGameData(SocialEnvironmentData instance)
		{
			if (instance.PublicPlayerInstances == null)
			{
				instance.PublicPlayerInstances = new List<PublicPlayerData>();
			}
			if (instance.FriendShipGateUnlocks == null)
			{
				instance.FriendShipGateUnlocks = new Dictionary<string, List<string>>();
			}
			if (instance.NewFriendShipGateUnlocks == null)
			{
				instance.NewFriendShipGateUnlocks = new Dictionary<string, List<string>>();
			}
			if (instance.LastMessagingCursor == null)
			{
				instance.LastMessagingCursor = string.Empty;
			}
			if (instance.IdLoginEmail == null)
			{
				IdLoginEmail = string.Empty;
			}
			if (instance.IdPassword == null)
			{
				IdPassword = string.Empty;
			}
			if (instance.AcceptedFriendIds != null)
			{
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				foreach (string acceptedFriendId in instance.AcceptedFriendIds)
				{
					uint value = 0u;
					bool freeGachaRoll = false;
					bool freePvpGachaRoll = false;
					dictionary.Add(acceptedFriendId, false);
					if (instance.FreeGachaRollFriendIds == null)
					{
						instance.FreeGachaRollFriendIds = new List<string>();
					}
					if (instance.FreeGachaRollFriendIds.Contains(acceptedFriendId))
					{
						freeGachaRoll = true;
						dictionary[acceptedFriendId] = true;
					}
					if (instance.FreePvpGachaRollFriendIds == null)
					{
						instance.FreePvpGachaRollFriendIds = new List<string>();
					}
					if (instance.FreePvpGachaRollFriendIds.Contains(acceptedFriendId))
					{
						freePvpGachaRoll = true;
						dictionary[acceptedFriendId] = true;
					}
					if (!instance.GetBirdCooldowns.TryGetValue(acceptedFriendId, out value))
					{
						instance.GetBirdCooldowns.Add(acceptedFriendId, 0u);
					}
					foreach (KeyValuePair<string, bool> item in dictionary)
					{
						if (!item.Value)
						{
							DebugLog.Log(GetType(), "InitGameData: Removing unfriended friend: " + acceptedFriendId);
							if (instance.FreeGachaRollFriendIds != null)
							{
								instance.FreeGachaRollFriendIds.Remove(item.Key);
							}
							if (instance.GetFreeGachaRollCooldowns != null)
							{
								instance.GetFreeGachaRollCooldowns.Remove(item.Key);
							}
							if (instance.FreePvpGachaRollFriendIds != null)
							{
								instance.FreePvpGachaRollFriendIds.Remove(item.Key);
							}
							if (instance.GetFreePvpGachaRollCooldowns != null)
							{
								instance.GetFreePvpGachaRollCooldowns.Remove(item.Key);
							}
						}
					}
					DebugLog.Log("Added FriendGameData for Id: " + acceptedFriendId);
					Friends.Add(acceptedFriendId, new FriendGameData(acceptedFriendId).SetLastBirdRequest(value).SetFreeGachaRoll(freeGachaRoll).SetFreePvpGachaRoll(freePvpGachaRoll));
				}
				instance.PublicPlayerInstances = new List<PublicPlayerData>
				{
					DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetPorkyFriend(1)),
					DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetAdventurerFriend(1)),
					DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetLowNPCFriend(1)),
					DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetHighNPCFriend(1))
				};
				foreach (PublicPlayerData publicPlayerInstance in instance.PublicPlayerInstances)
				{
					DebugLog.Log("Added public Profile for Id: " + publicPlayerInstance.SocialId);
					if (publicPlayerInstance.LocationProgress == null)
					{
						DebugLog.Error("No Location Progress! For player: " + publicPlayerInstance.SocialId);
					}
					if (!PublicPlayerDatas.ContainsKey(publicPlayerInstance.SocialId) && Friends.ContainsKey(publicPlayerInstance.SocialId))
					{
						PublicPlayerDatas.Add(publicPlayerInstance.SocialId, publicPlayerInstance);
					}
					else
					{
						DebugLog.Error("Invalid Public Player found: " + publicPlayerInstance.SocialId);
					}
				}
				foreach (FriendData nPCFriend in GetNPCFriends())
				{
					FriendGameData value2 = null;
					if (Friends.TryGetValue(nPCFriend.Id, out value2))
					{
						value2.SetFriendData(nPCFriend);
						if (value2.isNpcFriend && !value2.IsFriendBirdLoaded)
						{
							value2.SetFriendBird(new BirdGameData(DIContainerLogic.SocialService.GetRandomBirdWithLevel(value2.FriendLevel, string.Empty)));
						}
					}
				}
			}
			else
			{
				instance.AcceptedFriendIds = new List<string>();
			}
			if (instance.Messages != null)
			{
				foreach (MessageDataIncoming message in instance.Messages)
				{
					DebugLog.Log(string.Concat("[SocialEnvironmentGameData] calling MailboxMessages.Add(message.Id, DIContainerLogic.SocialService.GenerateMessageGameData(Id: ", message.Id, ", type: ", message.MessageType, "));"));
					MailboxMessages.Add(message.Id, DIContainerLogic.SocialService.GenerateMessageGameData(message));
				}
			}
			else
			{
				instance.Messages = new List<MessageDataIncoming>();
			}
			if (instance.GetFreeGachaRollCooldowns == null)
			{
				instance.GetFreeGachaRollCooldowns = new Dictionary<string, uint>();
			}
			if (instance.GetFreePvpGachaRollCooldowns == null)
			{
				instance.GetFreePvpGachaRollCooldowns = new Dictionary<string, uint>();
			}
			if (instance.InvitedFriendIds == null)
			{
				instance.InvitedFriendIds = new List<string>();
			}
			if (instance.PendingFriendIds == null)
			{
				instance.PendingFriendIds = new List<string>();
			}
		}

		private List<FriendData> GetNPCFriends()
		{
			int level = 1;
			List<FriendData> list = new List<FriendData>();
			list.Add(DIContainerLogic.SocialService.GetPorkyFriend(level));
			list.Add(DIContainerLogic.SocialService.GetAdventurerFriend(level));
			list.Add(DIContainerLogic.SocialService.GetLowNPCFriend(level));
			return list;
		}

		public static void FillSocialEnvironmentDataIfEmpty(SocialEnvironmentData envData)
		{
			envData.GetFreeGachaRollCooldowns = new Dictionary<string, uint>();
			envData.GetFreePvpGachaRollCooldowns = new Dictionary<string, uint>();
			envData.LastGachaFreeRollSpawn = 0u;
			envData.LastPvpGachaFreeRollSpawn = 0u;
			envData.PublicPlayerInstances = new List<PublicPlayerData>
			{
				DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetPorkyFriend(1)),
				DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetAdventurerFriend(1)),
				DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetLowNPCFriend(1)),
				DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetHighNPCFriend(1))
			};
			envData.FreeGachaRollFriendIds = new List<string>();
			envData.FreePvpGachaRollFriendIds = new List<string>();
			envData.GetBirdCooldowns = new Dictionary<string, uint>
			{
				{
					DIContainerLogic.SocialService.GetPorkyFriend(1).Id,
					0u
				},
				{
					DIContainerLogic.SocialService.GetAdventurerFriend(1).Id,
					0u
				},
				{
					DIContainerLogic.SocialService.GetLowNPCFriend(1).Id,
					0u
				},
				{
					DIContainerLogic.SocialService.GetHighNPCFriend(1).Id,
					0u
				}
			};
			envData.Messages = new List<MessageDataIncoming>();
			envData.AcceptedFriendIds = new List<string>
			{
				DIContainerLogic.SocialService.GetPorkyFriend(1).Id,
				DIContainerLogic.SocialService.GetAdventurerFriend(1).Id,
				DIContainerLogic.SocialService.GetLowNPCFriend(1).Id,
				DIContainerLogic.SocialService.GetHighNPCFriend(1).Id
			};
			envData.InvitedFriendIds = new List<string>();
			envData.FriendShipGateUnlocks = new Dictionary<string, List<string>>();
			envData.NewFriendShipGateUnlocks = new Dictionary<string, List<string>>();
			envData.LocationProgress = new Dictionary<LocationType, int>
			{
				{
					LocationType.World,
					1
				},
				{
					LocationType.ChronicleCave,
					0
				}
			};
		}

		protected override SocialEnvironmentData CreateNewInstance(string nameId)
		{
			SocialEnvironmentData socialEnvironmentData = new SocialEnvironmentData();
			socialEnvironmentData.NameId = nameId;
			socialEnvironmentData.GetFreeGachaRollCooldowns = new Dictionary<string, uint>();
			socialEnvironmentData.GetFreePvpGachaRollCooldowns = new Dictionary<string, uint>();
			socialEnvironmentData.LastGachaFreeRollSpawn = 0u;
			socialEnvironmentData.LastPvpGachaFreeRollSpawn = 0u;
			socialEnvironmentData.SocialId = string.Empty;
			socialEnvironmentData.PublicPlayerInstances = new List<PublicPlayerData>
			{
				DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetPorkyFriend(1)),
				DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetAdventurerFriend(1)),
				DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetLowNPCFriend(1)),
				DIContainerLogic.SocialService.GetNPCPlayer(DIContainerLogic.SocialService.GetHighNPCFriend(1))
			};
			socialEnvironmentData.FreeGachaRollFriendIds = new List<string>();
			socialEnvironmentData.FreePvpGachaRollFriendIds = new List<string>();
			socialEnvironmentData.GetBirdCooldowns = new Dictionary<string, uint>
			{
				{
					DIContainerLogic.SocialService.GetPorkyFriend(1).Id,
					0u
				},
				{
					DIContainerLogic.SocialService.GetAdventurerFriend(1).Id,
					0u
				},
				{
					DIContainerLogic.SocialService.GetLowNPCFriend(1).Id,
					0u
				},
				{
					DIContainerLogic.SocialService.GetHighNPCFriend(1).Id,
					0u
				}
			};
			socialEnvironmentData.Messages = new List<MessageDataIncoming>();
			socialEnvironmentData.AcceptedFriendIds = new List<string>
			{
				DIContainerLogic.SocialService.GetPorkyFriend(1).Id,
				DIContainerLogic.SocialService.GetAdventurerFriend(1).Id,
				DIContainerLogic.SocialService.GetLowNPCFriend(1).Id,
				DIContainerLogic.SocialService.GetHighNPCFriend(1).Id
			};
			socialEnvironmentData.InvitedFriendIds = new List<string>();
			socialEnvironmentData.FriendShipGateUnlocks = new Dictionary<string, List<string>>();
			socialEnvironmentData.NewFriendShipGateUnlocks = new Dictionary<string, List<string>>();
			socialEnvironmentData.LocationProgress = new Dictionary<LocationType, int>
			{
				{
					LocationType.World,
					1
				},
				{
					LocationType.ChronicleCave,
					0
				}
			};
			return socialEnvironmentData;
		}

		public FriendGameData AddFriend(FriendGameData friend)
		{
			return friend;
		}

		public void RefreshFriendsBirds(Dictionary<string, BirdData> BirdPerFriend)
		{
			foreach (string key in BirdPerFriend.Keys)
			{
				FriendGameData value = null;
				if (Friends.TryGetValue(key, out value))
				{
					BirdGameData birdGameData = new BirdGameData(BirdPerFriend[key]);
					birdGameData.IsNPC = true;
					value.SetFriendBird(birdGameData);
				}
			}
		}

		public void RefreshFriends(List<FriendData> friendDatas)
		{
			List<FriendGameData> list = new List<FriendGameData>();
			List<FriendData> list2 = new List<FriendData>();
			foreach (FriendData friendData in friendDatas)
			{
				DebugLog.Log("Friend id " + friendData.Id);
				FriendGameData value = null;
				if (Friends.TryGetValue(friendData.Id, out value))
				{
					DebugLog.Log("Friend contained id " + friendData.Id);
					value.SetFriendData(friendData);
					list.Add(value);
				}
				else if (!Data.InvitedFriendIds.Contains(friendData.Id))
				{
					list2.Add(friendData);
				}
			}
			list = list.OrderByDescending((FriendGameData f) => f.FriendLevel).ToList();
			if (this.AcceptedFriendsReceived != null)
			{
				this.AcceptedFriendsReceived(list);
			}
			if (this.InvitableFriendsReceived != null)
			{
				this.InvitableFriendsReceived(list2);
			}
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_friendlist", string.Empty);
		}

		public bool GetFriendsSocialIds()
		{
			DIContainerInfrastructure.GetFacebookWrapper().receivedFriends -= ReceivedFriends;
			DIContainerInfrastructure.GetFacebookWrapper().receivedFriends += ReceivedFriends;
			return DIContainerInfrastructure.GetFacebookWrapper().GetFriendIds();
		}

		public void ReceivedFriends(List<string> friendIds)
		{
			Dictionary<string, FriendGameData> dictionary = new Dictionary<string, FriendGameData>(Friends);
			Friends.Clear();
			foreach (string key in dictionary.Keys)
			{
				FriendGameData friendGameData = dictionary[key];
				if (friendGameData.isNpcFriend)
				{
					Friends.Add(key, friendGameData);
				}
			}
			DebugLog.Log("[SocialEnvironmentGameData] ReceivedFriends: received " + friendIds.Count + " friendIds, already have " + Friends.Count + " NPCs, " + (dictionary.Count - Friends.Count) + " Players");
			foreach (string friendId in friendIds)
			{
				FriendGameData value = null;
				if (dictionary.TryGetValue(friendId, out value))
				{
					Friends.Add(friendId, value);
				}
				else
				{
					Friends.Add(friendId, new FriendGameData(friendId));
				}
			}
			if (this.FriendIdsReceived != null)
			{
				this.FriendIdsReceived(friendIds);
			}
			dictionary.Clear();
			DIContainerInfrastructure.RemoteStorageService.RefreshPublicPlayerDataFromFriends(friendIds);
		}

		public void RefreshPublicPlayerDatas(Dictionary<string, PublicPlayerData> publicPlayerDictionary, List<string> friendsToRemove)
		{
			if (publicPlayerDictionary == null)
			{
				return;
			}
			DebugLog.Log("Refresh public Player Datas, there are " + publicPlayerDictionary.Count + " entries of public player profiles, " + friendsToRemove.Count + " to remove");
			PublicPlayerDatas.Clear();
			foreach (string item in friendsToRemove)
			{
				if (Friends != null)
				{
					Friends.Remove(item);
				}
				if (Data.FreeGachaRollFriendIds != null)
				{
					Data.FreeGachaRollFriendIds.Remove(item);
				}
				if (Data.FreePvpGachaRollFriendIds != null)
				{
					Data.FreePvpGachaRollFriendIds.Remove(item);
				}
				if (Data.GetFreeGachaRollCooldowns != null)
				{
					Data.GetFreeGachaRollCooldowns.Remove(item);
				}
				if (Data.GetFreePvpGachaRollCooldowns != null)
				{
					Data.GetFreePvpGachaRollCooldowns.Remove(item);
				}
				DebugLog.Error("Removed invalid Friend from Friends List: " + item);
			}
			foreach (PublicPlayerData publicPlayerInstance in Data.PublicPlayerInstances)
			{
				if (!PublicPlayerDatas.ContainsKey(publicPlayerInstance.SocialId))
				{
					PublicPlayerDatas.Add(publicPlayerInstance.SocialId, publicPlayerInstance);
				}
			}
			foreach (string key in publicPlayerDictionary.Keys)
			{
				PublicPlayerData publicPlayerData = publicPlayerDictionary[key];
				string socialId = publicPlayerData.SocialId;
				if (!PublicPlayerDatas.ContainsKey(key))
				{
					publicPlayerData.SocialId = key;
					PublicPlayerDatas.Add(key, publicPlayerData);
				}
				DebugLog.Log(GetType(), "Final Public Player Data: " + publicPlayerData.SocialId + ", old: " + socialId + " Name: " + publicPlayerData.SocialPlayerName + " Level: " + publicPlayerData.Level);
			}
			ReceivedPlayersOnce = true;
			if (this.PublicPlayersRefreshed != null)
			{
				this.PublicPlayersRefreshed();
			}
		}

		public bool RemoveFriend(FriendGameData friend)
		{
			if (this.FriendRemoved != null)
			{
				this.FriendRemoved(friend);
			}
			Data.GetBirdCooldowns.Remove(friend.FriendId);
			Data.FreeGachaRollFriendIds.Remove(friend.FriendId);
			Data.FreePvpGachaRollFriendIds.Remove(friend.FriendId);
			PublicPlayerData value = null;
			if (PublicPlayerDatas.TryGetValue(friend.FriendId, out value))
			{
				Data.PublicPlayerInstances.Remove(value);
				PublicPlayerDatas.Remove(friend.FriendId);
			}
			return Friends.Remove(friend.FriendId) && Data.AcceptedFriendIds.Remove(friend.FriendId);
		}

		public void AddIncomingMessages(IEnumerable<MessageDataIncoming> messages)
		{
			RefreshMessageLimitCounts();
			foreach (MessageDataIncoming message in messages)
			{
				AddIncomingMessage(message);
			}
		}

		private void RefreshMessageLimitCounts()
		{
			DateTime minValue = DateTime.MinValue;
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				return;
			}
			try
			{
				DebugLog.Log("Resets limit cap: " + Data.FriendShipEssenceMessageCapResetTime);
				minValue = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(Data.FriendShipEssenceMessageCapResetTime);
				if (trustedTime > new DateTime(minValue.Year, minValue.Month, minValue.Day + 1))
				{
					DebugLog.Log("Resets limit of essence messages.");
					Data.FriendShipEssenceMessageCapResetTime = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
					Data.FriendShipEssenceMessageCapCount = 0;
					Data.FriendShipEssenceMessageByBirdCapCount = 0;
				}
				else
				{
					DebugLog.Log("Time left to reset limit of essence messages : " + (new DateTime(minValue.Year, minValue.Month, minValue.Day + 1) - trustedTime));
				}
			}
			catch
			{
				Data.FriendShipEssenceMessageCapResetTime = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
				Data.FriendShipEssenceMessageCapCount = 0;
				Data.FriendShipEssenceMessageByBirdCapCount = 0;
			}
		}

		public bool GetAndSetFriendshipGateLockState(HotspotGameData friendshipGate)
		{
			bool friendshipGateLockState = GetFriendshipGateLockState(friendshipGate);
			if (!friendshipGateLockState)
			{
				if (friendshipGate.Data.UnlockState < HotspotUnlockState.ResolvedNew)
				{
					friendshipGate.Data.UnlockState = HotspotUnlockState.ResolvedNew;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
				ABHAnalyticsHelper.AddFriendsCountToTracking(dictionary);
				dictionary.Add("FriendshipGateName", friendshipGate.BalancingData.NameId);
				int num = 0;
				if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks != null && DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.ContainsKey(friendshipGate.BalancingData.NameId))
				{
					num = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks[friendshipGate.BalancingData.NameId].Count((string f) => f.Contains("NPC_"));
				}
				dictionary.Add("NPCFriendsUsed", num.ToString("0"));
				DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("FriendshipGateUnlocked", dictionary);
			}
			return friendshipGateLockState;
		}

		private static bool GetFriendshipGateLockState(HotspotGameData friendshipGate)
		{
			Requirement firstFailedReq = null;
			if (!DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), friendshipGate, out firstFailedReq) && firstFailedReq.RequirementType == RequirementType.UsedFriends)
			{
				return true;
			}
			return false;
		}

		public IMailboxMessageGameData AddIncomingMessage(MessageDataIncoming message)
		{
			IMailboxMessageGameData mailboxMessageGameData = DIContainerLogic.SocialService.GenerateMessageGameData(message);
			if (MailboxMessages.Values.Any((IMailboxMessageGameData m) => m.IsEqual(message)))
			{
				DebugLog.Log("Equal Message found!");
				return mailboxMessageGameData;
			}
			DebugLog.Log(string.Format("<b>AddMessage: {0}, sent at {1}</b>", message.MessageType, message.SentAt));
			if (mailboxMessageGameData == null || !mailboxMessageGameData.IsAddMessageAllowed(this))
			{
				DebugLog.Log("Message add not allowed!");
				return mailboxMessageGameData;
			}
			if (!DIContainerLogic.SocialService.AreMessageAddRequirementsFullfilled(DIContainerInfrastructure.GetCurrentPlayer(), mailboxMessageGameData))
			{
				if (!PendingMailboxMessages.Contains(mailboxMessageGameData))
				{
					DebugLog.Log("Added pending Message: " + mailboxMessageGameData.Id);
					PendingMailboxMessages.Add(mailboxMessageGameData);
				}
				return mailboxMessageGameData;
			}
			if (!MailboxMessages.ContainsKey(message.Id))
			{
				MailboxMessages.Add(message.Id, mailboxMessageGameData);
				Data.Messages.Add(message);
			}
			else
			{
				DebugLog.Error("Message ID duplicated: " + message.Id + " of type: " + message.MessageType);
				MailboxMessages[message.Id] = mailboxMessageGameData;
			}
			DebugLog.Log("Added Message: " + message.Id + " of type: " + message.MessageType);
			if (this.MessageAdded != null)
			{
				this.MessageAdded(MailboxMessages[message.Id]);
			}
			MailboxMessages[message.Id].OnAddMessage();
			return MailboxMessages[message.Id];
		}

		public bool RemoveMessageFromUI(IMailboxMessageGameData messageGameData)
		{
			if (this.MessageRemoved != null)
			{
				this.MessageRemoved(messageGameData);
			}
			return true;
		}

		public bool RemoveMessage(IMailboxMessageGameData messageGameData)
		{
			if (!MailboxMessages.Remove(messageGameData.Id) || !Data.Messages.Remove(Data.Messages.FirstOrDefault((MessageDataIncoming m) => m.Id == messageGameData.Id)))
			{
				return false;
			}
			if (this.MessageRemoved != null)
			{
				this.MessageRemoved(messageGameData);
			}
			return true;
		}

		private string EncryptPassword(string password)
		{
			return DIContainerInfrastructure.CryptographyService.EncryptString(password);
		}

		private string DecryptPassword(string encryptedPassword)
		{
			return DIContainerInfrastructure.CryptographyService.DecryptString(encryptedPassword);
		}

		public void SetNewPiggieMcCoolDate(MessageType type)
		{
			int mcCoolVisitMinCooldown = DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").McCoolVisitMinCooldown;
			int mcCoolVisitMaxCooldown = DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").McCoolVisitMaxCooldown;
			DateTime presentTime = DIContainerLogic.GetTimingService().GetPresentTime();
			DateTime p_dtFromTime = presentTime + new TimeSpan(0, 0, UnityEngine.Random.Range(mcCoolVisitMinCooldown, mcCoolVisitMaxCooldown));
			switch (type)
			{
			case MessageType.ResponseBirdBorrowMessage:
				DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.McCoolLendsBirdTimestamp = DIContainerLogic.GetTimingService().GetTimestamp(p_dtFromTime);
				break;
			case MessageType.ResponseFriendshipEssenceMessage:
				DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.McCoolSendsEssenceTimestamp = DIContainerLogic.GetTimingService().GetTimestamp(p_dtFromTime);
				break;
			case MessageType.ResponseGachaUseMessage:
				DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.McCoolVisitsGachaTimestamp = DIContainerLogic.GetTimingService().GetTimestamp(p_dtFromTime);
				break;
			case MessageType.ResponsePvpGachaUseMessage:
				DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.McCoolVisitsPvpGachaTimestamp = DIContainerLogic.GetTimingService().GetTimestamp(p_dtFromTime);
				break;
			default:
				DebugLog.Warn("Piggie McCool can't send this message type: " + type);
				break;
			}
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}
	}
}
