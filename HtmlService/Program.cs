using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using WebPageToHtml;

namespace HtmlService
{
    [ServiceContract]
    public interface IHtmlServer
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/htmlservice?uri={uri}")]
        string GetHtml(string uri);
    }

    public class Service : IHtmlServer
    {
        public string GetHtml(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }
            else
            {
                if ((uri.IndexOf("file:", System.StringComparison.Ordinal) < 0) &&
                    (uri.IndexOf("http", System.StringComparison.Ordinal) < 0))
                    uri = "http://" + uri;

                Thumbnail.Uri = uri;
                try
                {
                    string html =
                        WebHtml.GetHtml(Thumbnail.Uri);
                    return html;
                }
                catch (Exception)
                {
                    throw;
                }
                return null;
            }
        }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            string baseAddress = "http://" + Environment.MachineName + ":8040/";
            ServiceHost host = new ServiceHost(typeof (Service), new Uri(baseAddress));
            try
            {
                ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if( smb == null)
                    smb = new ServiceMetadataBehavior();

                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                host.AddServiceEndpoint(typeof (IHtmlServer), new WebHttpBinding(), "")
                   .Behaviors.Add(new WebHttpBehavior());
                host.Open();

                Console.WriteLine("Service is running");
                Console.Write("Press ENTER to close the host");
                Console.ReadLine();
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message);
                Console.Read();
            }
            finally
            {
                host.Close();
            }
        }
    }
}
