using UnityEngine;
using ColossalFramework.UI;


namespace MoreCitizenUnits
{

    /// <summary>
    /// Static utilities class for creating UI controls.
    /// </summary>
    public static class UIControls
    {
        /// <summary>
        /// Creates a plain checkbox using the game's option panel checkbox template.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="xPos">Relative x position)</param>
        /// <param name="yPos">Relative y position</param>
        /// <param name="text">Descriptive label text</param>
        /// <returns>New checkbox using the game's option panel template</returns>
        public static UICheckBox AddPlainCheckBox(UIComponent parent, float xPos, float yPos, string text)
        {
            UICheckBox checkBox = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsCheckBoxTemplate")) as UICheckBox;

            // Set text.
            checkBox.text = text;

            // Position.
            checkBox.relativePosition = new Vector2(xPos, yPos);

            return checkBox;
        }


        /// <summary>
        /// Adds a plain text label to the specified UI component.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="xPos">Relative x position)</param>
        /// <param name="yPos">Relative y position</param>
        /// <param name="text">Label text</param>
        /// <param name="width">Label width (-1 (default) for autosize)</param>
        /// <param name="width">Text scale (default 1.0)</param>
        /// <returns>New text label</returns>
        public static UILabel AddLabel(UIComponent parent, float xPos, float yPos, string text, float width = -1f, float textScale = 1.0f)
        {
            // Add label.
            UILabel label = (UILabel)parent.AddUIComponent<UILabel>();

            // Set sizing options.
            if (width > 0f)
            {
                // Fixed width.
                label.autoSize = false;
                label.width = width;
                label.autoHeight = true;
                label.wordWrap = true;
            }
            else
            {
                // Autosize.
                label.autoSize = true;
                label.autoHeight = false;
                label.wordWrap = false;
            }

            // Text.
            label.textScale = textScale;
            label.text = text;

            // Position.
            label.relativePosition = new Vector2(xPos, yPos);

            return label;
        }


        /// <summary>
        /// Creates a plain dropdown using the game's option panel dropdown template.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="text">Descriptive label text</param>
        /// <param name="items">Dropdown menu item list</param>
        /// <param name="selectedIndex">Initially selected index (default 0)</param>
        /// <param name="width">Width of dropdown (default 60)</param>
        /// <returns>New dropdown menu using game's option panel template</returns>
        public static UIDropDown AddPlainDropDown(UIComponent parent, string text, string[] items, int selectedIndex = 0, float width = 270f)
        {
            UIPanel panel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsDropdownTemplate")) as UIPanel;
            UIDropDown dropDown = panel.Find<UIDropDown>("Dropdown");

            // Set text.
            panel.Find<UILabel>("Label").text = text;

            // Slightly increase width.
            dropDown.autoSize = false;
            dropDown.width = width;

            // Add items.
            dropDown.items = items;
            dropDown.selectedIndex = selectedIndex;

            return dropDown;
        }


        /// <summary>
        /// Adds an options-panel-style spacer bar across the specified UIComponent.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="xPos">Relative y-position</param>
        /// <param name="yPos">Relative y-position</param>
        /// <param name="width">Spacer width</param>
        public static void OptionsSpacer(UIComponent parent, float xPos, float yPos, float width)
        {
            UIPanel spacerPanel = parent.AddUIComponent<UIPanel>();
            spacerPanel.width = width; ;
            spacerPanel.height = 5f;
            spacerPanel.relativePosition = new Vector2(xPos, yPos);
            spacerPanel.backgroundSprite = "ContentManagerItemBackground";
        }


        /// <summary>
        /// Returns a relative position to the right of a specified UI component, suitable for placing an adjacent component.
        /// </summary>
        /// <param name="uIComponent">Original (anchor) UI component</param>
        /// <param name="margin">Margin between components (default 8)</param>
        /// <param name="verticalOffset">Vertical offset from first to second component (default 0)</param>
        /// <returns>Offset position (to right of original)</returns>
        public static Vector3 PositionRightOf(UIComponent uIComponent, float margin = 8f, float verticalOffset = 0f)
        {
            return new Vector3(uIComponent.relativePosition.x + uIComponent.width + margin, uIComponent.relativePosition.y + verticalOffset);
        }


        /// <summary>
        /// Returns a relative position below a specified UI component, suitable for placing an adjacent component.
        /// </summary>
        /// <param name="uIComponent">Original (anchor) UI component</param>
        /// <param name="margin">Margin between components (default 8)</param>
        /// <param name="horizontalOffset">Horizontal offset from first to second component (default 0)</param>
        /// <returns>Offset position (below original)</returns>
        public static Vector3 PositionUnder(UIComponent uIComponent, float margin = 8f, float horizontalOffset = 0f)
        {
            return new Vector3(uIComponent.relativePosition.x + horizontalOffset, uIComponent.relativePosition.y + uIComponent.height + margin);
        }
    }
}