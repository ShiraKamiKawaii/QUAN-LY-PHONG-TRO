using GUI.Model;
using GUI.View;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace GUI.ViewModel
{
    public class KhachThueVM : BaseViewModel
    {
        private ObservableCollection<KhachThueFull> _List;
        public ObservableCollection<KhachThueFull> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Phong> _ListPhong;
        public ObservableCollection<Model.Phong> ListPhong { get => _ListPhong; set { _ListPhong = value; OnPropertyChanged(); } }
        public ICommand AddKhachThueFormCommand { get; set; }
        public ICommand EditKhachThueFormCommand { get; set; }
        public ICommand AddKhachThueCommand { get; set; }
        public ICommand EditKhachThueCommand { get; set; }
        public ICommand DeleteKhachThueCommand { get;set; }
        public ICommand ExportExcelCommand { get; set; }
        private string _tenKT;
        private DateTime? _ngaySinh;
        private string _gioiTinh;
        private string _diaChi;
        private string _dienThoai;
        private string _CCCD;
        private string _tenPhong;
        private int _maPhong;
        private KhachThueFull _SelectedItem;
        public KhachThueFull SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value; OnPropertyChanged();
                if (SelectedItem != null)
                {
                    tenKT = SelectedItem.tenkt;
                    ngaySinh = SelectedItem.ngaysinh;
                    gioiTinh = SelectedItem.gioitinh;
                    diaChi = SelectedItem.diachi;
                    dienThoai = SelectedItem.dienthoai;
                    CCCD = SelectedItem.cccd;
                    tenPhong = SelectedItem.tenPhong;
                    var phong = DataProvider.Ins.db.Phongs.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong);
                    tenPhong = phong != null ? phong.tenPhong : string.Empty;
                }
            }
        }
        private Model.Phong _SelectedPhong;
        public Model.Phong SelectedPhong
        {
            get => _SelectedPhong;
            set
            {
                _SelectedPhong = value;
                OnPropertyChanged();
            }
        }

        public string tenKT { get => _tenKT; set { _tenKT = value; OnPropertyChanged(); } }
        public DateTime? ngaySinh { get => _ngaySinh; set { _ngaySinh = value; OnPropertyChanged(); } }
        public string gioiTinh { get => _gioiTinh; set { _gioiTinh = value; OnPropertyChanged(); } }
        public string diaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); } }
        public string dienThoai { get => _dienThoai; set { _dienThoai = value; OnPropertyChanged(); } }
        public string CCCD { get => _CCCD; set { _CCCD = value; OnPropertyChanged(); } }
        public string tenPhong { get => _tenPhong; set { _tenPhong = value; OnPropertyChanged(); } }
        public int maPhong { get => _maPhong; set { _maPhong = value; OnPropertyChanged(); } }
        public class KhachThueFull
        {
            public int makt { get; set; }
            public string tenkt { get; set; }
            public DateTime? ngaysinh { get; set; }
            public string gioitinh { get; set; }
            public string diachi { get; set; }
            public string dienthoai { get; set; }
            public string cccd { get; set; }
            public int maPhong { get; set; }
            public string tenPhong { get; set; }
        }
        private void LoadList()
        {
            List = new ObservableCollection<KhachThueFull>(
                from kt in DataProvider.Ins.db.KhachThues
                join p in DataProvider.Ins.db.Phongs on kt.maPhong equals p.maPhong
                select new KhachThueFull
                {
                    makt = kt.maKT,
                    tenkt = kt.tenKT,
                    ngaysinh = kt.ngaySinh,
                    gioitinh = kt.gioiTinh,
                    diachi = kt.diaChi,
                    dienthoai = kt.dienThoai,
                    cccd = kt.CCCD,
                    maPhong = kt.maPhong,
                    tenPhong = p.tenPhong
                }
                );
        }
        public KhachThueVM()
        {
            List = new ObservableCollection<KhachThueFull>(
                from kt in DataProvider.Ins.db.KhachThues
                join p in DataProvider.Ins.db.Phongs on kt.maPhong equals p.maPhong
                select new KhachThueFull
                {
                    makt = kt.maKT,
                    tenkt = kt.tenKT,
                    ngaysinh = kt.ngaySinh,
                    gioitinh = kt.gioiTinh,
                    diachi = kt.diaChi,
                    dienthoai = kt.dienThoai,
                    cccd = kt.CCCD,
                    maPhong = kt.maPhong,
                    tenPhong = p.tenPhong
                }
                );
            ListPhong = new ObservableCollection<Model.Phong>(DataProvider.Ins.db.Phongs.ToList());
            AddKhachThueFormCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddKhachThue wd = new AddKhachThue(); wd.ShowDialog(); });
            EditKhachThueFormCommand = new RelayCommand<object>(
                (p) => SelectedItem != null,
                (p) =>
                {
                    var formEdit = new EditKhachThue();
                    var editVM = new KhachThueVM()
                    {
                        
                        tenKT = SelectedItem.tenkt,
                        ngaySinh = SelectedItem.ngaysinh,
                        gioiTinh = SelectedItem.gioitinh,
                        diaChi = SelectedItem.diachi,
                        dienThoai = SelectedItem.dienthoai,
                        CCCD = SelectedItem.cccd,
                        SelectedItem = SelectedItem
                    };
                    editVM.ListPhong = new ObservableCollection<Model.Phong>(DataProvider.Ins.db.Phongs.ToList());
                    editVM.SelectedPhong = editVM.ListPhong.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong);
                    formEdit.DataContext = editVM;
                    formEdit.ShowDialog();
                }
                );
            AddKhachThueCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedPhong == null)
                    return false;
                return true;
            }, (p) =>
            {
                if (string.IsNullOrWhiteSpace(tenKT))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập tên khách thuê!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(diaChi))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập địa chỉ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(dienThoai))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập điện thoại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(CCCD))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập căn cước công dân!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var khachthue = new Model.KhachThue() { tenKT = tenKT, ngaySinh = ngaySinh, gioiTinh = gioiTinh, diaChi = diaChi, dienThoai = dienThoai, CCCD = CCCD, maPhong = SelectedPhong.maPhong };
                DataProvider.Ins.db.KhachThues.Add(khachthue);
                var phong = DataProvider.Ins.db.Phongs.SingleOrDefault(x => x.maPhong == SelectedPhong.maPhong);
                if (phong != null)
                {
                    phong.ngayVao = SelectedPhong.ngayVao;
                    phong.ngayHet = SelectedPhong.ngayHet;
                    phong.tinhTrang = "Đang ở";  // Đảm bảo cột tinhTrang có kiểu NVARCHAR trong DB
                }
                DataProvider.Ins.db.SaveChanges();
                System.Windows.MessageBox.Show("Thêm khách thuê thành công");
                if (p is Window window)
                {
                    window.Close();
                }
            }
            );
            EditKhachThueCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return DataProvider.Ins.db.KhachThues.Any(x => x.maKT == SelectedItem.makt);
            }, (p) =>
            {
                if (string.IsNullOrWhiteSpace(tenKT) || string.IsNullOrWhiteSpace(diaChi) || string.IsNullOrWhiteSpace(dienThoai) || string.IsNullOrWhiteSpace(CCCD))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var kt = DataProvider.Ins.db.KhachThues.Where(x => x.maKT == SelectedItem.makt).SingleOrDefault();
                kt.tenKT = tenKT;
                kt.ngaySinh = ngaySinh;
                kt.gioiTinh = gioiTinh;
                kt.diaChi = diaChi;
                kt.dienThoai = dienThoai;
                kt.CCCD = CCCD;
                kt.maPhong = SelectedPhong.maPhong;
                DataProvider.Ins.db.SaveChanges();
                System.Windows.MessageBox.Show("Cập nhật khách thuê thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (p is Window window)
                {
                    window.Close();
                }

            });
            DeleteKhachThueCommand = new RelayCommand<object>(
                (p) => SelectedItem != null,
                (p) =>
                {
                    var result = System.Windows.MessageBox.Show("Bạn có chắc chắn muốn xóa khách thuê này không?", "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                        return;
                    var kt = DataProvider.Ins.db.KhachThues.FirstOrDefault(x => x.maKT == SelectedItem.makt);

                    if (kt != null)
                    {
                        DataProvider.Ins.db.KhachThues.Remove(kt);
                        DataProvider.Ins.db.SaveChanges();

                        System.Windows.MessageBox.Show("Xóa khách thuê thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Load lại danh sách nếu bạn có ListDichVu
                        LoadList();
                    }
                }
            );
            ExportExcelCommand = new RelayCommand<object>((p) => List != null && List.Count > 0, (p) => ExportToExcel());

        }
        private void ExportToExcel()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog()
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = "DanhSachKhachThue.xlsx"
                };

                if (dialog.ShowDialog() == true)
                {
                    ExcelPackage.License.SetNonCommercialPersonal("Tên của bạn");

                    using (var package = new ExcelPackage())
                    {
                        var ws = package.Workbook.Worksheets.Add("KhachThue");

                        ws.Cells[1, 1].Value = "Mã Khách Thuê";
                        ws.Cells[1, 2].Value = "Tên Khách Thuê";
                        ws.Cells[1, 3].Value = "Ngày Sinh";
                        ws.Cells[1, 4].Value = "Giới Tính";
                        ws.Cells[1, 5].Value = "Địa Chỉ";
                        ws.Cells[1, 6].Value = "Điện Thoại";
                        ws.Cells[1, 7].Value = "CCCD";
                        ws.Cells[1, 8].Value = "Tên Phòng";

                        using (var range = ws.Cells[1, 1, 1, 8])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                        for (int i = 0; i < List.Count; i++)
                        {
                            var item = List[i];
                            ws.Cells[i + 2, 1].Value = item.makt;
                            ws.Cells[i + 2, 2].Value = item.tenkt;
                            ws.Cells[i + 2, 3].Value = item.ngaysinh?.ToString("dd/MM/yyyy");
                            ws.Cells[i + 2, 4].Value = item.gioitinh;
                            ws.Cells[i + 2, 5].Value = item.diachi;
                            ws.Cells[i + 2, 6].Value = item.dienthoai;
                            ws.Cells[i + 2, 7].Value = item.cccd;
                            ws.Cells[i + 2, 8].Value = item.tenPhong;
                        }

                        ws.Cells.AutoFitColumns();
                        int totalRows = List.Count + 1;
                        using (var all = ws.Cells[1, 1, totalRows, 8])
                        {
                            all.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            all.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            all.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            all.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }

                        var file = new FileInfo(dialog.FileName);
                        package.SaveAs(file);

                        System.Windows.MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
