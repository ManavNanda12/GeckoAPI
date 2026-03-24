using GeckoAPI.Model.models;
using GeckoAPI.Repository.contactus;
using GeckoAPI.Repository.customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.contactus
{
    public class ContactusService:IContactusService
    {
        #region Constructor
        private readonly IContactusRepository _contactusRepository;
        public ContactusService(IContactusRepository contactusRepository)
        {
            _contactusRepository = contactusRepository;
        }
        #endregion

        #region Methods
        public async Task<long> SaveContactRequest(ContactUs model)
        {
            var data = await _contactusRepository.SaveContactRequest(model);
            return data;
        }

        public async Task<List<ContactUs>> GetContactUsList(CommonListRequestModel model)
        {
            var data = await _contactusRepository.GetContactUsList(model);
            return data;
        }

        public async Task<long> DeleteContactUsRequest(long contactUsId)
        {
            var data = await _contactusRepository.DeleteContactUsRequest(contactUsId);
            return data;
        }
        #endregion

    }
}
