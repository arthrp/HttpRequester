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

namespace HttpRequester.ViewModels
{
    public class HttpRequestViewModel : BaseModel
    {
        private const string DEFAULT_USER_AGENT = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

        public HttpRequestViewModel()
        {
            _httpClient.MaxResponseContentBufferSize = 512000;
            _httpClient.DefaultRequestHeaders.Add("user-agent", DEFAULT_USER_AGENT);
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
        
        #endregion

        public async Task<string> GetServerResponse()
        {
            ValidateFields();

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

        private void ValidateFields()
        {
            if (string.IsNullOrEmpty(Url))
                throw new ArgumentException("Url cannot be empty");
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
