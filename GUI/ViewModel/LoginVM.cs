using GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI.ViewModel
{
    class LoginVM : BaseViewModel
    {
        public bool IsLogin { get; set; }
        private string _taiKhoan;
        public string taiKhoan { get => _taiKhoan; set { _taiKhoan = value; OnPropertyChanged(); } }
        private string _matKhau;
        public string matKhau { get => _matKhau; set { _matKhau = value; OnPropertyChanged(); } }
        public ICommand CloseCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }

        public LoginVM()
        {
            IsLogin = false;
            taiKhoan = "";
            matKhau = "";
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p); });
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { matKhau = p.Password; });
        }
        void Login(Window p)
        {
            if (p == null)
                return;
            var accCount = DataProvider.Ins.db.NguoiDungs.Where(x => x.taiKhoan == taiKhoan && x.matKhau == matKhau).Count();

            if (accCount > 0)
            {
                IsLogin = true;

                p.Close();
            }
            else
            {
                IsLogin = false;
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
            }

        }
    }
}
