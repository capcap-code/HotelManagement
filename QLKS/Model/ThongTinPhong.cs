using QLKS.ViewModel;

namespace QLKS.Model
{
    public class ThongTinPhong : BaseViewModel
    {
        public PHONG Phong { get; set; }
        private LOAIPHONG _LoaiPhong;
        public LOAIPHONG LoaiPhong { get => _LoaiPhong; set { _LoaiPhong = value; OnPropertyChanged(); } }
    }
}
