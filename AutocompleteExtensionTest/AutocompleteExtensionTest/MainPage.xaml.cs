using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Namespace;
using System.Collections.ObjectModel;

namespace AutocompleteExtensionTest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        ObservableCollection<string> observableCollection = new ObservableCollection<string>() { "one", "two", "three" };
        public MainPage()
        {
            InitializeComponent();

            var testList1 = new List<string>() { "one", "two", "three", "four", "five", "six", "seven", "eight" };
            Autocomplete.SetBuild(entry1, (object item) =>
            {
                var render = new Label() { Text = item as string };
                return new DataTemplate(() => render);
            });
            Autocomplete.SetItemsSource(entry1, observableCollection);
            Autocomplete.GetAutocompleteController(entry1).ScrollCollectionView.MaxItemsShown = 7;
            var onTap = new TapGestureRecognizer();
            int i = 0;
            onTap.Tapped += (object sender, EventArgs e) => 
            {
                
                Console.WriteLine("Tapped");
                observableCollection.Add("item " + i);
                i++;
            };
            Task.Run(() =>
            {
                Task.Delay(6000).ContinueWith((action) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Console.WriteLine("adding");
                        observableCollection.Add("whoa1");
                        observableCollection.Add("whoa2");
                        observableCollection.Add("whoa3");
                        observableCollection.Add("whoa4");
                        observableCollection.Add("whoa5");
                    });
                });
            });
            stack.GestureRecognizers.Add(onTap);

            var autocompleteController = Autocomplete.GetAutocompleteController(entry1);
            var testTap = new TapGestureRecognizer();
            testTap.Tapped += (object sender, EventArgs e) =>
            {
                Console.WriteLine("Tapped scrollview");
            };
            autocompleteController.ScrollCollectionView.GestureRecognizers.Add(testTap);
        }

    }
}
