using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.Http;
using HttpRequester.Enums;
using Windows.ApplicationModel.DataTransfer;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using HttpRequester.Common;

namespace HttpRequester.ViewModels
{
    public class HttpRequestViewModel : BaseModel
    {
        private const string DEFAULT_USER_AGENT = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        private const string JSON_MEDIA_TYPE = "application/json";

        public static readonly HttpRequestViewModel Instance = new HttpRequestViewModel();

        public HttpRequestViewModel()
        {
            Url = "http://www.example.com";
            UserAgent = DEFAULT_USER_AGENT;

            Parameters = new ObservableCollection<HttpParameterModel>();
            Parameters.Add(new HttpParameterModel() { Name = "Test", Value = "true" });
        }

        //private readonly HttpClient _httpClient = new HttpClient();

        #region Properties

        private string _url;

        public string Url
        {
            get { return _url; }
            set 
            { 
                _url = value;
                NotifyPropertyChanged();
            }
        }

        private RequestTypeEnum _requestType;

        public RequestTypeEnum RequestType
        {
            get { return _requestType; }
            set 
            { 
                _requestType = value;
                NotifyPropertyChanged();
            }
        }

        private string _userAgent;

        public string UserAgent
        {
            get { return _userAgent; }
            set 
            { 
                _userAgent = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<HttpParameterModel> Parameters { get; set; }
        
        #endregion

        public async Task<string> GetServerResponse()
        {
            ValidateFields();
            using (HttpClient httpClient = new HttpClient())
            {
                InitializeClientDefaultValues(httpClient);
                UpdateRequestHeaders(httpClient);

                StringBuilder res = new StringBuilder();
                HttpResponseMessage response;

                if (RequestType == RequestTypeEnum.GET)
                {
                    string urlWithParams = ConcatenateParametersToUrl(Url);
                    response = await Task.Run(() => httpClient.GetAsync(urlWithParams));
                }
                else if (RequestType == RequestTypeEnum.POST)
                {
                    string requestParams = JSONHelper.Serialize<HttpParameterModel>(Parameters);
                    httpClient.DefaultRequestHeaders.Accept.Add((new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE)));
                    response = await Task.Run(() => 
                        httpClient.PostAsync(Url, new StringContent(requestParams, Encoding.UTF8, JSON_MEDIA_TYPE)));
                }
                else if (RequestType == RequestTypeEnum.PUT)
                    response = await Task.Run(() => httpClient.DeleteAsync(Url));
                else if (RequestType == RequestTypeEnum.DELETE)
                    response = await Task.Run(() => httpClient.DeleteAsync(Url));
                else throw new ArgumentException("Unknown request type");

                res.AppendLine("Status: " + (int)response.StatusCode + " (" +
                    response.StatusCode.ToString() + ")");
                res.AppendLine("Content:");
                string content = await response.Content.ReadAsStringAsync();
                res.AppendLine(response.Content.ReadAsStringAsync().Result);

                return res.ToString();
            }
        }

        public HttpParameterModel GetModelByParamName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new HttpParameterModel();
            return Parameters.Single(p => p.Name == name);
        }

        private void ValidateFields()
        {
            if (string.IsNullOrEmpty(Url))
                throw new ArgumentException("Url cannot be empty");
            if (string.IsNullOrEmpty(UserAgent))
                throw new ArgumentException("Please specify user agent");
        }

        private void InitializeClientDefaultValues(HttpClient httpClient)
        {
            httpClient.MaxResponseContentBufferSize = 512000;
            httpClient.DefaultRequestHeaders.Add("user-agent", DEFAULT_USER_AGENT);
        }

        private string ConcatenateParametersToUrl(string url)
        {
            if (Parameters.Count < 1)
                return url;
            StringBuilder ret = new StringBuilder(url);

            ret.Append("?");
            foreach (HttpParameterModel param in Parameters)
            {
                ret.Append(param.Name+"="+param.Value);
                ret.Append("&");
            }

            ret.Length -= 1;

            return ret.ToString();
        }

        private void UpdateRequestHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.AddOrUpdate("user-agent", UserAgent);
        }
    }

    public static class HttpRequestHeadersExtenstions
    {
        public static void AddOrUpdate(this HttpRequestHeaders headers, string name, string value)
        {
            if (headers.Contains(name))
            {
                headers.Remove(name);
            }

            headers.Add(name, value);
        }
    }
}
