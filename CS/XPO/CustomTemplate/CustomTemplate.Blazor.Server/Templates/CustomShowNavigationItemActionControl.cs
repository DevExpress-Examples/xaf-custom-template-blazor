using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls;
using Microsoft.AspNetCore.Components;

namespace CustomTemplate.Blazor.Server.Templates; 
public class CustomShowNavigationItemActionControl : ISingleChoiceActionControl {
    private ChoiceActionItemCollection choiceActionItems;
    private EventHandler<SingleChoiceActionControlExecuteEventArgs> execute;
    string IActionControl.ActionId => "ShowNavigationItem";
    object IActionControl.NativeControl => this;
    public IEnumerable<ChoiceActionItem> Items => choiceActionItems;
    // The CustomShowNavigationItemActionControlComponent is added in the next step.
    public RenderFragment GetComponentContent(RenderFragment titleTemplate) => CustomShowNavigationItemActionControlComponent.Create(titleTemplate, this);
    void ISingleChoiceActionControl.SetChoiceActionItems(ChoiceActionItemCollection choiceActionItems) => this.choiceActionItems = choiceActionItems;
    public void DoExecute(ChoiceActionItem choiceActionItem) {
        execute?.Invoke(this, choiceActionItem == null ? new SingleChoiceActionControlExecuteEventArgs() : new SingleChoiceActionControlExecuteEventArgs(choiceActionItem));
    }
    event EventHandler<SingleChoiceActionControlExecuteEventArgs> ISingleChoiceActionControl.Execute {
        add => execute += value;
        remove => execute -= value;
    }

    void IActionControl.SetCaption(string caption) { }
    void IActionControl.SetConfirmationMessage(string confirmationMessage) { }
    void IActionControl.SetEnabled(bool enabled) { }
    void IActionControl.SetImage(string imageName) { }
    void IActionControl.SetPaintStyle(ActionItemPaintStyle paintStyle) { }
    void ISingleChoiceActionControl.SetSelectedItem(ChoiceActionItem selectedItem) { }
    void IActionControl.SetShortcut(string shortcutString) { }
    void ISingleChoiceActionControl.SetShowItemsOnClick(bool value) { }
    void IActionControl.SetToolTip(string toolTip) { }
    void ISingleChoiceActionControl.Update(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo) { }
    void IActionControl.SetVisible(bool visible) { }
    event EventHandler IActionControl.NativeControlDisposed { add { } remove { } }
}
