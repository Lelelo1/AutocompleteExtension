using System;
namespace AutocompleteExtensionTest.ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        string text = "I am a test";
        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged("Text"); }
        }
    }
}
