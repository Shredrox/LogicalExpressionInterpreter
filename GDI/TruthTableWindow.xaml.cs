using System.Windows;
using System.Windows.Input;

namespace GDI
{
    public partial class TruthTableWindow : Window
    {
        public TruthTableWindow(string table)
        {
            InitializeComponent();

            TruthTableBox.Text = table;
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
