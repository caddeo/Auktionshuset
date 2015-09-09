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

namespace projekt_Auktionshuset
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<string> stringSourceList = new List<string>();

            for (int i = 0; i < 1000; i++)
            {
                stringSourceList.Add("test "+i);
            }

            ListBoxAuctionLog.ItemsSource = stringSourceList;

            /* Vælger sidste element, og scroller ned til det.*/
            ListBoxAuctionLog.ScrollIntoView(ListBoxAuctionLog.Items[ListBoxAuctionLog.Items.Count-1]);


        }
    }
}
