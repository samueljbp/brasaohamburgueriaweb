using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Mvc;
using System.Web;

namespace BrasaoSolution.Helper.Extentions
{
    public class JsonNetResult : JsonResult
    {

        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public JsonNetResult()
        {
            SerializerSettings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Unspecified, ContractResolver = new CamelCasePropertyNamesContractResolver(), PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

            Formatting = Formatting.Indented;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }
}