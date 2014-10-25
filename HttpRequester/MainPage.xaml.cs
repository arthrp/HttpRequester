using HttpRequester.Common;
using HttpRequester.Enums;
using HttpRequester.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace HttpRequester
{
    public sealed partial class MainPage : Page
    {
        private HttpRequestViewModel _viewModel;
        private readonly NavigationHelper _navigationHelper;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            _navigationHelper = new NavigationHelper(this);
            new HttpRequestViewModel();

            cbxRequestType.ItemsSource = Enum.GetValues(typeof(RequestTypeEnum)).Cast<RequestTypeEnum>();

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            DataContext = HttpRequestViewModel.Instance;
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string response = "";
            string exceptionMsg = null;
            try
            {
                response = await HttpRequestViewModel.Instance.GetServerResponse();
            }
            catch (ArgumentException ex)
            {
                exceptionMsg = ex.Message;
            }

            //Fix for crappy "all async" dialog show methods
            if (!string.IsNullOrEmpty(exceptionMsg))
            {
                var errorDialog = new MessageDialog(exceptionMsg, "Error");
                await errorDialog.ShowAsync();
                return;
            }

            var dialog = new MessageDialog(response, "Response");

            await dialog.ShowAsync(); 
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = ((HttpParameterModel)e.ClickedItem).Name;
            Frame.Navigate(typeof(HttpParameterPage), itemName);
        }

    }
}
