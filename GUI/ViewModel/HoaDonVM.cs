using GUI.Model;
using GUI.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private ObservableCollection<Model.ChiTietHoaDon> _ListCTHD;
        public ObservableCollection<Model.ChiTietHoaDon> ListCTHD { get => _ListCTHD; set { _ListCTHD = value; OnPropertyChanged(); } }
        public ICommand AddHoaDonFormCommand { get; set; }
        public ICommand EditHoaDonFormCommand { get; set;}
        public ICommand AddHoaDonCommand { get; set; }
        public ICommand EditHoaDonCommand { get; set ; }
        public ICommand DeleteHoaDonCommand{ get; set; }
        public ICommand ThuTienCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        private int _maHD;
        private int _maPhong;
        private int _maKT;
        private int _tong;
        private int _canThu;
        public class HoaDonFull
        {
            public int maHD { get; set; }
            public string tenPhong { get; set; }
            public int tienPhong { get; set; }
            public int tienDien { get; set; }
            public int tienNuoc { get; set; }
            public int veSinh { get; set; }
            public int internet { get; set; }
            public int tong { get; set; }
            public int canThu { get; set; }
            public string trangThai { get; set; }
        }

        public HoaDonVM()
        {
            ListPhong = new ObservableCollection<Model.Phong>(DataProvider.Ins.db.Phongs.ToList());
            List = new ObservableCollection<HoaDonFull>(
            from hd in DataProvider.Ins.db.HoaDons
            join p in DataProvider.Ins.db.Phongs on hd.maPhong equals p.maPhong
            select new HoaDonFull
            {
                maHD = hd.maHD,
                tenPhong = p.tenPhong,
                tienPhong = p.giaThue ?? 0,
                tienDien = (
                from ct in DataProvider.Ins.db.ChiTietHoaDons
                join dv in DataProvider.Ins.db.DichVus on ct.maDV equals dv.maDV
                where ct.maHD == hd.maHD && dv.tenDV == "Điện"
                select ct.thanhTien ?? 0
            ).FirstOrDefault(),

                tienNuoc = (
                from ct in DataProvider.Ins.db.ChiTietHoaDons
                join dv in DataProvider.Ins.db.DichVus on ct.maDV equals dv.maDV
                where ct.maHD == hd.maHD && dv.tenDV == "Nước"
                select ct.thanhTien ?? 0
            ).FirstOrDefault(),

                veSinh = (
                from ct in DataProvider.Ins.db.ChiTietHoaDons
                join dv in DataProvider.Ins.db.DichVus on ct.maDV equals dv.maDV
                where ct.maHD == hd.maHD && dv.tenDV == "Vệ Sinh"
                select ct.thanhTien ?? 0
            ).FirstOrDefault(),

                internet = (
                from ct in DataProvider.Ins.db.ChiTietHoaDons
                join dv in DataProvider.Ins.db.DichVus on ct.maDV equals dv.maDV
                where ct.maHD == hd.maHD && dv.tenDV == "Internet"
                select ct.thanhTien ?? 0
            ).FirstOrDefault(),

                tong = hd.tong ?? 0,
                canThu = hd.canThu ?? 0,
                trangThai = hd.trangThai
            }
            );
            AddHoaDonFormCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddHoaDon wd = new AddHoaDon(); wd.ShowDialog(); });

        }
    }
    
}
