using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderTicket : MonoBehaviour
{
    [SerializeField] Image dishIcon;
    [SerializeField] Image[] recipeIcons;
    
    [Space(10)]
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text orderNumberText;
    
    [Space(10)]
    [SerializeField] GameObject dineBackground;
    [SerializeField] GameObject deliveryBackground;

    public void UpdateInfo(RecipeID dishID, int number, bool isDelivery)
    {
        var recipeData = PandaResources.Instance.GetRecipeData(dishID);
        dishIcon.sprite = recipeData.GetIcon();
        orderNumberText.text = isDelivery ? $"D{number:000}" : $"{number:000}";

        foreach (var icon in recipeIcons)
        {
            icon.gameObject.SetActive(false);
        }
        
        int iconIdx = 0;
        foreach (var ingredientID in recipeData.GetIngredients())
        {
            recipeIcons[iconIdx].sprite = PandaResources.Instance.GetIngredientIcon(ingredientID);
            recipeIcons[iconIdx].gameObject.SetActive(true);
            
            iconIdx += 1;
        }

        dineBackground.SetActive(!isDelivery);
        deliveryBackground.SetActive(isDelivery);
    }
    
    public void UpdateNumber(int number)
    {
        orderNumberText.text = $"{number:000}";
    }

    public void UpdateTimerText(float time)
    {
        float timeToDisplay = Mathf.Max(0, time);
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
