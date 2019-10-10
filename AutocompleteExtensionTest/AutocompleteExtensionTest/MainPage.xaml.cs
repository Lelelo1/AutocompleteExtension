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

            var testList = new List<string>() { "one", "two", "three", "four", "five", "six", "seven" };
            
            Autocomplete.SetItemsSource(entry, testList);
            Autocomplete.SetBuild(entry, (number) =>
            {
                var label = new Label() { Text = number as string };
                var viewCell = new ViewCell() { View = label };
                viewCell.ContextActions.Add(new MenuItem() { Text = "info" });
                viewCell.ContextActions.Add(new MenuItem() { Text = "delete", IsDestructive = true });

                viewCell.Tapped += (object sender, EventArgs e) =>
                {
                    Console.WriteLine("tapped: " + ((sender as ViewCell).View as Label).Text);
                    var controller = Autocomplete.GetAutocompleteController(entry);
                    controller.OnTop.IsVisible = false;
                };
                var conatiner = new ListView();
                conatiner.ItemTemplate = new DataTemplate(() => viewCell);
                return new DataTemplate(() => label);
            });
            
            // var contentView = new TopContentView();

        }

    }
    class MyContentView : TopContentView
    {

    }

}

