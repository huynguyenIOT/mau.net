using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIS.Intranet.Common;
using AIS.Intranet.Data;

namespace AIS.Intranet.Business.Core.Membership
{
    public interface IOrganizationHandler
    {
        Response<IList<OrganizationClient>> GetAllOrGa();
        Response<OrganizationClient> GetById(string id);
        Response<IList<OrganizationClient>> GetBySearchResponse(string search);
        Response<IList<OrganizationClient>> GetOrganizations();
        IEnumerable<OrganizationClient> GetOrganizationsByType(string type);
        Response<IList<OrganizationClient>> GetByParentId(string id);
        OrganizationClient GetOrganizationClient(Guid id);
        OrganizationClient GetOrganizationClient(string id);
        IEnumerable<OrganizationClient> GetPageMany(int pageNum, int pageSize, string textSearch);
        OrganizationClient GetOrganization(aspnet_Organizations u);
        List<OrganizationClient> GetOrganization(IEnumerable<aspnet_Organizations> Organizations);
        Response<IList<OrganizationClient>> GetByLoai(int Loai);
        Response<IList<OrganizationClient>> GetUserByUserId(string userId);

        List<UserClient> GetUserInOrgnazition(string id);
        //   IEnumerable<UserClient> GetPageManyOrganization(string id, int pageNum, int pageSize, string textSearch);
        List<OrganizationClient> GetOrgnazitionByUser(string userId);
        Response<OrganizationClient> Add(OrganizationClient organization);
        Response<OrganizationClient> Update(OrganizationClient organization);
        //   OrganizationClient CreateOrganization(string ParentOrganizationID, string OrganizationName, DateTime CreatedOnDate, string CreatedByUserId);
        //  OrganizationClient UpdateOrganization(Guid OrganizationID, string ParentOrganizationID, string OrganizationName, DateTime LastModifiedOnDate, string LastModifiedByUserId);
        bool DeleteOrganization(Guid id);
        bool DeleteOrganization(string id);
        bool DeleteMany(List<OrganizationClient> organizations);


    }
}
