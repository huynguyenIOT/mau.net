using AIS.Intranet.API.App_Authentication;
using AIS.Intranet.Business.Core;
using AIS.Intranet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using AIS.Intranet.Business.Core.Membership;
using AIS.Intranet.Business.Config;

namespace AIS.Intranet.API.Controllers
{
    public class OrganizationsController : ApiController
    {
        ILogService logger = BusinessServiceLocator.Instance.GetService<ILogService>();
        IOrganizationHandler handler = BusinessServiceLocator.Instance.GetService<IOrganizationHandler>();

        // GET api/Organizations
        // [BasicAuthorizeAttribute]
        // [RequireHttps]
        [HttpGet]
        [Route("api/organizations")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public Response<IList<OrganizationClient>> GetAll()
        {
            return handler.GetOrganizations();
        }

        [HttpGet]
        [Route("api/allorganizations")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public Response<IList<OrganizationClient>> GetAllOrGa()
        {
            var handleror = new OrganizationHandler();
            return handleror.GetAllOrGa();
        }

        [HttpGet]
        [Route("api/organizations")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public IEnumerable<OrganizationClient> GetOrganizationsByType(string type)
        {
            return handler.GetOrganizationsByType(type);
        }


        [HttpGet]
        [Route("api/organizations")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IEnumerable<OrganizationClient> Get(int pageNumber, int pageSize, string textSearch)
        {
            //Return result
            var rt = handler.GetPageMany(pageNumber, pageSize, textSearch);

            //Log history
            if (rt != null)
            {
                string module = ApiUtils.GetRequestFields(Request, User).ModuleId.ToString();
                string user = ApiUtils.GetRequestFields(Request, User).UserId.ToString();

                Guid moduleId = string.IsNullOrEmpty(module) ? Guid.Empty : new Guid(module);
                Guid userId = string.IsNullOrEmpty(user) ? Guid.Empty : new Guid(user);
                string content = "Search : PageNumber: " + pageNumber + " PageSize : " + pageSize + " Keyword : " + textSearch;
              
            }
            return rt;
        }


        // GET api/Organizations/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public OrganizationClient Get(string id)
        {
            return handler.GetOrganizationClient(id);
        }

        [HttpGet]
        [Route("api/organizations/Parent/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Response<IList<OrganizationClient>> GetByParentId(string id)
        {
            return handler.GetByParentId(id);
        }

        [HttpGet]
        [Route("api/Organization/byloai/{Loai}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Response<IList<OrganizationClient>> GetByLoai(int Loai)
        {
            return handler.GetByLoai(Loai);
        }

        [HttpGet]
        [Route("api/Organization/byuserid/{userId}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Response<IList<OrganizationClient>> GetByUserId(string userId)
        {
            return handler.GetUserByUserId(userId);
        }

        [HttpGet]
        [Route("api/organizations/{id}/users")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public List<UserClient> GetUserInOrgnazition(string id)
        {
            var result = handler.GetUserInOrgnazition(id);
            return result;
        }
        [HttpGet]
        [Route("api/organizations/{organizationsid}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Response<OrganizationClient> GetOrgnazitionById(string organizationsid)
        {
            var result = handler.GetById(organizationsid);
            return result;
        }
        [HttpGet]
        [Route("api/organizations/users/{userId}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public List<OrganizationClient> GetOrgnazitionByUser(string userId)
        {
            return handler.GetOrgnazitionByUser(userId);
        }


        //  POST api/<controller>
        [HttpPost]
        [Route("api/organizations")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<OrganizationClient> Add([FromBody]OrganizationClient value)
        {
            return handler.Add(value);
        }

        [HttpPut]
        [Route("api/organizations/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "PUT")]
      
        // PUT api/<controller>/5
        public Response<OrganizationClient> Update(string id, [FromBody]OrganizationClient value)
        {
            //Return result
            var rt = handler.Update(value);
            return rt;

        }


        [HttpDelete]
        [Route("api/organization/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // DELETE api/<controller>/5
        public bool Delete(string id, string module, string user)
        {
            var rt = handler.DeleteOrganization(id);
            if (rt)
            {
                Guid moduleId = string.IsNullOrEmpty(module) ? Guid.Empty : new Guid(module);
                Guid userId = string.IsNullOrEmpty(user) ? Guid.Empty : new Guid(user);
                string content = "Xóa Tham Số: ID = " + id;
               
            }
            return rt;
        }
        [HttpPost]
        [Route("api/organizations/deletes")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<IList<OrganizationClient>> Deletes([FromBody]DeleteOrganizationPostModel value)
        {
            var listOrgaClient = new List<OrganizationClient>();

            string module = ApiUtils.GetRequestFields(Request, User).ModuleId.ToString();
            string user = ApiUtils.GetRequestFields(Request, User).UserId.ToString();

            Guid moduleId = string.IsNullOrEmpty(module) ? Guid.Empty : new Guid(module);
            Guid userId = string.IsNullOrEmpty(user) ? Guid.Empty : new Guid(user);

            if (value.ListItemDelete != null)
            {
                for (int i = 0; i < value.ListItemDelete.Count; i++)
                {
                    var dResult = handler.DeleteOrganization(value.ListItemDelete[i].OrganizationID);

                    if (dResult == true)
                    {
                        value.ListItemDelete[i].DeleteSuccess = true;
                        listOrgaClient.Add(value.ListItemDelete[i]); ;
                        //Log history                       

                        string content = "Delete : Location : " + value.ListItemDelete[i].OrganizationName;
                        // logger.Debug("Delete", userId, Application.ApplicationID, moduleId, content, new Guid(value.ListItemDelete[i].OrganizationID));

                    }
                    else
                    {
                        value.ListItemDelete[i].DeleteSuccess = false;
                        listOrgaClient.Add(value.ListItemDelete[i]);
                    }
                }
            }
            var dResults = new Response<IList<OrganizationClient>>(1, "", listOrgaClient);
            return dResults;
        }
    }
    public class DeleteOrganizationPostModel
    {
        public List<OrganizationClient> ListItemDelete { get; set; }
    }
    public class OrganizationsPostModel
    {
        public string OrganizationID { get; set; }
        public string ParentOrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string CreatedByUserId { get; set; }
        public string LastModifiedByUserId { get; set; }
        public string RightID { get; set; }
        public string Type { get; set; }

    }
}