using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        bool _isChange = false;
        Product p = new Product();
        public AddoOrRedactProduct()
        {
            InitializeComponent();
            ComboBoxProductType.ItemsSource = db.dbcon.ProductType.Select(x => x.Title).ToList();
            List<Material> list = db.dbcon.Material.ToList();
            foreach(Material m in list)
            {
                DBMaterials.Add(new MaterialsForProduct(m.ID, m.Title));
            }
            UpdateComboBoxMaterials();
            UpdateTextBoxMaterials();
        }
        public AddoOrRedactProduct(Product p)
        {
            InitializeComponent();
            BtnDel.Visibility = Visibility.Visible;
            this.p = p;
            _isChange = true;
            ComboBoxProductType.ItemsSource = db.dbcon.ProductType.Select(x => x.Title).ToList();
            try
            {
                ComboBoxProductType.SelectedValue = db.dbcon.ProductType.First(x => x.ID == p.ProductTypeID).Title;
            }
            catch
            {
                ComboBoxProductType.SelectedItem = 0;
            }
            TextBoxTitle.Text = p.Title;
            TextBoxWorkshopNumber.Text = ""+p.ProductionWorkshopNumber;
            TextBoxArticle.Text = "" + p.ArticleNumber;
            TextBoxPrice.Text = "" + p.MinCostForAgent;
            TextBoxPersonCount.Text = "" + p.ProductionPersonCount.ToString();
            TextBoxDescription.Text = "" + p.Description;
            TextBoxImage.Text = p.Image;
            List<Material> list = db.dbcon.Material.ToList();
            List<ProductMaterial> prodmat = db.dbcon.ProductMaterial.Where(x => x.ProductID == p.ID).ToList();
            foreach (Material m in list)
            {
                ProductMaterial pm=null;
                if (prodmat.Count>0)
                {
                    pm = prodmat.FirstOrDefault(x => x.MaterialID == m.ID);
                }
                if (pm==null)
                {
                    DBMaterials.Add(new MaterialsForProduct(m.ID, m.Title));
                }
                else
                {
                    DBMaterials.Add(new MaterialsForProduct(m.ID, m.Title, (int)pm.Count));
                }
            }

            UpdateComboBoxMaterials();
            UpdateTextBoxMaterials();
        }

        public void UpdateComboBoxMaterials()
        {
            int selectedIndex = ComboBoxMaterials.SelectedIndex >= 0 ? ComboBoxMaterials.SelectedIndex : 0;
            ComboBoxMaterials.Items.Clear();
            foreach(MaterialsForProduct mfp in DBMaterials)
            {
                ComboBoxMaterials.Items.Add("" + mfp.Name + " - " + mfp.Count);
            }
            ComboBoxMaterials.SelectedIndex = selectedIndex;
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
            UpdateTextBoxMaterials();
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

        private void ComboBoxMaterials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxMaterials.SelectedIndex > -1)
            {
                TextBoxCount.Text = "" + DBMaterials[ComboBoxMaterials.SelectedIndex].Count;
                TextBoxCount.SelectionStart = 0;
                TextBoxCount.SelectionLength = TextBoxCount.Text.Length;
                TextBoxCount.Focus();
            }
        }

        public void UpdateTextBoxMaterials()
        {
            string newMaterialInfo = "";
            foreach(MaterialsForProduct mfp in DBMaterials)
            {
                if(mfp.Count>0)
                {
                    newMaterialInfo += mfp.Name + " - " + mfp.Count + ", ";
                }
            }
            if (newMaterialInfo.Length > 0)
            {
                newMaterialInfo = newMaterialInfo.Substring(0, newMaterialInfo.Length - 2);
            }
            TextBoxMaterials.Text = newMaterialInfo;

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string title = TextBoxTitle.Text;
                string productTypeComboBoxText = ComboBoxProductType.Text;
                string articleNumber = TextBoxArticle.Text;
                string description = TextBoxDescription.Text;
                string productPersonCount = TextBoxPersonCount.Text;
                string productionWorkshop = TextBoxWorkshopNumber.Text;
                string minCost = TextBoxPrice.Text;
                string exceptionString = "";
                string image = TextBoxImage.Text;
                if(title.Length==0)
                {
                    exceptionString += "Введите название\n";
                }
                else
                {
                    if (!_isChange&&db.dbcon.Product.Where(x => x.Title == title).ToList().Count > 0)
                    {
                        exceptionString += "Такое название продукта уже зарегестрировано\n";
                    }
                }
                if(articleNumber.Length==0||articleNumber.Length>10)
                {
                    exceptionString += "Артикль должен содержать от 1 до 10 цифр\n";
                }
                if(minCost.Length==0)
                {
                    exceptionString += "Введите цену для агента\n";
                }
                if(productTypeComboBoxText.Length==0)
                {
                    exceptionString += "Выберите тип продукта";
                }
                if(exceptionString.Length>0)
                {
                    throw new Exception(exceptionString);
                }
                p.Title = title;
                p.ArticleNumber = articleNumber;
                p.MinCostForAgent = Convert.ToInt32(minCost);
                p.Description = description;
                p.Image = image;
                if (productTypeComboBoxText.Length > 0)
                {
                    p.ProductTypeID = db.dbcon.ProductType.First(x => x.Title == productTypeComboBoxText).ID;
                }
                p.Description = description;
                if (productPersonCount.Length > 0)
                {
                    p.ProductionPersonCount = Convert.ToInt32(productPersonCount);
                }
                if (productionWorkshop.Length > 0)
                {
                    p.ProductionWorkshopNumber = Convert.ToInt32(productionWorkshop);
                }
                if (_isChange)
                {

                }
                else
                {
                    db.dbcon.Product.Add(p);
                }
                db.dbcon.SaveChanges();
                int productId = p.ID;
                List<ProductMaterial> pm = db.dbcon.ProductMaterial.Where(x=>x.ProductID==productId).ToList();
                foreach(MaterialsForProduct mfp in DBMaterials)
                {
                    if (mfp.Count > 0&& pm.FirstOrDefault(x => x.MaterialID == mfp.Id) == null)
                    {
                        //ProductMaterial p = pm.First(x => x.MaterialID == mfp.Id);
                        db.dbcon.ProductMaterial.Add(new ProductMaterial() { ProductID = productId, MaterialID = mfp.Id, Count = mfp.Count });
                        db.dbcon.SaveChanges();

                    }
                    else if (mfp.Count == 0 && pm.FirstOrDefault(x => x.MaterialID == mfp.Id) != null)
                    {
                        db.dbcon.ProductMaterial.Remove(pm.FirstOrDefault(x => x.MaterialID == mfp.Id));
                        db.dbcon.SaveChanges();
                    }
                }
                Close();
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.ToString(),"Ошибка!");
            }
        }

        int _newCount = 0;
        int _newCountPerson = 0;
        int _newArticle = 0;
        int _newPrice = 0;
        int _newWorkshop = 0;
        public void UpdateTextBoxNum(TextBox tb, int num) //проверяем число ли в текстовом поле
        {
            try
            {
                if (tb.Text != "")
                {
                    num = Convert.ToInt32(tb.Text);
                }
            }
            catch
            {
                tb.Text = "" + num;
            }
        }
        private void TextBoxCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTextBoxNum(sender as TextBox, _newCount);
        }

        private void TextBoxArticle_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTextBoxNum(sender as TextBox, _newArticle);
        }

        private void TextBoxPersonCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTextBoxNum(sender as TextBox, _newCountPerson);
        }

        private void TextBoxPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTextBoxNum(sender as TextBox, _newPrice);
        }

        private void TextBoxWorkshopNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTextBoxNum(sender as TextBox, _newWorkshop);
        }

        private void TextBoxImage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxImage.IsFocused)
            {
                OpenFileDialog dial = new OpenFileDialog();
                dial.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
                string s = Directory.GetCurrentDirectory().Substring(0, Environment.CurrentDirectory.Length - 9);
                dial.InitialDirectory = s + "products";
                dial.ShowDialog();
                TextBoxImage.Text = "\\" + dial.FileName.Replace(s, "");
                Keyboard.ClearFocus();
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            if(_isChange)
            {

                MessageBoxResult res = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    db.dbcon.Product.Remove(p);
                    db.dbcon.SaveChanges();
                    Close();
                }
            }

        }
    }


}
