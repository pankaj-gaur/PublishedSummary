using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PublishedSummary.Models
{
    [XmlRoot(ElementName = "Item", Namespace = "http://www.tridion.com/ContentManager/5.0")]
    public class Item
    {
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
        [XmlAttribute(AttributeName = "SchemaId")]
        public string SchemaId { get; set; }
        [XmlAttribute(AttributeName = "OwningPublicationTitle")]
        public string OwningPublicationTitle { get; set; }
        [XmlAttribute(AttributeName = "OwningPublicationID")]
        public string OwningPublicationID { get; set; }
        [XmlAttribute(AttributeName = "Modified")]
        public DateTime Modified { get; set; }
        [XmlAttribute(AttributeName = "IsNew")]
        public string IsNew { get; set; }
        [XmlAttribute(AttributeName = "FromPub")]
        public string FromPub { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "Icon")]
        public string Icon { get; set; }
        [XmlAttribute(AttributeName = "Title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "Size")]
        public string Size { get; set; }
        public string openItem { get; set; }
        public string schemaname { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string iscurrentYear { get; set; }

        public List<string> PublicationTarget { get; set; }
        public List<string> User { get; set; }

        public List<DateTime> PublishedAt { get; set; }
    }

    [XmlRoot(ElementName = "ListItems", Namespace = "http://www.tridion.com/ContentManager/5.0")]
    public class ListItems
    {
        [XmlElement(ElementName = "Item", Namespace = "http://www.tridion.com/ContentManager/5.0")]
        public List<Item> Item { get; set; }
        [XmlAttribute(AttributeName = "ext", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ext { get; set; }
        [XmlAttribute(AttributeName = "tcm", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Tcm { get; set; }
        [XmlAttribute(AttributeName = "Managed")]
        public string Managed { get; set; }
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
        
        public string PublicationTarget { get; set; }
        public string User { get; set; }

        public DateTime PublishedAt { get; set; }
    }
}
