/**
 * Creates an anguilla command using a wrapper shorthand.
 *
 * Note the ${PluginName} will get replaced by the actual plugin name.
 */
Alchemy.command("${PluginName}", "PublishedSummary", {

    /**
     * If an init function is created, this will be called from the command's constructor when a command instance
     * is created.
     */
    init: function () {

    },

    /**
     * Whether or not the command is enabled for the user (will usually have extensions displayed but disabled).
     * @returns {boolean}
     */
    isEnabled: function () {
        return true;
    },

    /**
     * Whether or not the command is available to the user.
     * @returns {boolean}
     */
    isAvailable: function () {
        return true;
    },

    /**
     * Executes your command. You can use _execute or execute as the property name.
     */
    execute: function (selection) {
        var url = "${ViewsUrl}PublishedSummary.aspx";

        var onPopupClose = function () {
            $evt.removeEventHandler(this.properties.popupInstance, "unload", this.getDelegate(this.properties.popupCloseHandler));
            this.properties.popupInstance = null;
            this.properties.popupCloseHandler = null;
        }

        var popup = this.properties.popupInstance;
        //var args = { popupType: Tridion.Controls.Popup.Type.MODAL_IFRAME, items: selection.getItems() };
        var args = { items: selection.getItems() };
        if (popup) {
            popup.focus();
        }
        else {
            popup = $popup.create(url, "menubar=no,location=no,resizable=no,scrollbars=yes,status=no,width=800,height=350", args);

            this.properties.popupInstance = popup;
            this.properties.popupCloseHandler = onPopupClose;

            $evt.addEventHandler(popup, "unload", this.getDelegate(onPopupClose));
            $evt.addEventHandler(popup, "load", this.getDelegate(this.onLoad));

            popup.open();
        }
       
    },
    onLoad: function(selection)
    {
        
    }
});