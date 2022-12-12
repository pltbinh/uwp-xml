using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    public class Node
    {
        public string Feature { get; set; }
        public string AppName { get; set; }
        public string AppLink { get; set; }
        public string AppVersion { get; set; }
        public string IsChecked = "0";
        public List<Node> ChildNodes = new List<Node>();

        public void DeleteChildByName(string name)
        {
            foreach (Node child in ChildNodes)
            {
                if (child.Feature == name)
                {
                    ChildNodes.Remove(child);
                    return;
                } else if (child.ChildNodes.Count > 0)
                {
                    child.DeleteChildByName(name);
                }
            }
        }

        public void AddChildNode(Node node, string parrentName)
        {
            if (parrentName == "--none--")
            {
                ChildNodes.Add(node);
                return;
            } else
            {
                Node parrent = FindChildByName(parrentName);
                if (parrent != null)
                {
                    parrent.ChildNodes.Add(node);
                }
                return;
            }
        }

        public Node FindChildByName(string name)
        {
            if (Feature == name)
            {
                return this;
            }

            foreach (Node child in ChildNodes)
            {
                if (child.Feature == name)
                {
                    return child;
                }
                else if (child.ChildNodes.Count > 0)
                {
                    return child.FindChildByName(name);
                }
            }

            return null;
        }
    }

    public sealed partial class MainPage : Page
    {
        public List<Node> roots = new List<Node>();
        public string selectedText = null;
        public Node CreateNodeFromXmlNode(XmlNode xmlNode)
        {
            Node node = new Node();
            node.Feature = xmlNode.Name;
            node.AppName = xmlNode.Attributes["AppName"]?.InnerText;
            node.AppLink = xmlNode.Attributes["AppLink"]?.InnerText;
            node.AppVersion = xmlNode.Attributes["AppVersion"]?.InnerText;
            string isChecked = xmlNode.Attributes["IsChecked"]?.InnerText;
            node.IsChecked = isChecked != null ? isChecked : "0";

            if (xmlNode.ChildNodes.Count > 0)
            {
                List<Node> childNodes = new List<Node>();
                foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
                {
                    Node childNode = CreateNodeFromXmlNode(childXmlNode);
                    childNodes.Add(childNode);
                }
                node.ChildNodes = childNodes;
            }

            return node;
        }

        public void CreateCheckboxFromNode(Node root, int level = 0)
        {
            CheckBox checkbox = new CheckBox();
            checkbox.Content = root.Feature;
            checkbox.IsChecked = Convert.ToBoolean(Int32.Parse(root.IsChecked));
            checkbox.Margin = new Thickness(level * 40, 0, 0 ,0);
            checkbox.ContextFlyout = Menu;
            checkbox.RightTapped += RighTapped_Checkbox;
            MasterList.Items.Add(checkbox);

            if (root.ChildNodes.Count > 0)
            {
                foreach (Node childNode in root.ChildNodes)
                {
                    CreateCheckboxFromNode(childNode, level + 1);
                }
            }
        }

        public void UpdateUI()
        {
            MasterList.Items.Clear();
            foreach (Node root in roots)
            {
                CreateCheckboxFromNode(root);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            XmlDocument doc = new XmlDocument();
            doc.Load("./data.xml");
            
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                roots.Add(CreateNodeFromXmlNode(node));
            }

            UpdateUI();
        }

        private void RighTapped_Checkbox(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            selectedText = (sender as CheckBox).Content as string;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddOrEditForm), roots);
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedText != null)
            {
                foreach(Node node in roots)
                {
                    if (node.Feature == selectedText)
                    {
                        roots.Remove(node);
                    } else if (node.ChildNodes.Count > 0)
                    {
                        node.DeleteChildByName(selectedText);
                    }
                }

                UpdateUI();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
    }
}
