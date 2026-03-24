using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.contactus
{
    public class ContactusRepository : BaseRepository,IContactusRepository
    {
        #region Constructor
        public ContactusRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods
        public Task<long> SaveContactRequest(ContactUs model)
        {
            var param = new DynamicParameters();
            param.Add("@ContactUsId", model.ContactUsId);
            param.Add("@ContactSubject", model.ContactSubject);
            param.Add("@CustomerEmail", model.CustomerEmail);
            param.Add("@CustomerMessage", model.CustomerMessage);
            param.Add("@AdminMessage", model.AdminMessage);
            param.Add("@CustomerName", model.CustomerName);
            var response = Execute(StoredProcedures.SaveContactRequest, param);
            return Task.FromResult(response.Data);
        }


        public Task<List<ContactUs>> GetContactUsList(CommonListRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@PageNumber", model.PageNumber);
            param.Add("@PageSize", model.PageSize);
            param.Add("@SearchTerm", model.SearchTerm);
            param.Add("@SortColumn", model.SortColumn);
            param.Add("@SortDirection", model.SortDirection);
            var response = Query<ContactUs>(StoredProcedures.GetContactUsRequestList, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> DeleteContactUsRequest(long contactUsId)
        {
            var param = new DynamicParameters();
            param.Add("@ContactUsId", contactUsId);
            var response = Execute(StoredProcedures.DeleteContactRequest, param);
            return Task.FromResult(response.Data);
        }
        #endregion
    }
}
