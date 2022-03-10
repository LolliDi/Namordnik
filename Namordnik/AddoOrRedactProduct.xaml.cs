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
    /// Логика взаимодействия для AddoOrRedactProduct.xaml
    /// </summary>
    public partial class AddoOrRedactProduct : Window
    {
        List<MaterialsForProduct> DBMaterials = new List<MaterialsForProduct>();
        public AddoOrRedactProduct()
        {
            InitializeComponent();
            List<Material> list = db.dbcon.Material.ToList();
            foreach(Material pm in list)
            {
                DBMaterials.Add(new MaterialsForProduct(pm.ID, pm.Title));
            }
            UpdateComboBoxMaterials();
        }

        public void UpdateComboBoxMaterials()
        {
            ComboBoxMaterials.Items.Clear();
            foreach(MaterialsForProduct mfp in DBMaterials)
            {
                ComboBoxMaterials.Items.Add("" + mfp.Name + " " + mfp.Count);
            }
            ComboBoxMaterials.SelectedIndex = -1;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            MaterialsForProduct mfp = DBMaterials[ComboBoxMaterials.SelectedIndex];
            if (ComboBoxMaterials.SelectedIndex >= 0 && TextBoxCount.Text.Length>0)
            {
               mfp.Count = Convert.ToInt32(TextBoxCount.Text);
            }
            DBMaterials[ComboBoxMaterials.SelectedIndex] = mfp;
            UpdateComboBoxMaterials();
        }   

        struct MaterialsForProduct
        {
            int materialId;
            string materialName;
            int count;

            public int Id
            {
                get => materialId;
                set
                {
                    materialId = value;
                }
            }
            public string Name 
            {
                get => materialName;
                set
                {
                    materialName = value;
                }
            }
            public int Count
            {
                get => count;
                set
                {
                    count = value;
                }
            }


            public MaterialsForProduct(int id, string name, int count)
            {
                materialId = id;
                materialName = name;
                this.count = count;
            }
            public MaterialsForProduct(int id, string name)
            {
                materialId = id;
                materialName = name;
                count = 0;
            }
        }
    }


}
