using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
   public class OfferService
    {
        OfferRepository _offerRepository;
        public OfferService()
        {
            _offerRepository = new OfferRepository();
        }

        public async Task<IEnumerable<Offer>> GetOffers(long CountryId)
        {
            var result = await _offerRepository.GetOffers(CountryId);
            return result;
        }
    }
}
