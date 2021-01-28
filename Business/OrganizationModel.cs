using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIS.Intranet.Data;
using System.IdentityModel.Metadata;
using AIS.Intranet.Business.Logic;


namespace AIS.Intranet.Business.Core
{
    public class OrganizationClient
    {
        public string ApplicationID { get; set; }
        public string OrganizationID { get; set; }
        public string ParentOrganizationID { get; set; }
        public string ParentName { get; set; }
        public string OrganizationName { get; set; }
        public string Type { get; set; }
        public string CreatedByUserID { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string LastModifiedByUserID { get; set; }
        public DateTime LastModifiedOnDate { get; set; }
        public bool DeleteSuccess { get; set; }
        public string Description { get; set; }
        public string ChucVu { get; set; }
        public int? STT { get; set; }
        public string SoDienThoai { get; set; }
        public string SoFax { get; set; }
        public string Email { get; set; }
        public Guid? NguoiDaiDien { get; set; }
        public string TenNguoiDaiDien { get; set; }
        public string DiaChi { get; set; }
        public int? Loai { get; set; }
        public IList<OrganizationClient> LstDonVi { get; set; }
        public IList<TaskClient> ListCTCT { get; set; }
        public IList<UserInOrganizationModel> LstUser { get; set; }
        public bool IsExtent { get; set; }
        public string ShortName { get; set; }
    }
    public enum OrganizationQueryOder
    {
        Ngay_Tao_DESC,
        Ngay_Tao_ASC,
        ID_DESC,
        ID_ASC
    }
    public class OrganizationQueryFilter
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public string ApplicationID { get; set; }

        public string OrganizationID { get; set; }

        public string ParentOrganizationID { get; set; }

        public string OrganizationName { get; set; }

        public System.Guid CreatedByUserID { get; set; }

        public Nullable<int> SoLuongPhong { get; set; }

        public System.DateTime CreatedOnDate { get; set; }

        public System.Guid LastModifiedByUserID { get; set; }

        public System.DateTime LastModifiedOnDate { get; set; }

        public string Description { get; set; }

        public List<Organization> subOrganization { get; set; }

        public IList<aspnet_Roles> role { get; set; }

        public int SoLuongCanBo { get; set; }

        public int? STT { get; set; }
        public string TextSearch { get; set; }
        public string SoDienThoai { get; set; }

        public int? Loai { get; set; }
        public OrganizationQueryOder Order { get; set; }
        public OrganizationQueryFilter()
        {
            TextSearch = string.Empty;

            Order = OrganizationQueryOder.Ngay_Tao_DESC;
        }
    }
    public class UserInOrganizationModel
    {
        public string FullName { get; set; }
        public string EmailUser { get; set; }
        public Nullable<Guid> UserId { get; set; }
        public Nullable<Guid> CanBoId { get; set; }
        public Nullable<int> SoThuTuChucVu { get; set; }
        public Nullable<Guid> OrganizationID { get; set; }
        public string Username { get; set; }
    }
    
}