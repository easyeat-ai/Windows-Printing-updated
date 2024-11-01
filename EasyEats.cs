using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Microsoft.Win32;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Dynamic;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Drawing;
using RestSharp;
using System.Reflection;

namespace ReceiptPrint
{
    public partial class EasyEats : Form
    {
        private string encodedvalue; //Holds the encoded URL
        private string decodedvalue; //Holds the decoded URL
        private string applicationPath = AppDomain.CurrentDomain.BaseDirectory + @"ReceiptPrint.exe";
        private bool status = false;
        private string baseURL = @"https://api.easyeat.ai/";
        private string baseURL_DEV = @"https://api-dev.easyeat.ai/";
        private string nestbaseURL = @"https://napi.easyeat.ai/";
        private string nestbaseURL_DEV = @"https://napi-dev.easyeat.ai/";
        const string separator = "################################################################################################################################################";



        private string reqbody = "{\"order_id\":\"VxLQxMl6gE1730197941\",\"type\" :\"1\",\"counter_id\":\"\",\"itr\" :\"1\",\"new_format\" :1,\"token\" :\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkYXRhIjoiNjM3NDU4ZmE0Y2NmMTAyYjMzNDBiMDE2MmIzYzZmMjJkMjNjZDliY2NkZTg4NTg0NjM1MWEwYWEyMTA4OTc5Mzk1MjJiODUyNWJlMTM4ZGNjNThiMzlkMzY5NzBmMTZiN2VlOWQ4MDQ5MWQ1NTBjMDY4YjA4MGZmODFlNzNkNmNkMDliMDI3MzRjZjkxNTcyMzNiYjdjMzI0Y2M4NGRhNmE4YTBjN2M1ZmZhNWU2ZThiMGM1MjNlYzFlOWQyOWIxMjdjOTM3YjIwNTNkNzQzYTc1ODYzYTBiZTQ3YTdmZDY0NWY2ZDViOWE1OWI0OTZhNjAzN2I5MDQ3OWRlZDA0MDFmYmJkYTc0ZjVhNmM0NjE1YWU2ZjgzOTgyZmY5NDIxZTBkYWYyMDRhYTdmMmZiYzE0MTZiOWY2NWZhMTg2ODhiY2E2NDJmNzdjNmI4MTUwZWQ0OWJiZGZjZWJlM2JkNzU1MDNmYjhlMjRlMzc5NmYzZmU0MGIyNmNjN2Q0ZTI2ZTAyMDJiZjc0Y2I3YTgyOThlNzQzZTQ1N2ExNGQzMWZmOWIzMTUzZmVkMTI3OTIxMTE4MTEzZTJkMmFmNTAwZGM5N2ZmZDAxM2NkYjA2MThjMzdhMTgzODFhMGU0Y2Q2IiwiaWF0IjoxNzMwMzc2NTM2LCJleHAiOjE3NjE5MzQxMzZ9.G-Mpipu6XLC4BUFvA-A7mbwNFsJWV8K2De0qNm-iJtw\"}";

        private WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:7980");
        public class Print : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                try
                {
                    // Deserialize the incoming message as a dynamic object
                    dynamic data = JsonConvert.DeserializeObject(e.Data);
                    Console.WriteLine("Dataaa" + data);

                    // Check the command type
                    if (data?.command == "status")
                    {
                        string req_id = "123";
                        Send("{\"status\":\"OK\",\"request_id\":\"" + req_id + "\"}");
                    }
                    else if (data?.command == "print")
                    {
                        // Check if payload is a JArray or JObject
                        if (data?.payload is JArray printDataArray)
                        {
                            foreach (var item in printDataArray)
                            {
                                dynamic currobj = item;
                                ReceiptPrint.NewSlipDesign cd = new ReceiptPrint.NewSlipDesign(currobj);
                                cd.finalPrint();
                            }
                        }
                        else if (data?.payload is JObject printDataObject)
                        {
                           if( data?.payload?.type == "dynamicQR")
                            {
                                // Process payload as a single JObject
                                dynamic currobj = printDataObject;
                                ReceiptPrint.DynamicQR cd = new ReceiptPrint.DynamicQR(currobj);
                                cd.finalDynamicQRPrint();
                            }
                           
                        }
                        else
                        {
                            Console.WriteLine("Error: Payload is not an array or an object.");
                            Send("{\"status\":\"ERROR\",\"message\":\"Payload is not an array or an object\"}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unknown or missing command.");
                        Send("{\"status\":\"ERROR\",\"message\":\"Unknown or missing command\"}");
                    }

                    Console.WriteLine("Received from client: " + e.Data);
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine("JSON parsing error: " + jsonEx.Message);
                    Send("{\"status\":\"ERROR\",\"message\":\"Invalid JSON format\"}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing message: " + ex.Message);
                    Send("{\"status\":\"ERROR\",\"message\":\"Processing error\"}");
                }
            }

