using GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModel
{
    public class BaoCaoVM : BaseViewModel
    {
        private ObservableCollection<V_BaoCaoTinhTrang> _ListTinhTrang;
        public ObservableCollection<V_BaoCaoTinhTrang> ListTinhTrang
        {
            get { return _ListTinhTrang; }
            set { _ListTinhTrang = value; OnPropertyChanged(); }
        }
        private ObservableCollection<View_BaoCaoTaiChinh> _ListTaiChinh;
        public ObservableCollection<View_BaoCaoTaiChinh> ListTaiChinh
        {
            get => _ListTaiChinh;
            set { _ListTaiChinh = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Model.Nha> _ListNha;
        public ObservableCollection<Model.Nha> ListNha { get => _ListNha; set { _ListNha = value; OnPropertyChanged(); } }
        private Nha _SelectedNha;
        public Nha SelectedNha
        {
            get => _SelectedNha;
            set { _SelectedNha = value; LoadTaiChinh(); OnPropertyChanged(); }
        }
        private DateTime _SelectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get => _SelectedDate;
            set { _SelectedDate = value; LoadTaiChinh(); OnPropertyChanged(); }
        }

        private void LoadBaoCao()
        {
            ListTinhTrang = new ObservableCollection<V_BaoCaoTinhTrang>();

            var listNha = DataProvider.Ins.db.Nhas.ToList();

            foreach (var nha in listNha)
            {
                var tongPhong = DataProvider.Ins.db.Phongs.Count(p => p.maNha == nha.maNha);
                var phongDangThue = DataProvider.Ins.db.Phongs.Count(p => p.maNha == nha.maNha && p.tinhTrang == "Đang ở");
                var phongDangNo = DataProvider.Ins.db.Phongs.Count(p => p.maNha == nha.maNha && p.tienNo > 0);

                var tongKhach = (from kt in DataProvider.Ins.db.KhachThues
                                 join p in DataProvider.Ins.db.Phongs on kt.maPhong equals p.maPhong
                                 where p.maNha == nha.maNha
                                 select kt).Count();

                decimal tileDangThue = tongPhong == 0 ? 0 : (decimal)phongDangThue / tongPhong * 100;
                decimal tileDangNo = tongPhong == 0 ? 0 : (decimal)phongDangNo / tongPhong * 100;

                ListTinhTrang.Add(new V_BaoCaoTinhTrang()
                {
                    tenNha = nha.tenNha,
                    tongPhong = tongPhong,
                    soPhongDangThue = phongDangThue,
                    tyLePhongDangThue = Math.Round(tileDangThue, 2),
                    soPhongNoTien = phongDangNo,
                    tyLePhongNoTien = Math.Round(tileDangNo, 2),
                    tongKhachThue = tongKhach
                });
            }
        }
        private void LoadTaiChinh()
        {
            if (SelectedNha == null || SelectedDate == null) return;

            string thangNam = SelectedDate.ToString("MM/yyyy");

            var result = DataProvider.Ins.db.View_BaoCaoTaiChinh
                .Where(x => x.maNha == SelectedNha.maNha && x.thangNam == thangNam)
                .ToList();

            ListTaiChinh = new ObservableCollection<View_BaoCaoTaiChinh>(result);
        }

        public BaoCaoVM()
        {
            LoadBaoCao();
            ListNha = new ObservableCollection<Nha>(DataProvider.Ins.db.Nhas.ToList());
            if(ListNha != null && ListNha.Count > 0)
            {
                SelectedNha = ListNha.First();
            }
        }

    }
}
