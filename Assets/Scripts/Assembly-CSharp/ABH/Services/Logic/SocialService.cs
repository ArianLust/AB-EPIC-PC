using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using ABH.Shared.Models.InventoryItems;
using UnityEngine;

namespace ABH.Services.Logic
{
	public class SocialService
	{
		public class SocialAsyncResult : IAsyncResult
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

			public SocialAsyncResult(object state, Type returnType)
			{
				this.state = state;
				this.returnType = returnType;
			}
		}

		private Action<string> DebugLog;

		private Action<string> ErrorLog;

		public SocialService SetDebugLog(Action<string> debugLog)
		{
			DebugLog = debugLog;
			return this;
		}

		public SocialService SetErrorLog(Action<string> errorLog)
		{
			ErrorLog = errorLog;
			return this;
		}

		private void LogDebug(string message)
		{
			if (DebugLog != null)
			{
				DebugLog(message);
			}
		}

		private void LogError(string message)
		{
			if (ErrorLog != null)
			{
				ErrorLog(message);
			}
		}

		public FriendData GetPorkyFriend(int level)
		{
			FriendData friendData = new FriendData();
			friendData.IsNPC = true;
			friendData.FirstName = "npc_friend_prince";
			friendData.Id = "NPC_Porky";
			friendData.IsSilhouettePicture = false;
			friendData.Level = Math.Max(level + 1, 1);
			friendData.PictureUrl = string.Empty;
			return friendData;
		}

		public FriendData GetAdventurerFriend(int level)
		{
			FriendData friendData = new FriendData();
			friendData.IsNPC = true;
			friendData.FirstName = "npc_friend_adventurer";
			friendData.Id = "NPC_Adventurer";
			friendData.IsSilhouettePicture = false;
			friendData.Level = Math.Max(level, 1);
			friendData.PictureUrl = string.Empty;
			return friendData;
		}

		public FriendData GetLowNPCFriend(int level)
		{
			FriendData friendData = new FriendData();
			friendData.IsNPC = true;
			friendData.FirstName = "npc_friend_merchant";
			friendData.Id = "NPC_Low";
			friendData.IsSilhouettePicture = false;
			friendData.Level = Math.Max(level - 2, 1);
			friendData.PictureUrl = string.Empty;
			return friendData;
		}

		public FriendData GetHighNPCFriend(int level)
		{
			FriendData friendData = new FriendData();
			friendData.IsNPC = true;
			friendData.FirstName = "npc_friend_eagle";
			friendData.Id = "NPC_High";
			friendData.IsSilhouettePicture = false;
			friendData.Level = level + 5;
			friendData.PictureUrl = string.Empty;
			friendData.NeedsPayment = true;
			return friendData;
		}

		public BirdData GetNPCBird(int level, string id, int mastery)
		{
			switch (id)
			{
			case "NPC_Low":
				return GetMerchantBirdWithLevel(level, mastery);
			case "NPC_Adventurer":
				return GetAdventurerBirdWithLevel(level, mastery);
			case "NPC_Porky":
				return GetPorkyBirdWithLevel(level, mastery);
			default:
				return GetRandomBirdWithLevel(level, string.Empty);
			}
		}

		private BirdData GetPorkyBirdWithLevel(int level, int mastery)
		{
			BirdBalancingData balancing = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_prince_porky", out balancing))
			{
				BirdGameData birdGameData = new BirdGameData("bird_prince_porky", level);
				birdGameData.IsNPC = true;
				birdGameData.ClassItem.Data.Level = mastery;
				return birdGameData.Data;
			}
			return null;
		}

		private BirdData GetAdventurerBirdWithLevel(int level, int mastery)
		{
			BirdBalancingData balancing = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_adventurer", out balancing))
			{
				BirdGameData birdGameData = new BirdGameData("bird_adventurer", level);
				birdGameData.IsNPC = true;
				birdGameData.ClassItem.Data.Level = mastery;
				return birdGameData.Data;
			}
			return null;
		}

		private BirdData GetMerchantBirdWithLevel(int level, int mastery)
		{
			BirdBalancingData balancing = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_merchant", out balancing))
			{
				BirdGameData birdGameData = new BirdGameData("bird_merchant", level);
				birdGameData.IsNPC = true;
				birdGameData.ClassItem.Data.Level = mastery;
				return birdGameData.Data;
			}
			return null;
		}

		public List<BirdData> GetNPCBirds(int level)
		{
			List<BirdData> list = new List<BirdData>();
			foreach (BirdBalancingData item in from b in DIContainerBalancing.Service.GetBalancingDataList<BirdBalancingData>()
				where b.NameId.StartsWith("npc_")
				select b)
			{
				BirdGameData birdGameData = new BirdGameData(item.NameId, level);
				list.Add(birdGameData.Data);
				birdGameData.IsNPC = true;
			}
			return list;
		}

		public List<BirdData> GetTutorialBirds(int level)
		{
			List<BirdData> list = new List<BirdData>();
			foreach (BirdBalancingData item in from b in DIContainerBalancing.Service.GetBalancingDataList<BirdBalancingData>()
				where b.NameId.StartsWith("pvptut_")
				select b)
			{
				BirdGameData birdGameData = new BirdGameData(item.NameId, level);
				list.Add(birdGameData.Data);
				birdGameData.IsNPC = true;
			}
			return list;
		}

		public PublicPlayerData GetNPCPlayer(FriendGameData friend, int level, bool tutorial = false)
		{
			string nameId = ((!tutorial) ? "bird_banner_porky" : "bird_banner_pvptut");
			List<BirdData> birds = ((!tutorial) ? GetNPCBirds(level) : GetTutorialBirds(level));
			PublicPlayerData publicPlayerData = new PublicPlayerData();
			publicPlayerData.LastSaveTime = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
			publicPlayerData.LocationProgress = new Dictionary<LocationType, int>
			{
				{
					LocationType.World,
					0
				},
				{
					LocationType.ChronicleCave,
					0
				}
			};
			publicPlayerData.Inventory = GetNPCInventory(friend.FriendLevel);
			publicPlayerData.Birds = birds;
			publicPlayerData.SocialId = friend.FriendId;
			publicPlayerData.Banner = new BannerGameData(nameId, level).Data;
			publicPlayerData.PvPRank = 5;
			publicPlayerData.PvPIndices = new List<int> { 0, 1, 2 };
			publicPlayerData.League = 2;
			publicPlayerData.Level = level;
			PublicPlayerData publicPlayerData2 = publicPlayerData;
			publicPlayerData2.EventPlayerName = DIContainerInfrastructure.GetLocaService().Tr(friend.FriendName);
			return publicPlayerData2;
		}

		public PublicPlayerData GetNPCPlayerWithInventory(FriendData friend)
		{
			List<HotspotData> list = new List<HotspotData>();
			PublicPlayerData publicPlayerData = new PublicPlayerData();
			publicPlayerData.LastSaveTime = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
			publicPlayerData.LocationProgress = new Dictionary<LocationType, int>
			{
				{
					LocationType.World,
					0
				},
				{
					LocationType.ChronicleCave,
					0
				}
			};
			publicPlayerData.Inventory = GetNPCInventory(friend.Level);
			publicPlayerData.Birds = new List<BirdData>();
			publicPlayerData.SocialId = friend.Id;
			publicPlayerData.SocialPlayerName = friend.FirstName;
			publicPlayerData.Level = friend.Level;
			publicPlayerData.SocialAvatarUrl = friend.PictureUrl;
			return publicPlayerData;
		}

