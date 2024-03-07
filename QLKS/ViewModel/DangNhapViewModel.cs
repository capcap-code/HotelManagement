using QLKS.Model;
using SKM.V3;
using SKM.V3.Methods;
using SKM.V3.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QLKS.ViewModel
{
    public class DangNhapViewModel : BaseViewModel
    {
        private string _TenDangNhap;
        public string TenDangNhap { get => _TenDangNhap; set { _TenDangNhap = value; OnPropertyChanged(); } }
        private string _MatKhau;
        public string MatKhau { get => _MatKhau; set { _MatKhau = value; OnPropertyChanged(); } }
        private bool _IsValidationSuccessful;
        public bool IsValidationSuccessful { get => _IsValidationSuccessful; set { _IsValidationSuccessful = value; OnPropertyChanged(); } }
        public NHANVIEN NVDangNhap { get; set; }

        public ICommand DangNhapCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand ValidateCommand { get; set; }

        public DangNhapViewModel()
        {
            TenDangNhap = "";
            MatKhau = "";
            IsValidationSuccessful = false;

            DangNhapCommand = new RelayCommand<Window>((p) => { return p == null ? false : true; }, (p) => { DangNhap(p); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return p == null ? false : true; }, (p) => { MatKhau = p.Password; });
            ValidateCommand = new RelayCommand<string>(
                // CanExecute delegate: Enable the command when IsValidationSuccessful is false
                (licenseKey) => { return !IsValidationSuccessful; },
                // Execute delegate
                (licenseKey) => { Validate(licenseKey); }
            );

        }

        void DangNhap(Window p)
        {
            var taiKhoan = DataProvider.Ins.model.TAIKHOAN.Where(x => x.TENDANGNHAP_TK == TenDangNhap && x.MATKHAU_TK == MatKhau);
            if (taiKhoan.Count() > 0)
            {
                IsValidationSuccessful = true;
                int maTKDangNhap = taiKhoan.SingleOrDefault().MA_TK;
                NVDangNhap = DataProvider.Ins.model.NHANVIEN.Where(x => x.MA_TK == maTKDangNhap).SingleOrDefault();
            }
            else
            {
                IsValidationSuccessful = false;
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (IsValidationSuccessful)
            {
                p.Hide();
                MainWindow mainWindow = new MainWindow();
                if (mainWindow.DataContext == null)
                    return;
                var mainVM = mainWindow.DataContext as MainViewModel;
                mainVM.ChucNangKS = (int)MainViewModel.ChucNangKhachSan.TrangChu;
                mainVM.NhanVien = NVDangNhap;
                mainWindow.ShowDialog();
                p.Close();
            }
        }

        void Validate(string licenseKey)
        {
            var RSAPubKey = "<RSAKeyValue><Modulus>wWWWLy7xypS0CoNaTd+K1+tsifh0xdBkT98sSSha+b2IL/4zZPdPZeFhbM2kL7ZMwAF6nHFnRIOwASXycmEUJ7BGcdXjdYow3bTelZpeDdY6hAxaryl0Fd4tR/kaGkZcZ3vyWbC8frAko73tt+oir6H9xjM/mcuW42OfMLT/oAA30iUW46qGnbAOfwhrTWIDHK7fPX61ZAUKbvvYEAUm+0yAADfcm2/fkWL9olzTZRrwGvmOURlPdO3bNpCYXeWJpx+gX3mMDeVmc87iebDAZHCI9soGlwsWrzE63S18F9qYNYuX4inBMLktFFawteCDib5qdk7yeRcgbJid7KUqnQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            var auth = "WyI3NTQzNDg0OSIsIjAxdk1UeTRXd2FqRTVWSmo1ajVLVWlwTmcwaXhtWGFHWnIvRDRld3oiXQ==";
            var result = SKM.V3.Methods.Key.Activate(token: auth, parameters: new ActivateModel()
            {
                Key = licenseKey,
                ProductId = 24214,  // <--  remember to change this to your Product Id
                Sign = true,
                MachineCode = Helpers.GetMachineCodePI(v: 2)
            });

            if (result == null || result.Result == SKM.V3.Models.ResultType.Error ||
                !result.LicenseKey.HasValidSignature(RSAPubKey).IsValid())
            {
                // an error occurred or the key is invalid or it cannot be activated
                // (eg. the limit of activated devices was achieved)
                Console.WriteLine("The license does not work.");
                IsValidationSuccessful = false; // Set IsValidationSuccessful to false
            }
            else
            {
                // everything went fine if we are here!
                Console.WriteLine("The license is valid!");
                IsValidationSuccessful = true; // Set IsValidationSuccessful to true
            }

            Console.ReadLine();
        }

    }
}
