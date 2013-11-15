using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace WebPageToHtml
{
    public class WebHtml
    {
        protected string _url;
        protected string _html;
        protected int _width, _height;

        /// <summary>
        /// Generates a website thumbnail for the given URL
        /// </summary>
        /// <param name="url">Address of website from which to generate the thumbnail</param>
        /// <returns>string</returns>
        public static string GetHtml(string url)
        {
            WebHtml thumbnail = new WebHtml(url);
            return thumbnail.GetHtml();
         }

        /// <summary>
        /// Protected constructor
        /// </summary>
        protected WebHtml(string url)
        {
            _url = url;
            _html = "";
            _width = 1280;
            _height = 1024;
        }

        /// <summary>
        /// Returns a thumbnail for the current member values
        /// </summary>
        /// <returns>Thumbnail bitmap</returns>
        protected string GetHtml()
        {
            // WebBrowser is an ActiveX control that must be run in a
            // single-threaded apartment so create a thread to create the
            // control and generate the thumbnail
            Thread thread = new Thread(new ThreadStart(GetHtmlWorker));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return _html;
        }

        /// <summary>
        /// Creates a WebBrowser control to generate the thumbnail image
        /// Must be called from a single-threaded apartment
        /// </summary>
        protected void GetHtmlWorker()
        {
            try
            {
                // create a local temp file to load the HTML into
                var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Temp.htm");

                // this indicates that we have HTML and not a url
                if (string.IsNullOrEmpty(_url))
                {
                    System.IO.File.WriteAllText(fileName, _html);
                    _url = fileName;

                    // format url for browser
                    _url = "file:///" + _url.Replace(@"\", "/");;
                }

                using (WebBrowser browser = new WebBrowser())
                {
                    browser.ClientSize = new Size(_width, _height);
                    browser.ScrollBarsEnabled = false;
                    browser.ScriptErrorsSuppressed = true;

                    browser.Navigate(_url);
                    
                    // Wait for control to load page
                    while (browser.ReadyState != WebBrowserReadyState.Complete)
                        Application.DoEvents();

                    _html = browser.DocumentText;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
