using System;
using Xamarin.Forms;
// using NObservable;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

using Namespace;
using static Xamarin.Forms.VisualElement;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Specialized;

namespace Namespace
{
    // insperation from: https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/BindableLayout.cs
    public static class Autocomplete
    {

        /*
        public static readonly BindableProperty ItemTemplateSelectorProperty =
            BindableProperty.CreateAttached("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(InputView), default(DataTemplateSelector),
                propertyChanged: (b, o, n) => { GetBindableLayoutController(b).ItemTemplateSelector = (DataTemplateSelector)n; });
        */
        // defaultValueCreator creates 1 instance for the object it was invoked on. which basically acts like a singelton getter and makes it so that 
        // static AutocompleteController AutocompleteController { get; set; } = new AutocompleteController(new InputView(null));

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.CreateAttached("ItemsSource", typeof(IEnumerable<object>), typeof(InputView), default(IEnumerable<object>),
                propertyChanged: (b, o, n) => { GetAutocompleteController(b).ScrollCollectionView.ItemsSource = (IEnumerable<object>)n; });

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.CreateAttached("ItemTemplate", typeof(DataTemplate), typeof(InputView), default(DataTemplate),
                propertyChanged: (b, o, n) => { /*GetAutocompleteController(b).ScrollCollectionView.I = (DataTemplate)n;*/ }); // ScrollCollectionView does not have itemstemplate yet (only build)

        public static readonly BindableProperty BuildProperty =
            BindableProperty.CreateAttached("Build", typeof(Func<object, DataTemplate>), typeof(InputView), default(Func<object, DataTemplate>),
                propertyChanged: (b, o, n) => { GetAutocompleteController(b).ScrollCollectionView.Build = (Func<object, DataTemplate>)n; });
        /*
        static readonly BindableProperty IsFocusedProperty =
            BindableProperty.CreateAttached("IsFocused", typeof(bool), )
            */
        public static readonly BindableProperty AutocompleteControllerProperty =
             BindableProperty.CreateAttached("BindableLayoutController", typeof(AutocompleteController), typeof(InputView), default(AutocompleteController),
                 defaultValueCreator: (b) => {

                     return new AutocompleteController((InputView)b);
                 },
                 propertyChanged: (b, o, n) =>
                 {
                     Console.WriteLine("Autocompletecontroller property changed");
                     // OnControllerChanged(b, (BindableLayoutController)o, (BindableLayoutController)n)
                 });

        public static AutocompleteController GetAutocompleteController(BindableObject b)
        {
            // return (BindableLayoutController) b.GetValue(BindableLayoutControllerProperty)
            return (AutocompleteController)b.GetValue(AutocompleteControllerProperty); // here defaulvaluecreateor will create a new instance if there was no value
        }
        public static void SetItemsSource(BindableObject b, IEnumerable<object> itemsSource)
        {
            b.SetValue(ItemsSourceProperty, itemsSource);
            
        }
        public static void SetItemTemplate(BindableObject b, DataTemplate itemTemplate)
        {
            b.SetValue(ItemTemplateProperty, itemTemplate);
        }
        public static void SetBuild(BindableObject b, Func<object, DataTemplate> build)
        {
            b.SetValue(BuildProperty, build);
        }
    }

    public class AutocompleteController
    {

        public ScrollCollectionView ScrollCollectionView = new ScrollCollectionView();
        public TopContentView OnTop = new TopContentView();

        public bool IsFocused { get; set; } // not to use

        public AutocompleteController(InputView inputView)
        {
            OnTop.Content = ScrollCollectionView;
            var parent = (Layout<View>)inputView.Parent; //
            OnTop.IsVisible = false;

            parent.Children.Add(OnTop);

            inputView.SetTextChanged((object s, TextChangedEventArgs t) =>
            {
                Console.WriteLine("TextChanged: " + t.NewTextValue);
                var sender = (InputView)s;

                var set = OnTop;
                if(!string.IsNullOrEmpty(t.NewTextValue))
                {

                    if(!set.IsVisible)
                    {
                        OnTop.X = sender.X;
                        OnTop.Y = sender.Y + sender.Height;
                        set.IsVisible = true;
                        Console.WriteLine("show");
                        OnTop.BackgroundColor = Color.Aquamarine;
                        Console.WriteLine("items shown: " + ScrollCollectionView.ItemsSource.Count());
                    }
                }
                else
                {
                    if(set.IsVisible)
                    {
                        set.IsVisible = false;
                    }
                }
            });

        }        

        public async void HandleDeselectionOnTapOutsideAutocomplete(InputView inputView)
        {
            if(inputView as SearchBar == null && inputView as Entry == null)
            {
                throw new ArgumentException("Can't handle an InputView that's not a descendant of either Entry or SearchBar ");
            }
            // deselection
            if (inputView is SearchBar searchBar)
            {

#if __IOS__
                var iOSSearchBar = await searchBar.On<Xamarin.Forms.PlatformConfiguration.iOS>().iOSAsync();
                Console.WriteLine("iOSSearchBar is: " + iOSSearchBar);

#else
#if __ANDROID__
                var androidSearchBar = await searchBar.On<Xamarin.Forms.PlatformConfiguration.Android>().AndroidAsync();
                Console.WriteLine("androidSearchBar is: " + androidSearchBar);
#else
#endif
#endif
            }
            if(inputView is Entry entry)
            {
#if __IOS__
                var iOSEntry = await entry.On<Xamarin.Forms.PlatformConfiguration.iOS>().iOSAsync();
                Console.WriteLine("iOSSearchBar is: " + iOSEntry);

#else
#if __ANDROID__
                var androidEntry = await entry.On<Xamarin.Forms.PlatformConfiguration.Android>().AndroidAsync();
                Console.WriteLine("iOSSearchBar is: " + androidEntry);
#else
#endif
#endif
            }
        }

        public static void FocusChangeRequested(object sender, FocusRequestArgs e)
        {
            // https://stackoverflow.com/questions/56959599/adding-an-eventhandler-to-event-with-reflection-in-xamarin-forms
            Console.WriteLine(" Won't fire ");   // fires only when calling Focus()/Unfocus which xamarin itself doesen't use
        }

        public static void PropertyChanged(object s, PropertyChangedEventArgs p)
        {
            Console.WriteLine("gets called");
        }

        private void InputView_PropertyChanging(object sender, Xamarin.Forms.PropertyChangingEventArgs e)
        {
            if (e.PropertyName == "IsFocused")
            {

            }
        }

        /*
        [Observable]
        public int MyInteger { get; set; } = 0;
        public void Set()
        {
            MyInteger++;
            Console.WriteLine("MyInteger: " + MyInteger);
        }
        */
    }

    static class Extension 
    {
        static readonly BindableProperty IsFocusedProperty =
            BindableProperty.CreateAttached("IsFocused", typeof(bool), typeof(VisualElement), default(bool),
                propertyChanged: (b, o, n) =>
                {
                    Console.WriteLine("b: " + b + ", n: " + n);
                });

        public static void SetTextChanged(this InputView inputView, EventHandler<TextChangedEventArgs> textChangedEventHandler)
        {
            if(inputView is Entry entry)
            {
                entry.TextChanged -= textChangedEventHandler;
                entry.TextChanged += textChangedEventHandler;
                //entry.
            }
            else if(inputView is SearchBar searchBar)
            {
                searchBar.TextChanged -= textChangedEventHandler;
                searchBar.TextChanged += textChangedEventHandler;
            }
           
            // or other type of textinputs
        }
    }

    class ViewModel : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

#region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#endregion
    }
}
