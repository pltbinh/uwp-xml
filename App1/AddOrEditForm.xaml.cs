using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddOrEditForm : Page
    {
        ObservableCollection<Node> parents = new ObservableCollection<Node>();
        
        List<Node> roots;
        Node defaultNode = new Node();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            roots = (List<Node>)e.Parameter;

            List<Node> flattenedNodes = new List<Node>();
            foreach (Node node in roots)
            {
                flattenedNodes.AddRange(flattenNode(node));
            }

            foreach (Node node in flattenedNodes)
            {
                parents.Add(node);
            }
        }

        public AddOrEditForm()
        {
            this.InitializeComponent();
            defaultNode.Feature = "--none--";
            parents.Add(defaultNode);
        }

        public List<Node> flattenNode(Node node)
        {
            List<Node> result = new List<Node>();
            result.Add(node);

            if (node.ChildNodes.Count > 0)
            {
                foreach(Node child in node.ChildNodes)
                {
                    result.AddRange(flattenNode(child));
                }
            }

            return result;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string selectedParent = ((Node)cbParent.SelectedValue).Feature;
            string feature = Feature.Text;
            string appName = AppName.Text;
            string appLink = AppLink.Text;

            Node newNode = new Node();
            newNode.Feature = feature;
            newNode.AppName = appName;
            newNode.AppLink = appLink;

            if (selectedParent == "--none")
            {
                roots.Add(newNode);
            } else
            {
                foreach (Node node in roots)
                {
                    node.AddChildNode(newNode, selectedParent);
                }
            }

            WriteXmlFile();

            Frame.GoBack();
        }
        public void WriteXmlFile()
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load("./data.xml");
            //foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            //{
            //    node = null;
            //}
            //doc.DocumentElement.RemoveAll();
            //doc.Save(@"D:\datatest.xml");
        }

    }
}
