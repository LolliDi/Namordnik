using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Word = Microsoft.Office.Interop.Word;

namespace Namordnik
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Product> DB = db.dbcon.Product.ToList();
        PageChange pc = new PageChange(db.dbcon.Product.ToList().Count);
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
            DataContext = pc;
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

            switch (ComboBoxSort.SelectedIndex)
            {
                case 0:
                    DB = DB.OrderBy(x => x.Title).ToList();
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
            
            pc.CountInList = DB.Count;
            pc.CurrentPage = 0;
            for (int i = (pc.CurrentPage - 1) * 20; i < pc.CurrentPage * 20; i++)
            {
                if (DB.Count > i&&i>=0)
                {
                    ViewDB.Items.Add(DB[i]);
                }
            }
        }

        private void ViewDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewDB.SelectedItems.Count > 0)
            {
                BtnChangePrice.Visibility = Visibility.Visible;
            }
            else
            {
                BtnChangePrice.Visibility = Visibility.Collapsed;
            }
            if (ViewDB.SelectedItems.Count == 1)
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
            foreach (Product pp in ViewDB.SelectedItems)
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

        private void GoPage_Click(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            switch (tb.Uid)
            {
                case "Prev":
                    pc.CurrentPage--;
                    break;
                case "Next":
                    pc.CurrentPage++;
                    break;
                default:
                    pc.CurrentPage = Convert.ToInt32(tb.Text);
                    break;
            }
            ViewDB.Items.Clear();
            for (int i = (pc.CurrentPage - 1) * 20; i < pc.CurrentPage * 20; i++)
            {
                if (DB.Count > i)
                {
                    ViewDB.Items.Add(DB[i]);
                }
            }
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            Word.Application app = new Word.Application();
            Word.Document doc = app.Documents.Add();

            Word.Paragraph pp = doc.Paragraphs.Add();
            Word.Range pr = pp.Range;
            pr.Text = "Категория: " + ComboBoxFilt.Text + (TextBoxSearch.Text.Length>0? ". Ключевое слово: " + TextBoxSearch.Text : ".");
            pp.set_Style("Заголовок");
            pr.InsertParagraphAfter();

            Word.Paragraph tableParagraph = doc.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table table = doc.Tables.Add(tableRange, DB.Count + 1, 6);
            table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            Word.Range cellRange;
            table.Cell(1, 1).Range.Text = "Иконка";
            table.Cell(1, 2).Range.Text = "Название";
            table.Cell(1, 3).Range.Text = "Категория";
            table.Cell(1, 4).Range.Text = "Артикль";
            table.Cell(1, 5).Range.Text = "Материалы";
            table.Cell(1, 6).Range.Text = "Цена";
            table.Rows[1].Range.Bold = 1;
            table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;


            for (int i = 0; i<DB.Count;i++)
            {
                Product p = DB[i];
                Word.InlineShape image = table.Cell(i + 2, 1).Range.InlineShapes.AddPicture(AppDomain.CurrentDomain.BaseDirectory+p.GetIcon);
                image.Width = image.Height = 50;
                table.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table.Cell(i + 2, 2).Range.Text = p.Title;
                table.Cell(i + 2, 3).Range.Text = p.ProductType.Title;
                table.Cell(i + 2, 4).Range.Text = p.ArticleNumber;
                table.Cell(i + 2, 5).Range.Text = p.GetMaterials;
                table.Cell(i + 2, 6).Range.Text = p.CostProduct;
            }
            app.Visible = true;
            doc.SaveAs2(@"..\Products_"+ ComboBoxFilt.Text + (TextBoxSearch.Text.Length > 0 ? "_Ключевое_слово_" + TextBoxSearch.Text : "") + ".docx");

        }

        private void BtnReportMaterials_Click(object sender, RoutedEventArgs e)
        {
            Word.Application app = new Word.Application();
            Word.Document doc = app.Documents.Add();

            Word.Paragraph pp = doc.Paragraphs.Add();
            Word.Range pr = pp.Range;
            pr.Text = "Категория: " + ComboBoxFilt.Text + (TextBoxSearch.Text.Length > 0 ? ". Ключевое слово: " + TextBoxSearch.Text : ".");
            pp.set_Style("Заголовок");
            pr.InsertParagraphAfter();

            foreach (Product p in DB)
            {
                Word.Paragraph productParagraph = doc.Paragraphs.Add();
                Word.Range productRange = productParagraph.Range;
                productRange.Text = p.Title;
                productParagraph.set_Style("Заголовок");
                productRange.InsertParagraphAfter();

                Word.Paragraph tableParagraph = doc.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;

                List<ProductMaterial> pm = db.dbcon.ProductMaterial.Where(x => x.ProductID == p.ID).ToList();
                if (pm.Count > 0)
                {
                    Word.Table table = doc.Tables.Add(tableRange, pm.Count + 1, 2);
                    table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    table.Cell(1, 1).Range.Text = "Название";
                    table.Cell(1, 2).Range.Text = "Количество";
                    table.Rows[1].Range.Bold = 1;
                    table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    for (int i = 0; i < pm.Count; i++)
                    {
                        ProductMaterial prodMat = pm[i];
                        table.Cell(i + 2, 1).Range.Text = prodMat.Material.Title;
                        table.Cell(i + 2, 2).Range.Text = "" + prodMat.Count;

                    }
                }
                else
                {
                    tableRange.Text = "Нет данных о материалах.";
                }
                if (p != DB.LastOrDefault())
                {
                    doc.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                }
            }
            app.Visible = true;
            doc.SaveAs2(@"..\ProductMaterials_" + ComboBoxFilt.Text + (TextBoxSearch.Text.Length > 0 ? "_Ключевое_слово_" + TextBoxSearch.Text : "") + ".docx");
        }

        private void BtnReportMaterialsInfo_Click(object sender, RoutedEventArgs e)
        {
            List<MaterialType> materialType = db.dbcon.MaterialType.ToList();
            Word.Application app = new Word.Application();
            Word.Document doc = app.Documents.Add();

            foreach (MaterialType p in materialType)
            {
                Word.Paragraph productParagraph = doc.Paragraphs.Add();
                Word.Range productRange = productParagraph.Range;
                productRange.Text = p.Title;
                productParagraph.set_Style("Заголовок");
                productRange.InsertParagraphAfter();

                Word.Paragraph tableParagraph = doc.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;

                List<Material> pm = db.dbcon.Material.Where(x => x.MaterialTypeID == p.ID).ToList();
                if (pm.Count > 0)
                {
                    Word.Table table = doc.Tables.Add(tableRange, pm.Count + 1, 5);
                    table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    table.Cell(1, 1).Range.Text = "Название";
                    table.Cell(1, 2).Range.Text = "Количество в упаковке";
                    table.Cell(1, 3).Range.Text = "Ед. изм.";
                    table.Cell(1, 4).Range.Text = "Количество на складе";
                    table.Cell(1, 5).Range.Text = "Цена";

                    table.Rows[1].Range.Bold = 1;
                    table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    for (int i = 0; i < pm.Count; i++)
                    {
                        Material m = pm[i];
                        table.Cell(i + 2, 1).Range.Text = m.Title;
                        table.Cell(i + 2, 2).Range.Text = "" + m.CountInPack;
                        table.Cell(i + 2, 3).Range.Text = m.Unit;
                        table.Cell(i + 2, 4).Range.Text = "" + m.CountInStock;
                        table.Cell(i + 2, 5).Range.Text = "" + m.Cost;


                    }
                }
                else
                {
                    tableRange.Text = "Нет материалов их данного типа";
                }
                if (p != materialType.LastOrDefault())
                {
                    doc.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                }
            }
            app.Visible = true;
            doc.SaveAs2(@"..\MaterialsInfo.docx");
        }
    }
}
