﻿// ***********************************************************************
// Assembly         : PublishedSummary
// Author           : admin
// Created          : 08-20-2018
//
// Last Modified By : admin
// Last Modified On : 08-25-2018
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
using System;
using TCM=Tridion.ContentManager;
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
            GetPublishedInfo getPublishedInfo = new GetPublishedInfo();
            XmlDocument publicationList = new XmlDocument();
            PublicationsFilterData filter = new PublicationsFilterData();
            XElement publications = Client.GetSystemWideListXml(filter);
            if (publications == null) throw new ArgumentNullException(nameof(publications));
            List<Publications> publicationsList = getPublishedInfo.Publications(publicationList, publications);
            return publicationsList;
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
            if (allPublicationTargets == null) throw new ArgumentNullException(nameof(allPublicationTargets));
            return allPublicationTargets;
        }
        #endregion

        #region Get List of all published items from Folder, Publications
        /// <summary>
        /// Gets all published items.
        /// </summary>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException">listXml</exception>
        [HttpGet, Route("GetAllPublishedItems")]
        public object GetAllPublishedItems()
        {
            string[] tcmIds = { "tcm:0-14-1", "tcm:0-9-1" };
            GetPublishedInfo getFinalPublishedInfo = new GetPublishedInfo();
            var multipleListItems = new List<ListItems>();
            XmlDocument doc = new XmlDocument();
            foreach (var tcmId in tcmIds)
            {
                TCM.TcmUri iTcmUri = new TCM.TcmUri(tcmId);
                XElement listXml = null;
                switch (iTcmUri.ItemType.ToString())
                {
                    case "Publication":
                        listXml = Client.GetListXml(tcmId, new RepositoryItemsFilterData
                        {
                            ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate, ItemType.Category, ItemType.Page },
                            Recursive = true,
                            BaseColumns = ListBaseColumns.Extended
                        });
                        break;
                    case "Folder":
                        listXml = Client.GetListXml(tcmId, new OrganizationalItemItemsFilterData
                        {
                            ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate },
                            Recursive = true,
                            BaseColumns = ListBaseColumns.Extended
                        });
                        break;
                    case "StructureGroup":
                        listXml = Client.GetListXml(tcmId, new OrganizationalItemItemsFilterData()
                        {
                            ItemTypes = new[] { ItemType.Page },
                            Recursive = true,
                            BaseColumns = ListBaseColumns.Extended
                        });
                        break;
                    case "Category":
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (listXml == null) throw new ArgumentNullException(nameof(listXml));
                doc.LoadXml(listXml.ToString());
                multipleListItems.Add(TransformObjectAndXml.Deserialize<ListItems>(doc));
            }
            return getFinalPublishedInfo.FilterIsPublishedItem(multipleListItems)
                .SelectMany(publishedItem => publishedItem, (publishedItem, item) => new {publishedItem, item})
                .Select(@t => new {@t, publishInfo = Client.GetListPublishInfo(@t.item.ID)})
                .SelectMany(@t => getFinalPublishedInfo.ReturnFinalList(@t.publishInfo, @t.@t.item)).ToList();
        }
        #endregion

        #region Get GetAnalyticData
        /// <summary>
        /// Gets the analytic data.
        /// </summary>
        /// <returns>System.Object.</returns>
        [HttpGet, Route("GetAnalyticData")]
        public object GetAnalyticData()
        {
            var listXml = Client.GetListXml("tcm:0-14-1", new RepositoryItemsFilterData
            {
                ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate, ItemType.Category, ItemType.Page },
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
                    var lastPublishedDetails = publishInfo.OrderByDescending(pi => pi.PublishedAt).First();
                    item.PublicationTarget = lastPublishedDetails.PublicationTarget.Title;
                    item.User = lastPublishedDetails.User.Title;
                    item.Type = item.Type == "64" ? Enum.GetName(typeof(ItemType), 64) :
                        item.Type == "512" ? Enum.GetName(typeof(ItemType), 512) :
                        item.Type == "32" ? Enum.GetName(typeof(ItemType), 32) :
                        item.Type == "16" ? Enum.GetName(typeof(ItemType), 16) : item.Type;
                }

            }

            return compList.Item.Where(x => x.PublicationTarget != null && x.PublicationTarget == "DXA Staging")
                .GroupBy(x => x.Type).Select(c => new { key = c.Key, total = c.Count() }).Where(x => x.key != null);
        }
        #endregion

        #region Get Published History of an Item.
        /// <summary>
        /// Gets the analytic data.
        /// </summary>
        /// <returns>System.Object.</returns>
        [HttpGet, Route("GetItemPublishedHistory")]
        public object GetItemPublishedHistory()
        {
            GetPublishedInfo publishedInfos = new GetPublishedInfo();
            var publishInfos = Client.GetListPublishInfo("tcm:14-495-64");
            var itemPublishedHistory = publishedInfos.GetPublishedHistory(publishInfos);
            return itemPublishedHistory;
        }
        #endregion

    }
}
