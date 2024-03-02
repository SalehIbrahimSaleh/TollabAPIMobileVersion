using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
  public  class OfferRepository:GenericRepository<Offer>
    {

        public async Task<IEnumerable<Offer>> GetOffers(long CountryId)
        {
            var offers = new List<Offer>();
            var courseOffers = GetCourseOffers(CountryId);
            var trackOffers = GetTrackOffers(CountryId);
            var linkOffers = await GetLinkOffers(CountryId);
            return offers.Concat(courseOffers).Concat(trackOffers).Concat(linkOffers).OrderBy(a=>a.OrderNumber);   
        }


        IEnumerable<Offer> GetCourseOffers(long countryId)
        {
            string sql = @"SELECT Offer.*,Course.* FROM Offer INNER JOIN Course  ON Offer.CourseId = Course.Id  
join Track on Track.Id=Course.TrackId join Subject on Track.SubjectId=Subject.Id join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId join Category on Category.Id=SubCategory.CategoryId
join Section on Section.Id=Category.SectionId where Course.CourseStatusId=3 And Section.CountryId=" + countryId + " And Offer.EndOfferDate > (select getutcDate());";



            var offers = _connectionFactory.GetConnection.Query<Offer, Course, Offer>(
                    sql,
                    (offer, course) =>
                    {
                        offer.Course = course;
                        return offer;
                    },
                    splitOn: "Id")
                    .Distinct();
            return offers;
        }

        IEnumerable<Offer> GetTrackOffers(long countryId)
        {
            string sql = @"SELECT Offer.*,Track.* FROM Offer inner JOIN Track  ON Offer.TrackId = Track.Id 
join Subject on Track.SubjectId=Subject.Id join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId join Category on Category.Id=SubCategory.CategoryId
join Section on Section.Id=Category.SectionId where Section.CountryId=" + countryId + " And Offer.EndOfferDate > (select getutcDate());";



            var offers = _connectionFactory.GetConnection.Query<Offer, Track, Offer>(
                    sql,
                    (offer, Track) =>
                    {
                        offer.Track = Track;
                        return offer;
                    },
                    splitOn: "Id")
                    .Distinct();
            return offers;
        }


        async Task<IEnumerable<Offer>> GetLinkOffers(long countryId)
        {
            string sql = @"SELECT OfferCountry.* , Offer.* from OfferCountry inner join Offer 
on OfferCountry.OfferId = Offer.Id where OfferLinkTypeId = 3
And OfferCountry.CountryId = "+countryId+" And Offer.EndOfferDate > (select getutcDate())";



            var offers = await _connectionFactory.GetConnection.QueryAsync<Offer>(sql);

            return offers.GroupBy(offer=> offer.Id).Select(x=>x.FirstOrDefault());
        }



    }
}
