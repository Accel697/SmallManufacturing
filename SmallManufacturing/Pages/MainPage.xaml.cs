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
using SmallManufacturing.Database;

namespace SmallManufacturing.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            using (var context = new manufacturingEntities())
            {
                var orders = context.ProductionOrder.Include("Client1").ToList();
                LVOrder.ItemsSource = orders;
            }
        }

        private void btnMaterials_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MaterialPage());
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductPage());
        }

        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditOrder(null));
        }

        private void LVOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProductionOrder selectedOrder = LVOrder.SelectedItem as ProductionOrder;
            NavigationService.Navigate(new AddEditOrder(selectedOrder));
        }
    }
}
