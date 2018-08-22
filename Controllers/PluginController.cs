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
using System.Web;
using System;
using Newtonsoft.Json;
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

        #region  Get list of all publications
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
        #endregion

        #region Get List of all publication targets
        [HttpGet]
        [Route("GetPublicationTarget")]
        public object GetPublicationTarget()
        {
            var readoptions = new ReadOptions();
            var filter1 = new TargetTypesFilterData();
            var allPublicationTargets = Client.GetSystemWideList(filter1);

            return allPublicationTargets;
        }
        #endregion

        #region  Get list of all Pages inside SG
        [HttpPost]
        [Route("GetPagesInsideSG")]
        public List<Item> GetPagesInsideSG(JObject IDs)
        {
            string[] SgIDs = { "tcm:14-65-4"};
            List<ListItems> multipleListItems = new List<ListItems>();
            XmlDocument doc = new XmlDocument();
            foreach (var sgId in SgIDs)
            {
                var listXml = Client.GetListXml(sgId, new OrganizationalItemItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Page },
                    Recursive = true,
                    BaseColumns = ListBaseColumns.Default
                });


                doc.LoadXml(listXml.ToString());
                multipleListItems.Add(TransformObjectAndXml.Deserialize<ListItems>(doc));
            }

            foreach (var multipleListItem in multipleListItems)
            {
                foreach (var item in multipleListItem.Item)
                {
                    var publishInfo = Client.GetListPublishInfo(item.ID);
                    if (publishInfo.Any())
                    {
                        var ff = publishInfo.OrderByDescending(ww => ww.PublishedAt).GroupBy(x => x.PublicationTarget.Title).Select(x => x.FirstOrDefault());

                        foreach (var data in ff)
                        {

                            var lastPublishedDetails = publishInfo.OrderByDescending(pi => pi.PublishedAt).First();
                            item.PublishedAt.Add(data.PublishedAt);
                            item.PublicationTarget.Add(data.PublicationTarget.Title);
                            item.User.Add(data.User.Title);
                        }
                        //var lastPublishedDetails = publishInfo.OrderByDescending(pi => pi.PublishedAt).First();
                        //item.PublishedAt = lastPublishedDetails.PublishedAt;
                        //item.PublicationTarget = lastPublishedDetails.PublicationTarget.Title;
                        //item.User = lastPublishedDetails.User.Title;
                    }

                }

            }

            var publishedItems = from multipleListItem in multipleListItems
                select multipleListItem.Item.Where(x => x.PublicationTarget.Any()).ToList();

            List<Item> finalList = publishedItems.SelectMany(publishedItem => publishedItem).ToList();

            return finalList;
        }
        #endregion

        #region  Get list of all Components inside Folder
        [HttpGet]
        [Route("GetComponents")]
        public object GetComponents()
        {
            //string[] fodlerIds = { "tcm:35-504-2", "tcm:35-490-2" };
            var listXml = Client.GetListXml("tcm:14-217-2", new OrganizationalItemItemsFilterData
            {
                ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate },
                ComponentTypes=new[] { ComponentType.Normal,ComponentType.Multimedia },
                Recursive = true,
                BaseColumns = ListBaseColumns.Default
            });

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(listXml.ToString());
            ListItems componentsList = TransformObjectAndXml.Deserialize<ListItems>(doc);
            foreach (var item in componentsList.Item)
            {
                var publishInfo = Client.GetListPublishInfo(item.ID);
                var ff = publishInfo.OrderByDescending(y => y.PublishedAt).GroupBy(x => x.PublicationTarget.Title).Select(x => x.FirstOrDefault());
                foreach (var data in ff)
                {
                    item.PublishedAt.Add(data.PublishedAt);
                    item.PublicationTarget.Add(data.PublicationTarget.Title);
                    item.User.Add(data.User.Title);
                    
                }

            }
            return componentsList.Item.Where(x => x.PublicationTarget != null);
        }
        #endregion

        #region Get List of all published items of a Publication(s)
        [HttpGet]
        [Route("GetAllPublishedItems")]
        public object GetAllPublishedItems()
        {
            var listXml = Client.GetListXml("tcm:0-14-1", new RepositoryItemsFilterData
            {
                ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate, ItemType.Category, ItemType.Page },
                //ComponentTypes = new[] { ComponentType.Normal, ComponentType.Multimedia },
                Recursive = true,
                BaseColumns = ListBaseColumns.Default
            });
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(listXml.ToString());
            ListItems compList = TransformObjectAndXml.Deserialize<ListItems>(doc);

            foreach (var item in compList.Item)
            {
                var publishInfo = Client.GetListPublishInfo(item.ID);
                if (publishInfo.Any())
                {
                    var ff = publishInfo.OrderByDescending(y => y.PublishedAt).GroupBy(x => x.PublicationTarget.Title).Select(x => x.FirstOrDefault());
                    foreach (var data in ff)
                    {
                        item.PublishedAt.Add(data.PublishedAt);
                        item.PublicationTarget.Add(data.PublicationTarget.Title);
                        item.User.Add(data.User.Title);
                        item.openItem = PageURL.GetDomain() + "/WebUI/item.aspx?tcm=" + item.Type + "#id=" + item.ID;
                        item.Type = item.Type == "64" ? "Pages" : item.Type == "512" ? "Categories" : item.Type == "32" ? "Component Templates" : item.Type == "16" ? "Component" : item.Type;
                    }
                    
                }

            }
            return compList.Item.Where(x => x.PublicationTarget.Count>0);
        }
        #endregion

        #region Get GetAnalyticData
        [HttpGet]
        [Route("GetAnalyticData")]
        public object GetAnalyticData()
        {
            var listXml = Client.GetListXml("tcm:0-14-1", new RepositoryItemsFilterData
            {
                ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate, ItemType.Category, ItemType.Page },
                ComponentTypes = new[] {ComponentType.Normal,ComponentType.Multimedia},
                Recursive = true,
                BaseColumns = ListBaseColumns.Default
            });
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(listXml.ToString());
            ListItems compList = TransformObjectAndXml.Deserialize<ListItems>(doc);
            foreach (var item in compList.Item)
            {
                var publishInfo = Client.GetListPublishInfo(item.ID);
                if (publishInfo.Any())
                {
                    var ff = publishInfo.OrderByDescending(y => y.PublishedAt).GroupBy(x => x.PublicationTarget.Title).Select(x => x.FirstOrDefault());
                    foreach (var data in ff)
                    {
                        item.PublishedAt.Add(data.PublishedAt);
                        item.PublicationTarget.Add(data.PublicationTarget.Title);
                        item.User.Add(data.User.Title);
                        
                        item.Type = item.Type == "64" ? "Pages" : item.Type == "512" ? "Categories" : item.Type == "32" ? "Component Templates" : item.Type == "16" ? "Component" : item.Type;
                    }
                }

            } 

            return compList.Item.Where(x => x.PublicationTarget.Count>0 && x.PublicationTarget.Contains("DXA Staging")).GroupBy(x => x.Type).Select(c => new { key = c.Key, total = c.Count() }).Where(x => x.key != null);
        }
        #endregion

    }
}
