using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using AIS.Intranet.Business.Config;
using AIS.Intranet.Data;
using AIS.Intranet.Common;
using AIS.Intranet.Business.Core.Membership;

namespace AIS.Intranet.Business.Core
{
    public class OrganizationHandler : IOrganizationHandler
    {
        ILogService logger = BusinessServiceLocator.Instance.GetService<ILogService>();
        private IDatabaseFactory dbFactory = new DatabaseFactory();

        #region  Function Get Organization
        public Response<IList<OrganizationClient>> Get(OrganizationQueryFilter filter)
        {
            try
            {
                var startDate = DateTime.Now;


                var result = new Response<IList<OrganizationClient>>(1, string.Empty, new List<OrganizationClient>());

                using (var unitOfWork = new UnitOfWork())
                {
                    var query = from orga in unitOfWork.GetRepository<aspnet_Organizations>().GetAll()


                                select new { Organization = orga };


                    // Filter text search
                    if (!string.IsNullOrEmpty(filter.TextSearch))
                    {
                        query = from item in query
                                where item.Organization.OrganizationName.ToLower().Contains(filter.TextSearch.ToLower())
                                select item;
                    }

                    // Filter ParentId
                    if (!string.IsNullOrEmpty(filter.ParentOrganizationID))
                    {
                        var parenId = new Guid(filter.ParentOrganizationID);
                        query = from item in query
                                where item.Organization.ParentOrganizationID == parenId
                                select item;
                    }

                    // Filter by applicationId
                    if (!string.IsNullOrEmpty(filter.ApplicationID))
                    {
                        var applicationId = new Guid(filter.ApplicationID);
                        query = from item in query
                                where item.Organization.ApplicationID == applicationId
                                select item;
                    }

                    // Filter title
                    if (!string.IsNullOrEmpty(filter.OrganizationName))
                    {
                        query = from item in query
                                where item.Organization.OrganizationName == filter.OrganizationName
                                select item;
                    }


                    // Filter MenuId
                    if (!string.IsNullOrEmpty(filter.OrganizationID))
                    {
                        var guidOrgaId = new Guid(filter.OrganizationID);

                        query = from item in query
                                where item.Organization.OrganizationID == guidOrgaId
                                select item;
                    }
                    // Filter Order

                    // Filter Position
                    if (filter.Loai.HasValue)
                    {
                        query = from item in query
                                where item.Organization.Loai == filter.Loai
                                select item;
                    }

                    //// Filter list position


                    // Order
                    query = query.OrderBy(item => item.Organization.STT);


                    // Filter page size
                    if (filter.PageSize.HasValue && filter.PageNumber.HasValue)
                    {
                        filter.PageSize += 1;
                        filter.TextSearch = !string.IsNullOrEmpty(filter.TextSearch) ? filter.TextSearch.ToLower().Trim() : null;

                        if (filter.PageSize <= 0) filter.PageSize = 20;

                        // Calculate nunber of rows to skip on pagesize
                        int excludedRows = (filter.PageNumber.Value - 1) * (filter.PageSize.Value - 1);
                        if (excludedRows <= 0) excludedRows = 0;

                        // Query
                        query = query.Skip(excludedRows).Take(filter.PageSize.Value);
                    }

                    // Execute query
                    var OrganizationData = query.ToList();

                    // Add to result
                    if (OrganizationData != null && OrganizationData.Count > 0)
                    {
                        foreach (var organi in OrganizationData)
                        {
                            var orgaModel = ConvertData(organi.Organization);
                            result.Data.Add(orgaModel);
                        }
                    }


                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<IList<OrganizationClient>>(-1, ex.Message, null);
            }
        }
        public Response<IList<OrganizationClient>> GetAllOrGa()
        {
            //var unitOfWork = new UnitOfWork();
            //IEnumerable<aspnet_Organizations> data = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.OrganizationID != null, sp => sp.OrderBy(s => s.STT).ThenBy(s => s.OrganizationName));
            //return new Response<IList<OrganizationClient>>(0, "", ConvertDatasNormal(data));
            var queryFilter = new OrganizationQueryFilter();
            queryFilter.Loai = 1;
            // queryFilter.ApplicationID = applicationId;


            return Get(queryFilter);
        }
        public Response<OrganizationClient> GetById(string id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var cOrga = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.OrganizationID == new Guid(id)).FirstOrDefault();
                    if (cOrga != null)
                        return new Response<OrganizationClient>(1, "", ConvertData(cOrga));
                    else
                        return new Response<OrganizationClient>(0, "data not found", null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<OrganizationClient>(-1, ex.Message, null);
            }
        }
        public Response<IList<OrganizationClient>> GetBySearchResponse(string search)
        {
            var unitOfWork = new UnitOfWork();
            IEnumerable<aspnet_Organizations> data = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.OrganizationID != null, sp => sp.OrderBy(s => s.STT).ThenBy(s => s.OrganizationName)).Where(s => s.OrganizationName.Contains(search));
            return new Response<IList<OrganizationClient>>(0, "", ConvertDatasNormal(data));
        }
        public Response<IList<OrganizationClient>> GetByParentId(string id)
        {
            try
            {
                var unitOfWork = new UnitOfWork();
                IEnumerable<aspnet_Organizations> data = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.ParentOrganizationID == new Guid(id), sp => sp.OrderBy(s => s.STT));
                return new Response<IList<OrganizationClient>>(0, "", ConvertDatasNormal(data));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<IList<OrganizationClient>>(-1, ex.Message, null);
            }
        }

        public string GetParentName(string id)
        {
            try
            {
                var unitOfWork = new UnitOfWork();
                aspnet_Organizations data = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.OrganizationID == new Guid(id), sp => sp.OrderBy(s => s.OrganizationName)).FirstOrDefault();
                if (data != null)
                    return data.OrganizationName;
                return "Không có";
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

        public Response<IList<OrganizationClient>> GetOrganizations()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    IQueryable<aspnet_Organizations> data = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.OrganizationID != null, sp => sp.OrderBy(s => s.STT));
                    if (data != null)
                        return new Response<IList<OrganizationClient>>(1, "", ConvertDatasNormal(data));
                    else
                        return new Response<IList<OrganizationClient>>(0, "data not found", null);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<IList<OrganizationClient>>(-1, ex.Message, null);

            }
        }

        public Response<IList<OrganizationClient>> GetByLoai(int Loai)
        {
            try
            {
                var unitOfWork = new UnitOfWork();

                IEnumerable<aspnet_Organizations> data = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.Loai == Loai, sp => sp.OrderBy(s => s.STT).ThenBy(s => s.OrganizationName));

                return new Response<IList<OrganizationClient>>(0, "", ConvertDatasNormal(data));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<IList<OrganizationClient>>(-1, ex.Message, null);
            }
        }
        public Response<IList<OrganizationClient>> GetUserByUserId(string userId)
        {
            try
            {
                var unitOfWork = new UnitOfWork();
                var uOrganization = unitOfWork.GetRepository<aspnet_UsersInOrganization>().GetMany(sp => sp.UserID == new Guid(userId)).FirstOrDefault();
                IEnumerable<aspnet_Organizations> data = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.OrganizationID == uOrganization.OrganizationID, sp => sp.OrderBy(s => s.STT).ThenBy(s => s.OrganizationName));
                return new Response<IList<OrganizationClient>>(0, "", ConvertDatasNormal(data));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<IList<OrganizationClient>>(-1, ex.Message, null);
            }
        }


        public Response<IList<OrganizationClient>> GetUserByLoai(int Loai)
        {
            try
            {
                var unitOfWork = new UnitOfWork();

                IEnumerable<aspnet_Organizations> data = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.Loai == Loai, sp => sp.OrderBy(s => s.STT).ThenBy(s => s.OrganizationName));

                return new Response<IList<OrganizationClient>>(0, "", ConvertDatasNormal(data));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<IList<OrganizationClient>>(-1, ex.Message, null);
            }
        }

        public IEnumerable<OrganizationClient> GetOrganizationsByType(string type)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(dbFactory))
                {
                    var sOrganizations = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.ApplicationID == Application.ApplicationID && sp.Type == type,
                        sp => sp.OrderBy(s => s.STT), 0, "").ToList();
                    return GetOrganization(sOrganizations);

                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy về Organization theo mã Organization
        /// </summary>
        /// <param name="id">mã Organization</param>        
        /// <remarks>
        /// Mã Organization kiểu unique
        /// </remarks>
        public OrganizationClient GetOrganizationClient(Guid id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(dbFactory))
                {
                    return GetOrganization(unitOfWork.GetRepository<aspnet_Organizations>().GetById(id));
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy về Organization theo mã Organization
        /// </summary>
        /// <param name="id">mã Organization</param>        
        /// <remarks>
        /// Mã Organization kiểu string
        /// </remarks>
        public OrganizationClient GetOrganizationClient(string id)
        {
            try
            {
                return GetOrganizationClient(new Guid(id));
            }
            catch
            {
                return null;
            }
        }
        public IEnumerable<OrganizationClient> GetPageMany(int pageNum, int pageSize, string textSearch)
        {
            try
            {
                if (pageSize <= 0) pageSize = 10;

                pageSize = pageSize + 1;
                textSearch = textSearch != null ? textSearch.ToLower().Trim() : null;

                using (var unitOfWork = new UnitOfWork(dbFactory))
                {
                    IEnumerable<aspnet_Organizations> data;

                    if (string.IsNullOrEmpty(textSearch))
                    {
                        data = unitOfWork.GetRepository<aspnet_Organizations>().GetPageMany(sp => sp.ApplicationID == Application.ApplicationID, sp => sp.OrderBy(s => s.STT), pageNum, pageSize);
                    }
                    else
                    {
                        data = unitOfWork.GetRepository<aspnet_Organizations>().GetPageMany(sp => sp.ApplicationID == Application.ApplicationID && sp.OrganizationName.ToLower().Contains(textSearch), sp => sp.OrderBy(s => s.STT), pageNum, pageSize);
                    }
                    return ConvertDatasNormal(data);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }

        }
        public OrganizationClient GetOrganization(aspnet_Organizations u)
        {
            var org = new OrganizationClient
            {
                OrganizationID = u.OrganizationID.ToString(),
                ApplicationID = u.ApplicationID.ToString(),
                ParentOrganizationID = u.ParentOrganizationID.ToString(),
                OrganizationName = u.OrganizationName,
                Type = u.Type,
                CreatedByUserID = u.CreatedByUserID.ToString(),
                CreatedOnDate = u.CreatedOnDate,
                LastModifiedByUserID = u.LastModifiedByUserID.ToString(),
                LastModifiedOnDate = u.LastModifiedOnDate
            };
            return org;
        }
        public List<OrganizationClient> GetOrganization(IEnumerable<aspnet_Organizations> Organizations)
        {
            List<OrganizationClient> orgs = new List<OrganizationClient>();
            foreach (aspnet_Organizations org in Organizations)
            {
                orgs.Add(GetOrganization(org));
            }
            return orgs;
        }

        #region Function get UserinOrganizations

        public List<UserClient> GetUserInOrgnazition(string id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var CVRepo = unitOfWork.GetRepository<DM_ChucVu>().GetAll();
                    var QTRepo = unitOfWork.GetRepository<CV_CanBo_QTCongTac>().GetAll();
                    var query = (from a in unitOfWork.GetRepository<aspnet_Organizations>().GetAll()
                                 join b in unitOfWork.GetRepository<aspnet_UsersInOrganization>().GetAll()
                                 on a.OrganizationID equals b.OrganizationID
                                 join c in unitOfWork.GetRepository<aspnet_Membership>().GetAll()
                                 on b.UserID equals c.UserId
                                 join f in unitOfWork.GetRepository<aspnet_Users>().GetAll()
                                 on b.UserID equals f.UserId
                                 where
                                 a.OrganizationID == new Guid(id)

                                 select new
                                 {
                                     
                                     UserId = c.UserId,
                                     NickName = c.NickName,
                                     UserName = f.UserName,
                                     OrgaId = a.OrganizationID,
                                     FullName = c.FullName,
                                     Derpt = a.OrganizationName,
                                     ChucVu = b.Role,
                                     Password = c.Password,
                                     IsApproved = c.IsApproved,
                                     DeleteDate = c.DeleteDate,
                                     IsLockedOut = c.IsLockedOut,
                                     Email = c.Email,
                                     Active = c.IsApproved && c.DeleteDate == null && c.IsLockedOut == false,
                                     CreatedDate = c.CreateDate,
                                     CreatedByUserID = c.CreatedByUserID,
                                     LastModifiedOnDate = c.LastModifiedOnDate,
                                     LastModifiedByUserID = c.LastModifiedByUserID,
                                     Order = (from qt in QTRepo
                                              join cv in CVRepo
                                              on qt.ChucVuId equals cv.Id
                                              where qt.CanBoId == c.CanBoId
                                              orderby qt.TuNgay descending
                                              select cv).Take(1).ToList()
                                     //Order = (from qt in CVRepo select qt.ThuTu.Value)
                                 });

                    var queryResults = query.OrderByDescending(sp => sp.ChucVu).ThenBy(sp => sp.UserName).ToList();

                    var sUser = new List <UserClient>();

                    foreach (var u in queryResults)
                    {
                        var newU = new UserClient(){
                            UserId = u.UserId.ToString(),
                            UserName = u.UserName,
                            FullName = u.FullName,
                            NickName = u.NickName,
                            Active = u.Active,
                            Email = u.Email,
                            OrganizationId = u.OrgaId.ToString(),
                            CreatedDate = u.CreatedDate,
                            CreatedBy = u.CreatedByUserID.ToString(),
                            UpdateDate = u.LastModifiedOnDate,
                            UpdateBy = u.LastModifiedByUserID.ToString(),
                            Password = u.Password,
                            ChucVu = u.ChucVu,
                        };

                        try
                        {
                            var x = u.Order;
                            newU.ThuTu = x != null ? x[0].ThuTu.ToString() : "100";
                        }
                        catch (Exception)
                        {
                            newU.ThuTu = "100";
                        }

                        sUser.Add(newU);
                    }

                    List<UserClient> users = sUser;

                    return users;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }


        public List<OrganizationClient> GetOrgnazitionByUser(string userId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var query = (from a in unitOfWork.GetRepository<aspnet_Organizations>().GetAll()
                                 join b in unitOfWork.GetRepository<aspnet_UsersInOrganization>().GetAll()
                                 on a.OrganizationID equals b.OrganizationID
                                 join c in unitOfWork.GetRepository<aspnet_Users>().GetAll()
                                 on b.UserID equals c.UserId
                                 where
                                 c.UserId == new Guid(userId)
                                 select new
                                 {
                                     OrganizationID = a.OrganizationID,
                                     ApplicationID = a.ApplicationID,
                                     ParentOrganizationID = a.ParentOrganizationID,
                                     OrganizationName = a.OrganizationName,
                                     CreatedByUserID = a.CreatedByUserID,
                                     CreatedOnDate = a.CreatedOnDate,
                                     LastModifiedByUserID = a.LastModifiedByUserID,
                                     LastModifiedOnDate = a.LastModifiedOnDate,
                                     ChucVu = b.Role,
                                 });
                    var queryResults = query.OrderByDescending(sp => sp.OrganizationName).ThenBy(sp => sp.CreatedOnDate).ToList();

                    var listResult = new List<OrganizationClient>();

                    if (queryResults != null && queryResults.Count > 0)
                    {
                        foreach (var item in queryResults)
                        {
                            var uClient = new OrganizationClient()
                            {
                                OrganizationID = item.OrganizationID.ToString(),
                                ApplicationID = item.ApplicationID.ToString(),
                                ParentOrganizationID = item.ParentOrganizationID.ToString(),
                                OrganizationName = item.OrganizationName,
                                CreatedByUserID = item.CreatedByUserID.ToString(),
                                CreatedOnDate = item.CreatedOnDate,
                                LastModifiedByUserID = item.LastModifiedByUserID.ToString(),
                                LastModifiedOnDate = item.LastModifiedOnDate,
                                ChucVu = item.ChucVu,
                            };
                            listResult.Add(uClient);
                        }
                        return listResult;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        #endregion

        #endregion

        #region Function Add,Update,Delete

        public Response<OrganizationClient> Add(OrganizationClient organization)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var OrganizationID = Guid.NewGuid();
                    //  var ParentOrganizationID = Guid.NewGuid();
                    var ctemplate = new aspnet_Organizations()
                    {
                        OrganizationID = OrganizationID,
                        ApplicationID = Application.ApplicationID,
                        ParentOrganizationID = new Guid(organization.ParentOrganizationID),
                        OrganizationName = organization.OrganizationName,
                        Type = organization.Type,
                        Description = organization.Description,
                        LastModifiedByUserID = new Guid(organization.CreatedByUserID),
                        CreatedByUserID = new Guid(organization.CreatedByUserID),
                        LastModifiedOnDate = System.DateTime.Now,
                        CreatedOnDate = System.DateTime.Now,
                        STT = organization.STT,
                        DiaChi = organization.DiaChi,
                        SoFax = organization.SoFax,
                        SoDienThoai = organization.SoDienThoai,
                        Email = organization.Email,
                        NguoiDaiDien = organization.NguoiDaiDien,
                        Loai = organization.Loai,
                        ShortName = organization.ShortName
                    };

                    unitOfWork.GetRepository<aspnet_Organizations>().Add(ctemplate);
                    if (organization.IsExtent)
                    {
                        var roleHandler = new RoleHandler();
                        var createdRoleResult = roleHandler.CreateRole(OrganizationID.ToString(), organization.OrganizationName, organization.ShortName, true, organization.Description, DateTime.Now, organization.CreatedByUserID, DateTime.Now, organization.CreatedByUserID, new List<string>(), "DONVI",Application.ApplicationID.ToString());
                    }
                    if (unitOfWork.Save() >= 1)
                        return new Response<OrganizationClient>(1, "", ConvertData(ctemplate));
                    else
                        return new Response<OrganizationClient>(-1, "Data save with FileError", null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<OrganizationClient>(-1, ex.Message, null);
            }
        }

        public Response<OrganizationClient> Update(OrganizationClient organization)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {

                    var uOrganization = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.OrganizationID ==  new Guid(organization.OrganizationID) && sp.ApplicationID == Application.ApplicationID).FirstOrDefault();

                    uOrganization.ParentOrganizationID = new Guid(organization.ParentOrganizationID);

                    uOrganization.OrganizationName = organization.OrganizationName;
                    uOrganization.Type = organization.Type;
                    uOrganization.Description = organization.Description;
                    uOrganization.LastModifiedOnDate = System.DateTime.Now;
                    uOrganization.CreatedOnDate = System.DateTime.Now;
                    uOrganization.STT = organization.STT;
                    uOrganization.DiaChi = organization.DiaChi;
                    uOrganization.NguoiDaiDien = organization.NguoiDaiDien;
                    uOrganization.SoFax = organization.SoFax;
                    uOrganization.SoDienThoai = organization.SoDienThoai;
                    uOrganization.Email = organization.Email;
                    uOrganization.Loai = organization.Loai;
                    uOrganization.ShortName = organization.ShortName;

                    unitOfWork.GetRepository<aspnet_Organizations>().Update(uOrganization);

                    if (organization.IsExtent)
                    {
                        var roleHandler = new RoleHandler();
                        var role = unitOfWork.GetRepository<aspnet_Roles>().GetById(new Guid(organization.OrganizationID));
                        if (role != null)
                        {
                            var updatedRoleResult = roleHandler.UpdateRole(new Guid(organization.OrganizationID), organization.OrganizationName, organization.ShortName, organization.Description, DateTime.Now, organization.CreatedByUserID, new string[] { });
                        }
                        else
                        {
                            var createdRoleResult = roleHandler.CreateRole(organization.OrganizationID, organization.OrganizationName, organization.ShortName, true, organization.Description, DateTime.Now, organization.CreatedByUserID, DateTime.Now, organization.CreatedByUserID, new List<string>(), "DONVI", Application.ApplicationID.ToString());
                        }
                    }

                    if (unitOfWork.Save() >= 1)
                        return new Response<OrganizationClient>(1, "", ConvertData(uOrganization));
                    else
                        return new Response<OrganizationClient>(-1, "Data save with FileError", null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new Response<OrganizationClient>(-1, ex.Message, null);
            }
        }

        /// <summary>
        /// Xóa Organization
        /// </summary>
        /// <param name="id">mã Organization</param>        
        /// <remarks>
        /// mã Organization kiểu unique
        /// </remarks>
        public bool DeleteOrganization(Guid id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(dbFactory))
                {
                    var cOrganizationRepo = unitOfWork.GetRepository<aspnet_Organizations>();
                    var cOrganization = cOrganizationRepo.GetById(id);
                    cOrganizationRepo.Delete(cOrganization);
                    unitOfWork.Save();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Xóa Organization
        /// </summary>
        /// <param name="id">mã Organization</param>        
        /// <remarks>
        /// mã Organization kiểu string
        /// </remarks>
        public bool DeleteOrganization(string id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var organizationRepo = unitOfWork.GetRepository<aspnet_Organizations>();
                    var dOrganization = unitOfWork.GetRepository<aspnet_Organizations>().GetMany(sp => sp.OrganizationID == new Guid(id)).FirstOrDefault();

                    organizationRepo.Delete(dOrganization);
                    if (unitOfWork.Save() >= 1)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
        public bool DeleteMany(List<OrganizationClient> organizations)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    for (int i = 0; i < organizations.Count; i++)
                    {
                        var cOrganiRepo = unitOfWork.GetRepository<Province_City>();
                        var cOrgani = cOrganiRepo.GetById(organizations[i].OrganizationID);

                        cOrganiRepo.Delete(cOrgani);
                    }

                    if (unitOfWork.Save() >= 1)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }


        }

        #endregion

        #region Function Convert
        private OrganizationClient ConvertData(Data.aspnet_Organizations data)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    OrganizationClient organizationClient = new OrganizationClient()
                    {
                        OrganizationID = data.OrganizationID.ToString(),
                        OrganizationName = data.OrganizationName,
                        ApplicationID = data.ApplicationID.ToString(),
                        Type = data.Type,
                        Description = data.Description,
                        ParentName = GetParentName(data.ParentOrganizationID.ToString()),
                        ParentOrganizationID = data.ParentOrganizationID.ToString(),
                        CreatedByUserID = data.CreatedByUserID.ToString(),
                        CreatedOnDate = data.CreatedOnDate,
                        LastModifiedByUserID = data.LastModifiedByUserID.ToString(),
                        LastModifiedOnDate = data.LastModifiedOnDate,
                        STT = data.STT,
                        NguoiDaiDien = data.NguoiDaiDien,
                        DiaChi = data.DiaChi,
                        SoDienThoai = data.SoDienThoai,
                        SoFax = data.SoFax,
                        Email = data.Email,
                        Loai = data.Loai,
                        ShortName = data.ShortName
                    };
                    var lstChildOrganization = Get(new OrganizationQueryFilter() { ParentOrganizationID = data.OrganizationID.ToString() });
                    if (lstChildOrganization != null)
                    {
                        organizationClient.LstDonVi = new List<OrganizationClient>();
                        var lstnew = new List<UserInOrganizationModel>();
                        if (lstChildOrganization.Status == 1 && lstChildOrganization.Data != null && lstChildOrganization.Data.Count > 0)
                        {
                            organizationClient.LstDonVi = lstChildOrganization.Data.OrderBy(s => s.STT).ToList();
                        }
                        var YearNow = DateTime.Now.Year;
                        var NamSoSanh = YearNow + 2;
                        var dataUserPB = from sl in unitOfWork.GetRepository<aspnet_Organizations>().GetAll()
                                         join qtcongtac in unitOfWork.GetRepository<CV_CanBo_QTCongTac>().GetAll()
                                         on sl.OrganizationID equals qtcongtac.PhongBanId into joinphongban
                                         from qtcongtac in joinphongban.DefaultIfEmpty()

                                         //join qtcongtacPbkn in unitOfWork.GetRepository<CV_CanBo_QTCongTac>().GetAll()
                                         //on sl.OrganizationID equals qtcongtacPbkn.PhongBanKnId into joinphongbanKN
                                         //from qtcongtacPbkn in joinphongbanKN.DefaultIfEmpty()

                                         join thongtinchung in unitOfWork.GetRepository<CV_ThongTinChung>().GetAll()
                                         on qtcongtac.CanBoId equals thongtinchung.CanBoId into thongtinCanbo
                                         from thongtinchung in thongtinCanbo.DefaultIfEmpty()

                                         join chucvu in unitOfWork.GetRepository<DM_ChucVu>().GetAll()
                                         on qtcongtac.ChucVuId equals chucvu.Id into joinQtCongtacChucvu
                                         from chucvu in joinQtCongtacChucvu.DefaultIfEmpty()

                                         join user in unitOfWork.GetRepository<aspnet_Membership>().GetAll()
                                         on qtcongtac.CanBoId equals user.CanBoId into joinUser
                                         from user in joinUser.DefaultIfEmpty()
                                         where (qtcongtac.PhongBanId == data.OrganizationID && qtcongtac.PhongCongTacHienTai == true) || (qtcongtac.PhongBanKnId == data.OrganizationID && qtcongtac.PhongCongTacHienTai == true)
                                         select new UserInOrganizationModel
                                         {
                                             UserId = user.UserId,
                                             Username = user.NickName,
                                             CanBoId = qtcongtac.CanBoId,
                                             EmailUser = thongtinchung.Email,
                                             FullName = thongtinchung.TenCanBo,
                                             OrganizationID = qtcongtac.PhongBanId,
                                             SoThuTuChucVu = qtcongtac.ChucVuId == null ? 100 : chucvu.ThuTu,
                                         };

                        if (dataUserPB != null)
                        {
                            lstnew = dataUserPB.OrderBy(x => x.SoThuTuChucVu).ToList();

                            var list = lstnew.GroupBy(z => z.CanBoId).Select(grp => grp.First());

                            organizationClient.LstUser = list.ToList();

                        }
                         
                    }
                    if (data.NguoiDaiDien != null)
                    {
                        var user = unitOfWork.GetRepository<aspnet_Membership>().GetMany(s => s.UserId == organizationClient.NguoiDaiDien).FirstOrDefault();
                        organizationClient.TenNguoiDaiDien = user.FullName;
                    }
                    return organizationClient;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        private List<OrganizationClient> ConvertDatasNormal(IEnumerable<aspnet_Organizations> cApps)
        {
            List<OrganizationClient> appsList = new List<OrganizationClient>();

            foreach (aspnet_Organizations apps in cApps)
            {
                appsList.Add(ConvertData(apps));
            }
            return appsList;
        }

        #endregion
    }
}