		private InventoryData GetNPCInventory(int level)
		{
			InventoryData inventoryData = new InventoryData();
			inventoryData.StoryItems = new List<BasicItemData>();
			inventoryData.PlayerStats = new List<BasicItemData>();
			LootTableBalancingData balancingData = DIContainerBalancing.LootTableBalancingDataPovider.GetBalancingData("loot_npc_inventory");
			DebugLog("[SocialService] Generate NPC Inventory");
			foreach (LootTableEntry lootTableEntry in balancingData.LootTableEntries)
			{
				if (lootTableEntry.NameId == "star_collection")
				{
					inventoryData.PlayerStats.Add(new BasicItemData
					{
						NameId = lootTableEntry.NameId,
						IsNew = false,
						Level = lootTableEntry.CurrentPlayerLevelDelta,
						Quality = 0,
						Value = lootTableEntry.BaseValue
					});
				}
				else
				{
					inventoryData.StoryItems.Add(new BasicItemData
					{
						NameId = lootTableEntry.NameId,
						IsNew = false,
						Level = lootTableEntry.CurrentPlayerLevelDelta,
						Quality = 0,
						Value = lootTableEntry.BaseValue
					});
				}
			}
			InventoryGameData inventoryGameData = new InventoryGameData(inventoryData);
			int num = 0;
			return inventoryGameData.Data;
		}

		public InventoryData GetEmptyNPCInventory(int level)
		{
			InventoryData inventoryData = new InventoryData();
			inventoryData.StoryItems = new List<BasicItemData>();
			InventoryGameData inventoryGameData = new InventoryGameData(inventoryData);
			int num = 0;
			return inventoryGameData.Data;
		}

		public PublicPlayerData GetNPCPlayer(FriendData friend)
		{
			List<HotspotData> list = new List<HotspotData>();
			PublicPlayerData publicPlayerData = new PublicPlayerData();
			publicPlayerData.LastSaveTime = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
			publicPlayerData.LocationProgress = new Dictionary<LocationType, int>
			{
				{
					LocationType.World,
					0
				},
				{
					LocationType.ChronicleCave,
					0
				}
			};
			publicPlayerData.Inventory = GetEmptyNPCInventory(friend.Level);
			publicPlayerData.Birds = new List<BirdData>();
			publicPlayerData.SocialId = friend.Id;
			publicPlayerData.SocialPlayerName = friend.FirstName;
			publicPlayerData.Level = friend.Level;
			publicPlayerData.SocialAvatarUrl = friend.PictureUrl;
			publicPlayerData.Banner = new BannerGameData("bird_banner_porky", friend.Level).Data;
			publicPlayerData.PvPRank = 5;
			publicPlayerData.PvPIndices = new List<int> { 0, 1, 2 };
			publicPlayerData.League = 5;
			publicPlayerData.RandomDecisionSeed = UnityEngine.Random.Range(0f, float.MaxValue);
			return publicPlayerData;
		}

