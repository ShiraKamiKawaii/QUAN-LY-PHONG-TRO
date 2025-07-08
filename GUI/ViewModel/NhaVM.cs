using GUI.Model;
using GUI.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GUI.ViewModel
{
    public class NhaVM : BaseViewModel
    {
        private ObservableCollection<Model.Nha> _List;
        public ObservableCollection<Model.Nha> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        public ICommand AddNhaFormCommand { get; set; }
        public ICommand EditNhaFormCommand { get; set; }
        public ICommand AddNhaCommand { get; set; }
        public ICommand EditNhaCommand { get; set; }
        public ICommand DeleteNhaCommand { get;set; }
        public Action OnReLoad { get; set; }
        private string _tenNha;
        private string _diaChi;
        private int _soPhong;
        private int _dienTich;
        private int _giaThue;
        private int _maNha;

        
        private Model.Nha _SelectedItem;
        public Model.Nha SelectedItem
        {
            get => _SelectedItem;
            set 
            {
                _SelectedItem = value; OnPropertyChanged();
                if(SelectedItem != null)
                {
                    tenNha = SelectedItem.tenNha;
                    diaChi = SelectedItem.diaChi;
                    maNha = SelectedItem.maNha;
                }
            }
        }
        public int maNha { get => _maNha; set { _maNha = value;OnPropertyChanged(); } }
        public string tenNha { get => _tenNha; set { _tenNha = value; OnPropertyChanged(); } }
        public string diaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); } }
        public int soPhong { get => _soPhong; set { _soPhong = value; OnPropertyChanged(); } }
        public int dienTich { get => _dienTich; set { _dienTich = value; OnPropertyChanged(); } }
        public int giaThue { get => _giaThue; set { _giaThue = value; OnPropertyChanged(); } }
        private void Reload()
        {
            List = new ObservableCollection<Model.Nha>(DataProvider.Ins.db.Nhas);
        }
        public NhaVM()
        {
            List = new ObservableCollection<Model.Nha>(DataProvider.Ins.db.Nhas);
            AddNhaFormCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                AddNha wd = new AddNha(); NhaVM vm = new NhaVM(); vm.OnReLoad = Reload; wd.DataContext = vm; wd.ShowDialog(); });
            EditNhaFormCommand = new RelayCommand<Model.Nha>(
                (p) => SelectedItem != null,
                (p) =>
                {
                    var formEdit = new EditNha();
                    var editVM = new NhaVM
                    {
                        tenNha = SelectedItem.tenNha,
                        diaChi = SelectedItem.diaChi,
                        SelectedItem = SelectedItem
                    };
                    editVM.OnReLoad = Reload;
                    formEdit.DataContext = editVM;
                    formEdit.ShowDialog();
                }
                );
            AddNhaCommand = new RelayCommand<object>(
                (p) =>
                {
                    return !string.IsNullOrEmpty(tenNha) && soPhong > 0 && dienTich > 0 && giaThue > 0;

                },
                (p) =>
                {
                    if(string.IsNullOrWhiteSpace(tenNha) || string.IsNullOrWhiteSpace(diaChi))
                    {
                        System.Windows.MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var nha = new Model.Nha() { tenNha = tenNha, diaChi = diaChi };
                    DataProvider.Ins.db.Nhas.Add(nha);
                    DataProvider.Ins.db.SaveChanges();
                    for (int i =1; i <= soPhong; i++)
                    {
                        var phong = new Model.Phong { dienTich = dienTich, giaThue = giaThue, tenPhong = $"Phòng {i}", maNha=nha.maNha, tienNo = 0, tinhTrang ="Đang trống"};
                        DataProvider.Ins.db.Phongs.Add(phong);
                    }
                    DataProvider.Ins.db.SaveChanges();
                    System.Windows.MessageBox.Show("Thêm Nhà thành công");
                    OnReLoad?.Invoke();
                    if (p is Window window)
                    {
                        window.Close();
                    }
                }
                );
            EditNhaCommand = new RelayCommand<object>((p) =>
            {
                return SelectedItem != null && DataProvider.Ins.db.Nhas.Any(x => x.maNha == SelectedItem.maNha);
            }, (p) =>
            {
                if(string.IsNullOrWhiteSpace(tenNha) || string.IsNullOrWhiteSpace(diaChi))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var nha = DataProvider.Ins.db.Nhas.Where(x => x.maNha == SelectedItem.maNha).SingleOrDefault();
                nha.tenNha = tenNha;
                nha.diaChi = diaChi;
                DataProvider.Ins.db.SaveChanges();
                System.Windows.MessageBox.Show("Cập nhật nhà thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                OnReLoad?.Invoke();
                if (p is Window window)
                    window.Close();
            });
            DeleteNhaCommand = new RelayCommand<object>(
                (p) => SelectedItem != null,
                (p) =>
                {
                    var result = System.Windows.MessageBox.Show("Bạn có chắc chắn muốn xóa nhà này không?", "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                        return;
                    var nha = DataProvider.Ins.db.Nhas.FirstOrDefault(x => x.maNha == SelectedItem.maNha);

                    if (nha != null)
                    {
                        DataProvider.Ins.db.Nhas.Remove(nha);
                        DataProvider.Ins.db.SaveChanges();
                        System.Windows.MessageBox.Show("Xóa nhà thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    Reload();
                }
            );
        }
    }
}
