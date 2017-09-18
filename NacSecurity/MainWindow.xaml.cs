using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
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

namespace NacSecurity {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            // create your domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            // define a "query-by-example" principal - here, we search for a GroupPrincipal 
            GroupPrincipal qbeGroup = new GroupPrincipal(ctx);

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(qbeGroup);

            // find all matches
            foreach (var found in srch.FindAll()) {
                // do whatever here - "found" is of type "Principal" - it could be user, group, computer.....          
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            // create your domain context
            using (var ctx = new PrincipalContext(ContextType.Domain)) {
                UserPrincipal usr = UserPrincipal.FindByIdentity(ctx,
                                                           IdentityType.SamAccountName,
                                                           "franco.tiveron");

                if (usr != null) {
                    if (usr.Enabled == false)
                        usr.Enabled = true;

                    usr.Save();
                    usr.Dispose();
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            using (var ctx = new PrincipalContext(ContextType.Domain)) {
                UserPrincipal usr = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, "franco.tiveron");
                var groups = usr.GetGroups();
                foreach (var group in groups) Debug.Print(group.DisplayName);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e) {
            var curr = UserPrincipal.Current;
        }

        private void button4_Click(object sender, RoutedEventArgs e) {
            // set up domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Machine, "aunpmap3.npm.local");

            UserPrincipal user = UserPrincipal.Current;

            if (user != null) {
                GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "NacViewers");
                if (user.IsMemberOf(group)) Debug.Print("YES"); else Debug.Print("NO");
            }
        }
    }
}
