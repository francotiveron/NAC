using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nac.Wpf.Common {
    public class NacSecurity {
        private const ContextType cType = ContextType.Machine;
        //private const string cDomain = "aunpmap3.npm.local";
        private const string cDomain = "10.207.89.44";
        private const string cViewersGroup = "NacViewers";
        private const string cAdminsGroup = "NacAdmins";
        private const string cLogPath = @"\\aunpmap3\NACAccessLog\NACAccess{0}.log";

        private static string LogPath { get { return string.Format(cLogPath, Environment.MachineName); } }

        public static bool Off { get; set; }

        private static PrincipalContext _context;
        private static PrincipalContext Context {
            get {
                if (_context == null) _context = new PrincipalContext(cType, cDomain);
                return _context;
            }
        }

        private static UserPrincipal _user;
        private static UserPrincipal User {
            get {
                if (_user == null) _user = UserPrincipal.Current;
                return _user;
            }
        }

        private static GroupPrincipal _viewers;
        private static GroupPrincipal Viewers {
            get {
                if (_viewers == null) _viewers = GroupPrincipal.FindByIdentity(Context, cViewersGroup);
                return _viewers;
            }
        }

        private static GroupPrincipal _admins;
        private static GroupPrincipal Admins {
            get {
                if (_admins == null) _admins = GroupPrincipal.FindByIdentity(Context, cAdminsGroup);
                return _admins;
            }
        }

        private static bool CanView { get { return User.IsMemberOf(Viewers) || CanAdmin; } }
        private static bool CanAdmin { get { return User.IsMemberOf(Admins); } }

        public static bool CanAccessRemoteEngines { 
            get {
                return true;
                if (Off) return true;
                try { if (CanView) return true; } catch { }
                MessageBox.Show("Account not allowed to access remote Engines", "Nac Security");
                return false;
            }
        }
        public static bool CanChangeRemoteEngines {
            get {
                if (Off) return true;
                try { if (CanAdmin) return true; } catch { }
                MessageBox.Show("Account not allowed to change remote Engines", "Nac Security");
                return false;
            }
        }
        public static void Log(params object[] args) {
            string LogEntry =
                DateTime.Now.ToString() + "; "
                + User + "; "
                + string.Join(
                    "; ",
                    args.Select(arg => arg.ToString())
                  )
                + Environment.NewLine;

            Task.Run(() => File.AppendAllText(LogPath, LogEntry));
        }

    }
}
