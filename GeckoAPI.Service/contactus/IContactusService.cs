using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.contactus
{
    public interface IContactusService
    {
        Task<long> SaveContactRequest(ContactUs model);
        Task<List<ContactUs>> GetContactUsList(CommonListRequestModel model);
        Task<long> DeleteContactUsRequest(long contactUsId);
    }
}
