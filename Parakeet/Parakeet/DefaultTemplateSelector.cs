using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Parakeet.Presenters;
using Parakeet.Views;
using Parakeet.Sessions;
using Parakeet.DataModel;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using XamlReaderSettings = System.Xaml.XamlReaderSettings;
using System.Reflection;
using Parakeet.Infrastructure;

namespace Parakeet
{
    public class DefaultTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate activeThoughts;
        private readonly DataTemplate messageReview;
        private readonly DataTemplate messageEntry;

        public DefaultTemplateSelector()
        {
            activeThoughts = CreateTemplate(typeof(ActiveThoughtsView));
            messageReview = CreateTemplate(typeof(ThoughtReviewView));
            messageEntry = CreateTemplate(typeof(ThoughtEntryView));
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (typeof(CollectionsPresenter<SubjectSession, ObservableCollection<Message>, Message>).IsInstanceOfType(item))
                return activeThoughts;

            if (typeof(ThoughtReviewPresenter).IsInstanceOfType(item))
                return messageReview;

            if (typeof(ThoughtEntryPresenter).IsInstanceOfType(item))
                return messageEntry;
            
            return base.SelectTemplate(item, container);
        }

        public DataTemplate CreateTemplate(Type type)
        {
            string xaml =
            @"<DataTemplate 
                xmlns:views=""clr-namespace:Parakeet.Views;assembly=Parakeet""
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""                  
                >            
                    <views:" + type.Name + @" />
                </DataTemplate>";

            StringReader stringReader = new StringReader(xaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            return XamlReader.Load(xmlReader) as DataTemplate;

        }
    }
}
