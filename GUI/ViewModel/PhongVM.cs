using GUI.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GUI.Utilities;
using System.Collections.ObjectModel;
using GUI.Model;
using System.Windows;
using OfficeOpenXml;
using System.IO;

namespace GUI.ViewModel
{
    public class PhongVM : BaseViewModel
    {
        private ObservableCollection<Model.Phong> _List;
        public ObservableCollection<Model.Phong> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        public ICommand AddPhongFormCommand { get; set; }
        public ICommand EditPhongFormCommand { get; set; }
        public ICommand AddPhongCommand { get; set; }
        public ICommand EditPhongCommand { get; set; }
        public ICommand DeletePhongCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        private string _tenPhong;
        private int _giaThue;
        private int _tienNo;
        private int _dienTich;
        private DateTime? _ngayVao;
        private DateTime? _ngayHet;
        private Model.Phong _SelectedItem;
        public Model.Phong SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value; OnPropertyChanged();
                if (SelectedItem != null)
                {
                    tenPhong = SelectedItem.tenPhong;
                    giaThue = (int)SelectedItem.giaThue;
                    tienNo = (int)SelectedItem.tienNo;
                    dienTich = (int)SelectedItem.dienTich;
                    ngayVao = SelectedItem.ngayVao;
                    ngayHet = SelectedItem.ngayHet;
                }
            }
        }
        public string tenPhong { get => _tenPhong; set { _tenPhong = value; OnPropertyChanged(); } }
        public int giaThue { get => _giaThue; set { _giaThue = value; OnPropertyChanged(); } }
        public int tienNo { get => _tienNo; set { _tienNo = value; OnPropertyChanged(); } }
        public int dienTich { get => _dienTich; set { _dienTich = value; OnPropertyChanged(); } }
        public DateTime? ngayVao { get => _ngayVao; set { _ngayVao = value; OnPropertyChanged(); } }
        public DateTime? ngayHet { get => _ngayHet; set { _ngayHet = value; OnPropertyChanged(); } }
        private void LoadList()
        {
            List = new ObservableCollection<Model.Phong>(DataProvider.Ins.db.Phongs.ToList());
        }
        public PhongVM()
        {
            List = new ObservableCollection<Model.Phong>(DataProvider.Ins.db.Phongs);
            AddPhongFormCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddPhong wd = new AddPhong(); wd.ShowDialog(); });
            EditPhongFormCommand = new RelayCommand<object>(
                (p) => SelectedItem != null,
                (p) =>
                {
                    var formEdit = new EditPhong();
                    var editVM = new PhongVM()
                    {
                        tenPhong = SelectedItem.tenPhong,
                        giaThue = (int)SelectedItem.giaThue,
                        tienNo = (int)SelectedItem.tienNo,
                        dienTich = (int)SelectedItem.dienTich,
                        ngayVao = SelectedItem.ngayVao,
                        ngayHet = SelectedItem.ngayHet,
                        SelectedItem = SelectedItem
                    };
                    formEdit.DataContext = editVM;
                    formEdit.ShowDialog();
                }
                );
            AddPhongCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (string.IsNullOrWhiteSpace(tenPhong))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập tên phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if ( giaThue <= 0)
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập giá thuê!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (dienTich <= 0)
                {
                    System.Windows.MessageBox.Show("Diện tích phải lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var phong = new Model.Phong() { tenPhong = tenPhong, giaThue = giaThue, tienNo = 0, dienTich = dienTich, ngayVao = ngayVao, ngayHet = ngayHet, tinhTrang ="Đang Trống" };
                DataProvider.Ins.db.Phongs.Add(phong);
                DataProvider.Ins.db.SaveChanges();
                System.Windows.MessageBox.Show("Thêm phòng thành công");
                if (p is Window window)
                {
                    window.Close();
                }
            }
            );
            EditPhongCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return DataProvider.Ins.db.Phongs.Any(x => x.maPhong == SelectedItem.maPhong);
            }, (p) =>
            {
                if (string.IsNullOrWhiteSpace(tenPhong) || dienTich <=0 || giaThue <= 0)
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var phong = DataProvider.Ins.db.Phongs.Where(x => x.maPhong == SelectedItem.maPhong).SingleOrDefault();
                phong.tenPhong = tenPhong;
                phong.giaThue = giaThue;
                phong.tienNo = tienNo;
                phong.dienTich = dienTich;
                phong.ngayVao = ngayVao;
                phong.ngayHet = ngayHet;
                DataProvider.Ins.db.SaveChanges();
                System.Windows.MessageBox.Show("Cập nhật phòng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (p is Window window)
                {
                    window.Close();
                }
                   
            });
            DeletePhongCommand = new RelayCommand<object>(
                (p) => SelectedItem != null,
                (p) =>
                {
                    var result = System.Windows.MessageBox.Show("Bạn có chắc chắn muốn xóa phòng này không?", "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                        return;
                    var phong = DataProvider.Ins.db.Phongs.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong);

                    if (phong != null)
                    {
                        DataProvider.Ins.db.Phongs.Remove(phong);
                        DataProvider.Ins.db.SaveChanges();

                        System.Windows.MessageBox.Show("Xóa phòng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

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
                    FileName = "DanhSachPhong.xlsx"
                };

                if (dialog.ShowDialog() == true)
                {
                    // Khai báo License EPPlus 8+
                    ExcelPackage.License.SetNonCommercialPersonal("Tên của bạn");

                    using (var package = new ExcelPackage())
                    {
                        var ws = package.Workbook.Worksheets.Add("DanhSachPhong");

                        // Tiêu đề cột
                        ws.Cells[1, 1].Value = "Mã Phòng";
                        ws.Cells[1, 2].Value = "Tên Phòng";
                        ws.Cells[1, 3].Value = "Giá Thuê";
                        ws.Cells[1, 4].Value = "Tiền Nợ";
                        ws.Cells[1, 5].Value = "Diện Tích";
                        ws.Cells[1, 6].Value = "Ngày Vào";
                        ws.Cells[1, 7].Value = "Ngày Hết";
                        ws.Cells[1, 8].Value = "Tình Trạng";

                        using (var range = ws.Cells[1, 1, 1, 8])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        // Ghi dữ liệu từng dòng
                        for (int i = 0; i < List.Count; i++)
                        {
                            var item = List[i];
                            ws.Cells[i + 2, 1].Value = item.maPhong;
                            ws.Cells[i + 2, 2].Value = item.tenPhong;
                            ws.Cells[i + 2, 3].Value = item.giaThue;
                            ws.Cells[i + 2, 4].Value = item.tienNo;
                            ws.Cells[i + 2, 5].Value = item.dienTich;
                            ws.Cells[i + 2, 6].Value = item.ngayVao?.ToString("dd/MM/yyyy");
                            ws.Cells[i + 2, 7].Value = item.ngayHet?.ToString("dd/MM/yyyy");
                            ws.Cells[i + 2, 8].Value = item.tinhTrang;
                        }

                        ws.Cells.AutoFitColumns();

                        // Bo viền toàn bộ bảng
                        int totalRows = List.Count + 1;
                        using (var all = ws.Cells[1, 1, totalRows, 8])
                        {
                            all.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            all.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            all.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            all.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }

                        package.SaveAs(new FileInfo(dialog.FileName));

                        System.Windows.MessageBox.Show("Xuất danh sách phòng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
