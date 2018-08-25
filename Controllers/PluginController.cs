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
using TCM = Tridion.ContentManager;
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
        [HttpGet,Route("GetPublicationList")]
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
        [HttpGet,Route("GetPublicationTarget")]
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
        public object GetAllPublishedItems(/*JObject tcmIDs*/)
        {
           string[] demoids = { "tcm:0-14-1"};
            GetPublishedInfo getFinalPublishedInfo = new GetPublishedInfo();
            var multipleListItems = new List<ListItems>();
            XmlDocument doc = new XmlDocument();
            //dynamic iDs = tcmIDs;
            //var itemIDs = iDs.IDs;
            foreach (var tcmId in demoids)
            {
                TCM.TcmUri iTcmUri = new TCM.TcmUri(tcmId.ToString());
                XElement listXml = null;
                switch (iTcmUri.ItemType.ToString())
                {
                    case CONSTANTS.PUBLICATION:
                        listXml = Client.GetListXml(tcmId.ToString(), new RepositoryItemsFilterData
                        {
                            ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate, ItemType.Category, ItemType.Page },Recursive = true,
                            BaseColumns = ListBaseColumns.Extended
                        });
                        break;
                    case CONSTANTS.FOLDER:
                        listXml = Client.GetListXml(tcmId.ToString(), new OrganizationalItemItemsFilterData
                        {
                            ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate },Recursive = true,BaseColumns = ListBaseColumns.Extended
                        });
                        break;
                    case CONSTANTS.STRUCTUREGROUP:
                        listXml = Client.GetListXml(tcmId.ToString(), new OrganizationalItemItemsFilterData()
                        {
                            ItemTypes = new[] { ItemType.Page },Recursive = true,BaseColumns = ListBaseColumns.Extended
                        });
                        break;
                    case CONSTANTS.CATEGORY:
                        listXml = Client.GetListXml(tcmId.ToString(), new RepositoryItemsFilterData
                        {
                            ItemTypes = new[] { ItemType.Category },
                            Recursive = true,
                            BaseColumns = ListBaseColumns.Extended
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (listXml == null) throw new ArgumentNullException(nameof(listXml));
                doc.LoadXml(listXml.ToString());
                multipleListItems.Add(TransformObjectAndXml.Deserialize<ListItems>(doc));
            }
            return getFinalPublishedInfo.FilterIsPublishedItem(multipleListItems).SelectMany(publishedItem => publishedItem, (publishedItem, item) => new { publishedItem, item }).Select(@t => new { @t, publishInfo = Client.GetListPublishInfo(@t.item.ID) }).SelectMany(@t => getFinalPublishedInfo.ReturnFinalList(@t.publishInfo, @t.@t.item)).ToList();
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
            string[] tcmIds = { "tcm:0-14-1"};
            GetPublishedInfo getFinalPublishedInfo = new GetPublishedInfo();
            var multipleListItems = new List<ListItems>();
            XmlDocument doc = new XmlDocument();
            foreach (var tcmId in tcmIds)
            {
                var listXml = Client.GetListXml(tcmId, new RepositoryItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Component, ItemType.ComponentTemplate, ItemType.Category, ItemType.Page },Recursive = true,
                    BaseColumns = ListBaseColumns.Extended});
                if (listXml == null) throw new ArgumentNullException(nameof(listXml));
                doc.LoadXml(listXml.ToString());
                multipleListItems.Add(TransformObjectAndXml.Deserialize<ListItems>(doc));
            }
            List<Item> finalList = new List<Item>();
            foreach (var publishedItem in getFinalPublishedInfo.FilterIsPublishedItem(multipleListItems))
            foreach (var item in publishedItem)
            {
                var publishInfo = Client.GetListPublishInfo(item.ID);
                foreach (var item1 in getFinalPublishedInfo.ReturnFinalList(publishInfo, item)) finalList.Add(item1);
            }
            IEnumerable<Analytics> analytics = finalList.GroupBy(x => new { x.PublicationTarget, x.Type }).Select(g => new Analytics { Count = g.Count(), PublicationTarget = g.Key.PublicationTarget, ItemType = g.Key.Type, });
            return analytics;
        }
        #endregion

        #region Get Published History of an Item.
        /// <summary>
        /// Gets the analytic data.
        /// </summary>
        /// <returns>System.Object.</returns>
        [HttpGet, Route("GetItemPublishedHistory")]
        public object GetItemPublishedHistory(/*JObject IDs*/)
        {
            //dynamic iDs = tcmIDs;
            //var itemIDs = iDs.IDs;
            GetPublishedInfo publishedInfos = new GetPublishedInfo();
            return publishedInfos.GetPublishedHistory(Client.GetListPublishInfo("tcm:14-495-64"));
        }
        #endregion

    }
}
