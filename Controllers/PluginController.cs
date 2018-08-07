using Alchemy4Tridion.Plugins;
using System.Web.Http;
using Tridion.ContentManager.CoreService.Client;
using System.Xml.Linq;
using System.Xml;
using PublishedSummary.Models.Model;
using System.Collections.Generic;
using PublishedSummary.Helper;
using Newtonsoft.Json.Linq;
using PublishedSummary.Models;
using System.Linq;

namespace PublishedSummary.Controllers
{
    /// <summary>
    /// An ApiController to create web services that your plugin can interact with.
    /// </summary>
    /// <remarks>
    /// The AlchemyRoutePrefix accepts a Service Name as its first parameter.  This will be used by both
    /// the generated Url's as well as the generated JS proxy.
    /// <c>/Alchemy/Plugins/{YourPluginName}/api/{ServiceName}/{action}</c>
    /// <c>Alchemy.Plugins.YourPluginName.Api.Service.action()</c>
    /// 
    /// The attribute is optional and if you exclude it, url's and methods will be attached to "api" instead.
    /// <c>/Alchemy/Plugins/{YourPluginName}/api/{action}</c>
    /// <c>Alchemy.Plugins.YourPluginName.Api.action()</c>
    /// </remarks>
    [AlchemyRoutePrefix("Service")]
    public class PluginController : AlchemyApiController
    {
        /// // GET /Alchemy/Plugins/{YourPluginName}/api/{YourServiceName}/YourRoute
        /// <summary>
        /// Just a simple example..
        /// </summary>
        /// <returns>A string "Your Response" as the response message.</returns>
        /// </summary>
        /// <remarks>
        /// All of your action methods must have both a verb attribute as well as the RouteAttribute in
        /// order for the js proxy to work correctly.
        /// </remarks>
        /// 


        [HttpGet]
        [Route("GetPublicationList")]
        public List<Publications> GetPublicationList()
        {
            XmlDocument publicationList = new XmlDocument();
            PublicationsFilterData filter = new PublicationsFilterData();
            XElement publications = Client.GetSystemWideListXml(filter);
            publicationList.Load(publications.CreateReader());
            ListPublications pubList = TransformObjectAndXml.Deserialize<ListPublications>(publicationList);
            List<Publications> Item = pubList.Item;
            List<Publications> Item2 = new List<Publications>();
            foreach (var item in Item)
            {
                var splitTCMID = item.ID.Split('-');
                item.ID = splitTCMID[1];
                Item2.Add(item);
            }
            return Item2;
        }

        [HttpGet]
        [Route("AjaxGetComponent")]
        public object AjaxGetComponent()
        {
            var itemTypes = new List<ItemType>();
            //dynamic json = pubId;
            itemTypes.Add(ItemType.Component);
            var filter = new RepositoryItemsFilterData();
            filter.Recursive = true;
            filter.ItemTypes = itemTypes.ToArray();
            //var publishInfo = Client.GetListPublishInfo("tcm:14-1118-64");
            var listXml = Client.GetListXml("tcm:0-" + "14" + "-1", filter);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(listXml.ToString());
            ListItems compList = TransformObjectAndXml.Deserialize<ListItems>(doc);
            foreach (var item in compList.Item)
            {
                var publishInfo = Client.GetListPublishInfo(item.ID);
                if (publishInfo.Count() > 0)
                {
                    var lastPublishedDetials = publishInfo.OrderByDescending(pi => pi.PublishedAt).First();
                    item.PublishedAt = lastPublishedDetials.PublishedAt;
                    item.PublicationTarget = lastPublishedDetials.PublicationTarget.Title;
                    item.User = lastPublishedDetials.User.Title;
                }
                
            }
            return compList;
        }

        [HttpPost]
        [Route("GetPublishedSummary")]
        public string CreateMM(string tcmURI)
        {
            return "Success";
        }
    }
}
