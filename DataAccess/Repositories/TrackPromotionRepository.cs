using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TrackPromotionRepository : GenericRepository<TrackPromotion>
    {
        public async Task<IEnumerable<TrackPromotionDetail>> GetPromotionDetails(long trackId, long studentId)
        {
            try
            {
                
                var queryResult = await _connectionFactory.GetConnection.QueryAsync<TrackPromotionDetail>(@"select distinct (select count(*) from studenttransaction where PromotionId= " + trackId + " and studentid=" + studentId + ") as IsEnroll,TrackPromotion.Image,TrackPromotion.Id,TrackPromotion.SkuPrice,TrackPromotion.SkuNumber,TrackPromotion.Description, TrackPromotion.TrackId,TrackPromotion.Name,TrackPromotion.PromotionEndDate,TrackPromotion.PromotionStartDate,IsShowInMobile,NewPrice,Track.Image as TrackImage from Track inner join   TrackPromotion  on Track.id=TrackPromotion.TrackId left join StudentTransaction on TrackPromotion.id=StudentTransaction.PromotionId left join TrackPromotionCourse on TrackPromotion.Id=TrackPromotionCourse.TrackPromotionId where  CAST(PromotionStartDate AS DATE) <= CAST(GETDATE() AS DATE) and CAST(PromotionEndDate AS DATE) >= CAST(GETDATE() AS DATE)  and TrackPromotion.TrackId=" + trackId + " ",null);
                var result = new List<TrackPromotionDetail>();

                foreach (var item in queryResult)
                {
                    var courses = await _connectionFactory.GetConnection.QueryAsync<Course>(@" select * from Course as OuterCourse where OuterCourse.TrackId=" + item.TrackId + "");
                    result.Add(new TrackPromotionDetail()
                    {
                        Id=item.Id,
                        TrackId=item.TrackId,
                        Description=item.Description,
                        PromotionStartDate = item.PromotionStartDate,
                        PromotionEndDate =item.PromotionEndDate,
                        SkuPrice=item.SkuPrice,
                        SkuNumber=item.SkuNumber,
                        NewPrice=item.NewPrice,
                        Name = item.Name,
                        IsShowInMobile = item.IsShowInMobile,
                        Image=item.Image,
                        IsEnroll=item.IsEnroll,
                        TrackImage=item.TrackImage,
                        Courses = (IEnumerable<Course>)courses
                    });
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}