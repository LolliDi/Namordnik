using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Namordnik
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Product> DB;
        public MainWindow()
        {
            InitializeComponent();
            ComboBoxFilt.Items.Add("Все");
            foreach (ProductType p in db.dbcon.ProductType.ToList())
            {
                ComboBoxFilt.Items.Add(p.Title);
            }
            ComboBoxFilt.SelectedIndex = 0;
            ComboBoxSort.SelectedIndex = 0;
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filt();
        }

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filt();
        }

        public void Filt()
        {
            List<Product> newDB = new List<Product>();
            ViewDB.Items.Clear();
            DB = db.dbcon.Product.ToList();
            string searchStroke = TextBoxSearch.Text.ToLower();
            if (searchStroke.Length > 0)
            {
                foreach (Product p in DB)
                {
                    if (p.Title.ToLower().Contains(searchStroke))
                    {
                        newDB.Add(p);
                    }
                }
                DB = newDB;
            }

            if (ComboBoxFilt.SelectedValue.ToString() != "Все")
            {
                DB = DB.Where(x => x.ProductType.Title.ToString() == ComboBoxFilt.SelectedValue.ToString()).ToList();
            }

            switch(ComboBoxSort.SelectedIndex)
            {
                case 0:
                    DB = DB.OrderBy(x=>x.Title).ToList();
                    break;
                case 1:
                    DB = DB.OrderByDescending(x => x.Title).ToList();
                    break;
                case 2:
                    DB = DB.OrderBy(x => x.ProductionWorkshopNumber).ToList();
                    break;
                case 3:
                    DB = DB.OrderByDescending(x => x.ProductionWorkshopNumber).ToList();
                    break;
                case 4:
                    DB = DB.OrderBy(x => x.MinCostForAgent).ToList();
                    break;
                case 5:
                    DB = DB.OrderByDescending(x => x.MinCostForAgent).ToList();
                    break;
            }

            foreach (Product p in DB)
            {
                ViewDB.Items.Add(p);
            }
        }

        private void ViewDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ViewDB.SelectedItems.Count>0)
            {
                BtnChangePrice.Visibility = Visibility.Visible;
            }
            else
            {
                BtnChangePrice.Visibility = Visibility.Collapsed;
            }
            if(ViewDB.SelectedItems.Count==1)
            {
                BtnChange.Visibility = Visibility.Visible;
            }
            else
            {
                BtnChange.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnChangePrice_Click(object sender, RoutedEventArgs e)
        {
            
            List<Product> ppp = new List<Product>();
            foreach(Product pp in ViewDB.SelectedItems)
            {
                ppp.Add(pp);
            }
            ChangePriceForAgent window = new ChangePriceForAgent(ppp as List<Product>);
            window.Owner = this;
            window.Visibility = Visibility.Visible;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
            ViewDB.Items.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddoOrRedactProduct window = new AddoOrRedactProduct();
            window.Owner = this;
            window.Visibility = Visibility.Visible;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
            Filt();
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            Product p = (Product)ViewDB.SelectedItem;
            AddoOrRedactProduct window = new AddoOrRedactProduct(p);
            window.Owner = this;
            window.Visibility = Visibility.Visible;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
            Filt();
        }
    }
}
