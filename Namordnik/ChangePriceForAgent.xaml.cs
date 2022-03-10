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
using System.Windows.Shapes;

namespace Namordnik
{
    /// <summary>
    /// Логика взаимодействия для ChangePriceForAgent.xaml
    /// </summary>
    public partial class ChangePriceForAgent : Window
    {
        int _newPrice;
        List<Product> selectedProdicts;
        public ChangePriceForAgent(List<Product> selected)
        {
            InitializeComponent();
            selectedProdicts = selected;

            int midlePrice = 0; //подсчет средней цены для агента
            foreach (Product p in selectedProdicts)
            {
                midlePrice += p.MinCostForAgent;
            }
            midlePrice /= selectedProdicts.Count;
            TextBlockNewPrice.Text = "" + midlePrice;

        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlockNewPrice.Text.Length > 0)
            {
                foreach(Product p in selectedProdicts)
                {
                    p.MinCostForAgent += _newPrice;
                }
                db.dbcon.SaveChanges();
                Close();
            }
            else
            {
                MessageBox.Show("Введите число на которое хотите увеличить минимальную стоимость на выбранные товары");
            }
            

        }

        private void TextBlockNewPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TextBlockNewPrice.Text == "")
                {
                    _newPrice = 0;
                }
                else
                {
                    _newPrice = Convert.ToInt32(TextBlockNewPrice.Text);
                }
            }
            catch
            {
                TextBlockNewPrice.Text = "" + _newPrice;
            }
        }
    }
}
