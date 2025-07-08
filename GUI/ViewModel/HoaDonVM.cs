using GUI.Model;
using GUI.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static GUI.ViewModel.KhachThueVM;

namespace GUI.ViewModel
{
    public class HoaDonVM : BaseViewModel
    {
        private ObservableCollection<HoaDonFull> _List;
        public ObservableCollection<HoaDonFull> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Phong> _ListPhong;
        public ObservableCollection<Model.Phong> ListPhong { get => _ListPhong; set { _ListPhong = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Nha> _ListNha;
        public ObservableCollection<Model.Nha> ListNha { get => _ListNha; set { _ListNha = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.KhachThue> _ListKhach;
        public ObservableCollection<Model.KhachThue> ListKhach
        {
            get => _ListKhach;
            set { _ListKhach = value; OnPropertyChanged(); }
        }

        public ICommand AddHoaDonFormCommand { get; set; }
        public ICommand EditHoaDonFormCommand { get; set;}
        public ICommand AddHoaDonCommand { get; set; }
        public ICommand EditHoaDonCommand { get; set ; }
        public ICommand DeleteHoaDonCommand{ get; set; }
        public ICommand ThuTienCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public Action OnReload { get; set; }

        private int _tienPhong;
        private int _dien;
        private int _nuoc;
        private int _veSinh;
        private int _internet;
        private int _tong;
        private int _canThu;
        private int _chiSoCuDien;
        private int _chiSoMoiDien;
        private int _chiSoCuNuoc;
        private int _chiSoMoiNuoc;
        private string _tenPhong;
        private DateTime? _ngayLap = DateTime.Now;
        private int _soNguoi;
        private int _soThangInternet = 1;
        private string _tenKT;
        public string tenKT { get => _tenKT; set { _tenKT = value; OnPropertyChanged(); } }
        public string tenPhong { get => _tenPhong; set { _tenPhong = value; OnPropertyChanged(); } }
        public int soThangInternet { get => _soThangInternet; set { _soThangInternet = value; OnPropertyChanged(); TinhTongCong(); } }
        public int soNguoi { get => _soNguoi; set{_soNguoi = value; OnPropertyChanged(); TinhTongCong(); } }
        public DateTime? ngayLap { get => _ngayLap; set { _ngayLap = value; OnPropertyChanged(); } }
        public int tienPhong { get => _tienPhong; set { _tienPhong = value; OnPropertyChanged(); } }
        public int dien { get => _dien; set { _dien = value; OnPropertyChanged(); } }
        public int nuoc { get => _nuoc; set { _nuoc = value; OnPropertyChanged(); } }
        public int veSinh { get => _veSinh; set { _veSinh = value; OnPropertyChanged(); } }
        public int internet { get => _internet; set { _internet = value; OnPropertyChanged(); } }
        public int tong { get => _tong; set { _tong = value; OnPropertyChanged(); } }
        public int canThu { get => _canThu; set { _canThu = value; OnPropertyChanged(); } }
        public int chiSoCuDien { get => _chiSoCuDien; set { _chiSoCuDien = value; OnPropertyChanged(); TinhTongCong(); } }
        public int chiSoMoiDien { get => _chiSoMoiDien; set { _chiSoMoiDien = value; OnPropertyChanged(); TinhTongCong(); } }
        public int chiSoCuNuoc { get => _chiSoCuNuoc; set { _chiSoCuNuoc = value; OnPropertyChanged(); TinhTongCong(); } }
        public int chiSoMoiNuoc { get => _chiSoMoiNuoc; set { _chiSoMoiNuoc = value; OnPropertyChanged(); TinhTongCong(); } }
        private Model.Nha _SelectedNha;
        public Model.Nha SelectedNha
        {
            get => _SelectedNha;
            set
            {
                _SelectedNha = value;
                OnPropertyChanged();
                LoadHoaDonTheoNha();
                LoadPhongDangOTheoNha();
            }
        }
        private Model.KhachThue _SelectedKhach;
        public Model.KhachThue SelectedKhach
        {
            get => _SelectedKhach;
            set { _SelectedKhach = value; OnPropertyChanged(); }
        }
        private Model.Phong _SelectedPhong;
        public Model.Phong SelectedPhong
        {
            get => _SelectedPhong;
            set
            {
                _SelectedPhong = value;
                OnPropertyChanged();
                if (SelectedPhong != null)
                {
                    tienPhong = SelectedPhong.giaThue ?? 0;
                    LoadKhachTheoPhong();
                    LoadChiSoCu();
                    soNguoi = LaySoNguoiTrongPhong();
                    TinhTongCong();
                }
                else
                {
                    tienPhong = 0;
                    ListKhach = new ObservableCollection<Model.KhachThue>();
                }
            }
        }
        private HoaDonFull _SelectedItem;
        public HoaDonFull SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    tenPhong = SelectedItem.tenPhong;
                    tong = SelectedItem.tong;
                    ngayLap = SelectedItem.ngayLap;
                    SelectedPhong = ListPhong.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong);
                    SelectedKhach = ListKhach.FirstOrDefault(x => x.maKT == SelectedItem.maKT);
                    var phong = DataProvider.Ins.db.Phongs.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong);
                    tienPhong = phong?.giaThue ?? 0;
                    maHD = SelectedItem.maHD;
                    var hoaDon = DataProvider.Ins.db.HoaDons.FirstOrDefault(x => x.maHD == maHD);
                }
            }
        }
        private void LoadChiSoCu()
        {
            // Lấy mã dịch vụ trước
            int maDien = LayMaDichVu("Điện");
            int maNuoc = LayMaDichVu("Nước");

            // Sau đó truy vấn sử dụng biến số nguyên đã có
            var dien = DataProvider.Ins.db.DichVuPhongs.FirstOrDefault(x => x.maPhong == SelectedPhong.maPhong && x.maDV == maDien);
            chiSoCuDien = (int)(dien != null ? dien.chiSo : 0);

            var nuoc = DataProvider.Ins.db.DichVuPhongs.FirstOrDefault(x => x.maPhong == SelectedPhong.maPhong && x.maDV == maNuoc);
            chiSoCuNuoc = (int)(nuoc != null ? nuoc.chiSo : 0);
        }
        private void LoadKhachTheoPhong()
        {
            if (SelectedPhong != null)
            {
                ListKhach = new ObservableCollection<Model.KhachThue>(
                    DataProvider.Ins.db.KhachThues.Where(x => x.maPhong == SelectedPhong.maPhong)
                );
            }
        }
        private void LoadPhongDangOTheoNha()
        {
            if (SelectedNha == null)
            {
                ListPhong = new ObservableCollection<Model.Phong>();
                return;
            }

            ListPhong = new ObservableCollection<Model.Phong>(
                DataProvider.Ins.db.Phongs.Where(p =>
                    p.maNha == SelectedNha.maNha &&
                    p.tinhTrang.ToLower() == "đang ở"
                ).ToList()
            );
        }
        private bool ThemHoaDon()
        {
            if (SelectedPhong == null)
            {
                System.Windows.MessageBox.Show("Vui lòng chọn phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (SelectedKhach == null)
            {
                System.Windows.MessageBox.Show("Vui lòng chọn khách thuê!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var hoaDon = new Model.HoaDon
            {
                maPhong = SelectedPhong.maPhong,
                maKT = SelectedKhach.maKT,
                tong = tong,
                canThu = canThu,
                trangThai = "Chưa thu",
                ngayLap = ngayLap
            };

            DataProvider.Ins.db.HoaDons.Add(hoaDon);
            DataProvider.Ins.db.SaveChanges();

            // Thêm Chi Tiết Hóa Đơn
            ThemChiTietHoaDon(hoaDon.maHD, "Điện", chiSoMoiDien - chiSoCuDien);
            ThemChiTietHoaDon(hoaDon.maHD, "Nước", chiSoMoiNuoc - chiSoCuNuoc);
            ThemChiTietHoaDon(hoaDon.maHD, "Vệ Sinh", LaySoNguoiTrongPhong());
            ThemChiTietHoaDon(hoaDon.maHD, "Internet", soThangInternet);

            // Cập nhật chỉ số mới vào Dịch Vụ Phòng
            CapNhatChiSoDichVuPhong("Điện", chiSoMoiDien);
            CapNhatChiSoDichVuPhong("Nước", chiSoMoiNuoc);

            // Cập nhật tiền nợ của phòng
            var phong = DataProvider.Ins.db.Phongs.SingleOrDefault(p => p.maPhong == SelectedPhong.maPhong);
            if (phong != null)
            {
                phong.tienNo = canThu;
            }
            DataProvider.Ins.db.SaveChanges();
            CapNhatTienNoCuaPhong(hoaDon.maPhong);
            System.Windows.MessageBox.Show("Tạo hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        private void CapNhatChiSoDichVuPhong(string tenDV, int chiSoMoi)
        {
            int maDV = LayMaDichVu(tenDV);
            var dvp = DataProvider.Ins.db.DichVuPhongs.FirstOrDefault(x => x.maPhong == SelectedPhong.maPhong && x.maDV == maDV);

            if (dvp != null)
            {
                dvp.chiSo = chiSoMoi;
            }
            else
            {
                // Nếu chưa có thì thêm mới
                dvp = new Model.DichVuPhong
                {
                    maPhong = SelectedPhong.maPhong,
                    maDV = maDV,
                    chiSo = chiSoMoi
                };
                DataProvider.Ins.db.DichVuPhongs.Add(dvp);
            }
        }
        private void ThemChiTietHoaDon(int maHD, string tenDV, int soLuong)
        {
            var dv = DataProvider.Ins.db.DichVus.FirstOrDefault(x => x.tenDV == tenDV);
            if (dv == null) return;

            var ct = new Model.ChiTietHoaDon
            {
                maHD = maHD,
                maDV = dv.maDV,
                soLuong = soLuong,
                donGia = dv.donGia
            };

            DataProvider.Ins.db.ChiTietHoaDons.Add(ct);
        }
        private void CapNhatChiTietHoaDon(int maHD, string tenDV, int soLuong)
        {
            int maDV = LayMaDichVu(tenDV);
            var chiTiet = DataProvider.Ins.db.ChiTietHoaDons.FirstOrDefault(x => x.maHD == maHD && x.maDV == maDV);

            if (chiTiet != null)
            {
                chiTiet.soLuong = soLuong;
                chiTiet.donGia = LayDonGia(tenDV);
            }
            else
            {
                // Nếu chưa có thì thêm mới
                var ct = new Model.ChiTietHoaDon()
                {
                    maHD = maHD,
                    maDV = maDV,
                    soLuong = soLuong,
                    donGia = LayDonGia(tenDV)
                };
                DataProvider.Ins.db.ChiTietHoaDons.Add(ct);
            }
        }

        private int LayMaDichVu(string tenDV)
        {
            var dv = DataProvider.Ins.db.DichVus.FirstOrDefault(x => x.tenDV == tenDV);
            return (int)(dv != null ? dv.maDV : 0);
        }

        private int LayDonGia(string tenDV)
        {
            var dv = DataProvider.Ins.db.DichVus.FirstOrDefault(x => x.tenDV == tenDV);
            return dv?.donGia ?? 0;
        }
        private int LaySoNguoiTrongPhong()
        {
            if (SelectedPhong == null) return 0;
            return DataProvider.Ins.db.KhachThues.Count(x => x.maPhong == SelectedPhong.maPhong);
        }
        private void TinhTongCong()
        {
            dien = (chiSoMoiDien - chiSoCuDien) * LayDonGia("Điện");
            nuoc = (chiSoMoiNuoc - chiSoCuNuoc) * LayDonGia("Nước");
            veSinh = LaySoNguoiTrongPhong() * LayDonGia("Vệ Sinh");
            internet = soThangInternet * LayDonGia("Internet");

            tong = tienPhong + dien + nuoc + veSinh + internet;
            canThu = tong;
            OnPropertyChanged(nameof(dien));
            OnPropertyChanged(nameof(nuoc));
            OnPropertyChanged(nameof(veSinh));
            OnPropertyChanged(nameof(internet));
            OnPropertyChanged(nameof(tong));
            OnPropertyChanged(nameof(canThu));
        }
        private void CapNhatTienNoCuaPhong(int maPhong)
        {
            var phong = DataProvider.Ins.db.Phongs.FirstOrDefault(p => p.maPhong == maPhong);
            if (phong != null)
            {
                // Tính tổng các hóa đơn chưa thu xong của phòng
                var tongCanThu = DataProvider.Ins.db.HoaDons
                    .Where(hd => hd.maPhong == maPhong && hd.trangThai != "Đã thu xong")
                    .Sum(hd => hd.canThu) ?? 0;

                phong.tienNo = tongCanThu;
                DataProvider.Ins.db.SaveChanges();
            }  
        }
        private void LoadHoaDonTheoNha()
        {
            if (SelectedNha == null) return;

            var lst = new ObservableCollection<HoaDonFull>();

            var hoaDons = DataProvider.Ins.db.HoaDons.Where(hd =>
                DataProvider.Ins.db.Phongs.Any(p => p.maPhong == hd.maPhong && p.maNha == SelectedNha.maNha)
            ).ToList();

            foreach (var hd in hoaDons)
            {
                var phong = DataProvider.Ins.db.Phongs.FirstOrDefault(p => p.maPhong == hd.maPhong);
                int tienDien = 0, tienNuoc = 0, veSinh = 0, internet = 0, tienPhong = phong.giaThue ?? 0;

                var chiTiets = DataProvider.Ins.db.ChiTietHoaDons.Where(ct => ct.maHD == hd.maHD).ToList();

                foreach (var ct in chiTiets)
                {
                    var dichVu = DataProvider.Ins.db.DichVus.FirstOrDefault(dv => dv.maDV == ct.maDV);
                    if (dichVu == null) continue;

                    var tien = (ct.donGia ?? 0) * (ct.soLuong ?? 0);

                    switch (dichVu.tenDV.ToLower())
                    {
                        case "điện":
                            tienDien += tien;
                            break;
                        case "nước":
                            tienNuoc += tien;
                            break;
                        case "vệ sinh":
                            veSinh += tien;
                            break;
                        case "internet":
                            internet += tien;
                            break;
                    }
                }

                lst.Add(new HoaDonFull
                {
                    maHD = hd.maHD,
                    tenPhong = phong.tenPhong,
                    ngayLap = ngayLap,
                    tienPhong = tienPhong,
                    tienDien = tienDien,
                    tienNuoc = tienNuoc,
                    veSinh = veSinh,
                    internet = internet,
                    tong = (int)hd.tong,
                    canThu = hd.canThu ?? 0,
                    trangThai = hd.trangThai,
                    maKT = hd.maKT,
                    maPhong = hd.maPhong
                });
            }

            List = lst;
        }
        public class HoaDonFull
        {
            public int maHD { get; set; }
            public int maPhong { get; set; }
            public int maKT { get; set; }
            public string tenPhong { get; set; }
            public int tienPhong { get; set; }
            public int tienDien { get; set; }
            public int tienNuoc { get; set; }
            public int veSinh { get; set; }
            public int internet { get; set; }
            public int tong { get; set; }
            public int canThu { get; set; }
            public string trangThai { get; set; }
            public DateTime? ngayLap { get; set; }
            public int chiSoCuDien { get; set; }
            public int chiSoMoiDien { get; set; }
            public int chiSoCuNuoc { get; set; }
            public int chiSoMoiNuoc { get; set; }
            public int soNguoi { get; set; }
            public int soThangInternet { get; set; }
        }
        private int _maHD;
        public int maHD { get => _maHD; set { _maHD = value; OnPropertyChanged(); } }
        public HoaDonVM()
        {
            ListPhong = new ObservableCollection<Model.Phong>(DataProvider.Ins.db.Phongs.ToList());
            ListNha = new ObservableCollection<Model.Nha>(DataProvider.Ins.db.Nhas);
            if (ListNha != null && ListNha.Count > 0)
            {
                SelectedNha = ListNha.First();
            }
            AddHoaDonFormCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddHoaDon wd = new AddHoaDon(); HoaDonVM vm = new HoaDonVM(); vm.OnReload = LoadHoaDonTheoNha; wd.DataContext = vm; wd.ShowDialog(); });
            EditHoaDonFormCommand = new RelayCommand<object>((p) =>
            {
                return SelectedItem != null && SelectedItem.trangThai != "Đã thu xong";
            },
            (p) =>
            {
                var formEdit = new EditHoaDon();
                var editVM = new HoaDonVM();

                // Load danh sách phòng theo nhà
                editVM.ListNha = ListNha;
                editVM.SelectedNha = this.SelectedNha;
                editVM.ListPhong = new ObservableCollection<Model.Phong>(DataProvider.Ins.db.Phongs.Where(x => x.maNha == SelectedNha.maNha));

                // Load khách thuê theo phòng
                editVM.ListKhach = new ObservableCollection<Model.KhachThue>(DataProvider.Ins.db.KhachThues.Where(x => x.maPhong == SelectedItem.maPhong));

                // Chọn phòng, khách
                editVM.SelectedPhong = editVM.ListPhong.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong);
                editVM.SelectedKhach = editVM.ListKhach.FirstOrDefault(x => x.maKT == SelectedItem.maKT);

                // Lấy ngày lập hóa đơn
                editVM.ngayLap = SelectedItem.ngayLap;

                // Hiển thị tên phòng
                editVM.tenPhong = SelectedItem.tenPhong;
                editVM.maHD = SelectedItem.maHD;

                // Lấy tiền phòng chính xác từ bảng Phòng
                var phong = DataProvider.Ins.db.Phongs.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong);
                editVM.tienPhong = phong?.giaThue ?? 0;

                // Tổng, cần thu
                editVM.tong = SelectedItem.tong;
                editVM.canThu = SelectedItem.canThu;
                SelectedItem = SelectedItem;
                // Truy vấn chỉ số cũ/mới điện nước
                int maDien = LayMaDichVu("Điện");
                int maNuoc = LayMaDichVu("Nước");

                var dienPhong = DataProvider.Ins.db.DichVuPhongs.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong && x.maDV == maDien);
                var nuocPhong = DataProvider.Ins.db.DichVuPhongs.FirstOrDefault(x => x.maPhong == SelectedItem.maPhong && x.maDV == maNuoc);

                editVM.chiSoMoiDien = dienPhong?.chiSo ?? 0;
                editVM.chiSoMoiNuoc = nuocPhong?.chiSo ?? 0;

                var chiTietDien = DataProvider.Ins.db.ChiTietHoaDons.FirstOrDefault(x => x.maHD == SelectedItem.maHD && x.maDV == maDien);
                var chiTietNuoc = DataProvider.Ins.db.ChiTietHoaDons.FirstOrDefault(x => x.maHD == SelectedItem.maHD && x.maDV == maNuoc);

                if (chiTietDien != null)
                    editVM.chiSoCuDien = editVM.chiSoMoiDien - (chiTietDien.soLuong ?? 0);

                if (chiTietNuoc != null)
                    editVM.chiSoCuNuoc = editVM.chiSoMoiNuoc - (chiTietNuoc.soLuong ?? 0);

                // Lấy số người, tháng internet
                editVM.soNguoi = DataProvider.Ins.db.KhachThues.Count(x => x.maPhong == SelectedItem.maPhong);
                editVM.soThangInternet = 1;
                editVM.OnReload = LoadHoaDonTheoNha;
                formEdit.DataContext = editVM;
                formEdit.ShowDialog();
            });

            AddHoaDonCommand = new RelayCommand<object>(
            (p) =>
            {
                return SelectedPhong != null &&
                       !string.IsNullOrWhiteSpace(SelectedPhong.tinhTrang) &&
                       SelectedPhong.tinhTrang.ToLower() == "đang ở" &&
                       canThu > 0;
            },
            (p) =>
            {
                bool thanhCong = ThemHoaDon();
                if (thanhCong)
                {
                    OnReload?.Invoke();
                    if (p is Window wd) wd.Close();
                }
            }
            );
            EditHoaDonCommand = new RelayCommand<object>(
            (p) =>
            {   
                return SelectedPhong != null && SelectedKhach != null && canThu > 0;
            },
            (p) =>
            {
                var hoaDonedit = DataProvider.Ins.db.HoaDons.Where(x => x.maHD == maHD).SingleOrDefault();
                if (hoaDonedit == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Cập nhật thông tin hóa đơn
                hoaDonedit.maPhong = SelectedPhong.maPhong;
                hoaDonedit.maKT = SelectedKhach.maKT;
                hoaDonedit.ngayLap = ngayLap;
                hoaDonedit.tong = tong;
                hoaDonedit.canThu = canThu;

                // Cập nhật các chỉ số dịch vụ
                CapNhatChiSoDichVuPhong("Điện", chiSoMoiDien);
                CapNhatChiSoDichVuPhong("Nước", chiSoMoiNuoc);

                // Cập nhật Chi Tiết Hóa Đơn
                CapNhatChiTietHoaDon(hoaDonedit.maHD, "Điện", chiSoMoiDien - chiSoCuDien);
                CapNhatChiTietHoaDon(hoaDonedit.maHD, "Nước", chiSoMoiNuoc - chiSoCuNuoc);
                CapNhatChiTietHoaDon(hoaDonedit.maHD, "Vệ Sinh", soNguoi);
                CapNhatChiTietHoaDon(hoaDonedit.maHD, "Internet", soThangInternet);

                // Tính lại tiền nợ phòng
                var phong = DataProvider.Ins.db.Phongs.FirstOrDefault(x => x.maPhong == SelectedPhong.maPhong);
                DataProvider.Ins.db.SaveChanges();
                CapNhatTienNoCuaPhong(hoaDonedit.maPhong);
                MessageBox.Show("Cập nhật hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                OnReload?.Invoke();
                if (p is Window wd) wd.Close();
            }
            );

            DeleteHoaDonCommand = new RelayCommand<object>(
            (p) =>
            {
                return SelectedItem != null; // Chỉ cho phép xóa khi có item được chọn
            },
            (p) =>
            {
                var result = MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    // Xóa chi tiết hóa đơn trước (nếu có)
                    var chiTietList = DataProvider.Ins.db.ChiTietHoaDons.Where(x => x.maHD == SelectedItem.maHD).ToList();
                    foreach (var ct in chiTietList)
                    {
                        DataProvider.Ins.db.ChiTietHoaDons.Remove(ct);
                    }

                    // Xóa hóa đơn
                    var hoaDon = DataProvider.Ins.db.HoaDons.FirstOrDefault(hd => hd.maHD == SelectedItem.maHD);
                    if (hoaDon != null)
                    {
                        DataProvider.Ins.db.HoaDons.Remove(hoaDon);
                    }

                    DataProvider.Ins.db.SaveChanges();
                    MessageBox.Show("Xóa hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Load lại danh sách
                    LoadHoaDonTheoNha();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa hóa đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            );
            ThuTienCommand = new RelayCommand<object>(
            (param) =>
            {
                return SelectedItem != null && SelectedItem.trangThai != "Đã thu xong";
            },
            (param) =>
            {
                var result = MessageBox.Show($"Bạn xác nhận đã thu đủ số tiền {SelectedItem.canThu:N0} VNĐ chưa?", "Xác nhận thu tiền", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    var hoaDon = DataProvider.Ins.db.HoaDons.FirstOrDefault(hd => hd.maHD == SelectedItem.maHD);
                    if (hoaDon == null)
                    {
                        MessageBox.Show("Không tìm thấy hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Cập nhật trạng thái hóa đơn
                    hoaDon.trangThai = "Đã thu xong";
                    hoaDon.canThu = 0;

                    // Cập nhật tiền nợ của phòng
                    var phong = DataProvider.Ins.db.Phongs.FirstOrDefault(ph => ph.maPhong == hoaDon.maPhong); // Đổi tên thành 'ph' hoặc tên khác

                    if (phong != null)
                    {
                        phong.tienNo -= hoaDon.canThu;
                        if (phong.tienNo < 0) phong.tienNo = 0;
                    }

                    DataProvider.Ins.db.SaveChanges();
                    CapNhatTienNoCuaPhong(hoaDon.maPhong);
                    MessageBox.Show("Thu tiền thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadHoaDonTheoNha();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thu tiền: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            );
            PrintCommand = new RelayCommand<object>((p) =>
            {
                return SelectedItem != null;
            },
            (p) =>
            {
                var hoaDonPrintVM = new HoaDonVM
                {
                    tenPhong = SelectedItem.tenPhong,
                    ngayLap = SelectedItem.ngayLap,
                    tienPhong = SelectedItem.tienPhong,
                    dien = SelectedItem.tienDien,
                    nuoc = SelectedItem.tienNuoc,
                    veSinh = SelectedItem.veSinh,
                    internet = SelectedItem.internet,
                    tong = SelectedItem.tong
                };
                var khach = DataProvider.Ins.db.KhachThues.FirstOrDefault(x => x.maKT == SelectedItem.maKT);
                hoaDonPrintVM.tenKT = khach?.tenKT ?? "Chưa rõ";
                var hoaDonUC = new HoaDonPrintUC();
                hoaDonUC.DataContext = hoaDonPrintVM;

                PrintDialog printDlg = new PrintDialog();
                if (printDlg.ShowDialog() == true)
                {
                    // Kích thước in tương đối
                    hoaDonUC.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    hoaDonUC.Arrange(new Rect(0, 0, hoaDonUC.DesiredSize.Width, hoaDonUC.DesiredSize.Height));
                    printDlg.PrintVisual(hoaDonUC, "Hóa Đơn Thanh Toán");
                }
            });
        }
    } 
}
