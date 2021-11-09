using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Service.Utilities;
using YellowStone.Services.DocumentReviewService.DTOs;
using YellowStone.Services.FBNMobile;
using System.Drawing.Imaging;
using YellowStone.Models.DTO;
using System.Linq;

namespace YellowStone.Services.DocumentReviewService
{
    public class DocumentReviewService : IDocumentReview
    {
        private readonly HttpClient _client;
        private readonly FbnMobileMiddlewareSettings _settings;
        private readonly SystemSettings _sysSet;
        private readonly DocumentApprovalEndPoints _documentApprovalEndPoints;
        private readonly ILogger _logger;


        public DocumentReviewService(IOptions<FbnMobileMiddlewareSettings> settings, IHttpClientFactory factory, ILogger<DocumentReviewService> logger,
            IOptions<SystemSettings> sysSet, IOptions<DocumentApprovalEndPoints> documentApprovalEndPoints)
        {
            _settings = settings.Value;
            _client = factory.CreateClient("HttpMessageHandler");
            _client.DefaultRequestHeaders.Add("language", "en");
         
            _logger = logger;
            _sysSet = sysSet.Value;
            BuildDocumentReviewClient();
            // _documentApprovalEndPoints = documentApprovalEndPoints.Value;
        }

        public async Task<SendDocumentResponse> GetDocumentOld(string phoneNumber, int documentType)
        {
            try
            {


                var documenType_ = (DocumentType)documentType;
                var rawResponse = await _client.GetAsync($"document/{phoneNumber}/{(DocumentType)documentType}");
                var body = await rawResponse.Content.ReadAsStringAsync();
                var response = Util.DeserializeFromJson<SendDocumentResponse>(body);
                if (!rawResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"body: {body}");
                }

                _logger.LogError($"SEE_RESPONSE: {JsonConvert.SerializeObject(response)}");
                var image = string.Empty;

                image = GetImage(response.Path, documenType_);
                response.Image = image;
                return response;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<SendDocumentResponse> GetDocument(string phoneNumber, int documentType)
        {
            _logger.LogInformation("Inside the GetDocument method of the DocumetReviewService");
            var response = new SendDocumentResponse();

            try
            {
               
                _logger.LogInformation(_client.BaseAddress.ToString());
                #region Old 
                //if (_sysSet.IsTest == true)
                //{
                //    //Added newly on 30-10-2021
                //    var rawResponse_ = await  _client.GetAsync($"document/{phoneNumber}");
                //    var body_ = await rawResponse_.Content.ReadAsStringAsync();
                //    var response_ = Util.DeserializeFromJson<List<SendDocumentResponse>>(body_);

                //    var response = new SendDocumentResponse();


                //    if (response_.Any(p => p.DocumentType == 0) == true)
                //    {
                //        var documenType_ = (DocumentType)documentType;
                //        response = response_.Where(p => (int)p.DocumentType == (documentType-1)).FirstOrDefault();


                //        if (!rawResponse_.IsSuccessStatusCode)
                //        {
                //            _logger.LogError($"body: {body_}");
                //        }

                //        _logger.LogError($"SEE_RESPONSE: {JsonConvert.SerializeObject(response_)}");
                //        var image = string.Empty;


                //        image = GetImage(response.Path, documenType_);
                //        response.Image = image;
                //        return response;

                //        //  response_ = response_.Select(p => new SendDocumentResponse { AccountNumber = p.AccountNumber, Document = p.Document, DocumentType = (p.DocumentType + 1), Image = p.Image, Path = p.Path, PhoneNumber = p.PhoneNumber }).ToList();
                //    }
                //    else
                //    {
                //        ///////////////////////////////////////////////////////////
                //        //splitted newly on 30 - 10 - 2021
                //        var documenType_ = (DocumentType)documentType;
                //        var rawResponse = await _client.GetAsync($"document/{phoneNumber}/{(DocumentType)documentType}");
                //        var body = await rawResponse.Content.ReadAsStringAsync();
                //          response = Util.DeserializeFromJson<SendDocumentResponse>(body);

                //        if (!rawResponse.IsSuccessStatusCode)
                //        {
                //            _logger.LogError($"body: {body}");
                //        }

                //        _logger.LogError($"SEE_RESPONSE: {JsonConvert.SerializeObject(response)}");
                //        var image = string.Empty;


                //        image = GetImage(response.Path, documenType_);
                //        response.Image = image;
                //        return response;
                //    }
                   
               



                //}
                //else
                //{

                //    using (var client = new HttpClient())
                //    {
                //        ServicePointManager.Expect100Continue = true;
                //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //        ServicePointManager.ServerCertificateValidationCallback +=
                //            (sender, cert, chain, sslPolicyErrors) => true;
                //        var rawResponse = await client.GetAsync($"{_settings.BaseUrl}document/{phoneNumber}/{(DocumentType)documentType}");

                //        var body = await rawResponse.Content.ReadAsStringAsync();
                //        var response = Util.DeserializeFromJson<SendDocumentResponse>(body);

                //        var documenType_ = (DocumentType)documentType;

                //        if (!rawResponse.IsSuccessStatusCode)
                //        {
                //            _logger.LogError($"body: {body}");
                //        }

                //        _logger.LogError($"SEE_RESPONSE: {JsonConvert.SerializeObject(response)}");
                //        var image = string.Empty;

                //        image = GetImage(response.Path, documenType_);
                //        response.Image = image;
                //        return response;


                //    }
                //}

                #endregion


          

       
                using (var client = new HttpClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    var rawResponse_ = await _client.GetAsync($"document/{phoneNumber}");
                    var body_ = await rawResponse_.Content.ReadAsStringAsync();
                    var response_ = Util.DeserializeFromJson<List<SendDocumentResponse>>(body_);
 


                    if (response_.Any(p => p.DocumentType == 0) == true)
                    {
                        var documenType_ = (DocumentType)documentType;
                        response = response_.Where(p => (int)p.DocumentType == (documentType - 1)).FirstOrDefault();


                        if (!rawResponse_.IsSuccessStatusCode)
                        {
                            _logger.LogError($"body: {body_}");
                        }

                        _logger.LogError($"SEE_RESPONSE: {JsonConvert.SerializeObject(response_)}");
                        var image = string.Empty;


                        image = GetImage(response.Path, documenType_);
                        response.ErrorOcurred = errorOcurred;
                       
                        response.Image = image;
                        return response;

                        //  response_ = response_.Select(p => new SendDocumentResponse { AccountNumber = p.AccountNumber, Document = p.Document, DocumentType = (p.DocumentType + 1), Image = p.Image, Path = p.Path, PhoneNumber = p.PhoneNumber }).ToList();
                    }
                    else
                    {
                        ///////////////////////////////////////////////////////////
                        //splitted newly on 30 - 10 - 2021
                        var documenType_ = (DocumentType)documentType;
                        var rawResponse = await _client.GetAsync($"document/{phoneNumber}/{(DocumentType)documentType}");
                        var body = await rawResponse.Content.ReadAsStringAsync();
                        response = Util.DeserializeFromJson<SendDocumentResponse>(body);

                        if (!rawResponse.IsSuccessStatusCode)
                        {
                            _logger.LogError($"body: {body}");
                        }

                        _logger.LogError($"SEE_RESPONSE: {JsonConvert.SerializeObject(response)}");
                        var image = string.Empty;


                        image = GetImage(response.Path, documenType_);
                        response.Image = image;
                        response.ErrorOcurred = errorOcurred;
                        return response;
                    }
 
                    


                }

            }
            catch (Exception ex)
            {
                response.ErrorOcurred = true;
                _logger.LogCritical(ex, "Error occured inside the  GetDocument of documentReview service");
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<List<SendDocumentResponse>> GetDocumentsByPhoneNumber(string phoneNumber)
        {
            _logger.LogInformation("Inside the GetDocumentsByPhoneNumber  method of the DocumetReviewService");
            var rawResponse = await _client.GetAsync($"document/{phoneNumber}");
            var body = await rawResponse.Content.ReadAsStringAsync();
            var response = Util.DeserializeFromJson<List<SendDocumentResponse>>(body);

            if (!rawResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"body: {body}");
            }

            return response;
        }


        public async Task<List<UnapprovedDocumentResponse>> GetUnApprovedDocumentHeader()
        {
            
            _logger.LogInformation("Inside the GetUnApprovedDocumentHeader  method of the DocumetReviewService");
            _logger.LogInformation(_client.BaseAddress.ToString());
            try
            {
                if (_sysSet.IsTest == true)
                {
                    _logger.LogInformation("For test==============================");
                    var rawResponse = await _client.GetAsync($"document/customers");
                    var result = new List<UnapprovedDocumentResponse>();
                    if (!rawResponse.IsSuccessStatusCode)
                    {
                        result = new List<UnapprovedDocumentResponse>();
                        return result;
                    }
                    var body = await rawResponse.Content.ReadAsStringAsync();

                    result = Util.DeserializeFromJson<List<UnapprovedDocumentResponse>>(body);
                    _logger.LogInformation($"Response   { JsonConvert.SerializeObject( result)}");
                    return result;
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        _logger.LogInformation($"from production");
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
                        //  var response = await client.GetAsync("https://app.fbngroup.com/fbnmobilev2/api/document/customers");
                        var response = await client.GetAsync("document/customers");


                        //var response = await client.PostAsync(http://app.fbngroup.com/fbnmobilev2/api/document/customers, content);
                        var xml = await response.Content.ReadAsStringAsync();
                        var result = DeserializeFromJson<List<UnapprovedDocumentResponse>>(xml);
                        _logger.LogInformation("Response from the GetUnApprovedDocumentHeader  method of the DocumetReviewService>>>>>>>>>>>> {result}",JsonConvert.SerializeObject(result));
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "error occured in the GetUnApprovedDocumentHeader of documentreviewservice");
                throw new Exception(ex.Message, ex.InnerException);
            }


        }
        public async Task<List<UnapprovedDocumentResponse>> GetUnApprovedDocumentHeaderOld()
        {



            var rawResponse = await _client.GetAsync($"document/customers");
            var result = new List<UnapprovedDocumentResponse>();
            if (!rawResponse.IsSuccessStatusCode)
            {
                result = new List<UnapprovedDocumentResponse>();
                return result;
            }
            var body = await rawResponse.Content.ReadAsStringAsync();

            result = Util.DeserializeFromJson<List<UnapprovedDocumentResponse>>(body);
            return result;
        }

        private async  Task< int> CheckForDocumentType(string phoneNumber, int documentType)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            var rawResponse_ = await _client.GetAsync($"document/{phoneNumber}");
            var body_ = await rawResponse_.Content.ReadAsStringAsync();
            var response_ = Util.DeserializeFromJson<List<SendDocumentResponse>>(body_);



            if (response_.Any(p => p.DocumentType == 0) == true)
            {
                var documenType_ = (DocumentType)documentType;
              var  response = response_.Where(p => (int)p.DocumentType == (documentType-1)).FirstOrDefault();


                if (!rawResponse_.IsSuccessStatusCode)
                {
                    _logger.LogError($"body: {body_}");
                }
 
                return (int)response.DocumentType;

                //  response_ = response_.Select(p => new SendDocumentResponse { AccountNumber = p.AccountNumber, Document = p.Document, DocumentType = (p.DocumentType + 1), Image = p.Image, Path = p.Path, PhoneNumber = p.PhoneNumber }).ToList();
            }
            return documentType;
        }
          
        public async Task UpdateDocumentApprovalStatus(string phoneNumber, int documentType, bool? status)
        {
            _logger.LogInformation("Inside the UpdateDocumentApprovalStatus  method of the DocumetReviewService");
           
           



            documentType =await CheckForDocumentType(phoneNumber, documentType);

            //var rawResponse = await _client.PutAsync($"document/{phoneNumber}/{documentType}/{status}", contentData);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            //var rawResponse = await _client.GetAsync($"document/{phoneNumber}/{documentType}/{status}");
            var obj = new
            {
                PhoneNuumber = phoneNumber,
                DocumentType =documentType,
                DocumentStatus = status
            };
            var input = Util.SerializeAsJson<dynamic>(obj );
            var contentData = new StringContent(input, System.Text.Encoding.UTF8, "application/json");
 var rawResponse = await _client.PostAsync($"document/update-document-status",contentData);
            var xml = await rawResponse  .Content.ReadAsStringAsync();
            var result = DeserializeFromJson<dynamic>(xml );
            rawResponse.EnsureSuccessStatusCode();
            _logger.LogInformation($"Response in the UpdateDocumentApprovalStatus of DocumentReviewServices {JsonConvert.SerializeObject(rawResponse)}");

        }

        private void BuildDocumentReviewClient()
        {
            
            _client.BaseAddress = new Uri(_settings.BaseUrl);
            _logger.LogInformation(_client.BaseAddress.ToString());
        }
        public string GetImage(string photoUrl, DocumentType documentType)
        {
            _logger.LogInformation("Inside the GetImage  method of the DocumetReviewService");
            _logger.LogInformation($"Document URL={photoUrl} passed to the GetImage method of the DocumetReviewService");

            var finaleImagePath = string.Empty;
            var base64Image = string.Empty;
            string returnedBaseUrl = string.Empty;
            if (documentType == DocumentType.PICTURE)
            {
                returnedBaseUrl = _sysSet?.PhotoReturnedBaseUrl;
            }
            else
            {
                returnedBaseUrl = _sysSet?.DocumentReturnedBaseUrl;
            }
            if (!string.IsNullOrWhiteSpace(photoUrl))
            {
                var photoLocationSplit = photoUrl.Split('\\');


                var photoImage = photoLocationSplit[photoLocationSplit.Length - 1];

                string imageWithPath = $"{returnedBaseUrl}{photoImage}";// Path.Combine(returnedBaseUrl, photoImage);
                                                                        //string imageWithPath = $"{returnedBaseUrl}221771000000.jpg";

                finaleImagePath = imageWithPath.Replace('\\', '/');
                base64Image = ConvertImageURLToBase64(finaleImagePath);
                if (string.IsNullOrWhiteSpace(base64String))
                {
                    _logger.LogInformation("ConvertImageURLToBase64  returned an empty base64string");
                }
                else
                {
                    _logger.LogInformation("ConvertImageURLToBase64  returned  value");
                }
            }


            return base64Image;

        }



        public static T DeserializeFromJson<T>(string input)
        {

            return JsonConvert.DeserializeObject<T>(input);
        }
        static string base64String = null;

        public String ConvertImageURLToBase64(String url)
        {
            _logger.LogInformation("Inside the  ConvertImageURLToBase64 method of the DocumetReviewService");
            StringBuilder _sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(url))
            {
                Byte[] _byte = this.GetImage(url);

                if (_byte != null) { _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length)); }
                //_sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));
            }

