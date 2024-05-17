using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Blazor;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors.ActionControls;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.Blazor.Templates;
using DevExpress.ExpressApp.Blazor.Templates.ContextMenu.ActionControls;
using DevExpress.ExpressApp.Blazor.Templates.Navigation.ActionControls;
using DevExpress.ExpressApp.Blazor.Templates.Security.ActionControls;
using DevExpress.ExpressApp.Blazor.Templates.Toolbar.ActionControls;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.Persistent.Base;
using Microsoft.AspNetCore.Components;

namespace CustomTemplate.Blazor.Server.Templates {
    public class CustomMainFormTemplate : WindowTemplateBase, IMainFormTemplate, ISupportActionsToolbarVisibility, ISelectionDependencyToolbar, ISupportListEditorInlineActions, ISupportListEditorContextMenuActions, ITemplateToolbarProvider, ITabbedMdiMainFormTemplate {
        public CustomMainFormTemplate() : this(null) { }
        public CustomMainFormTemplate(IModelOptionsBlazor modelOptions) {
            ModelOptionsBlazor = modelOptions;

            NavigateBackActionControl = new NavigateBackActionControl();
            AddActionControl(NavigateBackActionControl);
            AccountComponent = new AccountComponentAdapter();
            AddActionControls(AccountComponent.ActionControls);
            CustomShowNavigationItemActionControl = new CustomShowNavigationItemActionControl();
            AddActionControl(CustomShowNavigationItemActionControl);

            HeaderToolbar = new DxToolbarAdapter(new DxToolbarModel() {
                CssClass = "ps-2"
            });
            HeaderToolbar.AddActionContainer(nameof(PredefinedCategory.QuickAccess), ToolbarItemAlignment.Right);
            HeaderToolbar.AddActionContainer(nameof(PredefinedCategory.Notifications), ToolbarItemAlignment.Right);
            HeaderToolbar.AddActionContainer(nameof(PredefinedCategory.Diagnostic), ToolbarItemAlignment.Right);

            if (IsTabbedMdi) {
                ChildTemplates = new ObservableCollection<ITabbedMdiDetailFormTemplate>();
                TabsModel = new DxTabsModel();
                TabsModel.TabsPosition = ModelOptionsBlazor?.TabPosition ?? TabsPosition.Top;
                TabsModel.RenderMode = TabsRenderMode.OnDemand;
                TabsModel.ScrollMode = TabsScrollMode.NavButtons;
                TabsModel.ActiveTabIndex = ActiveTemplateIndex;
                TabsModel.ActiveTabIndexChanged = EventCallback.Factory.Create<int>(this, OnActiveTabIndexChanged);
                TabsModel.CssClass = "xaf-tabbed-mdi h-100";
                TabsModel.ChildContent = builder => {
                    foreach (var item in ChildTemplates) {
                        item.IsActive = item == ActiveTemplate;
                        builder.AddContent(0, item.TabPageModel.GetComponentContent());
                    }
                };
            }
            else {
                IsActionsToolbarVisible = true;
                Toolbar = new DxToolbarAdapter(new DxToolbarModel());
                Toolbar.AddActionContainer(nameof(PredefinedCategory.ObjectsCreation));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.RecordsNavigation), ToolbarItemAlignment.Right);
                Toolbar.AddActionContainer(nameof(PredefinedCategory.SaveOptions), ToolbarItemAlignment.Right, isDropDown: true, defaultActionId: "SaveAndNew", autoChangeDefaultAction: true);
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Save), ToolbarItemAlignment.Right);
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Close));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.UndoRedo));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Edit));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.RecordEdit));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.View));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Reports));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Search));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Filters));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.FullTextSearch));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Tools));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Export));
                Toolbar.AddActionContainer(nameof(PredefinedCategory.Unspecified));

                ListEditorActionColumnAdapter = new ListEditorActionColumnAdapter();
                ListEditorActionColumnAdapter.AddActionContainer(nameof(PredefinedCategory.ListView));
                ListEditorActionColumnAdapter.AddActionContainer(nameof(PredefinedCategory.Edit));
                ListEditorActionColumnAdapter.AddActionContainer(nameof(PredefinedCategory.RecordEdit));

                DxContextMenuAdapter = new DxContextMenuAdapter(new DxContextMenuModel());
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.ObjectsCreation));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.Save));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.SaveOptions), isDropDown: true, defaultActionId: "SaveAndNew", autoChangeDefaultAction: true);
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.Edit));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.RecordEdit));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.UndoRedo));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.Print));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.View));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.Export));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.Reports));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.OpenObject));
                DxContextMenuAdapter.AddActionContainer(nameof(PredefinedCategory.Menu));
            }
        }
        protected IModelOptionsBlazor ModelOptionsBlazor { get; set; }
        protected override IEnumerable<IActionControlContainer> GetActionControlContainers() {
            if (IsTabbedMdi) {
                return HeaderToolbar.ActionContainers;
            }
            return Toolbar.ActionContainers.Union(HeaderToolbar.ActionContainers.Union(ListEditorActionColumnAdapter.ActionContainers.Union(DxContextMenuAdapter.ActionContainers)));
        }
        protected override RenderFragment CreateComponent() => CustomMainFormTemplateComponent.Create(this);
        protected override void BeginUpdate() {
            base.BeginUpdate();
            ((ISupportUpdate)Toolbar)?.BeginUpdate();
        }
        protected override void EndUpdate() {
            ((ISupportUpdate)Toolbar)?.EndUpdate();
            base.EndUpdate();
        }
        public void CloseViewTemplate(ITabbedMdiDetailFormTemplate childTemplate) {
            ChildTemplates.Remove(childTemplate);
            TemplateClosed?.Invoke(this, new DetailFormTemplateChangedEventArgs(childTemplate));
        }
        public void TryAddChildTemplate(ITabbedMdiDetailFormTemplate childTemplate) {
            if (childTemplate is null) {
                return;
            }
            if (!ChildTemplates.Contains(childTemplate)) {
                ChildTemplates.Add(childTemplate);

                childTemplate.Activated += ChildTemplate_Activated;
                childTemplate.Closed += ChildTemplate_Closed;
            }
            SetActiveTemplateIndex(ChildTemplates.IndexOf(childTemplate));
        }

        private void ChildTemplate_Activated(object sender, EventArgs e) {
            var activeTemplate = (ITabbedMdiDetailFormTemplate)sender;

            SetActiveTemplate(activeTemplate);
            ActiveTemplateChanged?.Invoke(this, new DetailFormTemplateChangedEventArgs(activeTemplate));
        }

        private void OnActiveTabIndexChanged(int index) {
            if (index >= 0) {
                SetActiveTemplateIndex(index);

                ActiveTemplateChanged?.Invoke(this, new DetailFormTemplateChangedEventArgs(ChildTemplates[index]));
            }
        }
        private void ChildTemplate_Closed(object sender, EventArgs e) {
            var childTemplate = (ITabbedMdiDetailFormTemplate)sender;
            childTemplate.Closed -= ChildTemplate_Closed;
            childTemplate.Activated -= ChildTemplate_Activated;

            CloseViewTemplate(childTemplate);
        }

        public void RemoveChildTemplate(ITabbedMdiDetailFormTemplate childTemplate) {
            if (ChildTemplates.Contains(childTemplate)) {
                ChildTemplates.Remove(childTemplate);

                RefreshTabs();
            }
        }
        public void RefreshTabs() {
            ChildTemplatesChanged?.Invoke(this, EventArgs.Empty);
        }
        public void SetActiveTemplate(ITabbedMdiDetailFormTemplate childTemplate) {
            var activeIndex = ChildTemplates.IndexOf(childTemplate);

            if (ActiveTemplateIndex != activeIndex) {
                SetActiveTemplateIndex(activeIndex);
            }
            RefreshTabs();
        }
        public void SetActiveTemplateIndex(int index) {
            ActiveTemplateIndex = index;
            TabsModel.ActiveTabIndex = ActiveTemplateIndex;
        }
        public int ActiveTemplateIndex { get; private set; } = 0;
        public ITabbedMdiDetailFormTemplate ActiveTemplate {
            get {
                if (ActiveTemplateIndex >= 0 && ChildTemplates.Count > ActiveTemplateIndex) {
                    return ChildTemplates[ActiveTemplateIndex];
                }
                return null;
            }
        }
        public bool IsActionsToolbarVisible { get; private set; }
        public NavigateBackActionControl NavigateBackActionControl { get; }
        public AccountComponentAdapter AccountComponent { get; }
        public ShowNavigationItemActionControl ShowNavigationItemActionControl => throw new NotSupportedException();
        public CustomShowNavigationItemActionControl CustomShowNavigationItemActionControl { get; }
        public DxToolbarAdapter Toolbar { get; }
        public DxToolbarAdapter HeaderToolbar { get; }
        public ListEditorActionColumnAdapter ListEditorActionColumnAdapter { get; }
        public DxContextMenuAdapter DxContextMenuAdapter { get; }
        public string AboutInfoString { get; set; }

        public DxTabsModel TabsModel { get; }
        public bool IsTabbedMdi => ModelOptionsBlazor?.UIType == UIType.TabbedMDI;
        protected ObservableCollection<ITabbedMdiDetailFormTemplate> ChildTemplates { get; private set; }
        public event EventHandler ChildTemplatesChanged;
        public event EventHandler<DetailFormTemplateChangedEventArgs> ActiveTemplateChanged;
        public event EventHandler<DetailFormTemplateChangedEventArgs> TemplateClosed;

        void ISupportActionsToolbarVisibility.SetVisible(bool isVisible) => IsActionsToolbarVisible = isVisible;
    }
}
