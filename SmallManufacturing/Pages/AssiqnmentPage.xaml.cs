using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для AssiqnmentPage.xaml
    /// </summary>
    public partial class AssiqnmentPage : Page
    {
        OrderAssignment _orderAssignment = new OrderAssignment();

        public AssiqnmentPage(ProductionOrder order)
        {
            _orderAssignment.production_order = order.id;

            InitializeComponent();

            using (var context = new manufacturingEntities())
            {
                var employees = context.Employee.ToList();
                cbEmployee.ItemsSource = employees;

                var equipments = context.Equipment.ToList();
                cbEquipment.ItemsSource = equipments;

                var orderAssiqnments = context.OrderAssignment.Where(oa => oa.production_order == _orderAssignment.production_order).Include("Employee1").Include("Equipment1").ToList();
                LVAssiqnment.ItemsSource = orderAssiqnments;
            }

            DataContext = _orderAssignment;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OrderAssignment selectedAssiqnment = LVAssiqnment.SelectedItem as OrderAssignment;

            using (var context = new manufacturingEntities())
            {
                try
                {
                    var assiqnmentInDb = context.OrderAssignment.FirstOrDefault(oa => oa.production_order == selectedAssiqnment.production_order && oa.employee == selectedAssiqnment.employee);

                    if (MessageBox.Show($"Вы действительно снять сотрудника с данного заказа?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (assiqnmentInDb == null)
                        {
                            MessageBox.Show("Запись не найдена в базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        context.OrderAssignment.Remove(assiqnmentInDb);
                        context.SaveChanges();
                        MessageBox.Show("Сотрудник успешно снят", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        LVAssiqnment.ItemsSource = null;
                        var orderAssiqnments = context.OrderAssignment.Where(oa => oa.production_order == _orderAssignment.production_order).Include("Employee1").Include("Equipment1").ToList();
                        LVAssiqnment.ItemsSource = orderAssiqnments;
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
                if (long.Parse(cbEmployee.SelectedValue.ToString()) == 0)
                {
                    MessageBox.Show("Выберите сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    if (context.OrderAssignment.FirstOrDefault(oa => oa.production_order == _orderAssignment.production_order && oa.employee == _orderAssignment.employee) != null)
                    {
                        MessageBox.Show("Сотрудник уже назначен", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    context.OrderAssignment.Add(_orderAssignment);
                    context.SaveChanges();
                    LVAssiqnment.ItemsSource = null;
                    var orderAssiqnments = context.OrderAssignment.Where(oa => oa.production_order == _orderAssignment.production_order).Include("Employee1").Include("Equipment1").ToList();
                    LVAssiqnment.ItemsSource = orderAssiqnments;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