            _logger.LogInformation("Finished converting imageto base64String in the ConvertImageURLToBase64  method of the DocumetReviewService");
            return _sb.ToString();
        }
        private bool errorOcurred = false;
        private byte[] GetImage(string url)
        {
            Stream stream = null;
            byte[] buf;
            _logger.LogInformation("Inside the GetImage byteArray  method of the DocumetReviewService");
            try
            {
                WebProxy myProxy = new WebProxy();
                if (!string.IsNullOrWhiteSpace(url))
                {HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

#region Added newly 30-10-2021
                    
                    //ServicePointManager.Expect100Continue = true;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    //ServicePointManager.ServerCertificateValidationCallback +=
                    //    (sender, cert, chain, sslPolicyErrors) => true;
                    //req.Credentials = CredentialCache.DefaultCredentials;
                    //req.Method = "GET";
                    //req.ContentType = "application/x-www-form-urlencoded";

                    #endregion

                    HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                    stream = response.GetResponseStream();

                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        int len = (int)(response.ContentLength);
                        buf = br.ReadBytes(len);
                        br.Close();
                    }

                    stream.Close();
                    response.Close();
                    _logger.LogInformation("Gtten the image and converted to byte in  the GetImage method of the DocumetReviewService");
                    return buf;
                }

            }
            catch (Exception exp)
            {
                errorOcurred = true;
                _logger.LogInformation("error occurred  the GetDocument method of the DocumetReviewService");
                buf = null;
            }

            return null;

        }

        public bool ErrorOcurred()
        {
            return errorOcurred;
        }
    }
}
