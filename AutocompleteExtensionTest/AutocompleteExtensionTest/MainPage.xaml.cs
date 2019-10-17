using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Namespace;

namespace AutocompleteExtensionTest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var testList1 = new List<string>() { "one", "two", "three", "four", "five", "six", "seven", "eight" };
            Autocomplete.SetBuild(entry1, (object item) =>
            {
                var render = new Label() { Text = item as string };
                return new DataTemplate(() => render);
            });
            Autocomplete.SetItemsSource(entry1, testList1);

            var testList2 = new List<string>() { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
            Autocomplete.SetBuild(entry2, (object item) =>
            {
                var render = new Label() { Text = item as string };
                return new DataTemplate(() => render);
            });
            Autocomplete.SetItemsSource(entry2, testList2);

        }
    }
}
