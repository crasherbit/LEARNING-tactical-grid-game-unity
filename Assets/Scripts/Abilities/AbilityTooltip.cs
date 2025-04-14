// using UnityEngine;
// using UnityEngine.UIElements;

// public class AbilityTooltip : MonoBehaviour
// {
//     public UIDocument uiDocument;
//     private VisualElement tooltipContainer;
//     private Label abilityNameLabel;
//     private Label abilityDescriptionLabel;
//     private Label abilityCostLabel;
//     private Label abilityCooldownLabel;

//     void Start()
//     {
//         if (uiDocument == null)
//         {
//             Debug.LogError("AbilityTooltip: UIDocument non assegnato!");
//             return;
//         }

//         // Ottieni i riferimenti agli elementi UI
//         var root = uiDocument.rootVisualElement;
//         tooltipContainer = root.Q<VisualElement>("tooltip-container");
//         abilityNameLabel = root.Q<Label>("ability-name");
//         abilityDescriptionLabel = root.Q<Label>("ability-description");
//         abilityCostLabel = root.Q<Label>("ability-cost");
//         abilityCooldownLabel = root.Q<Label>("ability-cooldown");

//         // Nascondi il tooltip inizialmente
//         HideTooltip();
//     }

//     // Mostra il tooltip con le informazioni dell'abilità
//     public void ShowTooltip(Ability ability, Vector2 position)
//     {
//         if (tooltipContainer == null) return;

//         abilityNameLabel.text = ability.name;
//         abilityDescriptionLabel.text = ability.description;
//         abilityCostLabel.text = $"Costo: {ability.actionPointCost} AP";
//         abilityCooldownLabel.text = ability.cooldown > 0 ? 
//             $"Cooldown: {ability.currentCooldown}/{ability.cooldown}" : 
//             "No Cooldown";

//         // Posiziona il tooltip vicino al cursore
//         tooltipContainer.style.left = position.x + 20;
//         tooltipContainer.style.top = position.y;

//         // Mostra il tooltip
//         tooltipContainer.style.display = DisplayStyle.Flex;
//     }

//     // Nasconde il tooltip
//     public void HideTooltip()
//     {
//         if (tooltipContainer == null) return;
//         tooltipContainer.style.display = DisplayStyle.None;
//     }
// }