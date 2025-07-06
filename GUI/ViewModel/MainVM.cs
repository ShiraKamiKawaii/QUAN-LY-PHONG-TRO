using GUI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GUI.Utilities;
using System.Windows;
namespace GUI.ViewModel
{
    class MainVM : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }
        public bool Isloaded = false;
        public ICommand NhaCommand { get; set; }
        public ICommand PhongCommand { get; set; }
        public ICommand HoaDonCommand { get; set; }
        public ICommand DichVuCommand { get; set; }
        public ICommand KhachThueCommand { get; set; }
        public ICommand BaoCaoCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        private void Nha(object obj) => CurrentView = new NhaVM();
        private void Phong(object obj) => CurrentView = new PhongVM();
        private void HoaDon(object obj) => CurrentView = new HoaDonVM();
        private void DichVu(object obj) => CurrentView = new DichVuVM();
        private void KhachThue(object obj) => CurrentView = new KhachThueVM();
        private void BaoCao(object obj) => CurrentView = new BaoCaoVM();
        

        public MainVM()
        {
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Isloaded = true;
                p.Hide();
                Login loginWindow = new Login();
                loginWindow.ShowDialog();
                if (loginWindow.DataContext == null)
                    return;
                var loginVM = loginWindow.DataContext as LoginVM;
                if (loginVM.IsLogin)
                {
                    p.Show();
                }
                else
                {
                    p.Close();
                }
            });
            NhaCommand = new RelayCommand(Nha);
            PhongCommand = new RelayCommand(Phong);
            HoaDonCommand = new RelayCommand(HoaDon);
            DichVuCommand = new RelayCommand(DichVu);
            KhachThueCommand = new RelayCommand(KhachThue);
            BaoCaoCommand = new RelayCommand(BaoCao);
            // Startup Page
            CurrentView = new PhongVM();
            LogoutCommand = new RelayCommand<Window>((p) =>
            {
                return p != null;
            },
            (p) =>
            {
                if (p == null) return;
                p.Hide();
                Login login= new Login();
                login.DataContext = new LoginVM();
                login.ShowDialog();
                if(login.DataContext is LoginVM vm && vm.IsLogin)
                {
                    p.Show();
                }
                else
                {
                    p.Close();
                }
            });
        }
    }
}
