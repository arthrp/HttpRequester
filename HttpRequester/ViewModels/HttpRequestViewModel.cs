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

namespace HttpRequester.ViewModels
{
    public class HttpRequestViewModel : BaseModel
    {
        private const string DEFAULT_USER_AGENT = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

        public static readonly HttpRequestViewModel Instance = new HttpRequestViewModel();

        public HttpRequestViewModel()
        {
            _httpClient.MaxResponseContentBufferSize = 512000;
            _httpClient.DefaultRequestHeaders.Add("user-agent", DEFAULT_USER_AGENT);
            UserAgent = DEFAULT_USER_AGENT;

            Parameters = new ObservableCollection<HttpParameterModel>();
            Parameters.Add(new HttpParameterModel() { Name = "Test", Value = "true" });
        }

        private readonly HttpClient _httpClient = new HttpClient();

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
            UpdateRequestHeaders(_httpClient);

            StringBuilder res = new StringBuilder();
            HttpResponseMessage response;

            if (RequestType == RequestTypeEnum.GET)
                response = _httpClient.GetAsync(Url).Result;
            //else if (RequestType == RequestTypeEnum.POST)
            //    response = _httpClient.PostAsync(Url).Result;
            else if (RequestType == RequestTypeEnum.PUT)
                response = _httpClient.DeleteAsync(Url).Result;
            else if (RequestType == RequestTypeEnum.DELETE)
                response = _httpClient.DeleteAsync(Url).Result;
            else throw new ArgumentException("Unknown request type");

            res.AppendLine("Status: " + (int)response.StatusCode + " (" + 
                response.StatusCode.ToString() + ")");
            res.AppendLine("Content:");
            string content = await response.Content.ReadAsStringAsync();
            res.AppendLine(response.Content.ReadAsStringAsync().Result);

            return res.ToString();
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
