using System;
using Xamarin.Forms;
// using NObservable;
// using ScrollCollectionView;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq;

using Namespace;

namespace Namsespace
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
                propertyChanged: (b, o, n) => { GetAutocompleteController(b).ItemsSource = (IEnumerable<object>)n; });

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.CreateAttached("ItemTemplate", typeof(DataTemplate), typeof(InputView), default(DataTemplate),
                propertyChanged: (b, o, n) => { GetAutocompleteController(b).ItemTemplate = (DataTemplate)n; });

        public static readonly BindableProperty BuildProperty =
            BindableProperty.CreateAttached("Build", typeof(Func<object, DataTemplate>), typeof(InputView), default(Func<object, DataTemplate>),
                propertyChanged: (b, o, n) => { GetAutocompleteController(b).Build = (Func<object, DataTemplate>)n; });
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
        /*
        [Observable]
        public IEnumerable<object> ItemsSource { get; set; }

        [Observable]
        public DataTemplate ItemTemplate { get; set; }

        [Observable]
        public Func<object, DataTemplate> Build { get; set; }
        */
        public IEnumerable<object> ItemsSource { get; set; }
        public DataTemplate ItemTemplate { get; set; }
        public Func<object, DataTemplate> Build { get; set; }
        public bool IsFocused { get; set; } // not to use

        public AutocompleteController(InputView inputView)
        {
            /*
            Observe.When(() => ItemsSource != null && (ItemTemplate != null || Build != null), () =>
            {
                ScrollCollectionView.ItemsSource = ItemsSource;
                if(Build != null)
                {
                    ScrollCollectionView.Build = Build;
                }
                else
                {
                    ScrollCollectionView.Build -= (model) => ItemTemplate;
                    ScrollCollectionView.Build += (model) => ItemTemplate;
                }
            });
            */
            OnTop.Content = ScrollCollectionView;
            var parent = (Layout<View>)inputView.Parent; //
            OnTop.BackgroundColor = Color.Bisque;
            OnTop.IsVisible = false;
            parent.Children.Add(OnTop);

            inputView.SetTextChanged((object s, TextChangedEventArgs t) =>
            {
                Console.WriteLine(t.NewTextValue);
                var sender = (InputView)s;

                var set = OnTop;
                if(!string.IsNullOrEmpty(t.NewTextValue))
                {

                    if(!set.IsVisible)
                    {
                        OnTop.X = sender.X;
                        OnTop.Y = sender.Y + sender.Height;
                        set.IsVisible = true;
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

            var setValue = typeof(BindableObject).GetMethods().Where((info) =>
            {
                if(info.Name == "SetValue")
                {
                    var parameters = info.GetParameters();
                    try
                    {
                        if(parameters[0].Name == "property" && parameters[1].Name == "value")
                        {
                            return true;
                        }
                    }
                    catch(Exception exception)
                    {

                    }
                }
                return false;
            }).FirstOrDefault();
            if(setValue == null)
            {
                throw new Exception("Could not get SetValue from BindableObject with reflection");
            }



            /*
            inputView.SetBinding(VisualElement.IsFocusedProperty, new Binding("IsFocused"));
            var ob = new { IsFocused = true };
            inputView.BindingContext = ob;
            // ob.IsFocused = true;
            */
            inputView.PropertyChanging += InputView_PropertyChanging;

            OnTop.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() =>
                {
                    Console.WriteLine("tapped");
                    // inputView.Unfocus(); won't be trigger since it already is false

                    //inputView.Focus();
                    /*
                    if (!inputView.IsFocused)
                    {
                        Console.WriteLine("focus");
                        inputView.Focus();
                    }
                    else
                    {
                        Console.WriteLine("unfocus");
                        inputView.Unfocus();
                    }
                    */

                    // setValue.Invoke(inputView, new object[] { VisualElement.IsFocusedProperty, true });
                    
                    
                })
            });

            // MyEffect.Test();
            // use behaviors?
            var focusChange = typeof(VisualElement).GetEvent("FocusChangeRequested"); 
            var eventTest = typeof(VisualElement).GetEvent("PropertyChanged");

            focusChange.AddEventHandler(inputView,
                Delegate.CreateDelegate(type: focusChange.EventHandlerType,
                target: typeof(AutocompleteController), method:"FocusChangeRequested"));
            eventTest.AddEventHandler(inputView,
                Delegate.CreateDelegate(type: eventTest.EventHandlerType,
                target: typeof(AutocompleteController), method:"PropertyChanged"));
            /*
            var v = new Entry().IOS();
            Console.WriteLine("v iz: " + v);
            */
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
