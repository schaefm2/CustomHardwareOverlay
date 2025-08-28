using Hardware_Monitor.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Hardware_Monitor.Views
{
    /// <summary>
    /// Interaction logic for SensorTree.xaml
    /// </summary>
    public partial class SensorTree : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<TypeSensorViewModel>), typeof(SensorTree));

        public ObservableCollection<TypeSensorViewModel> ItemsSource
        {
            get => (ObservableCollection<TypeSensorViewModel>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(AddCommand), typeof(RelayCommand), typeof(SensorTree));

        public RelayCommand AddCommand
        {
            get => (RelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public SensorTree()
        {
            InitializeComponent();
        }
    }
}
