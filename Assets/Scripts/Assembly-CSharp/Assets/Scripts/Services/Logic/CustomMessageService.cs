using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.GameDatas.MailboxMessages;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;

namespace Assets.Scripts.Services.Logic
{
	public class CustomMessageService
	{
		public static readonly string AssetName = DIContainerInfrastructure.GetTargetBuildGroup() + "_CustomMessages.txt";

		private readonly List<CustomMessageGameData> m_mails = new List<CustomMessageGameData>();

		private readonly Dictionary<string, CustomMessage> m_messages = new Dictionary<string, CustomMessage>();

		public void AcknowledgedCustomMessage(IMailboxMessageGameData msg)
		{
			if (msg is CustomMessageGameData)
			{
				CustomMessageGameData msgCustom = msg as CustomMessageGameData;
				int num = m_mails.RemoveAll((CustomMessageGameData md) => md.Equals(msgCustom));
				if (num > 0 && m_messages.ContainsKey(msgCustom.Id))
				{
					CustomMessage customMessage = m_messages[msgCustom.Id];
					m_messages.Remove(msgCustom.Id);
					DIContainerInfrastructure.GetCurrentPlayer().Data.AcknowledgedCustomMessages.Add(customMessage);
					DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
					DebugLog.Log("[CustomMessageService] AcknowledgedCustomMessage(" + customMessage.Key + ", " + customMessage.NameId + "): removed the message and saved the player data.");
				}
				else
				{
					DebugLog.Log(string.Concat("[CustomMessageService] AcknowledgedCustomMessage(", msg, "): not a CustomMessage, skipping. Detail: m_mails conatined it? ", num, ", m_messages? ", m_messages.ContainsKey(msg.Id)));
				}
			}
		}

		public IEnumerator Init(Action onDownloadStarted, Action<float> onDownloadProgress, Action<bool> onIsSlowCallback)
		{
			DebugLog.Log("[CustomMessageService] Start Init!");
			List<CustomMessage> msgs = new List<CustomMessage>();
			DebugLog.Log("[CustomMessageService] Need to retrieve the " + AssetName);
			onDownloadStarted();
			bool done = false;
			string result = string.Empty;
			DIContainerInfrastructure.GetAssetsService().Load(AssetName, delegate(string res)
			{
				result = res;
				done = true;
			}, onDownloadProgress, onIsSlowCallback);
			while (!done)
			{
				yield return new WaitForEndOfFrame();
			}
			if (result != null)
			{
				ParseCustomMessages(msgs, result);
			}
			else
			{
				DebugLog.Error("[CustomMessageService] Could not retrieve");
			}
			GenerateMessageGameDatas(msgs);
		}

		private void GenerateMessageGameDatas(List<CustomMessage> msgs)
		{
			foreach (CustomMessage msg in msgs)
			{
				DebugLog.Log("[CustomMessageService] Adding " + msg.Key + ", " + msg.NameId + " to the SocialEnvironmentGameData via AddIncomingMessage");
				GenerateSingleCustomMessageGameData(msg);
			}
		}

		private void GenerateSingleCustomMessageGameData(CustomMessage msg)
		{
			CustomMessageBalancingData balancing = null;
			IMailboxMessageGameData mailboxMessageGameData;
			if (DIContainerBalancing.Service.TryGetBalancingData<CustomMessageBalancingData>(msg.NameId, out balancing))
			{
				FriendData friendData = null;
				FriendGameData value = null;
				friendData = ((!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(balancing.NPCNameId, out value)) ? DIContainerLogic.SocialService.GetHighNPCFriend(1) : value.FriendData);
				mailboxMessageGameData = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.AddIncomingMessage(new MessageDataIncoming
				{
					Id = msg.Key,
					MessageType = MessageType.CustomMessageGameData,
					Sender = friendData,
					Parameter1 = msg.NameId
				});
			}
			else
			{
				mailboxMessageGameData = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.AddIncomingMessage(new MessageDataIncoming
				{
					Id = msg.Key,
					MessageType = MessageType.CustomMessageGameData,
					Sender = DIContainerLogic.SocialService.GetHighNPCFriend(1),
					Parameter1 = msg.NameId
				});
			}
			if (!m_mails.Contains((CustomMessageGameData)mailboxMessageGameData))
				m_mails.Add((CustomMessageGameData)mailboxMessageGameData);
			if (!m_messages.ContainsKey(msg.Key))
				m_messages.Add(msg.Key, msg);
		}

		private static void ParseCustomMessages(List<CustomMessage> msgs, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				DebugLog.Error("[CustomMessageService] ParseCustomMessages: path is null or empty, returning.");
				return;
			}
			byte[] array = FileHelper.ReadAllBytes(path);
			string @string = Encoding.UTF8.GetString(array, 0, array.Length);
			IEnumerable<string> source = from line in @string.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
				select line.Trim();
			List<CustomMessage> allMessages = (from msg in source.Select(ParseCustomMessage)
				where msg != null
				select msg).ToList();
			DetermineNewMessages(msgs, allMessages);
		}

		private static void DetermineNewMessages(List<CustomMessage> msgs, List<CustomMessage> allMessages)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.AcknowledgedCustomMessages == null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.AcknowledgedCustomMessages = new List<CustomMessage>();
			}
			foreach (CustomMessage allMessage in allMessages)
			{
				if (!DIContainerInfrastructure.GetCurrentPlayer().Data.AcknowledgedCustomMessages.Contains(allMessage))
				{
					msgs.Add(allMessage);
				}
			}
		}

		private static CustomMessage ParseCustomMessage(string line)
		{
			string[] array = (from part in line.Split(new char[1] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries)
				select part.Trim()).ToArray();
			if (array.Length != 2)
			{
				DebugLog.Error("[CustomMessageService] ParseCustomMessage: " + line + " is invalid (no ';')");
				return null;
			}
			string text = ((!string.IsNullOrEmpty(array[0])) ? array[0] : "default");
			string text2 = array[1];
			DebugLog.Log("[CustomMessageService] ParseCustomMessage: " + line + " is a valid Message: " + text + ", " + text2);
			CustomMessage customMessage = new CustomMessage();
			customMessage.Key = text;
			customMessage.NameId = text2;
			return customMessage;
		}
	}
}