            protected void printPayload()
            {
                string req_id = "123";
                Send("{\"status\":\"OK\",\"request_id\":\"" + req_id + "\"}");
            }
        }
        public EasyEats(string[] args)
        {
            if (args.Length != 0)
                encodedvalue = args[0];

            Console.WriteLine(reqbody);
            WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:7980");
            wssv.AddWebSocketService<Print>("/print");
            wssv.Start();
            InitializeComponent();
        }

        private void EasyEats_Load(object sender, EventArgs e)
        {

            /*Tasks Performed
             * Decoding the url
             * Extracting the protocol name
             * Extracting the body for POST Request
             */
            decodedvalue = System.Net.WebUtility.UrlDecode(encodedvalue);
            if (!String.IsNullOrEmpty(decodedvalue))
            {
                int idx = decodedvalue.IndexOf(':');
                string protocolName = decodedvalue.Substring(0, idx);
                this.reqbody = decodedvalue.Substring(idx + 1);
                //  Log("Protocol: " + protocolName);
                RegisterURLProtocol(protocolName, applicationPath);
            }

            else
            {
                //   Log("Protocol: receiptprint");
                RegisterURLProtocol("receiptprint", applicationPath);
            }

            Log("App Loaded");
            dynamic jsonReqBody = JObject.Parse(reqbody);
            Console.WriteLine(jsonReqBody);
            Loader(jsonReqBody);

        }

