Type.registerNamespace("Alchemy.Plugins.PublishedSummary.Views");

Alchemy.Plugins.PublishedSummary.Views.PublishedSummaryPopup = function PublishedSummaryPopup() {
    Tridion.OO.enableInterface(this, "Alchemy.Plugins.PublishedSummary.Views.PublishedSummaryPopup");
    this.addInterface("Tridion.Cme.View");
};

Alchemy.Plugins.PublishedSummary.Views.PublishedSummaryPopup.prototype.initialize = function PublishedSummary$initialize() {
    this.callBase("Tridion.Cme.View", "initialize");
};

$display.registerView(Alchemy.Plugins.PublishedSummary.Views.PublishedSummaryPopup);
