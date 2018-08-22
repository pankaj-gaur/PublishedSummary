using Alchemy4Tridion.Plugins;
using PublishedSummary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tridion.ContentManager.CoreService.Client;

namespace PublishedSummary.Helper
{
    public  class GetPublishedInfo : AlchemyApiController
    {
        public List<Item> GetPublishedInfoList(List<ListItems> multipleListItems)
        {
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
    }
}