		public IMailboxMessageGameData GenerateMessageGameData(MessageDataIncoming message)
		{
			try
			{
				IMailboxMessageGameData mailboxMessageGameData = Activator.CreateInstance(Type.GetType("ABH.GameDatas.MailboxMessages." + message.MessageType, true, true)) as IMailboxMessageGameData;
				if (mailboxMessageGameData == null)
				{
					return null;
				}
				mailboxMessageGameData.SetMessageData(message);
				return mailboxMessageGameData;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public bool RemoveMessage(PlayerGameData player, IMailboxMessageGameData message)
		{
			// if (message.IsUsed && message.IsViewed)
			// {
			bool result = player.SocialEnvironmentGameData.RemoveMessage(message);
			player.SavePlayerData();
			LogDebug("[SocialService] Message: " + message.Id + " removed.");
			return result;
			// }
			// return false;
		}

		public void RespondMessage(PlayerGameData player, FriendData sender, string reciverId, MessageType messageType, string parameter1, int paramter2, Action<bool> OnMessageSend)
		{
			OnMessageSend(true);
		}

		public bool HasAnyGachaFreeRoll(SocialEnvironmentGameData socialEnvironmentGameData)
		{
			return socialEnvironmentGameData.Friends.Values.Any((FriendGameData friend) => DIContainerLogic.SocialService.HasFreeGachaRoll(friend, DIContainerInfrastructure.GetCurrentPlayer()));
		}

		public bool HasAnyPvpGachaFreeRoll(SocialEnvironmentGameData socialEnvironmentGameData)
		{
			return socialEnvironmentGameData.Friends.Values.Any((FriendGameData friend) => DIContainerLogic.SocialService.HasFreePvpGachaRoll(friend, DIContainerInfrastructure.GetCurrentPlayer()));
		}

		public bool GetFriendsBirds(List<FriendGameData> friends, Action<Dictionary<string, BirdData>> callback)
		{
			Dictionary<string, BirdData> dictionary = new Dictionary<string, BirdData>();
			for (int i = 0; i < friends.Count; i++)
			{
				if (friends[i] != null)
				{
					if (friends[i].FriendBird != null)
					{
						dictionary.Add(friends[i].FriendId, friends[i].FriendBird.Data);
					}
					else if (friends[i].Data.IsNPC)
					{
						dictionary.Add(friends[i].FriendId, GetNPCBird(friends[i].FriendLevel, friends[i].FriendId, 5));
					}
					else
					{
						dictionary.Add(friends[i].FriendId, friends[i].FriendBird.Data);
					}
				}
			}
			callback(dictionary);
			return true;
		}

		private BirdData GetDummyBird(int i, int level)
		{
			BirdData birdData = new BirdData();
			string nameId = "bird_red";
			string nameId2 = "bird_weapon_red_sword_01";
			string nameId3 = "bird_offhand_red_shield_01";
			string nameId4 = "class_knight";
			switch (i % 3)
			{
			case 0:
				nameId = "bird_red";
				nameId2 = "bird_weapon_red_sword_01";
				nameId3 = "bird_offhand_red_shield_01";
				nameId4 = "class_knight";
				break;
			case 1:
				nameId = "bird_red";
				nameId2 = "bird_weapon_red_sword_01";
				nameId3 = "bird_offhand_red_shield_01";
				nameId4 = "class_guardian";
				break;
			case 2:
				nameId = "bird_yellow";
				nameId2 = "bird_weapon_yellow_staff_01";
				nameId3 = "bird_offhand_yellow_book_01";
				nameId4 = "class_mage";
				break;
			}
			birdData.NameId = nameId;
			birdData.Level = level;
			birdData.Inventory = new InventoryData
			{
				NameId = "inventory_empty"
			};
			birdData.Inventory.MainHandItems = new List<EquipmentData>();
			birdData.Inventory.OffHandItems = new List<EquipmentData>();
			birdData.Inventory.ClassItems = new List<ClassItemData>();
			birdData.Inventory.MainHandItems.Add(new EquipmentData
			{
				IsNew = true,
				ItemSource = EquipmentSource.Loot,
				NameId = nameId2,
				Quality = 1,
				ScrapLoot = new Dictionary<string, int>(),
				Level = level,
				Value = 1
			});
			birdData.Inventory.OffHandItems.Add(new EquipmentData
			{
				IsNew = true,
				ItemSource = EquipmentSource.Loot,
				NameId = nameId3,
				Quality = 1,
				ScrapLoot = new Dictionary<string, int>(),
				Level = level,
				Value = 1
			});
			birdData.Inventory.ClassItems.Add(new ClassItemData
			{
				IsNew = true,
				Level = 1,
				Quality = 1,
				Value = 1,
				NameId = nameId4
			});
			return birdData;
		}

		private List<FriendData> GetAllDummyFriends(int level)
		{
			List<FriendData> list = new List<FriendData>();
			list = GetDummyFriends(0);
			list.AddRange(GetDummyFriends(1));
			list.AddRange(GetDummyFriends(2));
			list.Add(GetLowNPCFriend(1));
			list.Add(GetHighNPCFriend(level));
			return list;
		}

		public FriendData GetDummyFriend(int index)
		{
			FriendData friendData = new FriendData();
			friendData.FirstName = "dummy_friend_" + index;
			friendData.Id = index.ToString();
			friendData.HasBonus = true;
			friendData.IsSilhouettePicture = false;
			friendData.Level = index + 1;
			friendData.PictureUrl = ((index % 2 != 1) ? "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-prn1/41625_1793213221_2338_q.jpg" : "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-ash2/161108_1695555912_1274771547_q.jpg");
			return friendData;
		}

		public List<FriendData> GetDummyFriends(int page)
		{
			List<FriendData> list = new List<FriendData>();
			switch (page)
			{
			case 0:
			{
				int friendsPerPage = GetFriendsPerPage();
				for (int j = 0; j < 10; j++)
				{
					list.Add(new FriendData
					{
						FirstName = "dummy_friend_" + j,
						Id = j.ToString(),
						HasBonus = true,
						IsSilhouettePicture = false,
						Level = j + 1,
						PictureUrl = ((j % 2 != 1) ? "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-prn1/41625_1793213221_2338_q.jpg" : "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-ash2/161108_1695555912_1274771547_q.jpg")
					});
				}
				break;
			}
			case 1:
			{
				int friendsPerPage2 = GetFriendsPerPage();
				for (int k = 10; k < 15; k++)
				{
					list.Add(new FriendData
					{
						FirstName = "dummy_friend_" + k,
						Id = k.ToString(),
						HasBonus = true,
						IsSilhouettePicture = false,
						Level = k + 1,
						PictureUrl = ((k % 2 != 1) ? "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-prn1/41625_1793213221_2338_q.jpg" : "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-ash2/161108_1695555912_1274771547_q.jpg")
					});
				}
				break;
			}
			case 2:
			{
				for (int i = 15; i < 40; i++)
				{
					list.Add(new FriendData
					{
						FirstName = "dummy_friend_" + i,
						Id = i.ToString(),
						HasBonus = true,
						IsSilhouettePicture = false,
						Level = i + 1,
						PictureUrl = ((i % 2 != 1) ? "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-prn1/41625_1793213221_2338_q.jpg" : "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-ash2/161108_1695555912_1274771547_q.jpg")
					});
				}
				break;
			}
			}
			return list;
		}

		public int GetFriendsPerPage()
		{
			return 10;
		}

		public int GetMaxInspectPageCount(PlayerGameData player, bool isPvP)
		{
			return (int)Math.Ceiling((float)DIContainerLogic.SocialService.GetFriendCount(player, true, true, isPvP) / (float)GetFriendsPerPage());
		}

		public int GetMaxGetBirdPageCount(PlayerGameData player)
		{
			return (int)Math.Ceiling((float)DIContainerLogic.SocialService.GetFriendCount(player, true, false, false) / (float)GetFriendsPerPage());
		}

		public FriendCountBonusInfo GetMaxFriendBonuses()
		{
			FriendCountBonusInfo friendCountBonusInfo = new FriendCountBonusInfo();
			friendCountBonusInfo.Classes = new List<string>();
			FriendCountBonusInfo friendCountBonusInfo2 = friendCountBonusInfo;
			foreach (BonusPerFriendBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<BonusPerFriendBalancingData>())
			{
				friendCountBonusInfo2.AttackBonus += balancingData.AttackBonus;
				friendCountBonusInfo2.HealthBonus += balancingData.HealthBonus;
				friendCountBonusInfo2.XPBonus += balancingData.XPBonus;
				if (!string.IsNullOrEmpty(balancingData.UnlockedClassNameId))
				{
					friendCountBonusInfo2.Classes.Add(balancingData.UnlockedClassNameId);
				}
			}
			return friendCountBonusInfo2;
		}

		public FriendCountBonusInfo GetCumulatedFriendBonuses(PlayerGameData player)
		{
			FriendCountBonusInfo friendCountBonusInfo = new FriendCountBonusInfo();
			friendCountBonusInfo.Classes = new List<string>();
			FriendCountBonusInfo friendCountBonusInfo2 = friendCountBonusInfo;
			foreach (BonusPerFriendBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<BonusPerFriendBalancingData>())
			{
				if (balancingData.Count > GetFriendCount(player, true, false, false))
				{
					break;
				}
				friendCountBonusInfo2.AttackBonus += balancingData.AttackBonus;
				friendCountBonusInfo2.HealthBonus += balancingData.HealthBonus;
				friendCountBonusInfo2.XPBonus += balancingData.XPBonus;
				if (!string.IsNullOrEmpty(balancingData.UnlockedClassNameId))
				{
					friendCountBonusInfo2.Classes.Add(balancingData.UnlockedClassNameId);
				}
			}
			return friendCountBonusInfo2;
		}

		public int GetFriendCount(PlayerGameData player, bool ignoreNpc, bool ignoreHighLevelNPC, bool isPvP)
		{
			if (player.SocialEnvironmentGameData == null)
			{
				return 0;
			}
			if (!isPvP)
			{
				return player.SocialEnvironmentGameData.Friends.Values.Count((FriendGameData f) => !f.isNpcFriend);
			}
			return player.SocialEnvironmentGameData.Friends.Values.Count((FriendGameData f) => !f.isNpcFriend && f.PublicPlayerData != null && f.PublicPlayerData.Banner != null);
		}

		public FriendCountBonusInfo GetFriendBonus(int friendCount)
		{
			FriendCountBonusInfo friendCountBonusInfo = new FriendCountBonusInfo();
			friendCountBonusInfo.Classes = new List<string>();
			FriendCountBonusInfo friendCountBonusInfo2 = friendCountBonusInfo;
			foreach (BonusPerFriendBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<BonusPerFriendBalancingData>())
			{
				if (balancingData.Count > friendCount)
				{
					break;
				}
				if (balancingData.Count == friendCount)
				{
					friendCountBonusInfo2.AttackBonus += balancingData.AttackBonus;
					friendCountBonusInfo2.HealthBonus += balancingData.HealthBonus;
					friendCountBonusInfo2.XPBonus += balancingData.XPBonus;
					if (!string.IsNullOrEmpty(balancingData.UnlockedClassNameId))
					{
						friendCountBonusInfo2.Classes.Add(balancingData.UnlockedClassNameId);
					}
				}
			}
			return friendCountBonusInfo2;
		}

		public FriendCountBonusInfo GetNextFriendBonuses(PlayerGameData player)
		{
			FriendCountBonusInfo friendCountBonusInfo = new FriendCountBonusInfo();
			friendCountBonusInfo.Classes = new List<string>();
			FriendCountBonusInfo friendCountBonusInfo2 = friendCountBonusInfo;
			foreach (BonusPerFriendBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<BonusPerFriendBalancingData>())
			{
				if (balancingData.Count > GetFriendCount(player, true, false, false))
				{
					break;
				}
				if (balancingData.Count == GetFriendCount(player, true, false, false))
				{
					friendCountBonusInfo2.AttackBonus += balancingData.AttackBonus;
					friendCountBonusInfo2.HealthBonus += balancingData.HealthBonus;
					friendCountBonusInfo2.XPBonus += balancingData.XPBonus;
					if (!string.IsNullOrEmpty(balancingData.UnlockedClassNameId))
					{
						friendCountBonusInfo2.Classes.Add(balancingData.UnlockedClassNameId);
					}
				}
			}
			return friendCountBonusInfo2;
		}

		public bool IsGetFriendBirdPossible(PlayerGameData player, FriendGameData friend)
		{
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				return false;
			}
			return friend.Data != null && (!friend.Data.NeedsPayment || (friend.Data.NeedsPayment && friend.HasPaid)) && (!player.SocialEnvironmentGameData.Data.GetBirdCooldowns.ContainsKey(friend.FriendId) || trustedTime > DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(player.SocialEnvironmentGameData.Data.GetBirdCooldowns[friend.FriendId]).AddSeconds(DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").TimeForGetFriendBird));
		}

		public bool HasFreeGachaRoll(FriendGameData friend, PlayerGameData player)
		{
			if (player.SocialEnvironmentGameData.Data.FreeGachaRollFriendIds.Contains(friend.FriendId))
			{
				return true;
			}
			return false;
		}

		public bool HasFreePvpGachaRoll(FriendGameData friend, PlayerGameData player)
		{
			if (player.SocialEnvironmentGameData.Data.FreePvpGachaRollFriendIds.Contains(friend.FriendId))
			{
				return true;
			}
			return false;
		}

		public void UsedFreeGachaRoll(FriendGameData friendGameData, PlayerGameData playerGameData)
		{
			if (playerGameData.SocialEnvironmentGameData.Data.FreeGachaRollFriendIds.Remove(friendGameData.FriendId) && playerGameData.SocialEnvironmentGameData.Data.FreeGachaRollFriendIds.Count + 1 == GetMaxParallelFreeGachaRolls(playerGameData) && DIContainerInfrastructure.GetCoreStateMgr() != null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(DIContainerLogic.GetTimingService().GetTrustedTime(delegate(DateTime trustedTime)
				{
					playerGameData.SocialEnvironmentGameData.Data.LastGachaFreeRollSpawn = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
					playerGameData.SavePlayerData();
				}));
			}
			if (!playerGameData.SocialEnvironmentGameData.Data.GetFreeGachaRollCooldowns.ContainsKey(friendGameData.FriendId) && DIContainerInfrastructure.GetCoreStateMgr() != null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(DIContainerLogic.GetTimingService().GetTrustedTime(delegate(DateTime trustedTime)
				{
					playerGameData.SocialEnvironmentGameData.Data.GetFreeGachaRollCooldowns.Add(friendGameData.FriendId, DIContainerLogic.GetTimingService().GetTimestamp(trustedTime));
					playerGameData.SavePlayerData();
				}));
			}
		}

		public void UsedFreePvpGachaRoll(FriendGameData friendGameData, PlayerGameData playerGameData)
		{
			if (playerGameData.SocialEnvironmentGameData.Data.FreePvpGachaRollFriendIds.Remove(friendGameData.FriendId) && playerGameData.SocialEnvironmentGameData.Data.FreePvpGachaRollFriendIds.Count + 1 == GetMaxParallelFreeGachaRolls(playerGameData) && DIContainerInfrastructure.GetCoreStateMgr() != null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(DIContainerLogic.GetTimingService().GetTrustedTime(delegate(DateTime trustedTime)
				{
					playerGameData.SocialEnvironmentGameData.Data.LastPvpGachaFreeRollSpawn = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
					playerGameData.SavePlayerData();
				}));
			}
			if (!playerGameData.SocialEnvironmentGameData.Data.GetFreePvpGachaRollCooldowns.ContainsKey(friendGameData.FriendId) && DIContainerInfrastructure.GetCoreStateMgr() != null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(DIContainerLogic.GetTimingService().GetTrustedTime(delegate(DateTime trustedTime)
				{
					playerGameData.SocialEnvironmentGameData.Data.GetFreePvpGachaRollCooldowns.Add(friendGameData.FriendId, DIContainerLogic.GetTimingService().GetTimestamp(trustedTime));
					playerGameData.SavePlayerData();
				}));
			}
		}

		public void UpdateFreeGachaRolls(PlayerGameData player, SocialEnvironmentGameData socialEnvironmentGameData)
		{
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				return;
			}
			SocialEnvironmentBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default");
			if (socialEnvironmentGameData.Data.FreeGachaRollFriendIds != null && socialEnvironmentGameData.ReceivedPlayersOnce)
			{
				List<string> list = new List<string>();
				if (trustedTime > DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironmentGameData.Data.LastGachaFreeRollSpawn).AddSeconds(balancingData.TimeForFreeGachaRollSpawn))
				{
					socialEnvironmentGameData.Data.FreeGachaRollFriendIds.Clear();
				}
				foreach (string freeGachaRollFriendId in socialEnvironmentGameData.Data.FreeGachaRollFriendIds)
				{
					FriendGameData value = null;
					if (socialEnvironmentGameData.Friends.TryGetValue(freeGachaRollFriendId, out value) && (value.PublicPlayerData == null || value.PublicPlayerData.Inventory == null || value.PublicPlayerData.Inventory.StoryItems == null || !value.PublicPlayerData.Inventory.StoryItems.Any((BasicItemData i) => i.NameId == "story_goldenpig")))
					{
						list.Add(freeGachaRollFriendId);
					}
				}
				foreach (string item in list)
				{
					socialEnvironmentGameData.Data.FreeGachaRollFriendIds.Remove(item);
				}
			}
			if (DIContainerLogic.InventoryService.GetItemValue(player.InventoryGameData, "story_goldenpig") <= 0 || socialEnvironmentGameData.Data.FreeGachaRollFriendIds == null || socialEnvironmentGameData.Data.FreeGachaRollFriendIds.Count >= GetMaxParallelFreeGachaRolls(player) || !(trustedTime > DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironmentGameData.Data.LastGachaFreeRollSpawn).AddSeconds(balancingData.TimeForFreeGachaRollSpawn)))
			{
				return;
			}
			DebugLog("Free Gacha Roll Spawn possible! Search for possible friends!");
			List<FriendGameData> list2 = new List<FriendGameData>();
			foreach (string key in socialEnvironmentGameData.Friends.Keys)
			{
				FriendGameData friendGameData = socialEnvironmentGameData.Friends[key];
				if (!friendGameData.canSpawnNewGachaRoll || socialEnvironmentGameData.Data.FreeGachaRollFriendIds.Contains(key) || (socialEnvironmentGameData.Data.GetFreeGachaRollCooldowns.ContainsKey(key) && DIContainerLogic.GetTimingService().IsBefore(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironmentGameData.Data.GetFreeGachaRollCooldowns[key]).AddSeconds(balancingData.TimeForFreeGachaRollSpawn * 2))) || (friendGameData.PublicPlayerData != null && !friendGameData.isNpcFriend && !friendGameData.PublicPlayerData.Inventory.StoryItems.Any((BasicItemData i) => i.NameId == "story_goldenpig")))
				{
					DebugLog("Free Gacha Roll Spawn not possible for friend: " + key);
					continue;
				}
				socialEnvironmentGameData.Data.GetFreeGachaRollCooldowns.Remove(key);
				list2.Add(friendGameData);
			}
			bool flag = false;
			int num = 0;
			while (list2.Count > 0 && socialEnvironmentGameData.Data.FreeGachaRollFriendIds.Count < GetMaxParallelFreeGachaRolls(player) && num < GetMaxParallelFreeGachaRolls(player) && !(trustedTime <= DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironmentGameData.Data.LastGachaFreeRollSpawn).AddSeconds(balancingData.TimeForFreeGachaRollSpawn * (num + 1))))
			{
				FriendGameData friendGameData2 = list2[UnityEngine.Random.Range(0, list2.Count)];
				DebugLog("Found free gacha roll: " + friendGameData2.FriendId);
				socialEnvironmentGameData.Data.FreeGachaRollFriendIds.Add(friendGameData2.FriendId);
				friendGameData2.SetFreeGachaRoll(true);
				flag = true;
				list2.Remove(friendGameData2);
				num++;
			}
			if (flag)
			{
				socialEnvironmentGameData.Data.LastGachaFreeRollSpawn = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
			}
		}

		public void UpdateFreePvpGachaRolls(PlayerGameData player, SocialEnvironmentGameData socialEnvironmentGameData)
		{
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				return;
			}
			if (socialEnvironmentGameData.Data.FreePvpGachaRollFriendIds != null && socialEnvironmentGameData.ReceivedPlayersOnce)
			{
				List<string> list = new List<string>();
				foreach (string freePvpGachaRollFriendId in socialEnvironmentGameData.Data.FreePvpGachaRollFriendIds)
				{
					FriendGameData value = null;
					if (socialEnvironmentGameData.Friends.TryGetValue(freePvpGachaRollFriendId, out value) && (value.PublicPlayerData == null || value.PublicPlayerData.Inventory == null || value.PublicPlayerData.Inventory.StoryItems == null || !value.PublicPlayerData.Inventory.StoryItems.Any((BasicItemData i) => i.NameId == "story_goldenpig")))
					{
						list.Add(freePvpGachaRollFriendId);
					}
				}
				foreach (string item in list)
				{
					socialEnvironmentGameData.Data.FreePvpGachaRollFriendIds.Remove(item);
				}
			}
			if (DIContainerLogic.InventoryService.GetItemValue(player.InventoryGameData, "story_goldenpig") <= 0 || socialEnvironmentGameData.Data.FreePvpGachaRollFriendIds == null || socialEnvironmentGameData.Data.FreePvpGachaRollFriendIds.Count >= GetMaxParallelFreeGachaRolls(player) || !(trustedTime > DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironmentGameData.Data.LastPvpGachaFreeRollSpawn).AddSeconds(DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").TimeForFreeGachaRollSpawn)))
			{
				return;
			}
			DebugLog("Free pvp Gacha Roll Spawn possible! Search for possible friends!");
			List<FriendGameData> list2 = new List<FriendGameData>();
			foreach (string key in socialEnvironmentGameData.Friends.Keys)
			{
				FriendGameData friendGameData = socialEnvironmentGameData.Friends[key];
				if (friendGameData.PublicPlayerData == null || friendGameData.PublicPlayerData.Banner == null || !friendGameData.canSpawnNewGachaRoll || socialEnvironmentGameData.Data.FreePvpGachaRollFriendIds.Contains(key) || (socialEnvironmentGameData.Data.GetFreePvpGachaRollCooldowns.ContainsKey(key) && DIContainerLogic.GetTimingService().IsBefore(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironmentGameData.Data.GetFreePvpGachaRollCooldowns[key]).AddSeconds(DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").TimeForFreeGachaRollSpawn * 2))) || (friendGameData.PublicPlayerData != null && !friendGameData.isNpcFriend && !friendGameData.PublicPlayerData.Inventory.StoryItems.Any((BasicItemData i) => i.NameId == "story_goldenpig")))
				{
					DebugLog("Free pvp Gacha Roll Spawn not possible for friend: " + key);
					continue;
				}
				socialEnvironmentGameData.Data.GetFreePvpGachaRollCooldowns.Remove(key);
				list2.Add(friendGameData);
			}
			bool flag = false;
			int num = 0;
			while (list2.Count > 0 && socialEnvironmentGameData.Data.FreePvpGachaRollFriendIds.Count < GetMaxParallelFreeGachaRolls(player) && num < GetMaxParallelFreeGachaRolls(player) && !(trustedTime <= DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironmentGameData.Data.LastPvpGachaFreeRollSpawn).AddSeconds(DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").TimeForFreeGachaRollSpawn * (num + 1))))
			{
				FriendGameData friendGameData2 = list2[UnityEngine.Random.Range(0, list2.Count)];
				DebugLog("Found free pvp gacha roll: " + friendGameData2.FriendId);
				socialEnvironmentGameData.Data.FreePvpGachaRollFriendIds.Add(friendGameData2.FriendId);
				friendGameData2.SetFreePvpGachaRoll(true);
				flag = true;
				list2.Remove(friendGameData2);
				num++;
			}
			if (flag)
			{
				socialEnvironmentGameData.Data.LastPvpGachaFreeRollSpawn = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
			}
		}

		private int GetMaxParallelFreeGachaRolls(PlayerGameData player)
		{
			return (DIContainerLogic.SocialService.GetFriendCount(player, true, true, false) >= DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").MinFriendsForMaxFreeRolls) ? DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").HighMaxFreeRolls : DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").LowMaxFreeRolls;
		}

		public bool IsSendFriendshipGateHelpPossible(string hotspotId, SocialEnvironmentGameData socialEnvironment)
		{
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				return false;
			}
			uint value = 0u;
			if (socialEnvironment.Data.FriendShipGateHelpCooldowns != null && socialEnvironment.Data.FriendShipGateHelpCooldowns.TryGetValue(hotspotId, out value))
			{
				DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(value);
				uint friendshipGateHelpCooldown = socialEnvironment.BalancingData.FriendshipGateHelpCooldown;
				if (friendshipGateHelpCooldown != 0)
				{
					return trustedTime > dateTimeFromTimestamp.AddSeconds(friendshipGateHelpCooldown);
				}
				DateTime dateTime = dateTimeFromTimestamp.ToLocalTime().AddDays(1.0);
				DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
				return trustedTime > dateTime2;
			}
			return true;
		}

		public TimeSpan GetSendFriendshipGateHelpTimeLeft(string hotspotId, SocialEnvironmentGameData socialEnvironment)
		{
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				trustedTime = DIContainerLogic.GetTimingService().GetPresentTime();
			}
			uint value = 0u;
			if (socialEnvironment.Data.FriendShipGateHelpCooldowns.TryGetValue(hotspotId, out value))
			{
				DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(value);
				uint friendshipGateHelpCooldown = socialEnvironment.BalancingData.FriendshipGateHelpCooldown;
				if (friendshipGateHelpCooldown != 0)
				{
					return dateTimeFromTimestamp.AddSeconds(friendshipGateHelpCooldown) - trustedTime;
				}
				DateTime dateTime = dateTimeFromTimestamp.ToLocalTime().AddDays(1.0);
				DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
				return dateTime2 - trustedTime;
			}
			return new TimeSpan(0L);
		}

		public bool IsGetFriendshipEssencePossible(SocialEnvironmentGameData socialEnvironment)
		{
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				return false;
			}
			DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironment.Data.FriendShipEssenceCooldown);
			uint friendshipEssenceCooldown = socialEnvironment.BalancingData.FriendshipEssenceCooldown;
			if (friendshipEssenceCooldown != 0)
			{
				return trustedTime > dateTimeFromTimestamp.AddSeconds(friendshipEssenceCooldown);
			}
			DateTime dateTime = dateTimeFromTimestamp.ToLocalTime().AddDays(1.0);
			DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
			return trustedTime > dateTime2;
		}

		public TimeSpan GetFriendshipEssenceTimeLeft(SocialEnvironmentGameData socialEnvironment)
		{
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				trustedTime = DIContainerLogic.GetTimingService().GetPresentTime();
			}
			DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(socialEnvironment.Data.FriendShipEssenceCooldown);
			uint friendshipEssenceCooldown = socialEnvironment.BalancingData.FriendshipEssenceCooldown;
			if (friendshipEssenceCooldown != 0)
			{
				return dateTimeFromTimestamp.AddSeconds(friendshipEssenceCooldown) - trustedTime;
			}
			DateTime dateTime = dateTimeFromTimestamp.ToLocalTime().AddDays(1.0);
			DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
			return dateTime2 - trustedTime;
		}

		public bool AddResendMessage(SocialEnvironmentGameData socialEnvironment, IMailboxMessageGameData message)
		{
			if (socialEnvironment.Data.ResendMessages == null)
			{
				socialEnvironment.Data.ResendMessages = new List<MessageDataIncoming>();
			}
			if (socialEnvironment.FailedMessagesCount.ContainsKey(message.GetMessageData().Id))
			{
				if (socialEnvironment.FailedMessagesCount[message.GetMessageData().Id] > 2)
				{
					return false;
				}
				Dictionary<string, int> failedMessagesCount;
				Dictionary<string, int> dictionary = (failedMessagesCount = socialEnvironment.FailedMessagesCount);
				string id;
				string key = (id = message.GetMessageData().Id);
				int num = failedMessagesCount[id];
				dictionary[key] = num + 1;
			}
			else
			{
				socialEnvironment.FailedMessagesCount.Add(message.GetMessageData().Id, 1);
			}
			if (!socialEnvironment.Data.ResendMessages.Contains(message.GetMessageData()))
			{
				socialEnvironment.Data.ResendMessages.Add(message.GetMessageData());
			}
			return true;
		}

		public void ReAddResendMessages(SocialEnvironmentGameData socialEnvironment)
		{
			if (socialEnvironment.PendingMailboxMessages != null)
			{
				List<IMailboxMessageGameData> list = new List<IMailboxMessageGameData>(socialEnvironment.PendingMailboxMessages);
				DebugLog(string.Concat(GetType(), "ReAddResendMessges: Number of pending Messages = ", list.Count));
				socialEnvironment.PendingMailboxMessages.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					socialEnvironment.AddIncomingMessage(list[i].GetMessageData());
				}
			}
			if (socialEnvironment.Data.ResendMessages != null)
			{
				DebugLog(string.Concat(GetType(), "ReAddResendMessges: Number of resend Messages = ", socialEnvironment.Data.ResendMessages.Count));
				socialEnvironment.AddIncomingMessages(socialEnvironment.Data.ResendMessages);
				socialEnvironment.Data.ResendMessages.Clear();
			}
		}

		public bool AreMessageAddRequirementsFullfilled(PlayerGameData playerGameData, IMailboxMessageGameData messageGameData)
		{
			return messageGameData.CheckAddRequirements(playerGameData);
		}

		public void SetMatchmakingPlayerName(string name, SocialEnvironmentGameData socialEnvironmentGameData)
		{
			socialEnvironmentGameData.Data.MatchmakingPlayerName = name;
		}

		public PublicPlayerData GetFallbackPlayer(int mastery, int level, EquipmentState eqState, int league)
		{
			List<BirdData> list = new List<BirdData>();
			list.Add(GetRandomBirdWithEquipmentState(level, (eqState != 0) ? 2 : 0, mastery, eqState, "bird_red"));
			list.Add(GetRandomBirdWithEquipmentState(level, (eqState != 0) ? 2 : 0, mastery, eqState, "bird_yellow"));
			list.Add(GetRandomBirdWithEquipmentState(level, (eqState != 0) ? 2 : 0, mastery, eqState, "bird_white"));
			list.Add(GetRandomBirdWithEquipmentState(level, (eqState != 0) ? 2 : 0, mastery, eqState, "bird_black"));
			list.Add(GetRandomBirdWithEquipmentState(level, (eqState != 0) ? 2 : 0, mastery, eqState, "bird_blue"));
			BannerData randomBanner = GetRandomBanner(level, (eqState != 0) ? 2 : 0, league, eqState);
			PublicPlayerData publicPlayerData = new PublicPlayerData();
			publicPlayerData.EventPlayerName = string.Empty;
			publicPlayerData.Birds = list;
			publicPlayerData.Level = level;
			publicPlayerData.SocialAvatarUrl = string.Empty;
			publicPlayerData.SocialPlayerName = string.Empty;
			publicPlayerData.Inventory = new InventoryGameData("npc_inventory").Data;
			publicPlayerData.Banner = randomBanner;
			publicPlayerData.RandomDecisionSeed = UnityEngine.Random.Range(0f, float.MaxValue);
			return publicPlayerData;
		}

		public BirdData GetRandomBirdWithLevel(int level, string birdname = "")
		{
			int level2 = Math.Max(level, 1);
			List<BirdBalancingData> list = new List<BirdBalancingData>();
			BirdBalancingData balancing = null;
			if (string.IsNullOrEmpty(birdname))
			{
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_red", out balancing))
				{
					list.Add(balancing);
				}
				BirdBalancingData balancing2 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_yellow", out balancing2))
				{
					list.Add(balancing2);
				}
				BirdBalancingData balancing3 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_white", out balancing3))
				{
					list.Add(balancing3);
				}
				BirdBalancingData balancing4 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_black", out balancing4))
				{
					list.Add(balancing4);
				}
				BirdBalancingData balancing5 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_blue", out balancing5))
				{
					list.Add(balancing5);
				}
			}
			else
			{
				BirdBalancingData balancing6 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>(birdname, out balancing6))
				{
					list.Add(balancing6);
				}
			}
			BirdBalancingData randomBird = list[UnityEngine.Random.Range(0, list.Count)];
			BirdData birdData = new BirdData();
			List<EquipmentBalancingData> list2 = (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.RestrictedBirdId == randomBird.NameId && e.ItemType == InventoryItemType.MainHandEquipment && string.IsNullOrEmpty(e.CorrespondingSetItemId)
				select e).ToList();
			EquipmentBalancingData equipmentBalancingData = list2[UnityEngine.Random.Range(0, list2.Count)];
			List<EquipmentBalancingData> list3 = (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.RestrictedBirdId == randomBird.NameId && e.ItemType == InventoryItemType.OffHandEquipment && string.IsNullOrEmpty(e.CorrespondingSetItemId)
				select e).ToList();
			EquipmentBalancingData equipmentBalancingData2 = list3[UnityEngine.Random.Range(0, list3.Count)];
			List<ClassItemBalancingData> list4 = (from c in DIContainerBalancing.Service.GetBalancingDataList<ClassItemBalancingData>()
				where c.RestrictedBirdId == randomBird.NameId
				select c).ToList();
			ClassItemBalancingData classItemBalancingData = list4[UnityEngine.Random.Range(0, list4.Count)];
			birdData.NameId = randomBird.NameId;
			birdData.Level = level2;
			birdData.Inventory = new InventoryData
			{
				NameId = randomBird.DefaultInventoryNameId
			};
			birdData.Inventory.MainHandItems = new List<EquipmentData>();
			birdData.Inventory.OffHandItems = new List<EquipmentData>();
			birdData.Inventory.ClassItems = new List<ClassItemData>();
			birdData.Inventory.MainHandItems.Add(new EquipmentData
			{
				IsNew = true,
				ItemSource = EquipmentSource.Loot,
				NameId = equipmentBalancingData.NameId,
				Quality = 2,
				ScrapLoot = new Dictionary<string, int>(),
				Level = level2,
				Value = 1
			});
			birdData.Inventory.OffHandItems.Add(new EquipmentData
			{
				IsNew = true,
				ItemSource = EquipmentSource.Loot,
				NameId = equipmentBalancingData2.NameId,
				Quality = 2,
				ScrapLoot = new Dictionary<string, int>(),
				Level = level2,
				Value = 1
			});
			birdData.Inventory.ClassItems.Add(new ClassItemData
			{
				IsNew = true,
				Level = 1,
				Quality = 1,
				Value = 1,
				NameId = classItemBalancingData.NameId
			});
			return birdData;
		}

		public BirdData GetRandomBirdWithLevelHigh(int level, int equipmentLevelOffset, int masteryLevel, int enchantmentlevel, string birdname = "")
		{
			int num = Math.Max(level, 1);
			BirdBalancingData randomBirdWithoutEquipment = GetRandomBirdWithoutEquipment(birdname);
			BirdData birdData = new BirdData();
			EquipmentBalancingData randomOffHand;
			EquipmentBalancingData randomSetEquipment = GetRandomSetEquipment(randomBirdWithoutEquipment, out randomOffHand);
			ClassItemBalancingData randomClass = GetRandomClass(randomBirdWithoutEquipment);
			birdData.NameId = randomBirdWithoutEquipment.NameId;
			birdData.Level = num;
			birdData.Inventory = new InventoryData
			{
				NameId = randomBirdWithoutEquipment.DefaultInventoryNameId
			};
			birdData.Inventory.MainHandItems = new List<EquipmentData>();
			birdData.Inventory.OffHandItems = new List<EquipmentData>();
			birdData.Inventory.ClassItems = new List<ClassItemData>();
			birdData.Inventory.MainHandItems.Add(new EquipmentData
			{
				IsNew = true,
				ItemSource = EquipmentSource.Loot,
				NameId = randomSetEquipment.NameId,
				Quality = 4,
				ScrapLoot = new Dictionary<string, int>(),
				Level = num + equipmentLevelOffset,
				Value = 1,
				EnchantmentLevel = enchantmentlevel
			});
			birdData.Inventory.OffHandItems.Add(new EquipmentData
			{
				IsNew = true,
				ItemSource = EquipmentSource.Loot,
				NameId = randomOffHand.NameId,
				Quality = 4,
				ScrapLoot = new Dictionary<string, int>(),
				Level = num + equipmentLevelOffset,
				Value = 1,
				EnchantmentLevel = enchantmentlevel
			});
			birdData.Inventory.ClassItems.Add(new ClassItemData
			{
				IsNew = true,
				Level = masteryLevel,
				Quality = 1,
				Value = 1,
				NameId = randomClass.NameId
			});
			return birdData;
		}

		public BirdData GetRandomBirdWithEquipmentState(int level, int equipmentLevelOffset, int masteryLevel, EquipmentState equipmentState, string birdname = "")
		{
			int num = Math.Max(level, 1);
			BirdBalancingData randomBirdWithoutEquipment = GetRandomBirdWithoutEquipment(birdname);
			BirdData birdData = new BirdData();
			EquipmentBalancingData equipmentBalancingData = null;
			EquipmentBalancingData randomOffHand;
			switch (equipmentState)
			{
			case EquipmentState.Random:
				equipmentBalancingData = GetRandomEquipment(randomBirdWithoutEquipment, out randomOffHand);
				break;
			case EquipmentState.PartlyMatchingSets:
				equipmentBalancingData = ((!(UnityEngine.Random.value < 0.66f)) ? GetRandomSetEquipment(randomBirdWithoutEquipment, out randomOffHand) : GetRandomEquipment(randomBirdWithoutEquipment, out randomOffHand));
				break;
			case EquipmentState.AllMatchingSets:
				equipmentBalancingData = GetRandomSetEquipment(randomBirdWithoutEquipment, out randomOffHand);
				break;
			default:
				LogError("Unkown Equipmentstate");
				return birdData;
			}
			int enchantmentLevel = Math.Min(10, GetAverageEnchantmentLevel());
			ClassItemBalancingData randomClass = GetRandomClass(randomBirdWithoutEquipment);
			ClassSkinBalancingData randomSkin = GetRandomSkin(randomClass);
			birdData.NameId = randomBirdWithoutEquipment.NameId;
			birdData.Level = num;
			birdData.Inventory = new InventoryData
			{
				NameId = randomBirdWithoutEquipment.DefaultInventoryNameId
			};
			birdData.Inventory.MainHandItems = new List<EquipmentData>();
			birdData.Inventory.OffHandItems = new List<EquipmentData>();
			birdData.Inventory.ClassItems = new List<ClassItemData>();
			birdData.Inventory.SkinItems = new List<SkinItemData>();
			birdData.Inventory.MainHandItems.Add(new EquipmentData
			{
				IsNew = true,
				ItemSource = EquipmentSource.Loot,
				NameId = equipmentBalancingData.NameId,
				Quality = 2,
				ScrapLoot = new Dictionary<string, int>(),
				Level = num + equipmentLevelOffset,
				Value = 1,
				EnchantmentLevel = enchantmentLevel
			});
			birdData.Inventory.OffHandItems.Add(new EquipmentData
			{
				IsNew = true,
				ItemSource = EquipmentSource.Loot,
				NameId = randomOffHand.NameId,
				Quality = 2,
				ScrapLoot = new Dictionary<string, int>(),
				Level = num + equipmentLevelOffset,
				Value = 1,
				EnchantmentLevel = enchantmentLevel
			});
			birdData.Inventory.ClassItems.Add(new ClassItemData
			{
				IsNew = true,
				Level = masteryLevel,
				Quality = 1,
				Value = 1,
				NameId = randomClass.NameId
			});
			birdData.Inventory.SkinItems.Add(new SkinItemData
			{
				IsNew = true,
				Level = 1,
				Quality = 1,
				Value = 1,
				NameId = randomSkin.NameId
			});
			return birdData;
		}

		private int GetAverageEnchantmentLevel()
		{
			int num = 0;
			int num2 = 0;
			foreach (BirdGameData bird in DIContainerInfrastructure.GetCurrentPlayer().Birds)
			{
				if (bird.MainHandItem.EnchantementLevel > 0 || bird.OffHandItem.EnchantementLevel > 0)
				{
					num += 2;
					num2 += bird.MainHandItem.EnchantementLevel;
					num2 += bird.OffHandItem.EnchantementLevel;
				}
			}
			if (num > 0)
			{
				num2 /= num;
			}
			return num2;
		}

		private static ClassItemBalancingData GetRandomClass(BirdBalancingData randomBird)
		{
			List<ClassItemBalancingData> list = (from c in DIContainerBalancing.Service.GetBalancingDataList<ClassItemBalancingData>()
				where c.RestrictedBirdId == randomBird.NameId && string.IsNullOrEmpty(c.ReplacementClassNameId) && !c.Inactive
				select c).ToList();
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		private static ClassSkinBalancingData GetRandomSkin(ClassItemBalancingData randomClass)
		{
			List<ClassSkinBalancingData> list = (from c in DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>()
				where c.OriginalClass == randomClass.NameId && c.UseInPvPFallback
				select c).ToList();
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		private static EquipmentBalancingData GetRandomSetEquipment(BirdBalancingData randomBird, out EquipmentBalancingData randomOffHand)
		{
			List<EquipmentBalancingData> list = (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.RestrictedBirdId == randomBird.NameId && e.ItemType == InventoryItemType.MainHandEquipment && !string.IsNullOrEmpty(e.CorrespondingSetItemId) && !e.HideInPreview
				select e).ToList();
			EquipmentBalancingData equipmentBalancingData = list[UnityEngine.Random.Range(0, list.Count)];
			string correspondingSetItem = equipmentBalancingData.CorrespondingSetItemId;
			List<EquipmentBalancingData> list2 = new List<EquipmentBalancingData>();
			list2 = (string.IsNullOrEmpty(correspondingSetItem) ? (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.RestrictedBirdId == randomBird.NameId && e.ItemType == InventoryItemType.OffHandEquipment && string.IsNullOrEmpty(e.CorrespondingSetItemId)
				select e).ToList() : (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.RestrictedBirdId == randomBird.NameId && e.ItemType == InventoryItemType.OffHandEquipment && e.NameId == correspondingSetItem
				select e).ToList());
			randomOffHand = list2[UnityEngine.Random.Range(0, list2.Count)];
			return equipmentBalancingData;
		}

		private BannerItemBalancingData GetRandomBannerSetEquipment(out BannerItemBalancingData randomTip)
		{
			List<BannerItemBalancingData> list = (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where e.ItemType == InventoryItemType.Banner && !e.HideInPreview && !string.IsNullOrEmpty(e.CorrespondingSetItem)
				select e).ToList();
			BannerItemBalancingData bannerItemBalancingData = list[UnityEngine.Random.Range(0, list.Count)];
			string correspondingSetItem = bannerItemBalancingData.CorrespondingSetItem;
			List<BannerItemBalancingData> list2 = new List<BannerItemBalancingData>();
			list2 = (string.IsNullOrEmpty(correspondingSetItem) ? (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where e.ItemType == InventoryItemType.BannerTip && !e.HideInPreview && string.IsNullOrEmpty(e.CorrespondingSetItem)
				select e).ToList() : (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where e.ItemType == InventoryItemType.BannerTip && e.NameId == correspondingSetItem
				select e).ToList());
			randomTip = list2[UnityEngine.Random.Range(0, list2.Count)];
			return bannerItemBalancingData;
		}

		private static BannerItemBalancingData GetRandomBannerEquipment(out BannerItemBalancingData randomTip)
		{
			List<BannerItemBalancingData> list = (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where e.ItemType == InventoryItemType.Banner && !e.HideInPreview
				select e).ToList();
			BannerItemBalancingData result = list[UnityEngine.Random.Range(0, list.Count)];
			List<BannerItemBalancingData> list2 = (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where e.ItemType == InventoryItemType.BannerTip && !e.HideInPreview
				select e).ToList();
			randomTip = list2[UnityEngine.Random.Range(0, list2.Count)];
			return result;
		}

		private static EquipmentBalancingData GetRandomEquipment(BirdBalancingData randomBird, out EquipmentBalancingData randomOffHand)
		{
			List<EquipmentBalancingData> list = (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.RestrictedBirdId == randomBird.NameId && e.ItemType == InventoryItemType.MainHandEquipment && !e.HideInPreview
				select e).ToList();
			EquipmentBalancingData result = list[UnityEngine.Random.Range(0, list.Count)];
			List<EquipmentBalancingData> list2 = new List<EquipmentBalancingData>();
			list2 = (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.RestrictedBirdId == randomBird.NameId && e.ItemType == InventoryItemType.OffHandEquipment && !e.HideInPreview
				select e).ToList();
			randomOffHand = list2[UnityEngine.Random.Range(0, list2.Count)];
			return result;
		}

		private static BirdBalancingData GetRandomBirdWithoutEquipment(string birdname)
		{
			List<BirdBalancingData> list = new List<BirdBalancingData>();
			if (string.IsNullOrEmpty(birdname))
			{
				BirdBalancingData balancing = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_red", out balancing))
				{
					list.Add(balancing);
				}
				BirdBalancingData balancing2 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_yellow", out balancing2))
				{
					list.Add(balancing2);
				}
				BirdBalancingData balancing3 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_white", out balancing3))
				{
					list.Add(balancing3);
				}
				BirdBalancingData balancing4 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_black", out balancing4))
				{
					list.Add(balancing4);
				}
				BirdBalancingData balancing5 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_blue", out balancing5))
				{
					list.Add(balancing5);
				}
			}
			else
			{
				BirdBalancingData balancing6 = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>(birdname, out balancing6))
				{
					list.Add(balancing6);
				}
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public BannerData GetRandomBanner(int level, int equipmentLevelOffset, int league, EquipmentState equipmentState)
		{
			BannerData bannerData = new BannerData();
			BannerItemBalancingData randomTip = null;
			BannerItemBalancingData bannerItemBalancingData;
			switch (equipmentState)
			{
			case EquipmentState.Random:
				bannerItemBalancingData = GetRandomBannerEquipment(out randomTip);
				break;
			case EquipmentState.PartlyMatchingSets:
				bannerItemBalancingData = ((!(UnityEngine.Random.value < 0.66f)) ? GetRandomBannerSetEquipment(out randomTip) : GetRandomBannerEquipment(out randomTip));
				break;
			case EquipmentState.AllMatchingSets:
				bannerItemBalancingData = GetRandomBannerSetEquipment(out randomTip);
				break;
			default:
				LogError("Unkown Equipmentstate");
				return bannerData;
			}
			List<BannerItemBalancingData> list = (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where e.ItemType == InventoryItemType.BannerEmblem && !e.HideInPreview
				select e).ToList();
			BannerItemBalancingData bannerItemBalancingData2 = list[UnityEngine.Random.Range(0, list.Count)];
			int averageEnchantmentLevel = GetAverageEnchantmentLevel();
			bannerData.NameId = "bird_banner";
			bannerData.Level = level;
			bannerData.Inventory = new InventoryData
			{
				NameId = "inventory_empty"
			};
			bannerData.Inventory.BannerItems = new List<BannerItemData>();
			bannerData.Inventory.BannerItems.Add(new BannerItemData
			{
				IsNew = true,
				NameId = randomTip.NameId,
				Quality = 2,
				Level = level + equipmentLevelOffset,
				Value = 1,
				EnchantmentLevel = averageEnchantmentLevel
			});
			bannerData.Inventory.BannerItems.Add(new BannerItemData
			{
				IsNew = true,
				NameId = bannerItemBalancingData.NameId,
				Quality = 2,
				Level = level + equipmentLevelOffset,
				Value = 1,
				EnchantmentLevel = averageEnchantmentLevel
			});
			bannerData.Inventory.BannerItems.Add(new BannerItemData
			{
				IsNew = true,
				NameId = bannerItemBalancingData2.NameId,
				Quality = 2,
				Level = level + equipmentLevelOffset,
				Value = 1,
				EnchantmentLevel = averageEnchantmentLevel
			});
			return bannerData;
		}
		
	}
}
