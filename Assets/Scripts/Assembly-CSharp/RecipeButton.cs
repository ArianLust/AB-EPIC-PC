using UnityEngine;

public class RecipeButton : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_Button;

	private string m_recipeID = string.Empty;

	private MonoBehaviour m_callbackScript;

	public void Awake()
	{
		m_Button.Clicked -= OnRecipeSelected;
		m_Button.Clicked += OnRecipeSelected;
	}

	public void OnDestroy()
	{
		if ((bool)m_Button)
		{
			m_Button.Clicked -= OnRecipeSelected;
		}
	}

	public void SetRecipe(string str, MonoBehaviour callbackScript)
	{
		m_recipeID = str;
		m_callbackScript = callbackScript;
	}

	public void OnRecipeSelected()
	{
		m_callbackScript.SendMessage("RecipeSelected", m_recipeID);
	}
}
