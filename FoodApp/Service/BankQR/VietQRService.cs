// File: WinP_Project/FoodApp/Service/BankQR/VietQRService.cs

using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FoodApp.Service.BankQR
{
    public class VietQRService
    {
        /// <summary>
        /// Generates a VietQR code based on the provided amount and order details.
        /// </summary>
        /// <param name="amount">The total amount for the order (float).</param>
        /// <param name="orderId">The unique identifier for the order.</param>
        /// <returns>A byte array representing the QR code image.</returns>
        public async Task<byte[]> GenerateVietQRAsync(float amount)
        {
            // Convert float amount to int (VND does not use subunits)
            int amountInt = Convert.ToInt32(Math.Round(amount, 0));

            var apiRequest = new ApiRequest
            {
                acqId = 970436, // Vietcombank Acquirer ID
                accountNo = 9399784975,
                accountName = "TRẦN VĂN PHƯƠNG",
                amount = amountInt, // API expects amount as integer
                addInfo = "Quý khách thanh toán đơn hàng",
                format = "text",
                template = "default" // Use 'default' to include more information
            };

            var jsonRequest = JsonConvert.SerializeObject(apiRequest);

            var client = new RestClient("https://api.vietqr.io/v2/generate");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var dataResult = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
                if (dataResult?.data?.qrDataURL != null)
                {
                    string base64String = dataResult.data.qrDataURL.Replace("data:image/png;base64,", "").Trim();
                    return Convert.FromBase64String(base64String);
                }
                else
                {
                    throw new Exception("Invalid response from VietQR API.");
                }
            }
            else
            {
                throw new Exception($"Failed to generate QR Code. Status Code: {response.StatusCode}, Message: {response.StatusDescription}");
            }
        }
    }

    public class ApiRequest
    {
        public int acqId { get; set; }
        public long accountNo { get; set; }
        public string accountName { get; set; }
        public int amount { get; set; }
        public string addInfo { get; set; }
        public string format { get; set; }
        public string template { get; set; }
    }

    public class ApiResponseData
    {
        public string qrDataURL { get; set; }
    }

    public class ApiResponse
    {
        public ApiResponseData data { get; set; }
    }
}