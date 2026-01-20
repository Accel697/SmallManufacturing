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
    /// Логика взаимодействия для AddEditOrder.xaml
    /// </summary>
    public partial class AddEditOrder : Page
    {
        ProductionOrder _order = new ProductionOrder();

        public AddEditOrder(ProductionOrder order)
        {
            InitializeComponent();

            using (var context = new manufacturingEntities())
            {
                var clients = context.Client.ToList();
                cbClient.ItemsSource = clients;

                var statuses = context.OrderStatus.ToList();
                cbStatus.ItemsSource = statuses;
            }

            if (order != null )
            {
                _order = order;
                btnAssiqnment.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
            }

            DataContext = _order;
        }

        private void btnAssiqnment_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AssiqnmentPage(_order));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new manufacturingEntities())
            {
                try
                {
                    if (MessageBox.Show($"Вы действительно данный заказ?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (context.OrderProduct.Any(op => op.production_order == _order.id))
                        {
                            var orderProducts = context.OrderProduct.Where(op => op.production_order == _order.id);

                            foreach (var product in orderProducts)
                            {
                                context.OrderProduct.Remove(product);
                            }
                        }

                        var orderInDb = context.ProductionOrder.FirstOrDefault(o => o.id == _order.id);

                        if (orderInDb == null)
                        {
                            MessageBox.Show("Заказ не найден в базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        context.ProductionOrder.Remove(orderInDb);
                        context.SaveChanges();
                        MessageBox.Show("Заказ успешно удален", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService.GoBack();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new manufacturingEntities())
            {
                List<string> errors = new List<string>();

                if (_order.client == 0)
                    errors.Add("Укажите клиента");

                if (_order.start_date == null)
                    errors.Add("Укажите дату заказа");

                if (_order.end_date != null && _order.end_date < _order.start_date)
                    errors.Add("Дата окончания заказа не должна быть меньше даты заказа");

                if (_order.status == 0)
                    errors.Add("Укажите статус");

                if (errors.Count != 0)
                {
                    MessageBox.Show(string.Join("\n", errors), "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    if (_order.id == 0)
                    {
                        context.ProductionOrder.Add(_order);
                        NavigationService.GoBack();
                    }
                    else
                    {
                        var orderInDb = context.ProductionOrder.FirstOrDefault(o => o.id == _order.id);

                        if (orderInDb == null)
                        {
                            MessageBox.Show("Заказ не найден в базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        orderInDb.client = _order.client;
                        orderInDb.start_date = _order.start_date;
                        orderInDb.end_date = _order.end_date;
                        orderInDb.status = _order.status;

                        context.SaveChanges();
                        MessageBox.Show("Данные сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
