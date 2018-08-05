using Alchemy4Tridion.Plugins.GUI.Configuration;
using Alchemy4Tridion.Plugins.GUI.Configuration.Elements;

namespace PublishedSummary.GUI
{
    public class PluginResourceGroup : ResourceGroup
    {
        public PluginResourceGroup()
        {
            AddFile("PublishedSummaryCommand.js");
            AddFile("Styles.css");
            AddFile<PluginCommandSet>();
            AddWebApiProxy();
            AttachToView("PublishedSummary.aspx");
            Dependencies.Add("Tridion.Web.UI.Editors.CME");
        }
    }
}
