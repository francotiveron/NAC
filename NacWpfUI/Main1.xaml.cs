using System.Windows;
using System.Windows.Controls;

namespace Nac.Wpf.UI {
    /// <summary>
    /// Interaction logic for Main1.xaml
    /// </summary>
    public partial class Main1 : ContentControl {
        public Main1() {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            //NacEngine.Init();
            //INacWcf engine = new NacEngine();

            //engine.CreateProject("P1");

            //NacVariable v1 = engine.CreateVariable("P1", "V1") as NacVariable;
            //NacVariable v2 = engine.CreateVariable("P1", "V2") as NacVariable;
            //NacVariable v3 = engine.CreateVariable("P1", "V3") as NacVariable;

            //v1.Value = 1; engine.CheckIn(v1);
            //v2.Value = 2; engine.CheckIn(v1);

            //engine.CreateTask("P1", "T1");
            //engine.CreateSection("P1/T1", "S1");
            //engine.CreateBlock("P1/T1/S1", "B1");

            //NacCollection<NacObject> projects = engine.GetProjects();

            //engine.StartTask("P1/T1");
        }

        private void button_Click_1(object sender, RoutedEventArgs e) {
            NacWpfContext ctx = DataContext as NacWpfContext;
            //ctx.Ciccia = "Piroga123";
        }
    }
}