        public static void RegisterURLProtocol(string protocolName, string applicationPath)
        {
            Console.WriteLine(protocolName);
            Console.WriteLine(applicationPath);
            try
            {
                // Create new key for desired URL protocol
                var KeyTest = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true);
                RegistryKey key = KeyTest.CreateSubKey(protocolName);
                key.SetValue("URL Protocol", protocolName);
                key.CreateSubKey(@"shell\open\command").SetValue("", "\"" + applicationPath + "\" \"%1\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        /*Task of Loader Function
         * 1. Get Data From the Server
         * 2. Validate, Parse and extract the Useful Data
         * 3. Print the Data
        */
     private async void Loader(dynamic jsonReqBody)
{
    await Task.Delay(500);
    Console.WriteLine(separator);
    Console.WriteLine("Printing the data coming from the API");

    dynamic reqData = null; // Initialize reqData

    try
    {
        // Get Data From the Server
        reqData = await getReceiptDataAsync(jsonReqBody); // Ensure to await the async method

        if (reqData == null)
        {
            Console.WriteLine("No data received from the API.");
            return; // Exit if no data is received
        }

        Console.WriteLine(reqData.ToString()); // Output raw API data
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while fetching data: " + ex.Message);
        return; // Exit on exception
    }

    Console.WriteLine(separator);

    /* 2. Validate, Parse, and extract the Data fetched from the API */
    dynamic finalData = jsonValidateParse(reqData);

    // Check if finalData is successfully parsed and contains a status property
    if (finalData != null && finalData.status != null)
    {
        bool status = finalData.status == true; // Assuming finalData has a 'status' property

        /* 3. Print the data if the status is true */
        if (status)
        {
            Console.WriteLine(separator);
            Console.WriteLine(finalData); // Output final data if needed
            Console.WriteLine(separator);
            printData(finalData, jsonReqBody);
        }
        else
        {
            Console.WriteLine("Socket Server Started"); // Log a message if status is not true
        }
    }
    else
    {
        Console.WriteLine("Final data is invalid or missing a status property.");
    }
}



        /*Fetches & Returns the Data from the server depending upon the reqbody
         * 1. Bill Data (Receipt, Counter)
         * 2. Void Items Data
         * 3. Cash Drawer Data
         */
        private async Task<string> getReceiptDataAsync(dynamic jsonReqBody)
        {
            string token = jsonReqBody.token;
            String reqData;
            dynamic client, request; // Declare the variables here
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            if (jsonReqBody.order_id != null && jsonReqBody.new_format == 0)
            {
                dynamic currB;
                if (jsonReqBody.oid != null)
                    currB = JsonConvert.DeserializeObject<VoidBill>(reqbody);
                else if (jsonReqBody.type == 6)
                    currB = JsonConvert.DeserializeObject<TableChange>(reqbody);
                else
                    currB = JsonConvert.DeserializeObject<Bill>(reqbody);

                // Log("Order Object Parsed");

                /* Making the Post Request to the API */
                client = new RestClient(this.baseURL);
                request = new RestRequest("api/orders/billing/get_receipt_data.php", Method.Post);

                // Add the request body as form data (optional)
                request.AddJsonBody(currB); // Assuming currB is JSON serializable

            }
            else if (jsonReqBody.order_id != null && jsonReqBody.new_format == 1)
            {
                dynamic currB;
                if (jsonReqBody.oid != null)
                    currB = JsonConvert.DeserializeObject<VoidBill>(reqbody);
                else if (jsonReqBody.type == 6)
                    currB = JsonConvert.DeserializeObject<TableChange>(reqbody);
                else
                    currB = JsonConvert.DeserializeObject<Bill>(reqbody);

                Log("Order Object Parsed");
                // Making the Post Request to the API
                client = new RestClient(nestbaseURL);
                request = new RestRequest("api/receipt/receipt-data", Method.Post);

                // Add the Authorization header using AddHeader
                request.AddJsonBody("authorization", "Bearer " + token); // This method works in 112.x

                // Add the request body (assuming reqbody and currB are your JSON payloads)
                request.AddJsonBody(reqbody);  // Assuming reqbody is a JSON object
                request.AddJsonBody(currB);     // Assuming currB is also a JSON object
            }

            //Dynamic QR API calling
            else if (jsonReqBody.restaurant_id != null && jsonReqBody.type == 11)
            {
                Log("Printing Dynamic QR");
                dynamic currB = JsonConvert.DeserializeObject<Dynamic>(reqbody);
                /*Making the Post Request to the API*/
                client = new RestClient(nestbaseURL);
                request = new RestRequest("api/receipt/generate-dynamic-qr", Method.Post);
                //request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
                //request.AddJsonBody(reqbody);
                request.AddJsonBody("authorization", "Bearer " + token);
                request.AddJsonBody(currB);
            }



            else
            {
                // For Cash Drawer
                client = new RestClient(this.nestbaseURL);
                request = new RestRequest("api/receipt/cash-mgt", Method.Post);

                // Set the Authorization header
                request.AddJsonBody("authorization", "Bearer " + token); // Correct header name

                // Add the request body
                request.AddJsonBody(reqbody); // Assuming reqbody is a JSON-serializable object


            }

            // If none of the conditions were met, you can assign a default client or handle it appropriately.
            if (client == null) // This condition checks if client is still uninitialized
            {
                throw new InvalidOperationException("Failed to initialize RestClient.");
            }

            // Log("API Request Send");
            var response = await client.ExecuteAsync(request);
            // Log("API Response Received");
            reqData = response.Content.ToString();
            return reqData;
        }


        private dynamic jsonValidateParse(string reqData)
        {

            dynamic reqJson = JObject.Parse(reqData);
            dynamic finalData = "";

            /*Validate the data from the api and get Useful Data*/
            if (reqJson.statusCode == 200 | reqJson.status == 1)
            {
                status = true;
                finalData = reqJson.data;

                /*Testing for Parallel Printing*/
                //dynamic tempJson = JObject.Parse(tempData);
                //finalData = tempJson.data;
            }

            return finalData;
        }

        private void printData(dynamic finalData, dynamic jsonReqBody)
        {
            /*Table Change Old Design*/
            if (jsonReqBody.order_id != null && jsonReqBody.type == 6 && jsonReqBody.new_format == 0)
            {
                dynamic currobj = finalData[0];
                Table tb = new Table(currobj);
                tb.finalPrint();
            }
            /*Table Change New Design*/
            else if (jsonReqBody.order_id != null && jsonReqBody.type == 6 && jsonReqBody.new_format == 1)
            {
                dynamic currobj = finalData[0];
                NewSlipDesign tb = new NewSlipDesign(currobj);
                tb.finalPrint();
            }

            // DynamicQR Printing
            else if ( jsonReqBody?.type == 11 )
            {
                dynamic currobj = finalData;
                DynamicQR qrPrinter = new DynamicQR(currobj);
                qrPrinter?.finalDynamicQRPrint();
            }



            // Cash Drawer , Cash-In
            else if (jsonReqBody.restaurant_id != null && jsonReqBody.new_format == 0)
            {
                dynamic currobj = finalData[0];
                NewSlipDesign cd = new NewSlipDesign(currobj);
                cd.finalPrint();
            }

            else if (jsonReqBody.order_id != null && jsonReqBody.new_format == 1)
            {
                /*Traversing all counter and receipt objects for NEW SLIP DESIGN*/
                int totalObject = ((JArray)finalData).Count;
                Console.WriteLine("The value of total Object Comes out to be:" + totalObject);
                //foreach (dynamic currobj in finalData)
                Parallel.For(0, totalObject, i =>
                {
                    dynamic currobj = finalData[i];
                    NewSlipDesign ccc = new NewSlipDesign(currobj);
                    ccc.finalPrint();
                }
                );
            }
            else if (jsonReqBody.order_id != null && jsonReqBody.new_format == 0)
            {


                /*Traversing all counter and receipt objects*/
                int totalObject = ((JArray)finalData).Count;
                Console.WriteLine("The value of total Object Comes out to be:" + totalObject);
                //foreach (dynamic currobj in finalData)
                Parallel.For(0, totalObject, i =>
                {
                    dynamic currobj = finalData[i];
                    /*
                     * Types of Receipts
                     * 1. Receipt
                     * 2. Counter
                     */

                    if (currobj.type.ToString() == "counter")
                    {
                        /* Finding the customisation code*/
                        int customisationCode = 0;
                        if (currobj.code != null)
                            customisationCode = currobj.code;

                        switch (customisationCode)
                        {

                            case 1:
                                CounterCustomisation1 cc1 = new CounterCustomisation1(currobj);
                                cc1.finalPrint();
                                break;

                            case 2:
                                CounterCustomisation2 cc2 = new CounterCustomisation2(currobj);
                                cc2.finalPrint();
                                break;

                            default:
                                CounterCustomisation cc = new CounterCustomisation(currobj);
                                cc.finalPrint();
                                break;
                        }
                    }

                    else if (currobj.type.ToString() == "receipt")
                    {
                        ReceiptCustomisation rc = new ReceiptCustomisation(currobj);
                        rc.finalPrint();
                    }
                });
            }




            /*For Cash Drawer*/
            else if (jsonReqBody.restaurant_id != null && jsonReqBody.new_format == 1) //changed to 0 --> 1
            {
                CashDrawer cd = new CashDrawer(finalData);
                cd.finalPrint();
            }
        }

        private async void exitApplication()
        {
            wssv.Stop();
            await Task.Delay(2000);
            Environment.Exit(0);
        }

        private void Log(string msg)
        {
            DateTime now = DateTime.Now;
            string currtime = now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();
            this.logger.Items.Add(currtime + " - " + msg);
        }

        private void EasyEats_FormClosing(object sender, FormClosingEventArgs e)
        {
            wssv.Stop();
            MessageBox.Show("WebSocket Server is  Closed", "EasyEat", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void EasyEats_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }


    }

}
