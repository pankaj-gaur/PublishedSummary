// ***********************************************************************
// Assembly         : PublishedSummary
// Author           : admin
// Created          : 08-20-2018
//
// Last Modified By : admin
// Last Modified On : 08-22-2018
// ***********************************************************************
// <copyright file="PluginController.cs" company="Content Bloom">
//     Copyright © Content Bloom 2018
// </copyright>
// <summary></summary>
// ***********************************************************************
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
    /// <seealso cref="Alchemy4Tridion.Plugins.AlchemyApiController" />
    /// <remarks>The AlchemyRoutePrefix accepts a Service Name as its first parameter.  This will be used by both
    /// the generated Url's as well as the generated JS proxy.
    /// <c>/Alchemy/Plugins/{YourPluginName}/api/{ServiceName}/{action}</c><c>Alchemy.Plugins.YourPluginName.Api.Service.action()</c>
    /// The attribute is optional and if you exclude it, url's and methods will be attached to "api" instead.
    /// <c>/Alchemy/Plugins/{YourPluginName}/api/{action}</c><c>Alchemy.Plugins.YourPluginName.Api.action()</c></remarks>
    [AlchemyRoutePrefix("Service")]
    public class PluginController : AlchemyApiController
    {


        #region  Get list of all publications
        /// <summary>
        /// Gets the publication list.
        /// </summary>
        /// <returns>List&lt;Publications&gt;.</returns>
        [HttpGet]
        [Route("GetPublicationList")]
        public List<Publications> GetPublicationList()
        {
            XmlDocument publicationList = new XmlDocument();
            PublicationsFilterData filter = new PublicationsFilterData();
            XElement publications = Client.GetSystemWideListXml(filter);
            publicationList.Load(publications.CreateReader());
            ListPublications pubList = TransformObjectAndXml.Deserialize<ListPublications>(publicationList);
            List<Publications> items = pubList.Item;
            List<Publications> item2 = new List<Publications>();
            foreach (var item in items)
            {
                var splitTcmid = item.ID.Split('-');
                item.ID = splitTcmid[1];
                item2.Add(item);
            }
            return item2;
        }
        #endregion

        #region Get List of all publication targets
        /// <summary>
        /// Gets the publication target.
        /// </summary>
        /// <returns>System.Object.</returns>
        [HttpGet]
        [Route("GetPublicationTarget")]
        public object GetPublicationTarget()
        {
            
            var filter = new TargetTypesFilterData();
            var allPublicationTargets = Client.GetSystemWideList(filter);

            return allPublicationTargets;
        }
        #endregion

        #region  Get list of all Pages inside SG
        /// <summary>
        /// Gets the pages inside sg.
        /// </summary>
        /// <param name="IDs">The ids.</param>
        /// <returns>List&lt;Item&gt;.</returns>
        /// <exception cref="ArgumentNullException">listXml</exception>
        [HttpPost]
        [Route("GetPagesInsideSG")]
        public List<Item> GetPagesInsideSG(JObject IDs)
        {
            
            dynamic tcmIDs = IDs;
            var sgIDs = tcmIDs.IDs;
            List<ListItems> multipleListItems = new List<ListItems>();
            XmlDocument doc = new XmlDocument();
            foreach (var sgId in sgIDs)
            {
                var listXml = Client.GetListXml(sgId.ToString(), new OrganizationalItemItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Page },
                    Recursive = true,
                    BaseColumns = ListBaseColumns.Extended
                });
                if (listXml == null) throw new ArgumentNullException(nameof(listXml));
                doc.LoadXml(listXml.ToString());
                multipleListItems.Add(TransformObjectAndXml.Deserialize<ListItems>(doc));
            }
            var publishedItems = from multipleListItem in multipleListItems
                select multipleListItem.Item.Where(x => x.IsPublished == "true").ToList();

            IEnumerable<List<Item>> listItems = publishedItems.ToList();
            
            foreach (var publishedItem in listItems)
            {
                foreach (var item in publishedItem)
                {
                    var publishInfo = Client.GetListPublishInfo(item.ID);
                    if (!publishInfo.Any()) continue;
                    IEnumerable<PublishInfoData> getPublishedInfos = publishInfo.OrderByDescending(pubAt => pubAt.PublishedAt).GroupBy(pubTarget => pubTarget.PublicationTarget.Title).Select(pubTarget => pubTarget.FirstOrDefault());

                    foreach (var getPublishedInfo in getPublishedInfos)
                    {
                        if (getPublishedInfo == null) continue;
                        item.PublishedAt?.Add(getPublishedInfo.PublishedAt);
                        item.PublicationTarget?.Add(getPublishedInfo.PublicationTarget.Title);
                        item.User?.Add(getPublishedInfo.User.Title);
                    }

                }

            }
            List<Item> finalList = listItems.SelectMany(publishedItem => publishedItem).ToList();

            return finalList;
        }
        #endregion

        #region  Get list of all Components inside Folder
        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentNullException">listXml</exception>
        [HttpGet]
        [Route("GetComponents")]
        public object GetComponents()
        {
            //string[] fodlerIds = { "tcm:35-504-2", "tcm:35-490-2" };
            string[] FolderIDs = { "tcm:14-62-2" };
            List<ListItems> multipleListItems = new List<ListItems>();
            XmlDocument doc = new XmlDocument();
            foreach (var folderID in FolderIDs)
            {
                var listXml = Client.GetListXml(folderID, new OrganizationalItemItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate },
                    ComponentTypes = new[] { ComponentType.Normal, ComponentType.Multimedia },
                    Recursive = true,
                    BaseColumns = ListBaseColumns.Extended
                });
                if (listXml == null) throw new ArgumentNullException(nameof(listXml));
                doc.LoadXml(listXml.ToString());
                multipleListItems.Add(TransformObjectAndXml.Deserialize<ListItems>(doc));
            }
            var publishedItems = from multipleListItem in multipleListItems
                                 select multipleListItem.Item.Where(x => x.IsPublished == "true").ToList();

            IEnumerable<List<Item>> listItems = publishedItems.ToList();

            foreach (var publishedItem in listItems)
            {
                foreach (var item in publishedItem)
                {
                    var publishInfo = Client.GetListPublishInfo(item.ID);
                    if (!publishInfo.Any()) continue;
                    IEnumerable<PublishInfoData> getPublishedInfos = publishInfo.OrderByDescending(pubAt => pubAt.PublishedAt).GroupBy(pubTarget => pubTarget.PublicationTarget.Title).Select(pubTarget => pubTarget.FirstOrDefault());

                    foreach (var getPublishedInfo in getPublishedInfos)
                    {
                        if (getPublishedInfo == null) continue;
                        item.PublishedAt?.Add(getPublishedInfo.PublishedAt);
                        item.PublicationTarget?.Add(getPublishedInfo.PublicationTarget.Title);
                        item.User?.Add(getPublishedInfo.User.Title);
                    }

                }

            }
            List<Item> finalList = listItems.SelectMany(publishedItem => publishedItem).ToList();
            return finalList;
        }
        #endregion

        #region Get List of all published items in a Publication(s)
        /// <summary>
        /// Gets all published items.
        /// </summary>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentNullException">listXml</exception>
        [HttpGet]
        [Route("GetAllPublishedItems")]
        public object GetAllPublishedItems()
        {
            string[] pubIDs = { "tcm:0-14-1", "tcm:0-9-1" };
            List<ListItems> multipleListItems = new List<ListItems>();
            XmlDocument doc = new XmlDocument();
            foreach (var pubId in pubIDs)
            {
                var listXml = Client.GetListXml(pubId, new RepositoryItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate, ItemType.Category, ItemType.Page },
                    Recursive = true,
                    BaseColumns = ListBaseColumns.Extended
                });
                if (listXml == null) throw new ArgumentNullException(nameof(listXml));
                doc.LoadXml(listXml.ToString());
                multipleListItems.Add(TransformObjectAndXml.Deserialize<ListItems>(doc));
            }
            var publishedItems = from multipleListItem in multipleListItems
                                 select multipleListItem.Item.Where(x => x.IsPublished == "true").ToList();

            IEnumerable<List<Item>> listItems = publishedItems.ToList();

            foreach (var publishedItem in listItems)
            {
                foreach (var item in publishedItem)
                {
                    var publishInfo = Client.GetListPublishInfo(item.ID);
                    if (!publishInfo.Any()) continue;
                    IEnumerable<PublishInfoData> getPublishedInfos = publishInfo.OrderByDescending(pubAt => pubAt.PublishedAt).GroupBy(pubTarget => pubTarget.PublicationTarget.Title).Select(pubTarget => pubTarget.FirstOrDefault());

                    foreach (var getPublishedInfo in getPublishedInfos)
                    {
                        if (getPublishedInfo == null) continue;
                        item.PublishedAt?.Add(getPublishedInfo.PublishedAt);
                        item.PublicationTarget?.Add(getPublishedInfo.PublicationTarget.Title);
                        item.User?.Add(getPublishedInfo.User.Title);
                        item.openItem = PageURL.GetDomain() + "/WebUI/item.aspx?tcm=" + item.Type + "#id=" + item.ID;
                        item.Type = item.Type == "64" ? "Pages" : item.Type == "512" ? "Categories" : item.Type == "32" ? "Component Templates" : item.Type == "16" ? "Component" : item.Type;
                    }

                }

            }
            List<Item> finalList = listItems.SelectMany(publishedItem => publishedItem).ToList();
            return finalList;
        }
        #endregion

        #region Get GetAnalyticData
        /// <summary>
        /// Gets the analytic data.
        /// </summary>
        /// <returns>System.Object.</returns>
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
