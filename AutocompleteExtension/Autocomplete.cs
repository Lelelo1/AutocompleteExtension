﻿using System;
using Xamarin.Forms;
// using NObservable;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
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
                propertyChanged: (b, o, n) =>
                {
                    GetAutocompleteController(b).ScrollCollectionView.ItemsSource = (IEnumerable<object>)n;
                    GetAutocompleteController(b).HandleDynamicItemsSource((IEnumerable<object>)n);
                });

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
        static readonly BindableProperty AutocompleteControllerProperty =
             BindableProperty.CreateAttached("BindableLayoutController", typeof(AutocompleteController), typeof(InputView), default(AutocompleteController),
                 defaultValueCreator: (b) => {

                     return new AutocompleteController((InputView)b);
                 },
                 propertyChanged: (b, o, n) =>
                 {
                     // Console.WriteLine("Autocompletecontroller property changed");
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
                // Console.WriteLine("TextChanged: " + t.NewTextValue);
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

            /*
             * have to manually tell scrollCollectionView to update for some reason (when it is inside topcontentview).
             * Otherwise if list is empty it will never get a height irregardless of items added later
             */

            

            HandleDeselectionOnTapOutside(() => { OnTop.IsVisible = false; }, inputView);
        }

        public void HandleDeselectionOnTapOutside(Action dismiss, InputView inputView)
        {
            inputView.Unfocused += (object sender, FocusEventArgs e) =>
            {
                // Console.WriteLine("focused: " + e.IsFocused);
                dismiss();
            };
        }

        // Boilerplate to preform a manual relayout of top contentview
        private INotifyCollectionChanged oldItemsSource = null;
        public void HandleDynamicItemsSource(IEnumerable<object> itemsSource)
        {
            if (ScrollCollectionView.ItemsSource is INotifyCollectionChanged collection)
            {
                if(oldItemsSource != itemsSource) // is new valid itemssource
                {
                    collection.CollectionChanged += Refresh_OnCollectionChanged;
                    if(oldItemsSource != null) // unssubscribe from old itemssource
                    {
                        oldItemsSource.CollectionChanged -= Refresh_OnCollectionChanged;
                    }
                    oldItemsSource = null;
                    oldItemsSource = collection;
                }
            }
        }

        private void Refresh_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            /*
             * forcelayout did not work. It could be so that it is the parent of the TopContentView
             * that needs forcelayout called. The parent could however be the layout of the whole
             * page - which might not be very performant to call forcelayout on.
             */
            if(OnTop.IsVisible)
            {
                Console.WriteLine("Refresh");
                OnTop.IsVisible = false;
                OnTop.IsVisible = true;
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
}
