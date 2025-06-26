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


namespace GUI.ViewModel
{
    public class DichVuVM : BaseViewModel
    {
        private ObservableCollection<Model.DichVu> _List;
        public ObservableCollection<Model.DichVu> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        public ICommand AddDichVuFormCommand { get; set; }
        public ICommand EditDichVuFormCommand { get; set; }
        public ICommand AddDichVuCommand { get; set; }
        public ICommand EditDichVuCommand { get; set; }
        public ICommand DeleteDichVuCommand { get; set; }
        

        private string _tenDV;
        private string _donVi;
        private int _donGia;
        private void LoadDichVu()
        {
            List = new ObservableCollection<Model.DichVu>(DataProvider.Ins.db.DichVus.ToList());
        }
        private Model.DichVu _SelectedItem;
        public Model.DichVu SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value; OnPropertyChanged();
                if (SelectedItem != null)
                {
                    tenDV = SelectedItem.tenDV;
                    donVi = SelectedItem.donVi;
                    donGia = (int)SelectedItem.donGia;
                }
            }
        }
        public string tenDV { get => _tenDV; set { _tenDV = value; OnPropertyChanged(); } }
        public string donVi { get => _donVi; set { _donVi = value; OnPropertyChanged(); } }
        public int donGia { get => _donGia; set { _donGia = value; OnPropertyChanged(); } }
        public DichVuVM()
        {
            List = new ObservableCollection<Model.DichVu>(DataProvider.Ins.db.DichVus);
            AddDichVuFormCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddDichVu wd = new AddDichVu(); wd.ShowDialog(); });
            EditDichVuFormCommand = new RelayCommand<object>(
                (p) => SelectedItem != null,
                (p) =>
                {
                    var formEdit = new EditDichVu();
                    var editVM = new DichVuVM
                    {
                        tenDV = SelectedItem.tenDV,
                        donVi = SelectedItem.donVi,
                        donGia = (int)SelectedItem.donGia,
                        SelectedItem = SelectedItem
                    };
                    formEdit.DataContext = editVM;
                    formEdit.ShowDialog();
                }
                );
            AddDichVuCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (string.IsNullOrWhiteSpace(tenDV))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập tên dịch vụ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(donVi))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập đơn vị tính!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (donGia <= 0)
                {
                    System.Windows.MessageBox.Show("Đơn giá phải lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var DichVu = new Model.DichVu() { tenDV = tenDV, donVi = donVi, donGia = donGia };
                DataProvider.Ins.db.DichVus.Add(DichVu);
                DataProvider.Ins.db.SaveChanges();
                System.Windows.MessageBox.Show("Thêm dịch vụ thành công");
                if (p is Window window)
                {
                    window.Close();
                }
            }
            );
            EditDichVuCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return DataProvider.Ins.db.DichVus.Any(x => x.maDV == SelectedItem.maDV);
            }, (p) =>
            {
                if (string.IsNullOrWhiteSpace(tenDV) || string.IsNullOrWhiteSpace(donVi) || donGia <= 0)
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var DV = DataProvider.Ins.db.DichVus.Where(x => x.maDV == SelectedItem.maDV).SingleOrDefault();
                DV.tenDV = tenDV;
                DV.donVi = donVi;
                DV.donGia = donGia;
                DataProvider.Ins.db.SaveChanges();
                System.Windows.MessageBox.Show("Cập nhật dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (p is Window window)
                window.Close();
            });
            DeleteDichVuCommand = new RelayCommand<object>(
                (p) => SelectedItem != null,
                (p) =>
                {
                    var result = System.Windows.MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này không?", "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                        return;
                    var dv = DataProvider.Ins.db.DichVus.FirstOrDefault(x => x.maDV == SelectedItem.maDV);

                    if (dv != null)
                    {
                        DataProvider.Ins.db.DichVus.Remove(dv);
                        DataProvider.Ins.db.SaveChanges();

                        System.Windows.MessageBox.Show("Xóa dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Load lại danh sách nếu bạn có ListDichVu
                        LoadDichVu();
                    }
                }
            );
        }
    }
}
