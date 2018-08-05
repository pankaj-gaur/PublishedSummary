using Alchemy4Tridion.Plugins;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.IO;
using System.Text.RegularExpressions;
using Tridion.ContentManager.CoreService.Client;
using System.Net;
using System.ServiceModel;
using System.Xml.Linq;
using System.Xml;
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
        [HttpPost]
        [Route("GetPublishedSummary")]
        public string CreateMM(string tcmURI)
        {
            return "Success";
        }
    }
}
