﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PublishedSummary.Helper
{
  public class TransformObjectAndXml
        {
            public static T Deserialize<T>(XmlDocument xmlDocument)
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));

                StringReader reader = new StringReader(xmlDocument.InnerXml);
                XmlReader xmlReader = new XmlTextReader(reader);
                //Deserialize the object.
                return (T)ser.Deserialize(xmlReader);
            }

            public static bool Serialize<T>(T value, ref string serializeXml)
            {
                try
                {
                    XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                    StringWriter stringWriter = new StringWriter();
                    XmlWriter writer = XmlWriter.Create(stringWriter);

                    xmlserializer.Serialize(writer, value);

                    serializeXml = stringWriter.ToString();

                    writer.Close();
                    return true;
                }
                catch (Exception ex)
                {

                    return false;
                }
            }
        }
    }

