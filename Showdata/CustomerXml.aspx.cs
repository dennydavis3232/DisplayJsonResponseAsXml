using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;

namespace Showdata
{
    public partial class CustomerXml : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(DisplayApiResponse));
            }
        }

        private async Task DisplayApiResponse()
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["url"];
                string PhoneNumber = Request.QueryString["Mobilenumber"].ToString();

                Logging($"Mobile number entered: {PhoneNumber}", "MobileNumberLogs");
                // Encode the phone number for the URL
                string encodedPhoneNumber = Uri.EscapeDataString(PhoneNumber);

                // Construct the request URL
                string requestUrl = $"{apiUrl}?a=CustomerInfo:searchByPhone&p=[\"{encodedPhoneNumber}\"]";

                string authTokenFilePath = Server.MapPath(ConfigurationManager.AppSettings["tokenpath"]);
                string authToken = System.IO.File.ReadAllText(authTokenFilePath);

                // Make the GET request and include the AuthToken header
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("AuthToken", authToken.Trim()); // Trim any leading or trailing whitespace
                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    Logging($"response statuscode: {response.StatusCode}", "");
                    Logging($"response content: {response.Content}", "");
                    string xmlResponse = "";
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        // Read and display the response content as XML
                        CustomerResponse customerResponse = JsonConvert.DeserializeObject<CustomerResponse>(jsonResponse);
                        if (customerResponse.data.Count > 0)
                        {
                            // Initialize properties into variables
                            string referenceNo = customerResponse.data[0].REFERENCE_NO;
                            string status = customerResponse.data[0].STATUS;
                            string nameEn = customerResponse.data[0].NAME_EN;
                            string nameAr = customerResponse.data[0].NAME_AR;
                            string idType = customerResponse.data[0].ID_TYPE;
                            string idNumber = customerResponse.data[0].ID_NUMBER;
                            string mobile = customerResponse.data[0].MOBILE;
                            string phone = customerResponse.data[0].PHONE;

                            xmlResponse = $"<response>\n" +
                          $"<referenceno>{referenceNo}</referenceno>\n" +
                          $"<status>{status}</status>\n" +
                          $"<name_english>{nameEn}</name_english>\n" +
                          $"<name_arabic>{nameAr}</name_arabic>\n" +
                          $"<id_type>{idType}</id_type>\n" +
                          $"<id_number>{idNumber}</id_number>\n" +
                          $"<mobile>{mobile}</mobile>\n" +
                          $"<phone>{phone}</phone>\n" +
                          $"</response>";

                           
                        }

                        else
                        {
                             xmlResponse = $"<response>\n" +
                         $"<referenceno>No data found</referenceno>\n" +
                         $"<status>0</status>\n" +
                         $"<name_english>0</name_english>\n" +
                         $"<name_arabic>0</name_arabic>\n" +
                         $"<id_type>0</id_type>\n" +
                         $"<id_number>0</id_number>\n" +
                         $"<mobile>0</mobile>\n" +
                         $"<phone>0</phone>\n" +
                         $"</response>";
                        }
                        Logging($"xmlresponse generated: {xmlResponse}", "");


                        Response.Clear();


                        Response.ContentType = "text/xml";
                        Response.Write(xmlResponse);
                        Response.End();
                    }
                    else
                    {
                       
                        Logging($"incorrect success statuscode", "");

                    }
                }

            }
            catch (Exception ex)
            {
                
                Logging($"error on Displayapiresponse" +ex.Message, "error");
            }
        }
        public void Logging(string str, string sType)
        {
            while (true)
            {
                try
                {
                    string file = AppDomain.CurrentDomain.BaseDirectory + "/LOGS/" + sType + DateTime.Now.ToString("ddMMyy") + ".LOG";
                    File.AppendAllText(file, DateTime.Now.ToLongTimeString() + "***" + str + Environment.NewLine);
                    break;
                }
                catch 
                {

                }
            }
        }
    }
}