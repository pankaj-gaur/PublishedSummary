using PublishedSummary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tridion.ContentManager.CoreService.Client;

namespace PublishedSummary.Helper
{
   public class GetPublishedInfo
    {
        //public List<Item> getPubInfo(List<ListItems> MultipleListItems)
        //{
        //    foreach (var MultipleListItem in MultipleListItems)
        //    {
        //        foreach (var item in MultipleListItem.Item)
        //        {
        //            var publishInfo = Client.GetListPublishInfo(item.ID);
        //            if (publishInfo.Count() > 0)
        //            {
        //                var lastPublishedDetials = publishInfo.OrderByDescending(pi => pi.PublishedAt).First();
        //                item.PublishedAt = lastPublishedDetials.PublishedAt;
        //                item.PublicationTarget = lastPublishedDetials.PublicationTarget.Title;
        //                item.User = lastPublishedDetials.User.Title;
        //            }

        //        }

        //    }

        //    var PublishedItems = from MultipleListItem in MultipleListItems
        //                         select MultipleListItem.Item.Where(x => x.PublicationTarget != null).ToList();

        //    List<Item> finalList = new List<Item>();
        //    foreach (var PublishedItem in PublishedItems)
        //    {
        //        foreach (var item in PublishedItem)
        //        {
        //            finalList.Add(item);
        //        }
        //    }

        //}
    }
}
