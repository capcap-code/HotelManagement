﻿using QLKS.ViewModel;

namespace QLKS.Model
{
    public class ThongTinOrder : BaseViewModel
    {
        public MATHANG MatHang { get; set; }
        private int _SoLuong;
        public int SoLuong { get => _SoLuong; set { _SoLuong = value; OnPropertyChanged(); } }
        private int _ThanhTien;
        public int ThanhTien { get => _ThanhTien; set { _ThanhTien = value; OnPropertyChanged(); } }

        public ThongTinOrder()
        {
            MatHang = new MATHANG();
        }
    }
}
