using System.Collections.Generic;
using ABH.Shared.BalancingData;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class BuyWorldShopOfferTutorialStep : BaseTutorialStep
	{
		private WorldMapShopMenuUI m_shopUI;

		private string m_itemName;

		private ShopOfferBlindWorldmap m_shopOffer;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "entered_workshop" && trigger != "triggered_forced")
			{
				return;
			}
			m_itemName = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				m_itemName = m_possibleParams[0];
			}
			m_shopUI = Object.FindObjectOfType(typeof(WorldMapShopMenuUI)) as WorldMapShopMenuUI;
			ShopOfferBlindWorldmap[] componentsInChildren = m_shopUI.GetComponentsInChildren<ShopOfferBlindWorldmap>();
			ShopOfferBlindWorldmap[] array = componentsInChildren;
			foreach (ShopOfferBlindWorldmap shopOfferBlindWorldmap in array)
			{
				DebugLog.Log("Shop Offer Name: " + shopOfferBlindWorldmap.GetModel().NameId);
				if (shopOfferBlindWorldmap.GetModel().NameId == m_itemName)
				{
					m_shopOffer = shopOfferBlindWorldmap;
					break;
				}
			}
			if ((bool)m_shopOffer)
			{
				DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
				List<Requirement> failed = new List<Requirement>();
				if (!DIContainerLogic.GetShopService().IsOfferBuyable(DIContainerInfrastructure.GetCurrentPlayer(), m_shopOffer.GetModel(), out failed))
				{
					DebugLog.Log("[BuyWorldShopOfferTutorialStep] Offer already bought");
					FinishStep("offer_bought", new List<string> { m_shopOffer.GetModel().NameId });
				}
				else
				{
					m_shopOffer.ShopOfferBought -= OnShopOfferBought;
					m_shopOffer.ShopOfferBought += OnShopOfferBought;
					AddHelpersAndBlockers();
					m_Started = true;
				}
			}
		}

		private void AddHelpersAndBlockers()
		{
			m_shopOffer.m_BuyButtonTrigger.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_shopOffer.m_BuyButtonTrigger.transform, TutorialStepType.BuyWorldShopOffer.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "offer_bought") || !(trigger != "triggered_forced"))
			{
				RemoveHelpersAndBlockers(false);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_shopOffer)
			{
				m_shopOffer.m_BuyButtonTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			m_TutorialMgr.HideHelp(TutorialStepType.BuyWorldShopOffer.ToString(), finish);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_shopOffer)
			{
				m_shopOffer.ShopOfferBought -= OnShopOfferBought;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}

		private void OnShopOfferBought(BasicShopOfferBalancingData offer)
		{
			if ((bool)m_shopOffer)
			{
				m_shopOffer.ShopOfferBought -= OnShopOfferBought;
			}
			FinishStep("offer_bought", new List<string> { offer.NameId });
		}
	}
}
